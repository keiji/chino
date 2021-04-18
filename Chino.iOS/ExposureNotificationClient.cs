using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExposureNotifications;
using Foundation;
using UIKit;

namespace Chino
{
    public class ExposureNotificationClient : AbsExposureNotificationClient
    {

        public static readonly ExposureNotificationClient Shared = new ExposureNotificationClient();


        private readonly ENManager EnManager = new ENManager();

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

        public async override Task Start()
        {
            await EnManager.SetExposureNotificationEnabledAsync(true);
        }

        public async override Task Stop()
        {
            await EnManager.SetExposureNotificationEnabledAsync(false);
        }

        public override Task<bool> IsEnabled()
        {
            throw new NotImplementedException();
        }

        public override Task<long> GetVersion()
        {
            throw new NotImplementedException();
        }

        public override Task<IExposureNotificationStatus> GetStatus()
        {
            return Task.Run(() =>
             {
                 return (IExposureNotificationStatus)new ExposureNotificationStatus(EnManager.ExposureNotificationStatus);
             });
        }

        public async override Task<List<ITemporaryExposureKey>> GetTemporaryExposureKeyHistory()
        {
            ENTemporaryExposureKey[] teks = await EnManager.GetDiagnosisKeysAsync();
            return teks.Select(tek => (ITemporaryExposureKey)new TemporaryExposureKey(tek)).ToList();
        }

        public override Task ProvideDiagnosisKeys(List<string> keyFiles)
        {
            throw new NotImplementedException();
        }

        public async override Task ProvideDiagnosisKeys(List<string> keyFiles, ExposureConfiguration configuration)
        {
            NSUrl[] urls = keyFiles.Select(keyFile => new NSUrl(keyFile)).ToArray();

            ExposureConfiguration.IAppleExposureConfiguration appleExposureConfiguration = configuration.AppleExposureConfiguration;
            ENExposureConfiguration exposureConfiguration = GetExposureConfiguration(appleExposureConfiguration);

            ENExposureDetectionSummary summary = await EnManager.DetectExposuresAsync(exposureConfiguration, urls);

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

        private ENExposureConfiguration GetExposureConfiguration(ExposureConfiguration.IAppleExposureConfiguration appleExposureConfiguration)
        {
            IDictionary<int, int> infectiousnessForDaysSinceOnsetOfSymptoms = appleExposureConfiguration.InfectiousnessForDaysSinceOnsetOfSymptoms;
            NSNumber[] infectiousnessForDaysSinceOnsetOfSymptomsKeys = infectiousnessForDaysSinceOnsetOfSymptoms.Keys.Select(key => (NSNumber)key).ToArray();
            NSNumber[] infectiousnessForDaysSinceOnsetOfSymptomsValues = infectiousnessForDaysSinceOnsetOfSymptoms.Values.Select(key => (NSNumber)key).ToArray();

            return new ENExposureConfiguration()
            {
                AttenuationDurationThresholds = appleExposureConfiguration.AttenuationDurationThreshold,
                ImmediateDurationWeight = appleExposureConfiguration.ImmediateDurationWeight,
                MediumDurationWeight = appleExposureConfiguration.MediumDurationWeight,
                NearDurationWeight = appleExposureConfiguration.NearDurationWeight,
                OtherDurationWeight = appleExposureConfiguration.OtherDurationWeight,
                DaysSinceLastExposureThreshold = appleExposureConfiguration.DaysSinceLastExposureThreshold,
                InfectiousnessForDaysSinceOnsetOfSymptoms = (NSDictionary<NSNumber, NSNumber>)NSDictionary.FromObjectsAndKeys(
                    infectiousnessForDaysSinceOnsetOfSymptomsKeys,
                    infectiousnessForDaysSinceOnsetOfSymptomsKeys
                    ),
                InfectiousnessHighWeight = appleExposureConfiguration.InfectiousnessHighWeight,
                InfectiousnessStandardWeight = appleExposureConfiguration.InfectiousnessStandardWeight,
                ReportTypeConfirmedClinicalDiagnosisWeight = appleExposureConfiguration.ReportTypeConfirmedClinicalDiagnosisWeight,
                ReportTypeConfirmedTestWeight = appleExposureConfiguration.ReportTypeConfirmedTestWeight,
                ReportTypeRecursiveWeight = appleExposureConfiguration.ReportTypeRecursiveWeight,
                ReportTypeSelfReportedWeight = appleExposureConfiguration.ReportTypeSelfReportedWeight,
                ReportTypeNoneMap = (ENDiagnosisReportType)Enum.ToObject(typeof(ENDiagnosisReportType), appleExposureConfiguration.ReportTypeNoneMap)
            };
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
