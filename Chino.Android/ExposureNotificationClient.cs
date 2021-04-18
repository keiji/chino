using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;

using Nearby = Android.Gms.Nearby.NearbyClass;
using Java.IO;
using Android.Gms.Nearby.ExposureNotification;
using System.Threading.Tasks;
using System.Linq;

[assembly: UsesFeature("android.hardware.bluetooth_le", Required = true)]
[assembly: UsesFeature("android.hardware.bluetooth")]
[assembly: UsesPermission(Android.Manifest.Permission.Bluetooth)]

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationClient
    public class ExposureNotificationClient : AbsExposureNotificationClient
    {
        public const string PERMISSION_EXPOSURE_CALLBACK = "com.google.android.gms.nearby.exposurenotification.EXPOSURE_CALLBACK";
        public const string ACTION_EXPOSURE_STATE_UPDATE = "com.google.android.gms.exposurenotification.ACTION_EXPOSURE_STATE_UPDATE";
        public const string ACTION_EXPOSURE_NOT_FOUND = "com.google.android.gms.exposurenotification.ACTION_EXPOSURE_NOT_FOUND";
        public const string SERVICE_STATE_UPDATED = "com.google.android.gms.exposurenotification.SERVICE_STATE_UPDATED";

        public const string EXTRA_TOKEN = "com.google.android.gms.exposurenotification.EXTRA_TOKEN";

        [BroadcastReceiver(
            Exported = true,
            Permission = PERMISSION_EXPOSURE_CALLBACK
            )]
        [IntentFilter(new[] { ACTION_EXPOSURE_STATE_UPDATE, ACTION_EXPOSURE_NOT_FOUND })]
        [Preserve]
        public class ExposureStateBroadcastReceiver : BroadcastReceiver
        {
            public override async void OnReceive(Context context, Intent intent)
            {
                if (Handler == null)
                {
                    Logger.E("ExposureStateBroadcastReceiver: Handler is not set.");
                    return;
                }

                ExposureNotificationClient? enClient = null;
                if (context.ApplicationContext is IExposureNotificationHandler exposureNotificationHandler)
                {
                    enClient = (ExposureNotificationClient)exposureNotificationHandler.GetEnClient();
                }

                if (enClient == null)
                {
                    Logger.E("ExposureStateBroadcastReceiver: enClient is null.");
                    return;
                }

                bool v1 = intent.HasExtra(EXTRA_TOKEN);

                string varsionStr = v1 ? "1" : "2";
                Logger.D($"EN version {varsionStr}");

                var action = intent.Action;
                switch (action)
                {
                    case ACTION_EXPOSURE_STATE_UPDATE:
                        Logger.D($"ACTION_EXPOSURE_STATE_UPDATE");
                        if (v1)
                        {
                            string token = intent.GetStringExtra(EXTRA_TOKEN);
                            await GetExposureV1Async(enClient.EnClient, token);
                        }
                        else
                        {
                            await GetExposureV2Async(enClient.EnClient);
                        }
                        break;
                    case ACTION_EXPOSURE_NOT_FOUND:
                        Logger.D($"ACTION_EXPOSURE_NOT_FOUND");
                        Handler.ExposureNotDetected();
                        break;
                }
            }

            private async Task GetExposureV1Async(IExposureNotificationClient enClient, string token)
            {
                Logger.D($"GetExposureV1Async");

                IList<Android.Gms.Nearby.ExposureNotification.ExposureInformation> eis = await enClient.GetExposureInformationAsync(token);
                List<IExposureWindow> exposureInformations = eis.Select(ei => (IExposureWindow)new ExposureInformation(ei)).ToList();
                Handler.ExposureDetected(exposureInformations);
            }

            private async Task GetExposureV2Async(IExposureNotificationClient enClient)
            {
                Logger.D($"GetExposureV2Async");

                IList<Android.Gms.Nearby.ExposureNotification.ExposureWindow> ews = await enClient.GetExposureWindowsAsync();
                List<IExposureWindow> exposureWindows = ews.Select(ew => (IExposureWindow)new ExposureWindow(ew)).ToList();
                Handler.ExposureDetected(exposureWindows);
            }

        }

#nullable enable
        private IExposureNotificationClient? EnClient = null;
#nullable disable

        public void Init(Context applicationContext)
        {
            EnClient = Nearby.GetExposureNotificationClient(applicationContext);
        }

        public override async Task Start()
        {
            await EnClient.StartAsync();
        }

        public override async Task Stop()
        {
            await EnClient.StopAsync();
        }

        public override async Task<bool> IsEnabled()
        {
            return await EnClient.IsEnabledAsync();
        }

        public override async Task<long> GetVersion()
        {
            return await EnClient.GetVersionAsync();
        }

        public override async Task<IExposureNotificationStatus> GetStatus()
        {
            return new ExposureNotificationStatus(await EnClient.GetStatusAsync());
        }

        public override async Task ProvideDiagnosisKeys(List<string> keyFiles)
        {
            Logger.D($"DiagnosisKey {keyFiles.Count}");

            if (keyFiles.Count == 0)
            {
                Logger.D($"No DiagnosisKey found.");
                return;
            }

            foreach (string path in keyFiles)
            {
                Logger.D($"{path}");
            }
            var files = keyFiles.Select(f => new File(f)).ToList();
            await EnClient.ProvideDiagnosisKeysAsync(new DiagnosisKeyFileProvider(files));
        }

        public override async Task ProvideDiagnosisKeys(List<string> keyFiles, ExposureConfiguration configuration)
        {
            await ProvideDiagnosisKeys(keyFiles, configuration, Guid.NewGuid().ToString());
        }

        public override async Task<List<ITemporaryExposureKey>> GetTemporaryExposureKeyHistory()
        {
            var teks = await EnClient.GetTemporaryExposureKeyHistoryAsync();

            return teks.Select(tek => (ITemporaryExposureKey)new TemporaryExposureKey(tek)).ToList();

        }

#pragma warning disable CS0618 // Type or member is obsolete
        public override async Task ProvideDiagnosisKeys(List<string> keyFiles, ExposureConfiguration configuration, string token)
        {
            var files = keyFiles.Select(f => new File(f)).ToList();
            await EnClient.ProvideDiagnosisKeysAsync(files, Convert(configuration), token);
        }
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
        private Android.Gms.Nearby.ExposureNotification.ExposureConfiguration Convert(ExposureConfiguration exposureConfiguration)
        {
            ExposureConfiguration.GoogleExposureConfiguration googleExposureConfiguration = exposureConfiguration.GoogleExposureConfig;

            return new Android.Gms.Nearby.ExposureNotification.ExposureConfiguration.ExposureConfigurationBuilder()
                .SetAttenuationScores(googleExposureConfiguration.AttenuationScores)
                .SetAttenuationWeight(googleExposureConfiguration.AttenuationWeight)
                .SetDaysSinceLastExposureScores(googleExposureConfiguration.DaysSinceLastExposureScores)
                .SetDaysSinceLastExposureWeight(googleExposureConfiguration.DaysSinceLastExposureWeight)
                .SetDurationAtAttenuationThresholds(googleExposureConfiguration.DurationAtAttenuationThresholds)
                .SetDurationScores(googleExposureConfiguration.DurationScores)
                .SetDurationWeight(googleExposureConfiguration.DurationWeight)
                .SetMinimumRiskScore(googleExposureConfiguration.MinimumRiskScore)
                .SetTransmissionRiskScores(googleExposureConfiguration.TransmissionRiskScores)
                .SetTransmissionRiskWeight(googleExposureConfiguration.TransmissionRiskWeight)
                .Build();
        }
#pragma warning restore CS0618 // Type or member is obsolete

    }
}
