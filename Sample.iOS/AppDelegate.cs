using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Chino;
using Foundation;
using Newtonsoft.Json;
using Sample.Common;
using Sample.Common.Model;
using UIKit;
using Xamarin.Essentials;
using Logger = Chino.ChinoLogger;

namespace Sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate, IExposureNotificationHandler
    {
        private const string USER_EXPLANATION = "Chino.Sample.iOS";
        private const string EXPOSURE_DETECTION_RESULT_DIR = "exposure_detection_result";

        private string _exposureDetectionResultDir;

        private string _configurationDir;
        private ExposureConfiguration _exposureConfiguration;

        [Export("window")]
        public UIWindow Window { get; set; }

        [Export("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Logger.D("FinishedLaunching");

            InitializeDirs();

            AbsExposureNotificationClient.Handler = this;
            ExposureNotificationClientManager.Shared.UserExplanation = USER_EXPLANATION;

            return true;
        }

        private async Task<ExposureConfiguration> LoadExposureConfigurationAsync()
        {
            var exposureConfigurationPath = Path.Combine(_configurationDir, Constants.EXPOSURE_CONFIGURATION_FILENAME);
            if (File.Exists(exposureConfigurationPath))
            {
                return JsonConvert.DeserializeObject<ExposureConfiguration>(
                    await File.ReadAllTextAsync(exposureConfigurationPath)
                    );
            }

            var exposureConfiguration = new ExposureConfiguration();
            var json = JsonConvert.SerializeObject(exposureConfiguration, Formatting.Indented);
            await File.WriteAllTextAsync(exposureConfigurationPath, json);

            return _exposureConfiguration;
        }

        private void InitializeDirs()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            _exposureDetectionResultDir = Path.Combine(documents, EXPOSURE_DETECTION_RESULT_DIR);
            if (!Directory.Exists(_exposureDetectionResultDir))
            {
                Directory.CreateDirectory(_exposureDetectionResultDir);
            }

            _configurationDir = Path.Combine(documents, Constants.CONFIGURATION_DIR);
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
                    await File.ReadAllTextAsync(serverConfigurationPath)
                    );
            }

            var exposureDataServerConfiguration = new ExposureDataServerConfiguration();
            var json = JsonConvert.SerializeObject(exposureDataServerConfiguration, Formatting.Indented);
            await File.WriteAllTextAsync(serverConfigurationPath, json);
            return exposureDataServerConfiguration;
        }

        // UISceneSession Lifecycle

        [Export("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create("Default Configuration", connectingSceneSession.Role);
        }

        [Export("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions(UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }

        public AbsExposureNotificationClient GetEnClient() => ExposureNotificationClientManager.Shared;

        public Task TemporaryExposureKeyReleasedAsync(IList<TemporaryExposureKey> temporaryExposureKeys)
        {
            Logger.D("TemporaryExposureKeyReleased");

            foreach (TemporaryExposureKey tek in temporaryExposureKeys)
            {
                Logger.D(Convert.ToBase64String(tek.KeyData));
            }

            return Task.CompletedTask;
        }

        public Task PreExposureDetectedAsync(ExposureConfiguration exposureConfiguration)
        {
            Logger.D($"PreExposureDetected: {DateTime.UtcNow}");

            return Task.CompletedTask;
        }

        public async Task ExposureDetectedAsync(ExposureSummary exposureSummary, IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows, ExposureConfiguration exposureConfiguration)
        {
            Logger.D($"ExposureDetected V2: {DateTime.UtcNow}");

            var enVersion = (await ExposureNotificationClientManager.Shared.GetVersionAsync()).ToString();

            var exposureResult = new ExposureResult(
                exposureConfiguration,
                DateTime.Now,
                exposureSummary,
                null,
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
                exposureSummary,
                dailySummaries, exposureWindows
                );

            if (exposureDataResponse != null)
            {
                await SaveExposureResult(exposureDataResponse);
                File.Delete(filePath);
            }
        }

        public async Task ExposureDetectedAsync(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations, ExposureConfiguration exposureConfiguration)
        {
            Logger.D($"ExposureDetected V1: {DateTime.UtcNow}");

            var enVersion = (await ExposureNotificationClientManager.Shared.GetVersionAsync()).ToString();

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
                exposureConfiguration,
                DeviceInfo.Model,
                enVersion,
                exposureSummary, exposureInformations
                );

            if (exposureDataResponse != null)
            {
                await SaveExposureResult(exposureDataResponse);
                File.Delete(filePath);
            }
        }

        public async Task ExposureNotDetectedAsync(ExposureConfiguration exposureConfiguration)
        {
            Logger.D($"ExposureNotDetected: {DateTime.UtcNow}");

            var enVersion = (await ExposureNotificationClientManager.Shared.GetVersionAsync()).ToString();

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
                exposureConfiguration,
                DeviceInfo.Model,
                enVersion
                );

            if (exposureDataResponse != null)
            {
                await SaveExposureResult(exposureDataResponse);
                File.Delete(filePath);
            }
        }

        public Task ExceptionOccurredAsync(Exception exception)
        {
            Logger.E(exception);

            return Task.CompletedTask;
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

        public Task<ExposureConfiguration> GetExposureConfigurationAsync()
            => LoadExposureConfigurationAsync();
    }
}
