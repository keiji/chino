using System;
using Android.App;
using Android.Content;
using Android.Runtime;

using AndroidExposureSummary = Android.Gms.Nearby.ExposureNotification.ExposureSummary;
using AndroidDailySummary = Android.Gms.Nearby.ExposureNotification.DailySummary;
using AndroidExposureInformation = Android.Gms.Nearby.ExposureNotification.ExposureInformation;
using AndroidExposureWindow = Android.Gms.Nearby.ExposureNotification.ExposureWindow;

using Logger = Chino.ChinoLogger;
using Android.App.Job;
using Android.OS;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Android.Gms.Common.Apis;

namespace Chino.Android.Google
{
    [BroadcastReceiver(
        Exported = true,
        Permission = PERMISSION_EXPOSURE_CALLBACK
        )]
    [IntentFilter(new[] { ACTION_EXPOSURE_STATE_UPDATED, ACTION_EXPOSURE_NOT_FOUND })]
    [Preserve]
    public class ExposureStateBroadcastReceiver : BroadcastReceiver
    {
        private const string PERMISSION_EXPOSURE_CALLBACK = "com.google.android.gms.nearby.exposurenotification.EXPOSURE_CALLBACK";
        private const string ACTION_EXPOSURE_STATE_UPDATED = "com.google.android.gms.exposurenotification.ACTION_EXPOSURE_STATE_UPDATED";
        private const string ACTION_EXPOSURE_NOT_FOUND = "com.google.android.gms.exposurenotification.ACTION_EXPOSURE_NOT_FOUND";
        private const string SERVICE_STATE_UPDATED = "com.google.android.gms.exposurenotification.SERVICE_STATE_UPDATED";

        private const string PERMISSION_BIND_JOB_SERVICE = "android.permission.BIND_JOB_SERVICE";

        private const string EXTRA_TOKEN = "com.google.android.gms.exposurenotification.EXTRA_TOKEN";
        private const string EXTRA_EXPOSURE_SUMMARY = "com.google.android.gms.exposurenotification.EXTRA_EXPOSURE_SUMMARY";

        private const int BASE_JOB_ID = 0x01 << 8;

        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;
            Logger.D($"Intent Action {action}");

            ExposureNotificationClient? enClient = null;
            if (context.ApplicationContext is IExposureNotificationHandler exposureNotificationHandler)
            {
                enClient = (ExposureNotificationClient)exposureNotificationHandler.GetEnClient();
            }

            if (enClient is null)
            {
                Logger.E("ExposureStateBroadcastReceiver: enClient is null.");
                return;
            }

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
                            ExposureDetectedV1Job.Enqueue(
                                context,
                                token,
                                enClient.ExposureDetectedV1JobSetting
                                );
                        }
                        else
                        {
                            ExposureDetectedV2Job.Enqueue(
                                context,
                                enClient.ExposureDetectedV2JobSetting
                                );
                        }
                        break;
                    case ACTION_EXPOSURE_NOT_FOUND:
                        Logger.D($"ACTION_EXPOSURE_NOT_FOUND");
                        ExposureNotDetectedJob.Enqueue(
                            context,
                            enClient.ExposureNotDetectedJobSetting
                            );
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.E($"Exception occurred: {e}");
            }
            finally
            {
                var exposureStateBroadcastReceiveTaskCompletionSourceDict = enClient.ExposureStateBroadcastReceiveTaskCompletionSourceDict;

                lock (exposureStateBroadcastReceiveTaskCompletionSourceDict)
                {
                    foreach (var key in exposureStateBroadcastReceiveTaskCompletionSourceDict.Keys)
                    {
                        exposureStateBroadcastReceiveTaskCompletionSourceDict.TryGetValue(key, out var value);
                        value?.TrySetResult(true);
                    }
                }
            }
        }

#pragma warning disable CS0612, CS0618 // Type or member is obsolete
        [Service(Permission = PERMISSION_BIND_JOB_SERVICE)]
        [Preserve]
        class ExposureDetectedV1Job : JobService
        {
            private const int JOB_ID = BASE_JOB_ID | 0x01;
            private const string EXTRA_TOKEN = "extra_token";

            public static void Enqueue(
                Context context, string token,
                JobSetting jobSetting
                )
            {
                PersistableBundle bundle = new PersistableBundle();
                bundle.PutString(EXTRA_TOKEN, token);

                JobInfo.Builder jobInfoBuilder = new JobInfo.Builder(
                    JOB_ID,
                    new ComponentName(context, Java.Lang.Class.FromType(typeof(ExposureDetectedV1Job))))
                    .SetExtras(bundle)
                    .SetOverrideDeadline(0);

                if (jobSetting != null)
                {
                    jobSetting.Apply(jobInfoBuilder);
                }

                JobInfo jobInfo = jobInfoBuilder.Build();

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
                IExposureNotificationHandler? handler = null;
                ExposureNotificationClient? enClient = null;
                if (ApplicationContext is IExposureNotificationHandler exposureNotificationHandler)
                {
                    handler = exposureNotificationHandler;
                    enClient = (ExposureNotificationClient)exposureNotificationHandler.GetEnClient();
                }

                if (enClient is null)
                {
                    Logger.E("ExposureDetectedV1Job: enClient is null.");
                    return false;
                }
                if (handler is null)
                {
                    Logger.E("ExposureDetectedV1Job: handler is null.");
                    return false;
                }

                var token = @params.Extras.GetString(EXTRA_TOKEN);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        handler.PreExposureDetected();

                        var (exposureSummary, exposureInformations) = await GetExposureV1Async(enClient, token);
                        handler.ExposureDetected(exposureSummary, exposureInformations);
                    }
                    catch (ApiException exception)
                    {
                        if (exception.IsENException())
                        {
                            var enException = exception.ToENException();
                            handler.ExceptionOccurred(enException);
                            throw enException;
                        }
                        else
                        {
                            handler.ExceptionOccurred(exception);
                            throw exception;
                        }
                    }
                    finally
                    {
                        JobFinished(@params, false);
                    }
                });

                return true;
            }

            public override bool OnStopJob(JobParameters @params)
            {
                Logger.E("ExposureDetectedV1Job stopped.");
                return false;
            }

            private async Task<(ExposureSummary, IList<ExposureInformation> exposureInformations)> GetExposureV1Async(
                ExposureNotificationClient enClient,
                string token
                )
            {
                Logger.D($"GetExposureV1Async");

                AndroidExposureSummary exposureSummary = await enClient.EnClient.GetExposureSummaryAsync(token);

                IList<AndroidExposureInformation> eis = await enClient.EnClient.GetExposureInformationAsync(token);
                IList<ExposureInformation> exposureInformations = eis.Select(ei => (ExposureInformation)new PlatformExposureInformation(ei)).ToList();

                return (new PlatformExposureSummary(exposureSummary), exposureInformations);
            }
        }
