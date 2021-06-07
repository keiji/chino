using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ExposureNotifications;
using Foundation;
using UIKit;

using Logger = Chino.ChinoLogger;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enmanager
    public class ExposureNotificationClient : AbsExposureNotificationClient
    {
        private const long MAXIMUM_ZIP_ARCHIVE_ENTRY_SIZE = 10 * 1024 * 1024;

        private readonly ENManager EnManager = new ENManager()
        {
            DiagnosisKeysAvailableHandler = new ENDiagnosisKeysAvailableHandler(teks =>
            {
                if (Handler == null)
                {
                    Logger.E("ENDiagnosisKeysAvailableHandler is called but ENDiagnosisKeysAvailableHandler is not set.");
                    return;
                }

                IList<ITemporaryExposureKey> temporaryExposureKeys = teks.Select(tek => (ITemporaryExposureKey)new TemporaryExposureKey(tek)).ToList();
                Handler.TemporaryExposureKeyReleased(temporaryExposureKeys);
            })
        };

        public bool IsTest = false;

        public string UserExplanation { private get; set; }

        private bool IsActivated = false;

        public async Task ActivateAsync()
        {
            if (IsActivated)
            {
                return;
            }
            await EnManager.ActivateAsync();
            IsActivated = true;
        }

        ~ExposureNotificationClient()
        {
            EnManager.Invalidate();
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
            await EnManager.SetExposureNotificationEnabledAsync(true);
        }

        public async override Task StopAsync()
        {
            CheckActivated();

            await EnManager.SetExposureNotificationEnabledAsync(false);
        }

        public async override Task<bool> IsEnabledAsync()
        {
            await ActivateAsync();
            return EnManager.ExposureNotificationEnabled;
        }

        public override Task<long> GetVersionAsync()
        {
            return Task.Run(() => long.Parse(NSBundle.MainBundle.InfoDictionary["ENAPIVersion"].ToString()));
        }

        public async override Task<IExposureNotificationStatus> GetStatusAsync()
        {
            await ActivateAsync();
            return new ExposureNotificationStatus(EnManager.ExposureNotificationStatus);
        }

        public async override Task<List<ITemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
        {
            CheckActivated();

            if (!IsTest)
            {
                ENTemporaryExposureKey[] teks = await EnManager.GetDiagnosisKeysAsync();
                return teks.Select(tek => (ITemporaryExposureKey)new TemporaryExposureKey(tek)).ToList();
            }
            else
            {
                ENTemporaryExposureKey[] teks = await EnManager.GetTestDiagnosisKeysAsync();
                return teks.Select(tek => (ITemporaryExposureKey)new TemporaryExposureKey(tek)).ToList();
            }
        }

        public override Task ProvideDiagnosisKeysAsync(List<string> keyFiles) => ProvideDiagnosisKeysAsync(keyFiles, new ExposureConfiguration());

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

        public async override Task ProvideDiagnosisKeysAsync(List<string> zippedKeyFiles, ExposureConfiguration configuration)
        {
            CheckActivated();

            long enAPiVersion = await GetVersionAsync();

            ExposureConfiguration = configuration;

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

            ENExposureConfiguration exposureConfiguration = enAPiVersion switch
            {
                2 => ExposureConfiguration.AppleExposureConfigV2.ToENExposureConfiguration(),
                _ => ExposureConfiguration.AppleExposureConfigV1.ToENExposureConfiguration(),

            };
            Logger.D(exposureConfiguration.ToString());

            ENExposureDetectionSummary summary = await EnManager.DetectExposuresAsync(exposureConfiguration, urls);

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

            if (enAPiVersion == 2 && UIDevice.CurrentDevice.CheckSystemVersion(13, 7))
            {
                await GetExposureV2(summary);
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(13, 5))
            {
                await GetExposureV1(summary);
            }
            else if (ObjCRuntime.Class.GetHandle("ENManager") != null)
            {
                await GetExposureV2(summary);
            }
            else
            {
                Logger.I("Exposure Notifications not supported on this version of iOS.");
            }
        }

        private void Print(ENExposureDetectionSummary summary)
        {
            Logger.D("ExposureDetectionSummary");
            Logger.D($"AttenuationDurations: {summary.AttenuationDurations}");
            Logger.D($"DaysSinceLastExposure: {summary.DaysSinceLastExposure}");
            Logger.D($"MatchedKeyCount: {summary.MatchedKeyCount}");
            Logger.D($"MaximumRiskScore: {summary.MaximumRiskScore}");
            Logger.D($"MaximumRiskScoreFullRange: {summary.MaximumRiskScoreFullRange}");
            Logger.D($"RiskScoreSumFullRange: {summary.RiskScoreSumFullRange}");
            Logger.D($"Metadata: {summary.Metadata}");

            if (summary.DaySummaries == null)
            {
                Logger.D($"DaySummaries are null.");
                return;
            }

            foreach (var daySummary in summary.DaySummaries)
            {
                Logger.D($"Date: {daySummary.Date}");
                Print(daySummary.ConfirmedClinicalDiagnosisSummary);
                Print(daySummary.ConfirmedTestSummary);
                Print(daySummary.RecursiveSummary);
                Print(daySummary.SelfReportedSummary);
                Print(daySummary.DaySummary);
            }
        }

        private void Print(ENExposureSummaryItem daySummary)
        {
            if (daySummary == null)
            {
                return;
            }

            Logger.D($"ENExposureSummaryItem");
            Logger.D($"MaximumScore: {daySummary.MaximumScore}");
            Logger.D($"ScoreSum: {daySummary.ScoreSum}");
            Logger.D($"WeightedDurationSum: {daySummary.WeightedDurationSum}");
        }

        [Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration, string token)
        {
            CheckActivated();

            await ProvideDiagnosisKeysAsync(keyFiles, configuration);
        }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        private async Task GetExposureV2(ENExposureDetectionSummary summary)
        {
            Logger.D($"GetExposureV2");

            List<IDailySummary> dailySummaries = summary.DaySummaries.Select(ds => (IDailySummary)new DailySummary(ds)).ToList();

            if (dailySummaries.Count > 0)
            {
                ENExposureWindow[] ews = await EnManager.GetExposureWindowsAsync(summary);
                Print(summary);

                List<IExposureWindow> exposureWindows = ews.Select(ew => (IExposureWindow)new ExposureWindow(ew)).ToList();

                Logger.D(exposureWindows);

                Handler.ExposureDetected(dailySummaries, exposureWindows);
            }
            else
            {
                Handler.ExposureNotDetected();
            }
        }

        private async Task GetExposureV1(ENExposureDetectionSummary summary)
        {
            Logger.D($"GetExposureV1");

            if (summary.MatchedKeyCount > 0)
            {
                ENExposureInfo[] eis = await EnManager.GetExposureInfoAsync(summary, UserExplanation);
                List<IExposureInformation> exposureInformations = eis.Select(ei => (IExposureInformation)new ExposureInformation(ei)).ToList();

                Handler.ExposureDetected(new ExposureSummary(summary), exposureInformations);
            }
            else
            {
                Handler.ExposureNotDetected();
            }
        }

        public override async Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
        {
            CheckActivated();

            await EnManager.PreAuthorizeDiagnosisKeysAsync();
        }
        public override async Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
        {
            CheckActivated();

            await EnManager.RequestPreAuthorizedDiagnosisKeysAsync();
        }
    }
}
