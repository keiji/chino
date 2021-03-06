using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExposureNotifications;
using Foundation;
using ObjCRuntime;
using UIKit;

using Logger = Chino.ChinoLogger;

#nullable enable

namespace Chino.iOS
{
    // https://developer.apple.com/documentation/exposurenotification/enmanager
    [Introduced(PlatformName.iOS, 12, 5)]
    public class ExposureNotificationClient : AbsExposureNotificationClient
    {
        private const long MAXIMUM_ZIP_ARCHIVE_ENTRY_SIZE = 10 * 1024 * 1024;

        private Lazy<ENManager> EnManager = new Lazy<ENManager>(() => CreateEnManager());

        private static ENManager CreateEnManager() => new ENManager()
        {
            DiagnosisKeysAvailableHandler = new ENDiagnosisKeysAvailableHandler(async teks =>
            {
                if (Handler is null)
                {
                    Logger.E("ENDiagnosisKeysAvailableHandler is called but ENDiagnosisKeysAvailableHandler is not set.");
                    return;
                }

                IList<TemporaryExposureKey> temporaryExposureKeys = teks.Select(tek => (TemporaryExposureKey)new PlatformTemporaryExposureKey(tek)).ToList();
                await Handler.TemporaryExposureKeyReleasedAsync(temporaryExposureKeys);
            })
        };

        public string UserExplanation { private get; set; }

        private bool IsActivated = false;

        private readonly SemaphoreSlim ActivateSemaphore = new SemaphoreSlim(1, 1);

