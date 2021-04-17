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
using Android.Gms.Common.Apis;

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
                    return;
                }

                ExposureNotificationClient? enClient = null;
                if (context.ApplicationContext is ExposureNotificationHandler)
                {
                    var exposureNotificationHandler = (ExposureNotificationHandler)context.ApplicationContext;
                    enClient = exposureNotificationHandler.GetEnClient();
                }

                if (enClient == null)
                {
                    return;
                }

                var action = intent.Action;
                switch (action)
                {
                    case ACTION_EXPOSURE_STATE_UPDATE:
                        var exposureWindows = await enClient.GetExposureWindowsAsync();
                        Handler.ExposureDetected(exposureWindows);
                        break;
                    case ACTION_EXPOSURE_NOT_FOUND:
                        Handler.ExposureNotDetected();
                        break;
                }
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

        public override async Task<bool> IsEnabledAsync()
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
            var files = keyFiles.Select(f => new File(f)).ToList();
            await EnClient.ProvideDiagnosisKeysAsync(new DiagnosisKeyFileProvider(files));
        }

        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration)
        {
            await ProvideDiagnosisKeys(keyFiles, configuration, Guid.NewGuid().ToString());
        }

        public override async Task<List<IExposureWindow>> GetExposureWindowsAsync()
        {
            var exposureWindows = await EnClient.GetExposureWindowsAsync();

            return exposureWindows.Select(ew => (IExposureWindow)new ExposureWindow(ew)).ToList();

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
            return new Android.Gms.Nearby.ExposureNotification.ExposureConfiguration.ExposureConfigurationBuilder()
                .SetAttenuationScores(exposureConfiguration.AttenuationScores)
                .SetAttenuationWeight(exposureConfiguration.AttenuationWeight)
                .SetDaysSinceLastExposureScores(exposureConfiguration.DaysSinceLastExposureScores)
                .SetDaysSinceLastExposureWeight(exposureConfiguration.DaysSinceLastExposureWeight)
                .SetDurationAtAttenuationThresholds(exposureConfiguration.DurationAtAttenuationThresholds)
                .SetDurationScores(exposureConfiguration.DurationScores)
                .SetDurationWeight(exposureConfiguration.DurationWeight)
                .SetMinimumRiskScore(exposureConfiguration.MinimumRiskScore)
                .SetTransmissionRiskScores(exposureConfiguration.TransmissionRiskScores)
                .SetTransmissionRiskWeight(exposureConfiguration.TransmissionRiskWeight)
                .Build();
        }
#pragma warning restore CS0618 // Type or member is obsolete

    }
}
