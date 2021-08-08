using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Runtime;
using Chino;
using Chino.Android.Google;
using Java.IO;
using Newtonsoft.Json;
using Sample.Common.Model;
using Xamarin.Essentials;
using Logger = Chino.ChinoLogger;

namespace Sample.Android
{

#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application, IExposureNotificationHandler
    {
        private const long INITIAL_BACKOFF_MILLIS = 60 * 60 * 1000;

        private const string EXPOSURE_DETECTION_RESULT_DIR = "exposure_detection_result";

        private readonly Action<JobInfo.Builder> _temporaryExposureKeyReleasedJobInfoBuildAction = builder =>
        {
            builder.SetBackoffCriteria(INITIAL_BACKOFF_MILLIS, BackoffPolicy.Linear)
                .SetPersisted(true);
        };
        private readonly Action<JobInfo.Builder> _exposureDetectedV1JobInfoBuildAction = builder =>
        {
            builder.SetBackoffCriteria(INITIAL_BACKOFF_MILLIS, BackoffPolicy.Linear)
                .SetPersisted(true);
        };
        private readonly Action<JobInfo.Builder> _exposureDetectedV2JobInfoBuildAction = builder =>
        {
            builder.SetBackoffCriteria(INITIAL_BACKOFF_MILLIS, BackoffPolicy.Linear)
                .SetPersisted(true);
        };
        private readonly Action<JobInfo.Builder> _exposureNotDetectedJobInfoBuildAction = builder =>
        {
        };

        private File _exposureDetectionResultDir;

        private ExposureNotificationClient EnClient = null;

        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            InitializeDirs();

            AbsExposureNotificationClient.Handler = this;
        }

        private void InitializeDirs()
        {
            _exposureDetectionResultDir = new File(FilesDir, EXPOSURE_DETECTION_RESULT_DIR);
            if (!_exposureDetectionResultDir.Exists())
            {
                _exposureDetectionResultDir.Mkdirs();
            }
        }

        public AbsExposureNotificationClient GetEnClient()
        {
            if (EnClient == null)
            {
                EnClient = new ExposureNotificationClient();
                EnClient.TemporaryExposureKeyReleasedJobInfoBuildAction = _temporaryExposureKeyReleasedJobInfoBuildAction;
                EnClient.ExposureDetectedV1JobInfoBuildAction = _exposureDetectedV1JobInfoBuildAction;
                EnClient.ExposureDetectedV2JobInfoBuildAction = _exposureDetectedV2JobInfoBuildAction;
                EnClient.ExposureNotDetectedJobInfoBuildAction = _exposureNotDetectedJobInfoBuildAction;
                EnClient.Init(this);
            }

            return EnClient;
        }

        public void TemporaryExposureKeyReleased(IList<TemporaryExposureKey> temporaryExposureKeys)
        {
            Logger.D("TemporaryExposureKeyReleased");

            var serializedJson = JsonConvert.SerializeObject(temporaryExposureKeys, Formatting.Indented);
            Logger.D(serializedJson);
        }

        public void ExposureDetected(IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
        {
            Logger.D("ExposureDetected ExposureWindows");

            var exposureResult = new ExposureResult(EnClient.ExposureConfiguration,
                DateTime.Now,
                dailySummaries, exposureWindows);

            Task.Run(async () => await SaveExposureResult(exposureResult));
        }

        public void ExposureDetected(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
        {
            var exposureResult = new ExposureResult(EnClient.ExposureConfiguration,
                DateTime.Now,
                exposureSummary, exposureInformations);

            Task.Run(async () => await SaveExposureResult(exposureResult));
        }

        public void ExposureNotDetected()
        {
            var exposureResult = new ExposureResult(EnClient.ExposureConfiguration,
                DateTime.Now);

            Task.Run(async () => await SaveExposureResult(exposureResult));
        }

        private async Task SaveExposureResult(ExposureResult exposureResult)
        {
            exposureResult.Device = DeviceInfo.Model;
            exposureResult.EnVersion = (await EnClient.GetVersionAsync()).ToString();

            string fileName = $"{exposureResult.Id}.json";
            string json = exposureResult.ToJsonString();

            var filePath = new File(_exposureDetectionResultDir, fileName);

            using BufferedWriter bw = new BufferedWriter(new FileWriter(filePath));
            await bw.WriteAsync(json);
            await bw.FlushAsync();
        }
    }
}