        public async Task ActivateAsync()
        {
            await ActivateSemaphore.WaitAsync();
            try
            {
                if (IsActivated)
                {
                    return;
                }

                await EnManager.Value.ActivateAsync();
                IsActivated = true;
            }
            catch (NSErrorException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
            finally
            {
                ActivateSemaphore.Release();
            }
        }

        ~ExposureNotificationClient()
        {
            try
            {
                EnManager.Value.Invalidate();
            }
            catch (NSErrorException exception)
            {
                exception.LogD();
            }
        }

        private void CheckActivated()
        {
            if (!IsActivated)
            {
                Logger.E("StartAsync method must be called first.");
            }
        }

        public async override Task StartAsync()
        {
            await ActivateAsync();

            try
            {
                await EnManager.Value.SetExposureNotificationEnabledAsync(true);
            }
            catch (NSErrorException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        public async override Task StopAsync()
        {
            CheckActivated();

            try
            {
                await EnManager.Value.SetExposureNotificationEnabledAsync(false);
            }
            catch (NSErrorException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        public async override Task<bool> IsEnabledAsync()
        {
            await ActivateAsync();

            try
            {
                return EnManager.Value.ExposureNotificationEnabled;
            }
            catch (NSErrorException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        public override Task<long> GetVersionAsync()
        {
            return Task.Run(() => long.Parse(NSBundle.MainBundle.InfoDictionary["ENAPIVersion"].ToString()));
        }

        public async override Task<IList<ExposureNotificationStatus>> GetStatusesAsync()
        {
            await ActivateAsync();

            try
            {
                ENStatus status = EnManager.Value.ExposureNotificationStatus;
                return new List<ExposureNotificationStatus>
                {
                    status.ToExposureNotificationStatus()
                };
            }
            catch (NSErrorException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        public async override Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
        {
            CheckActivated();

            try
            {
#if DEBUG
                ENTemporaryExposureKey[] teks = await EnManager.Value.GetTestDiagnosisKeysAsync();
#else
                ENTemporaryExposureKey[] teks = await EnManager.Value.GetDiagnosisKeysAsync();
#endif
                return teks.Select(tek => (TemporaryExposureKey)new PlatformTemporaryExposureKey(tek)).ToList();
            }
            catch (NSErrorException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        // https://developer.apple.com/documentation/exposurenotification/enmanager/3586331-detectexposures
        private async Task<(string?, string?)> DecompressZip(string zipFilePath)
        {
            string baseDirectory = Path.GetDirectoryName(zipFilePath);
            string baseFileName = Guid.NewGuid().ToString();

            string binFilePath = Path.Combine(baseDirectory, $"{baseFileName}.bin");
            string sigFilePath = Path.Combine(baseDirectory, $"{baseFileName}.sig");

            Logger.D($"binFilePath: {binFilePath}");
            Logger.D($"sigFilePath: {sigFilePath}");

            using FileStream fs = File.OpenRead(zipFilePath);
            using ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Read);

            // Check entry size
            foreach (ZipArchiveEntry entry in zipArchive.Entries)
            {
                if (entry.Length > MAXIMUM_ZIP_ARCHIVE_ENTRY_SIZE)
                {
                    Logger.E($"Maximum zip archive entry size exceeded - ${zipFilePath} - Size: {entry.Length}");
                    return (null, null);
                }
            }

            using (FileStream binFileWriter = File.Create(binFilePath))
            {
                var entry = zipArchive.GetEntry("export.bin");
                Logger.D($"Extract export.bin - Size: {entry.Length}");

                using var reader = entry.Open();
                await reader.CopyToAsync(binFileWriter);
            }

            using (FileStream binFileWriter = File.Create(sigFilePath))
            {
                var entry = zipArchive.GetEntry("export.sig");
                Logger.D($"Extract export.sig - Size: {entry.Length}");

                using var reader = entry.Open();
                await reader.CopyToAsync(binFileWriter);
            }

            return (binFilePath, sigFilePath);
        }

        public async override Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> zippedKeyFiles,
            CancellationTokenSource? cancellationTokenSource = null
            )
        {
            if (zippedKeyFiles.Count == 0)
            {
                return ProvideDiagnosisKeysResult.NoDiagnosisKeyFound;
            }

            CheckActivated();

            if (Handler is null)
            {
                throw new IllegalStateException("IExposureNotificationHandler is not set.");
            }

            ExposureConfiguration configuration = await Handler.GetExposureConfigurationAsync();
            if (configuration is null)
            {
                throw new IllegalStateException("ExposureConfiguration is null.");
            }

            cancellationTokenSource ??= new CancellationTokenSource();

            List<string> decompressedFiles = new List<string>();

            foreach (string filePath in zippedKeyFiles)
            {
                if (!filePath.EndsWith(".zip"))
                {
                    Logger.W($"File {filePath} do not seem Zip file.");
                    continue;
                }

                var (binFilePath, sigFilePath) = await DecompressZip(filePath);
                if (binFilePath != null && sigFilePath != null)
                {
                    decompressedFiles.Add(binFilePath);
                    decompressedFiles.Add(sigFilePath);
                }
            }

            Logger.D($"{decompressedFiles.Count()} files are created.");

            NSUrl[] urls = decompressedFiles.Select(keyFile => new NSUrl(keyFile, false)).ToArray();

            foreach (NSUrl url in urls)
            {
                Logger.D(url.AbsoluteString);
            }

            long enAPiVersion = await GetVersionAsync();

            ENExposureConfiguration exposureConfiguration = enAPiVersion switch
            {
                2 => configuration.AppleExposureConfigV2.ToENExposureConfiguration(),
                _ => configuration.AppleExposureConfigV1.ToENExposureConfiguration(),

            };
            Logger.D(exposureConfiguration.ToString());

            try
            {
                var detectExposuresTask = EnManager.Value.DetectExposuresAsync(
                    exposureConfiguration,
                    urls,
                    out NSProgress result
                    );

                using (cancellationTokenSource.Token.Register(result.Cancel))
                {
                    ENExposureDetectionSummary summary = await detectExposuresTask;

                    await Handler.DiagnosisKeysDataMappingAppliedAsync();

                    if (enAPiVersion == 2 && UIDevice.CurrentDevice.CheckSystemVersion(13, 7))
                    {
                        await GetExposureV2(summary, configuration, Handler);
                    }
                    else if (UIDevice.CurrentDevice.CheckSystemVersion(13, 5))
                    {
                        await GetExposureV1(summary, configuration, Handler);
                    }
                    else if (Class.GetHandle("ENManager") != null)
                    {
                        await GetExposureV2(summary, configuration, Handler);
                    }
                    else
                    {
                        Logger.I("Exposure Notifications not supported on this version of iOS.");
                    }
                }

                return ProvideDiagnosisKeysResult.Completed;
            }
            catch (NSErrorException exception)
            {
                if (exception.IsENException())
                {
                    var enException = exception.ToENException();
                    await Handler.ExceptionOccurredAsync(enException);
                    throw enException;
                }
                else
                {
                    await Handler.ExceptionOccurredAsync(exception);
                    throw;
                }
            }
            finally
            {
                // Delete decompressed files
                Logger.D($"{decompressedFiles.Count()} files will be deleted.");

                foreach (string file in decompressedFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception exception)
                    {
                        Logger.E(exception);
                    }
                }
            }
        }

        [Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            string token,
            CancellationTokenSource? cancellationTokenSource = null
            )
            => await ProvideDiagnosisKeysAsync(keyFiles, cancellationTokenSource);
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        private async Task GetExposureV2(ENExposureDetectionSummary summary, ExposureConfiguration exposureConfiguration, IExposureNotificationHandler handler)
        {
            Logger.D($"GetExposureV2");

            List<DailySummary> dailySummaries = summary.DaySummaries.Select(ds => (DailySummary)new PlatformDailySummary(ds)).ToList();

            if (dailySummaries.Count > 0)
            {
                await handler.PreExposureDetectedAsync(exposureConfiguration);

                ENExposureWindow[] ews = await EnManager.Value.GetExposureWindowsAsync(summary);
                List<ExposureWindow> exposureWindows = ews.Select(ew => (ExposureWindow)new PlatformExposureWindow(ew)).ToList();

                await handler.ExposureDetectedAsync(dailySummaries, exposureWindows, exposureConfiguration);
                await handler.ExposureDetectedAsync(new PlatformExposureSummary(summary), dailySummaries, exposureWindows, exposureConfiguration);
            }
            else
            {
                await handler.ExposureNotDetectedAsync(exposureConfiguration);
            }
        }

        private async Task GetExposureV1(ENExposureDetectionSummary summary, ExposureConfiguration exposureConfiguration, IExposureNotificationHandler handler)
        {
            Logger.D($"GetExposureV1");

            if (summary.MatchedKeyCount > 0)
            {
                await handler.PreExposureDetectedAsync(exposureConfiguration);

                ENExposureInfo[] eis = await EnManager.Value.GetExposureInfoAsync(summary, UserExplanation);
                List<ExposureInformation> exposureInformations = eis.Select(ei => (ExposureInformation)new PlatformExposureInformation(ei)).ToList();

                await handler.ExposureDetectedAsync(new PlatformExposureSummary(summary), exposureInformations, exposureConfiguration);
            }
            else
            {
                await handler.ExposureNotDetectedAsync(exposureConfiguration);
            }
        }

        [Introduced(PlatformName.iOS, 14, 4)]
        public override async Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(14, 4))
            {
                throw new NotSupportedException("RequestPreAuthorizedTemporaryExposureKeyHistoryAsync is only available on iOS 14.4 or newer.");
            }

            CheckActivated();

            try
            {
                await EnManager.Value.PreAuthorizeDiagnosisKeysAsync();
            }
            catch (NSErrorException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        [Introduced(PlatformName.iOS, 14, 4)]
        public override async Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(14, 4))
            {
                throw new NotSupportedException("RequestPreAuthorizedTemporaryExposureKeyReleaseAsync is only available on iOS 14.4 or newer.");
            }

            CheckActivated();

            try
            {
                await EnManager.Value.RequestPreAuthorizedDiagnosisKeysAsync();
            }
            catch (NSErrorException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }
    }
}
