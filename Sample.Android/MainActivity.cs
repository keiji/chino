using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Chino;
using Newtonsoft.Json;
using Sample.Common;
using Sample.Common.Model;
using Xamarin.Essentials;
using System.IO;

using AndroidFile = Java.IO.File;

using Logger = Chino.ChinoLogger;

namespace Sample.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", Icon = "@mipmap/ic_launcher", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int REQUEST_EN_START = 0x10;
        private const int REQUEST_GET_TEK_HISTORY = 0x11;
        private const int REQUEST_PREAUTHORIZE_KEYS = 0x12;

        private AbsExposureNotificationClient? EnClient = null;

        private IDiagnosisKeyServer _diagnosisKeyServer;

        private Button? buttonEn = null;
        private Button? buttonGetTekHistory = null;
        private Button? buttonProvideDiagnosisKeys = null;
        private Button? buttonProvideDiagnosisKeysV1 = null;
        private Button? buttonRequestPreauthorizedKeys = null;
        private Button? buttonRequestReleaseKeys = null;

        private TextView? serverInfo = null;
        private Button? buttonDownloadDiagnosisKeys = null;
        private Button? buttonUploadDiagnosisKeys = null;

        private TextView? status = null;

        private AndroidFile _teksDir;
        private AndroidFile _configurationDir;
        private AndroidFile _exposureDetectionDir;

        private DiagnosisKeyServerConfiguration _diagnosisKeyServerConfiguration;
        private ExposureConfiguration _exposureConfiguration;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            PrepareDirs();

            EnClient = ((MainApplication)ApplicationContext).GetEnClient();

            buttonEn = FindViewById<Button>(Resource.Id.btn_enable_en);
            buttonEn.Click += async delegate
            {
                Logger.D("EnableEnAsync clicked");

                await EnableEnAsync();
            };

            buttonGetTekHistory = FindViewById<Button>(Resource.Id.btn_get_teks);
            buttonGetTekHistory.Click += async delegate
            {
                Logger.D("buttonGetTekHistory clicked");

                List<TemporaryExposureKey> teks = await GetTekHistory();

                ShowTekHistory(teks);
                await SaveTekHistoryAsync(teks);
            };

            buttonProvideDiagnosisKeys = FindViewById<Button>(Resource.Id.btn_provide_diagnosis_keys);
            buttonProvideDiagnosisKeys.Click += async delegate
            {
                Logger.D("buttonProvideDiagnosisKeys clicked");

                await ProvideDiagnosisKeys();
            };

            buttonProvideDiagnosisKeysV1 = FindViewById<Button>(Resource.Id.btn_provide_diagnosis_keys_legacy_v1);
            buttonProvideDiagnosisKeysV1.Click += async delegate
            {
                Logger.D("buttonProvideDiagnosisKeysV1 clicked");

                await ProvideDiagnosisKeysV1();
            };

            buttonRequestPreauthorizedKeys = FindViewById<Button>(Resource.Id.btn_request_preauthorize_keys);
            buttonRequestPreauthorizedKeys.Click += async delegate
            {
                Logger.D("buttonRequestPreauthorizedKeys clicked");

                await RequestPreAuthorizeKeys();
            };

            buttonRequestReleaseKeys = FindViewById<Button>(Resource.Id.btn_request_release_keys);
            buttonRequestReleaseKeys.Click += async delegate
            {
                Logger.D("buttonRequestReleaseKeys clicked");

                await RequestReleaseKeys();
            };

            buttonUploadDiagnosisKeys = FindViewById<Button>(Resource.Id.btn_upload_diagnosis_keys);
            buttonUploadDiagnosisKeys.Click += async delegate
            {
                Logger.D("buttonUploadDiagnosisKeys clicked");

                await UploadDiagnosisKeys();
            };

            buttonDownloadDiagnosisKeys = FindViewById<Button>(Resource.Id.btn_download_diagnosis_keys);
            buttonDownloadDiagnosisKeys.Click += async delegate
            {
                Logger.D("buttonDownloadDiagnosisKeys clicked");

                await DownloadDiagnosisKeys();
            };

            status = FindViewById<TextView>(Resource.Id.tv_status);

            serverInfo = FindViewById<TextView>(Resource.Id.tv_server_info);
        }

        private async Task UploadDiagnosisKeys()
        {
            Logger.D("UploadDiagnosisKeys");
            status.Text = "UploadDiagnosisKeys is clicked.\n";

            try
            {
                IList<TemporaryExposureKey> teks = await EnClient.GetTemporaryExposureKeyHistoryAsync();
                DateTime symptomOnsetDate = DateTime.UtcNow.Date - TimeSpan.FromDays(teks.Count / 2);
                string idempotencyKey = Guid.NewGuid().ToString();

                await _diagnosisKeyServer.UploadDiagnosisKeysAsync(symptomOnsetDate, teks, idempotencyKey);

                status.Append($"diagnosisKeyEntryList have been uploaded.\n");

            }
            catch (ENException enException)
            {
                ShowENException(enException);
            }
            catch (ApiException apiException)
            {
                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(this, REQUEST_GET_TEK_HISTORY);
                }
                else
                {
                    ShowApiException("GetTekHistory", apiException);
                }
            }
        }

        private async Task DownloadDiagnosisKeys()
        {
            Logger.D("DownloadDiagnosisKeys");
            status.Text = "DownloadDiagnosisKeys is clicked.\n";

            var diagnosisKeyEntryList = await _diagnosisKeyServer.GetDiagnosisKeysListAsync();
            foreach(var diagnosisKeyEntry in diagnosisKeyEntryList)
            {
                await _diagnosisKeyServer.DownloadDiagnosisKeysAsync(diagnosisKeyEntry, _exposureDetectionDir.AbsolutePath);

                status.Append($"{diagnosisKeyEntry.Url} has been downloaded.\n");
            }
        }

        protected override async void OnStart()
        {
            base.OnStart();

            buttonProvideDiagnosisKeys.Enabled = false;
            buttonProvideDiagnosisKeysV1.Enabled = false;

            _diagnosisKeyServerConfiguration = await LoadDiagnosisKeyServerConfiguration();
            serverInfo.Text = $"Endpoint: {_diagnosisKeyServerConfiguration.ApiEndpoint}\n";
            serverInfo.Append($"Cluster ID: {_diagnosisKeyServerConfiguration.ClusterId}");

            _diagnosisKeyServer = new DiagnosisKeyServer(_diagnosisKeyServerConfiguration);

            _exposureConfiguration = await LoadExposureConfiguration();

            await InitializeExposureNotificationApiStatus();

            buttonProvideDiagnosisKeys.Enabled = true;
            buttonProvideDiagnosisKeysV1.Enabled = true;
        }

        private async Task InitializeExposureNotificationApiStatus()
        {
            try
            {
                var enStatuses = await EnClient.GetStatusesAsync();
                var version = await EnClient.GetVersionAsync();
                ShowStatus(enStatuses, version);
            }
            catch (ENException enException)
            {
                ShowENException(enException);
            }
            catch (ApiException apiException)
            {
                ShowApiException("GetStatusAsync GetVersionAsync", apiException);
            }
        }

        private void PrepareDirs()
        {
            _teksDir = new AndroidFile(FilesDir, Constants.TEKS_DIR);
            if (!_teksDir.Exists())
            {
                _teksDir.Mkdirs();
            }

            _configurationDir = new AndroidFile(FilesDir, Constants.CONFIGURATION_DIR);
            if (!_configurationDir.Exists())
            {
                _configurationDir.Mkdirs();
            }

            _exposureDetectionDir = new AndroidFile(FilesDir, Constants.EXPOSURE_DETECTION_DIR);
            if (!_exposureDetectionDir.Exists())
            {
                _exposureDetectionDir.Mkdirs();
            }
        }

        private async Task<ExposureConfiguration> LoadExposureConfiguration()
        {
            var exposureConfigurationPath = new AndroidFile(_configurationDir, Constants.EXPOSURE_CONFIGURATION_FILENAME);
            if (exposureConfigurationPath.Exists())
            {
                string content = await File.ReadAllTextAsync(exposureConfigurationPath.AbsolutePath);
                return JsonConvert.DeserializeObject<ExposureConfiguration>(content);
            }

            var exposureConfiguration = new ExposureConfiguration();
            var json = JsonConvert.SerializeObject(exposureConfiguration, Formatting.Indented);
            await File.WriteAllTextAsync(exposureConfigurationPath.AbsolutePath, json);

            return exposureConfiguration;
        }

        private async Task<DiagnosisKeyServerConfiguration> LoadDiagnosisKeyServerConfiguration()
        {
            var diagnosisKeyserverConfigurationPath = new AndroidFile(_configurationDir, Constants.DIAGNOSIS_KEY_SERVER_CONFIGURATION_FILENAME);
            if (diagnosisKeyserverConfigurationPath.Exists())
            {
                var content = await File.ReadAllTextAsync(diagnosisKeyserverConfigurationPath.AbsolutePath);
                return JsonConvert.DeserializeObject<DiagnosisKeyServerConfiguration>(content);
            }

            var diagnosisKeyServerConfiguration = new DiagnosisKeyServerConfiguration();
            var json = JsonConvert.SerializeObject(diagnosisKeyServerConfiguration, Formatting.Indented);
            await File.WriteAllTextAsync(diagnosisKeyserverConfigurationPath.AbsolutePath, json);

            return diagnosisKeyServerConfiguration;
        }

        private async Task RequestReleaseKeys()
        {
            Logger.D("RequestReleaseKeys");
            try
            {
                await EnClient.RequestPreAuthorizedTemporaryExposureKeyReleaseAsync();
            }
            catch (ENException enException)
            {
                ShowENException(enException);
            }
            catch (ApiException apiException)
            {
                ShowApiException("RequestReleaseKeys", apiException);
            }

        }

        private async Task RequestPreAuthorizeKeys()
        {
            Logger.D("RequestPreAuthorizeKeys");
            try
            {
                await EnClient.RequestPreAuthorizedTemporaryExposureKeyHistoryAsync();
                Logger.D("RequestPreAuthorizeKeys Success");
            }
            catch (ENException enException)
            {
                ShowENException(enException);
            }
            catch (ApiException apiException)
            {
                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(this, REQUEST_PREAUTHORIZE_KEYS);
                }
                else
                {
                    ShowApiException("RequestPreAuthorizeKeys", apiException);
                }
            }
        }

        private async Task<List<string>> PrepareDiagnosisKeyFilesAsync()
        {
            AndroidFile[] diagnosisKeyFiles = await _exposureDetectionDir.ListFilesAsync();
            return diagnosisKeyFiles.ToList()
                .FindAll(file => file.IsFile)
                .Select(file => file.AbsolutePath).ToList()
                .FindAll(file => file.EndsWith(".zip"));
        }

        private async Task ProvideDiagnosisKeysV1()
        {
            var diagnosisKeyPaths = await PrepareDiagnosisKeyFilesAsync();
            if (diagnosisKeyPaths.Count == 0)
            {
                status.Text = "No diagnosisKey file found";
                return;
            }

            try
            {
                await EnClient.ProvideDiagnosisKeysAsync(
                    diagnosisKeyPaths,
                    _exposureConfiguration,
                    Guid.NewGuid().ToString()
                    );
            }
            catch (TaskCanceledException exception)
            {
                Logger.E(exception);
            }
            catch (ENException enException)
            {
                ShowENException(enException);
            }
            catch (ApiException apiException)
            {
                ShowApiException("ProvideDiagnosisKeys", apiException);
            }
        }

        private async Task ProvideDiagnosisKeys()
        {
            var diagnosisKeyPaths = await PrepareDiagnosisKeyFilesAsync();
            if (diagnosisKeyPaths.Count == 0)
            {
                status.Text = "No diagnosisKey file found";
                return;
            }

            try
            {
                await EnClient.ProvideDiagnosisKeysAsync(diagnosisKeyPaths, _exposureConfiguration);
            }
            catch (TaskCanceledException exception)
            {
                Logger.E(exception);
            }
            catch (ENException enException)
            {
                ShowENException(enException);
            }
            catch (ApiException apiException)
            {
                ShowApiException("ProvideDiagnosisKeys", apiException);
            }
        }

        private void ShowTekHistory(List<TemporaryExposureKey> temporaryExposureKeys)
        {
            if (buttonGetTekHistory == null)
            {
                return;
            }

            List<string> tekKeyData = temporaryExposureKeys.Select(tek => Convert.ToBase64String(tek.KeyData)).ToList();
            status.Text = string.Join("\n", tekKeyData);
        }

        private async Task SaveTekHistoryAsync(List<TemporaryExposureKey> temporaryExposureKeys)
        {
            TemporaryExposureKeys teks = new TemporaryExposureKeys(temporaryExposureKeys, DateTime.Now)
            {
                Device = DeviceInfo.Model
            };

            string fileName = $"{teks.Id}.json";
            string json = teks.ToJsonString();
            Logger.D(json);

            AndroidFile filePath = new AndroidFile(_teksDir, fileName);
            await File.WriteAllTextAsync(filePath.AbsolutePath, json);
        }

        private async Task<List<TemporaryExposureKey>> GetTekHistory()
        {
            Logger.D("GetTekHistory");
            try
            {
                return await EnClient.GetTemporaryExposureKeyHistoryAsync();
            }
            catch (ENException enException)
            {
                ShowENException(enException);
            }
            catch (ApiException apiException)
            {
                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(this, REQUEST_GET_TEK_HISTORY);
                }
                else
                {
                    ShowApiException("GetTekHistory", apiException);
                }
            }

            return new List<TemporaryExposureKey>();
        }

        private async Task EnableEnAsync()
        {
            Logger.D("EnableEnAsync");
            try
            {
                await EnClient.StartAsync();

                long version = await EnClient.GetVersionAsync();
                Logger.D($"Version: {version}");
            }
            catch (ENException enException)
            {
                ShowENException(enException);
            }
            catch (ApiException apiException)
            {
                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(this, REQUEST_EN_START);
                }
                else
                {
                    ShowApiException("EnableEnAsync", apiException);
                }
            }
        }

        private void ShowStatus(IList<ExposureNotificationStatus> enStatuses, long version)
        {
            status.Text = $"EN version: {version}\n";
            status.Text += string.Join("\n", enStatuses.Select(status => ConvertToStatus(status)).ToList());
        }

        private static string ConvertToStatus(ExposureNotificationStatus enStatus)
        {
            return enStatus.Code switch
            {
                ExposureNotificationStatus.Code_Android.ACTIVATED => $"EN is ACTIVATED",
                ExposureNotificationStatus.Code_Android.BLUETOOTH_DISABLED => $"EN is BLUETOOTH_DISABLED",
                ExposureNotificationStatus.Code_Android.BLUETOOTH_SUPPORT_UNKNOWN => $"EN is BLUETOOTH_SUPPORT_UNKNOWN",
                ExposureNotificationStatus.Code_Android.EN_NOT_SUPPORT => $"EN is EN_NOT_SUPPORT",
                ExposureNotificationStatus.Code_Android.FOCUS_LOST => $"EN is FOCUS_LOST",
                ExposureNotificationStatus.Code_Android.HW_NOT_SUPPORT => $"EN is HW_NOT_SUPPORT",
                ExposureNotificationStatus.Code_Android.INACTIVATED => $"EN is INACTIVATED",
                ExposureNotificationStatus.Code_Android.LOCATION_DISABLED => $"EN is LOCATION_DISABLED",
                ExposureNotificationStatus.Code_Android.LOW_STORAGE => $"EN is LOW_STORAGE",
                ExposureNotificationStatus.Code_Android.NOT_IN_ALLOWLIST => $"EN is NOT_IN_ALLOWLIST",
                ExposureNotificationStatus.Code_Android.NO_CONSENT => $"EN is NO_CONSENT",
                ExposureNotificationStatus.Code_Android.USER_PROFILE_NOT_SUPPORT => $"EN is USER_PROFILE_NOT_SUPPORT",
                _ => "ENStatus is Unknown"
            };
        }

        private void ShowENException(ENException enException)
        {
            Logger.D($"ENException - Code:{enException.Code}, {enException.Message}");

            string message = enException.Code switch
            {
                ENException.Code_Android.FAILED => "FAILED",
                ENException.Code_Android.FAILED_ALREADY_STARTED => "FAILED_ALREADY_STARTED",
                ENException.Code_Android.FAILED_BLUETOOTH_DISABLED => "FAILED_BLUETOOTH_DISABLED",
                ENException.Code_Android.FAILED_DISK_IO => "FAILED_DISK_IO",
                ENException.Code_Android.FAILED_KEY_RELEASE_NOT_PREAUTHORIZED => "FAILED_KEY_RELEASE_NOT_PREAUTHORIZED",
                ENException.Code_Android.FAILED_NOT_IN_FOREGROUND => "FAILED_NOT_IN_FOREGROUND",
                ENException.Code_Android.FAILED_NOT_SUPPORTED => "FAILED_NOT_SUPPORTED",
                ENException.Code_Android.FAILED_RATE_LIMITED => "FAILED_RATE_LIMITED",
                ENException.Code_Android.FAILED_REJECTED_OPT_IN => "FAILED_REJECTED_OPT_IN",
                ENException.Code_Android.FAILED_SERVICE_DISABLED => "FAILED_SERVICE_DISABLED",
                ENException.Code_Android.FAILED_TEMPORARILY_DISABLED => "FAILED_TEMPORARILY_DISABLED",
                ENException.Code_Android.FAILED_UNAUTHORIZED => "FAILED_UNAUTHORIZED",
                _ => "Unknown",
            };

            status.Text = $"ENException: {message}";
        }

        private void ShowApiException(string callFrom, ApiException apiException)
        {
            Logger.D($"{callFrom} ApiException {apiException.StatusCode}");

            status.Text = $"ApiException: {apiException.Message}";
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
            {
                return;
            }

            switch (requestCode)
            {
                case REQUEST_EN_START:
                    await EnableEnAsync();
                    ShowEnEnabled();
                    break;
                case REQUEST_GET_TEK_HISTORY:
                    List<TemporaryExposureKey> teks = await GetTekHistory();
                    ShowTekHistory(teks);
                    await SaveTekHistoryAsync(teks);
                    break;
                case REQUEST_PREAUTHORIZE_KEYS:
                    await RequestPreAuthorizeKeys();
                    break;
            }
        }

        private void ShowEnEnabled()
        {
            if (buttonEn == null)
            {
                return;
            }

            status.Text = "Exposure Notification is enabled.";
            buttonEn.Text = "Exposure Notification is enabled";
            buttonEn.Enabled = false;
        }
    }
}