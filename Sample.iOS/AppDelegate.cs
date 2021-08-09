using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Chino;
using Foundation;
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

        [Export("window")]
        public UIWindow Window { get; set; }

        [Export("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Logger.D("FinishedLaunching");

            InitializeDirs();

            AbsExposureNotificationClient.Handler = this;
            ExposureNotificationClientManager.Shared.UserExplanation = USER_EXPLANATION;
            ExposureNotificationClientManager.Shared.IsTest = true;

            return true;
        }

        private void InitializeDirs()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            _exposureDetectionResultDir = Path.Combine(documents, EXPOSURE_DETECTION_RESULT_DIR);
            if (!Directory.Exists(_exposureDetectionResultDir))
            {
                Directory.CreateDirectory(_exposureDetectionResultDir);
            }
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

        public void TemporaryExposureKeyReleased(IList<TemporaryExposureKey> temporaryExposureKeys)
        {
            Logger.D("TemporaryExposureKeyReleased");

            foreach (TemporaryExposureKey tek in temporaryExposureKeys)
            {
                Logger.D(Convert.ToBase64String(tek.KeyData));
            }
        }

        public void PreExposureDetected()
        {
            Logger.D($"PreExposureDetected: {DateTime.UtcNow}");
        }

        public void ExposureDetected(IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
        {
            Logger.D($"ExposureDetected V2: {DateTime.UtcNow}");

            var exposureResult = new ExposureResult(ExposureNotificationClientManager.Shared.ExposureConfiguration,
                DateTime.Now,
                dailySummaries, exposureWindows);

            Task.Run(async () => await SaveExposureResult(exposureResult));
        }

        public void ExposureDetected(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
        {
            Logger.D($"ExposureDetected V1: {DateTime.UtcNow}");

            var exposureResult = new ExposureResult(ExposureNotificationClientManager.Shared.ExposureConfiguration,
                DateTime.Now,
                exposureSummary, exposureInformations);

            Task.Run(async () => await SaveExposureResult(exposureResult));
        }

        public void ExposureNotDetected()
        {
            Logger.D($"ExposureNotDetected: {DateTime.UtcNow}");

            var exposureResult = new ExposureResult(ExposureNotificationClientManager.Shared.ExposureConfiguration,
                DateTime.Now);

            Task.Run(async () => await SaveExposureResult(exposureResult));
        }

        private async Task SaveExposureResult(ExposureResult exposureResult)
        {
            exposureResult.Device = DeviceInfo.Model;
            exposureResult.EnVersion = (await ExposureNotificationClientManager.Shared.GetVersionAsync()).ToString();

            string fileName = $"{exposureResult.Id}.json";
            string json = exposureResult.ToJsonString();

            var filePath = Path.Combine(_exposureDetectionResultDir, fileName);

            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
