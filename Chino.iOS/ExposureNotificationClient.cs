using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ExposureNotifications;
using Foundation;
using UIKit;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enmanager
    public class ExposureNotificationClient : AbsExposureNotificationClient
    {
        private const long MAXIMUM_ZIP_ARCHIVE_ENTRY_SIZE = 10 * 1024 * 1024;

        public static readonly ExposureNotificationClient Shared = new ExposureNotificationClient();

        private readonly ENManager EnManager = new ENManager();

        public bool IsTest = false;

        public string UserExplanation { private get; set; }

        public Task Init(string userExplanation)
        {
            UserExplanation = userExplanation;

            return Task.Run(async () =>
            {
                await EnManager.ActivateAsync();
            });
        }

        ~ExposureNotificationClient()
        {
            EnManager.Invalidate();
        }

        public async override Task Start() => await EnManager.SetExposureNotificationEnabledAsync(true);

        public async override Task Stop() => await EnManager.SetExposureNotificationEnabledAsync(false);

        public override Task<bool> IsEnabled() => Task.Run(() => EnManager.ExposureNotificationEnabled);

        public override Task<long> GetVersion() => Task.Run(() => long.Parse(NSBundle.MainBundle.InfoDictionary["ENAPIVersion"].ToString()));

        public override Task<IExposureNotificationStatus> GetStatus()
        {
            return Task.Run(() =>
             {
                 return (IExposureNotificationStatus)new ExposureNotificationStatus(EnManager.ExposureNotificationStatus);
             });
        }

        public async override Task<List<ITemporaryExposureKey>> GetTemporaryExposureKeyHistory()
        {
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

        public override Task ProvideDiagnosisKeys(List<string> keyFiles) => ProvideDiagnosisKeys(keyFiles, new ExposureConfiguration());

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

        public async override Task ProvideDiagnosisKeys(List<string> zippedKeyFiles, ExposureConfiguration configuration)
        {
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

            ENExposureConfiguration exposureConfiguration = GetExposureConfiguration(configuration);

            ENExposureDetectionSummary summary = await EnManager.DetectExposuresAsync(exposureConfiguration, urls);

            // Delete decompressed files
            Logger.D($"{decompressedFiles.Count()} files will be deleted.");

            foreach (string file in decompressedFiles)
            {
                File.Delete(file);
                Logger.D($"File {file} is deleted.");
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 7))
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

        [Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override async Task ProvideDiagnosisKeys(List<string> keyFiles, ExposureConfiguration configuration, string token)
            => await ProvideDiagnosisKeys(keyFiles, configuration);
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        private async Task GetExposureV2(ENExposureDetectionSummary summary)
        {
            Logger.D($"GetExposureV2");

            if (summary.MatchedKeyCount > 1)
            {
                ENExposureWindow[] ews = await EnManager.GetExposureWindowsAsync(summary);
                List<IExposureWindow> exposureWindows = ews.Select(ew => (IExposureWindow)new ExposureWindow(ew)).ToList();

                Handler.ExposureDetected(exposureWindows);
            }
            else
            {
                Handler.ExposureNotDetected();
            }
        }

        private async Task GetExposureV1(ENExposureDetectionSummary summary)
        {
            Logger.D($"GetExposureV1");

            if (summary.MatchedKeyCount > 1)
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

        private ENExposureConfiguration GetExposureConfiguration(ExposureConfiguration exposureConfiguration)
        {
            var appleExposureConfiguration = exposureConfiguration.AppleExposureConfig;

            if (appleExposureConfiguration == null)
            {
                Logger.E("appleExposureConfiguration is not set.");
                return new ENExposureConfiguration();
            }

            NSDictionary<NSNumber, NSNumber> infectiousnessForDaysSinceOnsetOfSymptomsNSDict
                = GetInfectiousnessForDaysSinceOnsetOfSymptomsNSDict(appleExposureConfiguration.InfectiousnessForDaysSinceOnsetOfSymptoms);

            ENExposureConfiguration configuration = new ENExposureConfiguration();

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 6))
            {
                Logger.D("Set configuration values for iOS 13.6 later.");
                configuration.AttenuationDurationThresholds = appleExposureConfiguration.AttenuationDurationThreshold;
                configuration.MinimumRiskScoreFullRange = appleExposureConfiguration.MinimumRiskScoreFullRange;
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 5))
            {
                Logger.D("Set configuration values for iOS 13.5 later.");
                configuration.ImmediateDurationWeight = appleExposureConfiguration.ImmediateDurationWeight;
                configuration.MediumDurationWeight = appleExposureConfiguration.MediumDurationWeight;
                configuration.NearDurationWeight = appleExposureConfiguration.NearDurationWeight;
                configuration.OtherDurationWeight = appleExposureConfiguration.OtherDurationWeight;
                configuration.DaysSinceLastExposureThreshold = appleExposureConfiguration.DaysSinceLastExposureThreshold;
                configuration.InfectiousnessForDaysSinceOnsetOfSymptoms = infectiousnessForDaysSinceOnsetOfSymptomsNSDict;
                configuration.InfectiousnessHighWeight = appleExposureConfiguration.InfectiousnessHighWeight;
                configuration.InfectiousnessStandardWeight = appleExposureConfiguration.InfectiousnessStandardWeight;
                configuration.ReportTypeConfirmedClinicalDiagnosisWeight = appleExposureConfiguration.ReportTypeConfirmedClinicalDiagnosisWeight;
                configuration.ReportTypeConfirmedTestWeight = appleExposureConfiguration.ReportTypeConfirmedTestWeight;
                configuration.ReportTypeRecursiveWeight = appleExposureConfiguration.ReportTypeRecursiveWeight;
                configuration.ReportTypeSelfReportedWeight = appleExposureConfiguration.ReportTypeSelfReportedWeight;
                configuration.ReportTypeNoneMap = (ENDiagnosisReportType)Enum.ToObject(typeof(ENDiagnosisReportType), appleExposureConfiguration.ReportTypeNoneMap);
                configuration.AttenuationLevelValues = appleExposureConfiguration.AttenuationLevelValues;
                configuration.DaysSinceLastExposureLevelValues = appleExposureConfiguration.DaysSinceLastExposureLevelValues;
                configuration.DurationLevelValues = appleExposureConfiguration.DurationLevelValues;
                configuration.TransmissionRiskLevelValues = appleExposureConfiguration.TransmissionRiskLevelValues;
                // Metadata = appleExposureConfigurationV1.Metadata;
                configuration.MinimumRiskScore = appleExposureConfiguration.MinimumRiskScore;
            }

            // iOS 12.5
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 6)
                && !UIDevice.CurrentDevice.CheckSystemVersion(13, 5)
                && ObjCRuntime.Class.GetHandle("ENManager") != null)
            {
                Logger.D("Set configuration values for iOS 12.5.");
                configuration.AttenuationDurationThresholds = appleExposureConfiguration.AttenuationDurationThreshold;
                configuration.MinimumRiskScoreFullRange = appleExposureConfiguration.MinimumRiskScoreFullRange;

                configuration.ImmediateDurationWeight = appleExposureConfiguration.ImmediateDurationWeight;
                configuration.MediumDurationWeight = appleExposureConfiguration.MediumDurationWeight;
                configuration.NearDurationWeight = appleExposureConfiguration.NearDurationWeight;
                configuration.OtherDurationWeight = appleExposureConfiguration.OtherDurationWeight;
                configuration.DaysSinceLastExposureThreshold = appleExposureConfiguration.DaysSinceLastExposureThreshold;
                configuration.InfectiousnessForDaysSinceOnsetOfSymptoms = infectiousnessForDaysSinceOnsetOfSymptomsNSDict;
                configuration.InfectiousnessHighWeight = appleExposureConfiguration.InfectiousnessHighWeight;
                configuration.InfectiousnessStandardWeight = appleExposureConfiguration.InfectiousnessStandardWeight;
                configuration.ReportTypeConfirmedClinicalDiagnosisWeight = appleExposureConfiguration.ReportTypeConfirmedClinicalDiagnosisWeight;
                configuration.ReportTypeConfirmedTestWeight = appleExposureConfiguration.ReportTypeConfirmedTestWeight;
                configuration.ReportTypeRecursiveWeight = appleExposureConfiguration.ReportTypeRecursiveWeight;
                configuration.ReportTypeSelfReportedWeight = appleExposureConfiguration.ReportTypeSelfReportedWeight;
                configuration.ReportTypeNoneMap = (ENDiagnosisReportType)Enum.ToObject(typeof(ENDiagnosisReportType), appleExposureConfiguration.ReportTypeNoneMap);
                configuration.AttenuationLevelValues = appleExposureConfiguration.AttenuationLevelValues;
                configuration.DaysSinceLastExposureLevelValues = appleExposureConfiguration.DaysSinceLastExposureLevelValues;
                configuration.DurationLevelValues = appleExposureConfiguration.DurationLevelValues;
                configuration.TransmissionRiskLevelValues = appleExposureConfiguration.TransmissionRiskLevelValues;
                // Metadata = appleExposureConfigurationV1.Metadata;
                configuration.MinimumRiskScore = appleExposureConfiguration.MinimumRiskScore;
            }

            return configuration;
        }

        private NSDictionary<NSNumber, NSNumber> GetInfectiousnessForDaysSinceOnsetOfSymptomsNSDict(IDictionary<int, int> infectiousnessForDaysSinceOnsetOfSymptoms)
        {
            var pairs = infectiousnessForDaysSinceOnsetOfSymptoms.Keys.Zip(infectiousnessForDaysSinceOnsetOfSymptoms.Values, (k, v) => new NSNumber[] { k, v });
            NSMutableDictionary<NSNumber, NSNumber> infectiousnessForDaysSinceOnsetOfSymptomsMutableDict = new NSMutableDictionary<NSNumber, NSNumber>();
            foreach (NSNumber[] pair in pairs)
            {
                infectiousnessForDaysSinceOnsetOfSymptomsMutableDict.Add(pair[0], pair[1]);
            }

            return NSDictionary<NSNumber, NSNumber>.FromObjectsAndKeys(
                            infectiousnessForDaysSinceOnsetOfSymptomsMutableDict.Values,
                            infectiousnessForDaysSinceOnsetOfSymptomsMutableDict.Keys,
                            (nint)infectiousnessForDaysSinceOnsetOfSymptomsMutableDict.Count);
        }

        //public override Task RequestPreAuthorizedTemporaryExposureKeyHistory()
        //{
        //    throw new NotImplementedException();
        //}

        //public override Task RequestPreAuthorizedTemporaryExposureKeyRelease()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
