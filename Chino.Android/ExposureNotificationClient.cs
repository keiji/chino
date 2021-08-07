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

using AndroidTemporaryExposureKey = Android.Gms.Nearby.ExposureNotification.TemporaryExposureKey;
using AndroidExposureSummary = Android.Gms.Nearby.ExposureNotification.ExposureSummary;
using AndroidDailySummary = Android.Gms.Nearby.ExposureNotification.DailySummary;
using AndroidExposureInformation = Android.Gms.Nearby.ExposureNotification.ExposureInformation;
using AndroidExposureWindow = Android.Gms.Nearby.ExposureNotification.ExposureWindow;

using Logger = Chino.ChinoLogger;
using Android.Gms.Common.Apis;
using Android.App.Job;
using Newtonsoft.Json;
using Android.OS;

[assembly: UsesFeature("android.hardware.bluetooth_le", Required = true)]
[assembly: UsesFeature("android.hardware.bluetooth")]
[assembly: UsesPermission(Android.Manifest.Permission.Bluetooth)]

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationClient
    public class ExposureNotificationClient : AbsExposureNotificationClient
    {
        private const string PERMISSION_EXPOSURE_CALLBACK = "com.google.android.gms.nearby.exposurenotification.EXPOSURE_CALLBACK";
        private const string ACTION_EXPOSURE_STATE_UPDATED = "com.google.android.gms.exposurenotification.ACTION_EXPOSURE_STATE_UPDATED";
        private const string ACTION_EXPOSURE_NOT_FOUND = "com.google.android.gms.exposurenotification.ACTION_EXPOSURE_NOT_FOUND";
        private const string ACTION_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED = "com.google.android.gms.exposurenotification.ACTION_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED";
        private const string SERVICE_STATE_UPDATED = "com.google.android.gms.exposurenotification.SERVICE_STATE_UPDATED";

        private const string PERMISSION_BIND_JOB_SERVICE = "android.permission.BIND_JOB_SERVICE";

        private const string EXTRA_TOKEN = "com.google.android.gms.exposurenotification.EXTRA_TOKEN";
        private const string EXTRA_EXPOSURE_SUMMARY = "com.google.android.gms.exposurenotification.EXTRA_EXPOSURE_SUMMARY";
        private const string EXTRA_TEMPORARY_EXPOSURE_KEY_LIST = "com.google.android.gms.exposurenotification.EXTRA_TEMPORARY_EXPOSURE_KEY_LIST";

        [BroadcastReceiver(
            Exported = true,
            Permission = PERMISSION_EXPOSURE_CALLBACK
            )]
        [IntentFilter(new[] { ACTION_EXPOSURE_STATE_UPDATED, ACTION_EXPOSURE_NOT_FOUND, ACTION_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED })]
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

                var action = intent.Action;
                Logger.D($"Intent Action {action}");

                var pendingResult = GoAsync();

                try
                {
                    switch (action)
                    {
                        case ACTION_EXPOSURE_STATE_UPDATED:
                            Logger.D($"ACTION_EXPOSURE_STATE_UPDATED");
                            bool v1 = intent.HasExtra(EXTRA_EXPOSURE_SUMMARY);

                            string varsionStr = v1 ? "1" : "2";
                            Logger.D($"EN version {varsionStr}");

                            if (v1)
                            {
                                string token = intent.GetStringExtra(EXTRA_TOKEN);
                                ExposureDetectedV1Job.Enqueue(context, token);
                            }
                            else
                            {
                                ExposureDetectedV2Job.Enqueue(context);
                            }
                            break;
                        case ACTION_EXPOSURE_NOT_FOUND:
                            Logger.D($"ACTION_EXPOSURE_NOT_FOUND");
                            ExposureNotDetectedJob.Enqueue(context);
                            break;
                        case ACTION_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED:
                            Logger.D($"ACTION_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED");
                            IList<TemporaryExposureKey> temporaryExposureKeys = await ConvertToITemporaryExposureKeyList(intent);
                            TemporaryExposureKeyReleasedJob.Enqueue(context, temporaryExposureKeys);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.E($"Exception occurred: {e}");
                }
                finally
                {
                    pendingResult.Finish();
                }
            }

#pragma warning disable CS0612,CS0618 // Type or member is obsolete
            [Service(Permission = PERMISSION_BIND_JOB_SERVICE)]
            [Preserve]
            class ExposureDetectedV1Job : JobService
            {
                private const int JOB_ID = 0x01;
                private const string EXTRA_TOKEN = "extra_token";

                public static void Enqueue(Context context, string token)
                {
                    PersistableBundle bundle = new PersistableBundle();
                    bundle.PutString(EXTRA_TOKEN, token);

                    JobInfo jobInfo = new JobInfo.Builder(
                        JOB_ID,
                        new ComponentName(context, Java.Lang.Class.FromType(typeof(ExposureDetectedV1Job))))
                        .SetExtras(bundle)
                        .SetOverrideDeadline(0)
                        .Build();
                    JobScheduler jobScheduler = (JobScheduler)context.GetSystemService(JobSchedulerService);
                    int result = jobScheduler.Schedule(jobInfo);
                    if (result == JobScheduler.ResultSuccess)
                    {
                        Logger.D("ExposureDetectedV1Job scheduled");
                    }
                    else if (result == JobScheduler.ResultFailure)
                    {
                        Logger.D("ExposureDetectedV1Job schedule failed");
                    }
                }

                public override bool OnStartJob(JobParameters @params)
                {
                    ExposureNotificationClient? enClient = null;
                    if (ApplicationContext is IExposureNotificationHandler exposureNotificationHandler)
                    {
                        enClient = (ExposureNotificationClient)exposureNotificationHandler.GetEnClient();
                    }

                    if (enClient == null)
                    {
                        Logger.E("ExposureStateBroadcastReceiver: enClient is null.");
                        return false;
                    }

                    var token = @params.Extras.GetString(EXTRA_TOKEN);

                    var (exposureSummary, exposureInformations) = GetExposureV1Async(enClient, token).GetAwaiter().GetResult();
                    Handler.ExposureDetected(exposureSummary, exposureInformations);

                    JobFinished(@params, false);

                    return true;
                }

                public override bool OnStopJob(JobParameters @params) => false;

                private async Task<(ExposureSummary, List<ExposureInformation> exposureInformations)> GetExposureV1Async(
                    ExposureNotificationClient enClient,
                    string token
                    )
                {
                    Logger.D($"GetExposureV1Async");

                    try
                    {
                        AndroidExposureSummary exposureSummary = await enClient.EnClient.GetExposureSummaryAsync(token);

                        IList<AndroidExposureInformation> eis = await enClient.EnClient.GetExposureInformationAsync(token);
                        List<ExposureInformation> exposureInformations = eis.Select(ei => (ExposureInformation)new PlatformExposureInformation(ei)).ToList();

                        return (new PlatformExposureSummary(exposureSummary), exposureInformations);
                    }
                    catch (ApiException exception)
                    {
                        if (exception.IsENException())
                        {
                            throw exception.ToENException();
                        }
                        throw exception;
                    }
                }
            }
#pragma warning restore CS06122,CS0618 // Type or member is obsolete

            [Service(Permission = PERMISSION_BIND_JOB_SERVICE)]
            [Preserve]
            class ExposureDetectedV2Job : JobService
            {
                private const int JOB_ID = 0x02;

                public static void Enqueue(Context context)
                {
                    PersistableBundle bundle = new PersistableBundle();
                    JobInfo jobInfo = new JobInfo.Builder(
                        JOB_ID,
                        new ComponentName(context, Java.Lang.Class.FromType(typeof(ExposureDetectedV2Job))))
                        .SetExtras(bundle)
                        .SetOverrideDeadline(0)
                        .Build();
                    JobScheduler jobScheduler = (JobScheduler)context.GetSystemService(JobSchedulerService);
                    int result = jobScheduler.Schedule(jobInfo);
                    if (result == JobScheduler.ResultSuccess)
                    {
                        Logger.D("ExposureDetectedV2Job scheduled");
                    }
                    else if (result == JobScheduler.ResultFailure)
                    {
                        Logger.D("ExposureDetectedV2Job schedule failed");
                    }
                }

                public override bool OnStartJob(JobParameters @params)
                {
                    ExposureNotificationClient? enClient = null;
                    if (ApplicationContext is IExposureNotificationHandler exposureNotificationHandler)
                    {
                        enClient = (ExposureNotificationClient)exposureNotificationHandler.GetEnClient();
                    }

                    if (enClient == null)
                    {
                        Logger.E("ExposureStateBroadcastReceiver: enClient is null.");
                        return false;
                    }

                    var (dailySummaries, exposureWindows) = GetExposureV2Async(enClient).GetAwaiter().GetResult();

                    Handler.ExposureDetected(
                        dailySummaries,
                        exposureWindows
                        );
                    JobFinished(@params, false);

                    return true;
                }

                public override bool OnStopJob(JobParameters @params) => false;

                private async Task<(List<DailySummary> dailySummaries, List<ExposureWindow> exposureWindows)> GetExposureV2Async(
                    ExposureNotificationClient enClient
                    )
                {
                    Logger.D($"GetExposureV2Async");

                    try
                    {
                        IList<AndroidDailySummary> dss = await enClient.EnClient.GetDailySummariesAsync(
                            enClient.ExposureConfiguration.GoogleDailySummariesConfig.ToAndroidDailySummariesConfig()
                            );
                        List<DailySummary> dailySummaries = dss.Select(ds => (DailySummary)new PlatformDailySummary(ds)).ToList();

                        Print(dailySummaries);

                        IList<AndroidExposureWindow> ews = await enClient.EnClient.GetExposureWindowsAsync();
                        List<ExposureWindow> exposureWindows = ews.Select(ew => (ExposureWindow)new PlatformExposureWindow(ew)).ToList();

                        Logger.D(exposureWindows);

                        return (dailySummaries, exposureWindows);
                    }
                    catch (ApiException exception)
                    {
                        if (exception.IsENException())
                        {
                            throw exception.ToENException();
                        }
                        throw exception;
                    }
                }
            }

            private static void Print(IList<DailySummary> dailySummaries)
            {
                Logger.D($"dailySummaries - {dailySummaries.Count()}");

                foreach (var d in dailySummaries)
                {
                    Logger.D($"MaximumScore: {d.DaySummary.MaximumScore}");
                    Logger.D($"ScoreSum: {d.DaySummary.ScoreSum}");
                    Logger.D($"WeightedDurationSum: {d.DaySummary.WeightedDurationSum}");
                }
            }

            private Task<List<TemporaryExposureKey>> ConvertToITemporaryExposureKeyList(Intent intent)
            {
                return Task.Run(() => intent.GetParcelableArrayListExtra(EXTRA_TEMPORARY_EXPOSURE_KEY_LIST)
                            .Cast<AndroidTemporaryExposureKey>()
                            .Select(tek => (TemporaryExposureKey)new PlatformTemporaryExposureKey(tek))
                            .ToList()
                            );
            }

            [Service(Permission = PERMISSION_BIND_JOB_SERVICE)]
            [Preserve]
            class ExposureNotDetectedJob : JobService
            {
                private const int JOB_ID = 0x03;

                public static void Enqueue(Context context)
                {
                    PersistableBundle bundle = new PersistableBundle();

                    JobInfo jobInfo = new JobInfo.Builder(
                        JOB_ID,
                        new ComponentName(context, Java.Lang.Class.FromType(typeof(ExposureNotDetectedJob))))
                        .SetExtras(bundle)
                        .SetOverrideDeadline(0)
                        .Build();
                    JobScheduler jobScheduler = (JobScheduler)context.GetSystemService(JobSchedulerService);
                    int result = jobScheduler.Schedule(jobInfo);
                    if (result == JobScheduler.ResultSuccess)
                    {
                        Logger.D("ExposureNotDetectedJob scheduled");
                    }
                    else if (result == JobScheduler.ResultFailure)
                    {
                        Logger.D("ExposureNotDetectedJob schedule failed");
                    }
                }

                public override bool OnStartJob(JobParameters @params)
                {
                    Handler.ExposureNotDetected();

                    JobFinished(@params, false);

                    return true;
                }

                public override bool OnStopJob(JobParameters @params) => false;
            }

            [Service(Permission = PERMISSION_BIND_JOB_SERVICE)]
            [Preserve]
            class TemporaryExposureKeyReleasedJob : JobService
            {
                private const int JOB_ID = 0x04;
                private const string EXTRA_TEMPORARY_EXPOSURE_KEYS = "extra_temporary_exposure_keys";

                public static void Enqueue(Context context, IList<TemporaryExposureKey> temporaryExposureKeys)
                {
                    var serializedJsonTemporaryExposureKeys = JsonConvert.SerializeObject(temporaryExposureKeys);

                    PersistableBundle bundle = new PersistableBundle();
                    bundle.PutString(EXTRA_TEMPORARY_EXPOSURE_KEYS, serializedJsonTemporaryExposureKeys);

                    JobInfo jobInfo = new JobInfo.Builder(
                        JOB_ID,
                        new ComponentName(context, Java.Lang.Class.FromType(typeof(TemporaryExposureKeyReleasedJob))))
                        .SetExtras(bundle)
                        .SetOverrideDeadline(0)
                        .Build();
                    JobScheduler jobScheduler = (JobScheduler)context.GetSystemService(JobSchedulerService);
                    int result = jobScheduler.Schedule(jobInfo);
                    if (result == JobScheduler.ResultSuccess)
                    {
                        Logger.D("TemporaryExposureKeyReleasedJob scheduled");
                    }
                    else if (result == JobScheduler.ResultFailure)
                    {
                        Logger.D("TemporaryExposureKeyReleasedJob schedule failed");
                    }
                }

                public override bool OnStartJob(JobParameters @params)
                {
                    var serializedJsonTemporaryExposureKeys = @params.Extras.GetString(EXTRA_TEMPORARY_EXPOSURE_KEYS);
                    IList<TemporaryExposureKey> temporaryExposureKeys = JsonConvert.DeserializeObject<List<TemporaryExposureKey>>(serializedJsonTemporaryExposureKeys);

                    Handler.TemporaryExposureKeyReleased(temporaryExposureKeys);

                    JobFinished(@params, false);

                    return true;
                }

                public override bool OnStopJob(JobParameters @params) => false;
            }
        }