#pragma warning restore CS06122, CS0618 // Type or member is obsolete

        [Service(Permission = PERMISSION_BIND_JOB_SERVICE)]
        [Preserve]
        class ExposureDetectedV2Job : JobService
        {
            private const int JOB_ID = BASE_JOB_ID | 0x02;

            public static void Enqueue(
                Context context,
                JobSetting jobSetting
                )
            {
                JobInfo.Builder jobInfoBuilder = new JobInfo.Builder(
                    JOB_ID,
                    new ComponentName(context, Java.Lang.Class.FromType(typeof(ExposureDetectedV2Job))))
                    .SetOverrideDeadline(0);

                if (jobSetting != null)
                {
                    jobSetting.Apply(jobInfoBuilder);
                }

                JobInfo jobInfo = jobInfoBuilder.Build();

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
                IExposureNotificationHandler? handler = null;
                ExposureNotificationClient? enClient = null;
                if (ApplicationContext is IExposureNotificationHandler exposureNotificationHandler)
                {
                    handler = exposureNotificationHandler;
                    enClient = (ExposureNotificationClient)exposureNotificationHandler.GetEnClient();
                }

                if (enClient is null)
                {
                    Logger.E("ExposureDetectedV2Job: enClient is null.");
                    return false;
                }
                if (handler is null)
                {
                    Logger.E("ExposureDetectedV2Job: handler is null.");
                    return false;
                }

                _ = Task.Run(async () =>
                {
                    try
                    {
                        handler.PreExposureDetected();

                        var (dailySummaries, exposureWindows) = await GetExposureV2Async(enClient);

                        handler.ExposureDetected(dailySummaries, exposureWindows);
                    }
                    catch (ApiException exception)
                    {
                        if (exception.IsENException())
                        {
                            var enException = exception.ToENException();
                            handler.ExceptionOccurred(enException);
                            throw enException;
                        }
                        else
                        {
                            handler.ExceptionOccurred(exception);
                            throw exception;
                        }
                    }
                    finally
                    {
                        JobFinished(@params, false);
                    }
                });

                return true;
            }

            public override bool OnStopJob(JobParameters @params)
            {
                Logger.E("ExposureDetectedV2Job stopped.");
                return false;
            }

            private async Task<(List<DailySummary> dailySummaries, List<ExposureWindow> exposureWindows)> GetExposureV2Async(
                ExposureNotificationClient enClient
                )
            {
                Logger.D($"GetExposureV2Async");

                IList<AndroidDailySummary> dss = await enClient.EnClient.GetDailySummariesAsync(
                    enClient.ExposureConfiguration.GoogleDailySummariesConfig.ToAndroidDailySummariesConfig()
                    );
                List<DailySummary> dailySummaries = dss.Select(ds => (DailySummary)new PlatformDailySummary(ds)).ToList();

                IList<AndroidExposureWindow> ews = await enClient.EnClient.GetExposureWindowsAsync();
                List<ExposureWindow> exposureWindows = ews.Select(ew => (ExposureWindow)new PlatformExposureWindow(ew)).ToList();

                return (dailySummaries, exposureWindows);
            }
        }

        [Service(Permission = PERMISSION_BIND_JOB_SERVICE)]
        [Preserve]
        class ExposureNotDetectedJob : JobService
        {
            private const int JOB_ID = BASE_JOB_ID | 0x03;

            public static void Enqueue(
                Context context,
                JobSetting jobSetting
                )
            {
                JobInfo.Builder jobInfoBuilder = new JobInfo.Builder(
                    JOB_ID,
                    new ComponentName(context, Java.Lang.Class.FromType(typeof(ExposureNotDetectedJob))))
                    .SetOverrideDeadline(0);

                if (jobSetting != null)
                {
                    jobSetting.Apply(jobInfoBuilder);
                }

                JobInfo jobInfo = jobInfoBuilder.Build();

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
                IExposureNotificationHandler? handler = null;
                if (ApplicationContext is IExposureNotificationHandler exposureNotificationHandler)
                {
                    handler = exposureNotificationHandler;
                }

                if (handler is null)
                {
                    Logger.E("ExposureDetectedV2Job: handler is null.");
                    return false;
                }

                _ = Task.Run(() =>
                {
                    try
                    {
                        handler.ExposureNotDetected();
                    }
                    finally
                    {
                        JobFinished(@params, false);
                    }
                });

                return true;
            }

            public override bool OnStopJob(JobParameters @params)
            {
                Logger.E("ExposureNotDetectedJob stopped.");
                return false;
            }
        }
    }
}
