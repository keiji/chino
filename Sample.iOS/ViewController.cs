using Chino;
using Chino.Common;
using Chino.iOS;
using Foundation;
using Newtonsoft.Json;
using Sample.Common;
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

        private IEnServer _enServer = new EnServer();

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

            IList<ExposureNotificationStatus> statuses = await ExposureNotificationClientManager.Shared.GetStatusesAsync();
            long version = await ExposureNotificationClientManager.Shared.GetVersionAsync();
            ShowStatus(statuses, version);

            buttonEnableEn.TouchUpInside += async (sender, e) =>
            {
                Logger.D("buttonEnableEn");

                try
                {
                    await ExposureNotificationClientManager.Shared.StartAsync();

                    IList<ExposureNotificationStatus> statuses = await ExposureNotificationClientManager.Shared.GetStatusesAsync();
                    long version = await ExposureNotificationClientManager.Shared.GetVersionAsync();
                    ShowStatus(statuses, version);
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
            buttonUploadDiagnosisKeys.TouchUpInside += async (sender, e) =>
            {
                try
                {
                    await UploadDiagnosisKeys();
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
            buttonDownloadDiagnosisKeys.TouchUpInside += async (sender, e) =>
            {
                try
                {
                    await DownloadDiagnosisKeys();
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

        private async Task UploadDiagnosisKeys()
        {
            Logger.D("UploadDiagnosisKeys");
            status.Text = "UploadDiagnosisKeys is clicked.\n";

            List<ITemporaryExposureKey> teks = await ExposureNotificationClientManager.Shared.GetTemporaryExposureKeyHistoryAsync();
            await _enServer.UploadDiagnosisKeysAsync(Constants.CLUSTER_ID, teks);

            status.Text += $"diagnosisKeyEntryList have been uploaded.\n";
        }

        private async Task DownloadDiagnosisKeys()
        {
            Logger.D("DownloadDiagnosisKeys");
            status.Text = "DownloadDiagnosisKeys is clicked.\n";

            var diagnosisKeyEntryList = await _enServer.GetDiagnosisKeysListAsync(Constants.CLUSTER_ID);
            foreach (var diagnosisKeyEntry in diagnosisKeyEntryList)
            {
                await _enServer.DownloadDiagnosisKeysAsync(diagnosisKeyEntry, _exposureDetectionDir);
                status.Text += $"{diagnosisKeyEntry.Url} has been downloaded.\n";
            }
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

            status.Text = $"ENException: {message}";
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
            status.Text = string.Join("\n", tekKeyData);
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

        private void ShowStatus(IList<ExposureNotificationStatus> statuses, long version)
        {
            status.Text = $"EN Version: {version}\n";
            status.Text += string.Join("\n", statuses.Select(status => $"EN is {ConvertToStatus(status)}"));
        }

        private static string ConvertToStatus(ExposureNotificationStatus status)
        {
            return status.Code switch
            {
                ExposureNotificationStatus.Code_iOS.Active => "Active",
                ExposureNotificationStatus.Code_iOS.BluetoothOff => "BluetoothOff",
                ExposureNotificationStatus.Code_iOS.Disabled => "Disabled",
                ExposureNotificationStatus.Code_iOS.Paused => "Paused",
                ExposureNotificationStatus.Code_iOS.Restricted => "Restricted",
                ExposureNotificationStatus.Code_iOS.Unauthorized => "Unauthorized",
                _ => "Unknown",
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}