#nullable enable
        private IExposureNotificationClient? EnClient = null;
#nullable disable

        public void Init(Context applicationContext)
        {
            try
            {
                EnClient = Nearby.GetExposureNotificationClient(applicationContext);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }

        private void CheckInitialized()
        {
            if (EnClient == null)
            {
                Logger.E("Init method must be called first.");
                throw new UnInitializedException("Init method must be called first.");
            }
        }

        public override async Task StartAsync()
        {
            CheckInitialized();

            try
            {
                await EnClient.StartAsync();
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }

        public override async Task StopAsync()
        {
            CheckInitialized();

            try
            {
                await EnClient.StopAsync();
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }

        public override async Task<bool> IsEnabledAsync()
        {
            CheckInitialized();

            try
            {
                return await EnClient.IsEnabledAsync();
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }

        public override async Task<long> GetVersionAsync()
        {
            CheckInitialized();

            try
            {
                return await EnClient.GetVersionAsync();
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }

        public override async Task<IList<ExposureNotificationStatus>> GetStatusesAsync()
        {
            CheckInitialized();

            try
            {
                var statuses = await EnClient.GetStatusAsync();
                return statuses.Select(status => status.ToExposureNotificationStatus()).ToList();
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }

        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles)
        {
            await ProvideDiagnosisKeysAsync(keyFiles, new ExposureConfiguration()
            {
                GoogleExposureConfig = new ExposureConfiguration.GoogleExposureConfiguration(),
                GoogleDailySummariesConfig = new DailySummariesConfig()
            });
        }

        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration)
        {
            CheckInitialized();

            if (Handler == null)
            {
                Logger.E("ExposureNotificationClient: Handler is not set.");
                return;
            }

            Logger.D($"DiagnosisKey {keyFiles.Count}");

            if (keyFiles.Count == 0)
            {
                Logger.D($"No DiagnosisKey found.");
                return;
            }

            ExposureConfiguration = configuration;
            ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration googleDiagnosisKeysDataMappingConfig = ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfig;
            IDictionary<int, Infectiousness> InfectiousnessForDaysSinceOnsetOfSymptoms
                = googleDiagnosisKeysDataMappingConfig.InfectiousnessForDaysSinceOnsetOfSymptoms;

            IDictionary<Java.Lang.Integer, Java.Lang.Integer> daysSinceOnsetToInfectiousness = new Dictionary<Java.Lang.Integer, Java.Lang.Integer>();
            foreach (var key in InfectiousnessForDaysSinceOnsetOfSymptoms.Keys)
            {
                var value = InfectiousnessForDaysSinceOnsetOfSymptoms[key];
                daysSinceOnsetToInfectiousness.Add(new Java.Lang.Integer(key), new Java.Lang.Integer((int)value));
            }

            DiagnosisKeysDataMapping diagnosisKeysDataMapping = new DiagnosisKeysDataMapping.DiagnosisKeysDataMappingBuilder()
                .SetDaysSinceOnsetToInfectiousness(daysSinceOnsetToInfectiousness)
                .SetInfectiousnessWhenDaysSinceOnsetMissing((int)googleDiagnosisKeysDataMappingConfig.InfectiousnessWhenDaysSinceOnsetMissing)
                .SetReportTypeWhenMissing((int)googleDiagnosisKeysDataMappingConfig.ReportTypeWhenMissing)
                .Build();

            try
            {
                await EnClient.SetDiagnosisKeysDataMappingAsync(diagnosisKeysDataMapping);

                var files = keyFiles.Select(f => new File(f)).ToList();
                DiagnosisKeyFileProvider diagnosisKeyFileProvider = new DiagnosisKeyFileProvider(files);
                await EnClient.ProvideDiagnosisKeysAsync(diagnosisKeyFileProvider);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }

        public override async Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
        {
            try
            {
                var teks = await EnClient.GetTemporaryExposureKeyHistoryAsync();
                return teks.Select(tek => (TemporaryExposureKey)new PlatformTemporaryExposureKey(tek)).ToList();
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration, string token)
        {
            CheckInitialized();

            if (Handler == null)
            {
                Logger.E("ExposureNotificationClient: Handler is not set.");
                return;
            }

            Logger.D($"DiagnosisKey {keyFiles.Count}");

            if (keyFiles.Count == 0)
            {
                Logger.D($"No DiagnosisKey found.");
                return;
            }

            ExposureConfiguration = configuration;

            var files = keyFiles.Select(f => new File(f)).ToList();

            try
            {
                await EnClient.ProvideDiagnosisKeysAsync(files, configuration.ToAndroidExposureConfiguration(), token);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public override async Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
        {
            CheckInitialized();

            try
            {
                await EnClient.RequestPreAuthorizedTemporaryExposureKeyHistoryAsync();
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }
        public override async Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
        {
            CheckInitialized();

            try
            {
                await EnClient.RequestPreAuthorizedTemporaryExposureKeyReleaseAsync();
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw exception;
            }
        }
    }

    class UnInitializedException : Exception
    {
        public UnInitializedException(string message) : base(message)
        {
        }
    }
}
