using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using Chino;
using Java.IO;
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
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        private const string EXPOSURE_DETECTION_RESULT_DIR = "exposure_detection_result";

        private File _exposureDetectionResultDir;

        private ExposureNotificationClient EnClient = null;

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
                EnClient.Init(this);
            }

            return EnClient;
        }

        public void TemporaryExposureKeyReleased(IList<ITemporaryExposureKey> temporaryExposureKeys)
        {
            Logger.D("TemporaryExposureKeyReleased");
        }

        public void ExposureDetected(IList<IDailySummary> dailySummaries, IList<IExposureWindow> exposureWindows)
        {
            Logger.D("ExposureDetected ExposureWindows");

            var exposureResult = new ExposureResult(EnClient.ExposureConfiguration,
                DateTime.Now,
                dailySummaries, exposureWindows);

            Task.Run(async () => await SaveExposureResult(exposureResult));
        }

        public void ExposureDetected(IExposureSummary exposureSummary, IList<IExposureInformation> exposureInformations)
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
