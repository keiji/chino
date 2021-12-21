using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Runtime;
using Chino;
using Chino.Android.Google;
using Newtonsoft.Json;
using Sample.Common;
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

        private readonly JobSetting _temporaryExposureKeyReleasedJobSetting
            = new JobSetting(INITIAL_BACKOFF_MILLIS, BackoffPolicy.Linear, true);
        private readonly JobSetting _exposureDetectedV1JobSetting
            = new JobSetting(INITIAL_BACKOFF_MILLIS, BackoffPolicy.Linear, true);
        private readonly JobSetting _exposureDetectedV2JobSetting
            = new JobSetting(INITIAL_BACKOFF_MILLIS, BackoffPolicy.Linear, true);
        private readonly JobSetting _exposureNotDetectedJobSetting = null;

        private string _exposureDetectionResultDir;

        private string _configurationDir;

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
            _exposureDetectionResultDir = Path.Combine(FilesDir.AbsolutePath, EXPOSURE_DETECTION_RESULT_DIR);
            if (!Directory.Exists(_exposureDetectionResultDir))
            {
                Directory.CreateDirectory(_exposureDetectionResultDir);
            }

            _configurationDir = Path.Combine(FilesDir.AbsolutePath, Constants.CONFIGURATION_DIR);
            if (!Directory.Exists(_configurationDir))
            {
                Directory.CreateDirectory(_configurationDir);
            }
        }

        private async Task<ExposureDataServerConfiguration> LoadExposureDataServerConfiguration()
        {
            var serverConfigurationPath = Path.Combine(_configurationDir, Constants.EXPOSURE_DATA_SERVER_CONFIGURATION_FILENAME);
            if (File.Exists(serverConfigurationPath))
            {
                return JsonConvert.DeserializeObject<ExposureDataServerConfiguration>(
                    await System.IO.File.ReadAllTextAsync(serverConfigurationPath)
                    );
            }

            var serverConfiguration = new ExposureDataServerConfiguration();
            var json = JsonConvert.SerializeObject(serverConfiguration, Formatting.Indented);
            await File.WriteAllTextAsync(serverConfigurationPath, json);
            return serverConfiguration;
        }

        public AbsExposureNotificationClient GetEnClient()
        {
            if (EnClient == null)
            {
                EnClient = new ExposureNotificationClient()
                {
                    ExposureDetectedV1JobSetting = _exposureDetectedV1JobSetting,
                    ExposureDetectedV2JobSetting = _exposureDetectedV2JobSetting,
                    ExposureNotDetectedJobSetting = _exposureNotDetectedJobSetting
                };
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

        public void PreExposureDetected(ExposureConfiguration exposureConfiguration)
        {
            Logger.D($"PreExposureDetected: {DateTime.UtcNow}");
        }

        public void ExposureDetected(IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows, ExposureConfiguration exposureConfiguration)
        {
            Logger.D($"ExposureDetected ExposureWindows: {DateTime.UtcNow}");

            Task.Run(async () =>
            {
                var enVersion = (await EnClient.GetVersionAsync()).ToString();

                var exposureResult = new ExposureResult(
                    exposureConfiguration,
                    DateTime.Now,
                    dailySummaries, exposureWindows
                    )
                {
                    Device = DeviceInfo.Model,
                    EnVersion = enVersion
                };
                var filePath = await SaveExposureResult(exposureResult);

                var exposureDataServerConfiguration = await LoadExposureDataServerConfiguration();

                var exposureDataResponse = await new ExposureDataServer(exposureDataServerConfiguration).UploadExposureDataAsync(
                    exposureConfiguration,
                    DeviceInfo.Model,
                    enVersion,
                    dailySummaries, exposureWindows
                    );

                if (exposureDataResponse != null)
                {
                    await SaveExposureResult(exposureDataResponse);
                    File.Delete(filePath);
                }
            });
        }

        public void ExposureDetected(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations, ExposureConfiguration exposureConfiguration)
        {
            Logger.D($"ExposureDetected Legacy-v1: {DateTime.UtcNow}");

            Task.Run(async () =>
            {
                var enVersion = (await EnClient.GetVersionAsync()).ToString();

                var exposureResult = new ExposureResult(
                    exposureConfiguration,
                    DateTime.Now,
                    exposureSummary, exposureInformations
                    )
                {
                    Device = DeviceInfo.Model,
                    EnVersion = enVersion
                };
                var filePath = await SaveExposureResult(exposureResult);

                var exposureDataServerConfiguration = await LoadExposureDataServerConfiguration();

                var exposureDataResponse = await new ExposureDataServer(exposureDataServerConfiguration).UploadExposureDataAsync(
                    EnClient.ExposureConfiguration,
                    DeviceInfo.Model,
                    enVersion,
                    exposureSummary, exposureInformations
                    );

                if (exposureDataResponse != null)
                {
                    await SaveExposureResult(exposureDataResponse);
                    File.Delete(filePath);
                }
            });
        }

        public void ExposureNotDetected(ExposureConfiguration exposureConfiguration)
        {
            Logger.D($"ExposureNotDetected: {DateTime.UtcNow}");

            Task.Run(async () =>
            {
                var enVersion = (await EnClient.GetVersionAsync()).ToString();

                var exposureResult = new ExposureResult(
                    exposureConfiguration,
                    DateTime.Now
                    )
                {
                    Device = DeviceInfo.Model,
                    EnVersion = enVersion
                };
                var filePath = await SaveExposureResult(exposureResult);

                var exposureDataServerConfiguration = await LoadExposureDataServerConfiguration();

                var exposureDataResponse = await new ExposureDataServer(exposureDataServerConfiguration).UploadExposureDataAsync(
                    EnClient.ExposureConfiguration,
                    DeviceInfo.Model,
                    enVersion
                    );

                if (exposureDataResponse != null)
                {
                    await SaveExposureResult(exposureDataResponse);
                    File.Delete(filePath);
                }
            });

        }

        public void ExceptionOccurred(Exception exception)
        {
            Logger.E(exception);
        }

        private async Task<string> SaveExposureResult(ExposureResult exposureResult)
        {
            string fileName = $"exposuredata-{exposureResult.GetHashCode()}.json";
            string json = exposureResult.ToJsonString();

            var filePath = Path.Combine(_exposureDetectionResultDir, fileName);
            await File.WriteAllTextAsync(filePath, json);

            return filePath;
        }

        private async Task SaveExposureResult(ExposureDataResponse exposureDataResponse)
        {
            string fileName = exposureDataResponse.FileName;
            var filePath = Path.Combine(_exposureDetectionResultDir, fileName);

            await File.WriteAllTextAsync(
                filePath,
                JsonConvert.SerializeObject(exposureDataResponse, Formatting.Indented)
                );
        }
    }
}
