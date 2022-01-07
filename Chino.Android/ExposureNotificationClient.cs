using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;

using Nearby = Android.Gms.Nearby.NearbyClass;
using Java.IO;
using Android.Gms.Nearby.ExposureNotification;
using System.Threading.Tasks;
using System.Linq;

using AndroidTemporaryExposureKey = Android.Gms.Nearby.ExposureNotification.TemporaryExposureKey;
using JavaTimeoutException = Java.Util.Concurrent.TimeoutException;

using Logger = Chino.ChinoLogger;
using Android.Gms.Common.Apis;
using System.Threading;
using System.Collections.Concurrent;

[assembly: UsesFeature("android.hardware.bluetooth_le", Required = true)]
[assembly: UsesFeature("android.hardware.bluetooth")]
[assembly: UsesPermission(Android.Manifest.Permission.Bluetooth)]

#nullable enable

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationClient
    public class ExposureNotificationClient : AbsExposureNotificationClient
    {
        private const string ACTION_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED = "com.google.android.gms.exposurenotification.ACTION_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED";

        private const string EXTRA_TEMPORARY_EXPOSURE_KEY_LIST = "com.google.android.gms.exposurenotification.EXTRA_TEMPORARY_EXPOSURE_KEY_LIST";

        private static readonly IntentFilter INTENT_FILTER_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED
            = new IntentFilter(ACTION_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED);

        private const int API_TIMEOUT_MILLIS = 3 * 60 * 1000;
        private const int API_PROVIDE_DIAGNOSIS_KEYS_TIMEOUT_MILLIS = 55 * 60 * 1000;

        private Context? _appContext = null;
        internal IExposureNotificationClient? EnClient = null;

        internal readonly IDictionary<string, TaskCompletionSource<bool>> ExposureStateBroadcastReceiveTaskCompletionSourceDict
            = new ConcurrentDictionary<string, TaskCompletionSource<bool>>();

        public JobSetting? ExposureDetectedV1JobSetting { get; set; }
        public JobSetting? ExposureDetectedV2JobSetting { get; set; }
        public JobSetting? ExposureNotDetectedJobSetting { get; set; }

        public void Init(Context applicationContext)
        {
            _appContext = applicationContext;

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
                throw;
            }
        }

        private IExposureNotificationClient GetEnClient()
        {
            if (EnClient is null)
            {
                Logger.E("Init method must be called first.");
                throw new UnInitializedException("Init method must be called first.");
            }

            return EnClient;
        }

        public override async Task StartAsync()
        {
            var enClient = GetEnClient();

            try
            {
                await enClient.StartAsync();
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        public override async Task StopAsync()
        {
            var enClient = GetEnClient();

            try
            {
                await enClient.StopAsync();
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        public override async Task<bool> IsEnabledAsync()
        {
            var enClient = GetEnClient();

            try
            {
                return await enClient.IsEnabledAsync();
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        public override async Task<long> GetVersionAsync()
        {
            var enClient = GetEnClient();

            try
            {
                return await enClient.GetVersionAsync();
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        public override async Task<IList<ExposureNotificationStatus>> GetStatusesAsync()
        {
            var enClient = GetEnClient();

            try
            {
                var statuses = await enClient.GetStatusAsync();
                return statuses.Select(status => status.ToExposureNotificationStatus()).ToList();
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            CancellationTokenSource? cancellationTokenSource = null
            )
        {
            var enClient = GetEnClient();

            Logger.D($"DiagnosisKey {keyFiles.Count}");

            if (keyFiles.Count == 0)
            {
                Logger.D($"No DiagnosisKey found.");
                return ProvideDiagnosisKeysResult.NoDiagnosisKeyFound;
            }

            if (Handler is null)
            {
                throw new IllegalStateException("IExposureNotificationHandler is not set.");
            }

            ExposureConfiguration configuration = await Handler.GetExposureConfigurationAsync();

            string token = Guid.NewGuid().ToString();
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Check and add taskCompletionSource for prevent multiple starts.
            lock (ExposureStateBroadcastReceiveTaskCompletionSourceDict)
            {
                Logger.D($"ExposureStateBroadcastReceiveTaskCompletionSourceDict count {ExposureStateBroadcastReceiveTaskCompletionSourceDict.Count}");
                if (ExposureStateBroadcastReceiveTaskCompletionSourceDict.Count > 0)
                {
                    Logger.E($"Task ProvideDiagnosisKeysAsync(ExposureWindow mode) is already started.");
                    return ProvideDiagnosisKeysResult.Completed;
                }

                ExposureStateBroadcastReceiveTaskCompletionSourceDict.Add(token, taskCompletionSource);
            }

            cancellationTokenSource ??= new CancellationTokenSource(API_PROVIDE_DIAGNOSIS_KEYS_TIMEOUT_MILLIS);

            DiagnosisKeysDataMapping diagnosisKeysDataMapping = configuration.GoogleDiagnosisKeysDataMappingConfig.ToDiagnosisKeysDataMapping();

            try
            {
                DiagnosisKeysDataMapping currentDiagnosisKeysDataMapping = await enClient.GetDiagnosisKeysDataMappingAsync();

                // https://github.com/google/exposure-notifications-internals/blob/aaada6ce5cad0ea1493930591557f8053ef4f113/exposurenotification/src/main/java/com/google/samples/exposurenotification/nearby/DiagnosisKeysDataMapping.java#L113
                if (!diagnosisKeysDataMapping.Equals(currentDiagnosisKeysDataMapping))
                {
                    await enClient.SetDiagnosisKeysDataMappingAsync(diagnosisKeysDataMapping);
                    await Handler.DiagnosisKeysDataMappingAppliedAsync();

                    Logger.I("DiagnosisKeysDataMapping have been updated.");
                }
                else
                {
                    Logger.D("DiagnosisKeysDataMapping is not updated.");
                }

                var files = keyFiles.Select(f => new File(f)).ToList();
                DiagnosisKeyFileProvider diagnosisKeyFileProvider = new DiagnosisKeyFileProvider(files);

                using (cancellationTokenSource.Token.Register(() =>
                {
                    Logger.D("ProvideDiagnosisKeysAsync cancellationTokenSource canceled.");
                    taskCompletionSource?.TrySetException(
                        new TimeoutException($"ExposureStateBroadcastReceiver was not called in {API_PROVIDE_DIAGNOSIS_KEYS_TIMEOUT_MILLIS} millis.")
                        );
                    lock (ExposureStateBroadcastReceiveTaskCompletionSourceDict)
                    {
                        ExposureStateBroadcastReceiveTaskCompletionSourceDict.Remove(token);
                    }
                }))
                {
                    await enClient.ProvideDiagnosisKeysAsync(diagnosisKeyFileProvider);
                    _ = await taskCompletionSource.Task;
                }

                Logger.D("ExposureStateBroadcastReceiveTaskCompletionSource is completed.");
                return ProvideDiagnosisKeysResult.Completed;
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
            finally
            {
                lock (ExposureStateBroadcastReceiveTaskCompletionSourceDict)
                {
                    ExposureStateBroadcastReceiveTaskCompletionSourceDict.Remove(token);
                }
            }
        }

        public override async Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
        {
            var enClient = GetEnClient();

            try
            {
                var teks = await enClient.GetTemporaryExposureKeyHistoryAsync();
                return teks.Select(tek => (TemporaryExposureKey)new PlatformTemporaryExposureKey(tek)).ToList();
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            string token,
            CancellationTokenSource? cancellationTokenSource = null
            )
        {
            var enClient = GetEnClient();

            Logger.D($"DiagnosisKey {keyFiles.Count}");

            if (keyFiles.Count == 0)
            {
                Logger.D($"No DiagnosisKey found.");
                return ProvideDiagnosisKeysResult.NoDiagnosisKeyFound;
            }

            if (Handler is null)
            {
                throw new IllegalStateException("IExposureNotificationHandler is not set.");
            }

            ExposureConfiguration configuration = await Handler.GetExposureConfigurationAsync();

            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Check and add taskCompletionSource for prevent multiple starts.
            lock (ExposureStateBroadcastReceiveTaskCompletionSourceDict)
            {
                Logger.D($"ExposureStateBroadcastReceiveTaskCompletionSourceDict count {ExposureStateBroadcastReceiveTaskCompletionSourceDict.Count}");
                if (ExposureStateBroadcastReceiveTaskCompletionSourceDict.ContainsKey(token))
                {
                    Logger.E($"Task ProvideDiagnosisKeysAsync(Legacy-V1 mode) token {token} is already started.");
                    return ProvideDiagnosisKeysResult.Completed;
                }

                ExposureStateBroadcastReceiveTaskCompletionSourceDict.Add(token, taskCompletionSource);
            }

            cancellationTokenSource ??= new CancellationTokenSource(API_PROVIDE_DIAGNOSIS_KEYS_TIMEOUT_MILLIS);

            var files = keyFiles.Select(f => new File(f)).ToList();

            try
            {
                using (cancellationTokenSource.Token.Register(() =>
                {
                    Logger.D("ProvideDiagnosisKeysAsync cancellationTokenSource canceled.");
                    taskCompletionSource?.TrySetException(
                        new TimeoutException($"ExposureStateBroadcastReceiver was not called in {API_PROVIDE_DIAGNOSIS_KEYS_TIMEOUT_MILLIS} millis.")
                        );
                    lock (ExposureStateBroadcastReceiveTaskCompletionSourceDict)
                    {
                        ExposureStateBroadcastReceiveTaskCompletionSourceDict.Remove(token);
                    }
                }))
                {
                    await enClient.ProvideDiagnosisKeysAsync(files, configuration.ToAndroidExposureConfiguration(), token);
                    _ = await taskCompletionSource.Task;

                    lock (ExposureStateBroadcastReceiveTaskCompletionSourceDict)
                    {
                        ExposureStateBroadcastReceiveTaskCompletionSourceDict.Remove(token);
                    }
                }

                Logger.D("ExposureStateBroadcastReceiveTaskCompletionSource is completed.");
                return ProvideDiagnosisKeysResult.Completed;
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public override async Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
        {
            var enClient = GetEnClient();

            try
            {
                await enClient.RequestPreAuthorizedTemporaryExposureKeyHistoryAsync();
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
        }

        class PreAuthorizeReleasePhoneUnlockedBroadcastReceiver : BroadcastReceiver
        {
            private readonly TaskCompletionSource<Intent> _taskCompletionSource;

            public PreAuthorizeReleasePhoneUnlockedBroadcastReceiver(TaskCompletionSource<Intent> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }
            public override void OnReceive(Context context, Intent intent)
            {
                Logger.D($"ACTION_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED");
                context.UnregisterReceiver(this);
                _taskCompletionSource.SetResult(intent);
            }
        }

        public override async Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
        {
            Logger.D("RequestPreAuthorizedTemporaryExposureKeyReleaseAsync");

            IExposureNotificationHandler? handler = null;
            ExposureNotificationClient? enClient = null;

            if (_appContext is null)
            {
                throw new IllegalStateException("IExposureNotificationHandler is not set.");
            }

            if (_appContext is IExposureNotificationHandler exposureNotificationHandler)
            {
                handler = exposureNotificationHandler;
                enClient = (ExposureNotificationClient)exposureNotificationHandler.GetEnClient();
            }

            if (handler is null)
            {
                Logger.E("TemporaryExposureKeyReleasedJob: handler is null.");
                return;
            }
            if (enClient is null)
            {
                Logger.E("TemporaryExposureKeyReleasedJob: enClient is null.");
                return;
            }

            try
            {
                IList<TemporaryExposureKey> temporaryExposureKeys = await GetReleasedTemporaryExposureKeys(enClient, _appContext);
                await handler.TemporaryExposureKeyReleasedAsync(temporaryExposureKeys);
            }
            catch (JavaTimeoutException exception)
            {
                // Wrap exception
                throw new TimeoutException(exception.Message);
            }
            catch (ApiException exception)
            {
                if (exception.IsENException())
                {
                    throw exception.ToENException();
                }
                throw;
            }
            finally
            {
            }
        }

        private async Task<IList<TemporaryExposureKey>> GetReleasedTemporaryExposureKeys(
            ExposureNotificationClient enClient,
            Context appContext
            )
        {
            TaskCompletionSource<Intent> taskCompletionSource = new TaskCompletionSource<Intent>(TaskCreationOptions.RunContinuationsAsynchronously);
            BroadcastReceiver receiver = new PreAuthorizeReleasePhoneUnlockedBroadcastReceiver(taskCompletionSource);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(API_TIMEOUT_MILLIS);
            using (cancellationTokenSource.Token.Register(() =>
            {
                Logger.D("cancellationTokenSource canceled.");
                taskCompletionSource.TrySetCanceled();
                appContext.UnregisterReceiver(receiver);
            }))
            {
                appContext.RegisterReceiver(
                    receiver,
                    INTENT_FILTER_PRE_AUTHORIZE_RELEASE_PHONE_UNLOCKED
                    );

                await enClient.RequestPreAuthorizedTemporaryExposureKeyReleaseAsync();

                Intent intent = await taskCompletionSource.Task;

                IList<TemporaryExposureKey> temporaryExposureKeys = intent.GetParcelableArrayListExtra(EXTRA_TEMPORARY_EXPOSURE_KEY_LIST)
                    .Cast<AndroidTemporaryExposureKey>()
                    .Select(tek => (TemporaryExposureKey)new PlatformTemporaryExposureKey(tek))
                    .ToList();

                return temporaryExposureKeys;
            }
        }
    }
}
