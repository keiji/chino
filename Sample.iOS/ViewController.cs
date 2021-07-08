using Chino;
using Chino.Common;
using Chino.iOS;
using Foundation;
using Newtonsoft.Json;
using Sample.Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using Logger = Chino.ChinoLogger;

namespace Sample.iOS
{
    public partial class ViewController : UIViewController
    {
        private const string TEKS_DIR = "temporary_exposure_keys";
        private const string EXPOSURE_DETECTION = "exposure_detection";
        private const string EXPOSURE_CONFIGURATION_FILENAME = "exposure_configuration.json";

        private string _teksDir;
        private string _exposureDetectionDir;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            Logger.D("ViewDidLoad");

            InitializeDirs();

            IExposureNotificationStatus status = await ExposureNotificationClientManager.Shared.GetStatusAsync();
            long version = await ExposureNotificationClientManager.Shared.GetVersionAsync();
            ShowStatusAsync(status, version);

            buttonEnableEn.TouchUpInside += async (sender, e) =>
            {
                Logger.D("buttonEnableEn");

                try
                {
                    await ExposureNotificationClientManager.Shared.StartAsync();

                    IExposureNotificationStatus status = await ExposureNotificationClientManager.Shared.GetStatusAsync();
                    long version = await ExposureNotificationClientManager.Shared.GetVersionAsync();
                    ShowStatusAsync(status, version);
                }
                catch (ENException enException)
                {
                    ShowENException(enException);
                }
                catch (NSErrorException exception)
                {
                    exception.LogD();
                }
            };
            buttonShowTeksHistory.TouchUpInside += async (sender, e) =>
            {
                try
                {
                    List<ITemporaryExposureKey> teks = await ExposureNotificationClientManager.Shared.GetTemporaryExposureKeyHistoryAsync();

                    ShowTeks(teks);
                    await SaveTeksAsync(teks);
                }
                catch (ENException enException)
                {
                    ShowENException(enException);
                }
                catch (NSErrorException exception)
                {
                    exception.LogD();
                }
            };
            buttonDetectExposure.TouchUpInside += async (sender, e) =>
            {
                try
                {
                    await DetectExposure();
                }
                catch (ENException enException)
                {
                    ShowENException(enException);
                }
                catch (NSErrorException exception)
                {
                    exception.LogD();
                }
            };
            buttonRequestPreauthorizedKeys.TouchUpInside += async (sender, e) =>
            {
                try
                {
                    await RequestPreauthorizedKeys();
                }
                catch (ENException enException)
                {
                    ShowENException(enException);
                }
                catch (NSErrorException exception)
                {
                    exception.LogD();
                }
            };
            buttonRequestReleaseKeys.TouchUpInside += async (sender, e) =>
            {
                try
                {
                    await RequestReleaseKeys();
                }
                catch (ENException enException)
                {
                    ShowENException(enException);
                }
                catch (NSErrorException exception)
                {
                    exception.LogD();
                }
            };

            await InitializeExposureConfiguration();
        }

        private void ShowENException(ENException enException)
        {
            Logger.D($"ENException - Code:{enException.Code}, {enException.Message}");

            string message = enException.Code switch
            {
                ENException.Code_iOS.ApiMisuse => "ApiMisuse",
                ENException.Code_iOS.BadFormat => "BadFormat",
                ENException.Code_iOS.BadParameter => "BadParameter",
                ENException.Code_iOS.BluetoothOff => "BluetoothOff",
                ENException.Code_iOS.DataInaccessible => "DataInaccessible",
                ENException.Code_iOS.InsufficientMemory => "InsufficientMemory",
                ENException.Code_iOS.InsufficientStorage => "InsufficientStorage",
                ENException.Code_iOS.Internal => "Internal",
                ENException.Code_iOS.Invalidated => "Invalidated",
                ENException.Code_iOS.NotAuthorized => "NotAuthorized",
                ENException.Code_iOS.NotEnabled => "NotEnabled",
                ENException.Code_iOS.NotEntitled => "NotEntitled",
                ENException.Code_iOS.RateLimited => "RateLimited",
                ENException.Code_iOS.Restricted => "Restricted",
                ENException.Code_iOS.TravelStatusNotAvailable => "TravelStatusNotAvailable",
                ENException.Code_iOS.Unsupported => "Unsupported",
                _ => "Unknown",
            };

            labelStatus.Text = $"ENException: {message}";
        }

        private void InitializeDirs()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            _teksDir = Path.Combine(documents, TEKS_DIR);
            if (!Directory.Exists(_teksDir))
            {
                Directory.CreateDirectory(_teksDir);
            }

            _exposureDetectionDir = Path.Combine(documents, EXPOSURE_DETECTION);
            if (!Directory.Exists(_exposureDetectionDir))
            {
                Directory.CreateDirectory(_exposureDetectionDir);
            }
        }

        private ExposureConfiguration _exposureConfiguration;

        private async Task InitializeExposureConfiguration()
        {
            var exposureConfigurationPath = Path.Combine(_exposureDetectionDir, EXPOSURE_CONFIGURATION_FILENAME);
            if (File.Exists(exposureConfigurationPath))
            {
                _exposureConfiguration = JsonConvert.DeserializeObject<ExposureConfiguration>(
                    await File.ReadAllTextAsync(exposureConfigurationPath)
                    );
                return;
            }

            _exposureConfiguration = new ExposureConfiguration();
            var json = JsonConvert.SerializeObject(_exposureConfiguration, Formatting.Indented);

            await File.WriteAllTextAsync(exposureConfigurationPath, json);
        }

        private async Task RequestPreauthorizedKeys()
            => await ExposureNotificationClientManager.Shared.RequestPreAuthorizedTemporaryExposureKeyHistoryAsync();

        private async Task RequestReleaseKeys()
            => await ExposureNotificationClientManager.Shared.RequestPreAuthorizedTemporaryExposureKeyReleaseAsync();

        private async Task DetectExposure()
        {
            List<string> diagnosisKeyPaths = Directory.GetFiles(_exposureDetectionDir).ToList()
                .FindAll(path => !Directory.Exists(path))
                .FindAll(path => path.EndsWith(".zip"));

            foreach (string path in diagnosisKeyPaths)
            {
                Logger.D($"path {path}");
            }

            await ExposureNotificationClientManager.Shared.ProvideDiagnosisKeysAsync(diagnosisKeyPaths, _exposureConfiguration);
        }

        private void ShowTeks(IList<ITemporaryExposureKey> temporaryExposureKeys)
        {
            List<string> tekKeyData = temporaryExposureKeys.Select(teks => Convert.ToBase64String(teks.KeyData)).ToList();
            labelStatus.Text = string.Join("\n", tekKeyData);
        }

        private async Task SaveTeksAsync(IList<ITemporaryExposureKey> temporaryExposureKeys)
        {
            TemporaryExposureKeys teks = new TemporaryExposureKeys(temporaryExposureKeys, DateTime.Now)
            {
                Device = DeviceInfo.Model
            };

            string fileName = $"{teks.Id}.json";
            string json = teks.ToJsonString();
            Logger.D(json);

            var filePath = Path.Combine(_teksDir, fileName);
            await File.WriteAllTextAsync(filePath, json);
        }

        private void ShowStatusAsync(IExposureNotificationStatus status, long version)
        {
            switch (status.Status())
            {
                case Status.Active:
                    buttonEnableEn.SetTitle($"EN v{version} is Active.", UIControlState.Normal);
                    break;
                case Status.NotActive:
                    buttonEnableEn.SetTitle($"EN v{version} is NotActive.", UIControlState.Normal);
                    break;
                case Status.BluetoothOff:
                    buttonEnableEn.SetTitle($"EN v{version} is BluetoothOff.", UIControlState.Normal);
                    break;
                case Status.Unauthorized:
                    buttonEnableEn.SetTitle($"EN v{version} is Unauthorized.", UIControlState.Normal);
                    break;
                case Status.Unknown:
                    buttonEnableEn.SetTitle($"EN v{version} is Unknown.", UIControlState.Normal);
                    break;
                case Status.Misc:
                    buttonEnableEn.SetTitle($"EN v{version} is Misc.", UIControlState.Normal);
                    break;
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}