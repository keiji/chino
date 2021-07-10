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
using Chino.Common;
using Java.IO;
using Java.Lang;
using Newtonsoft.Json;
using Sample.Common.Model;
using Xamarin.Essentials;
using Logger = Chino.ChinoLogger;

namespace Sample.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", Icon = "@mipmap/ic_launcher", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int REQUEST_EN_START = 0x10;
        private const int REQUEST_GET_TEK_HISTORY = 0x11;
        private const int REQUEST_PREAUTHORIZE_KEYS = 0x12;

        private const string TEKS_DIR = "temporary_exposure_keys";
        private const string EXPOSURE_DETECTION = "exposure_detection";
        private const string EXPOSURE_CONFIGURATION_FILENAME = "exposure_configuration.json";

        private AbsExposureNotificationClient? EnClient = null;

        private Button? buttonEn = null;
        private Button? buttonGetTekHistory = null;
        private Button? buttonProvideDiagnosisKeys = null;
        private Button? buttonRequestPreauthorizedKeys = null;
        private Button? buttonRequestReleaseKeys = null;
        private TextView? status = null;

        private File _teksDir;
        private File _exposureDetectionDir;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            InitializeDirs();

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

                List<ITemporaryExposureKey> teks = await GetTekHistory();

                ShowTekHistory(teks);
                await SaveTekHistoryAsync(teks);
            };

            buttonProvideDiagnosisKeys = FindViewById<Button>(Resource.Id.btn_provide_diagnosis_keys);
            buttonProvideDiagnosisKeys.Click += async delegate
            {
                Logger.D("buttonProvideDiagnosisKeys clicked");

                await ProvideDiagnosisKeys();
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

            status = FindViewById<TextView>(Resource.Id.tv_status);
        }

        protected override async void OnStart()
        {
            base.OnStart();

            buttonProvideDiagnosisKeys.Enabled = false;

            await InitializeExposureConfiguration();

            await InitializeExposureNotificationApiStatus();

            buttonProvideDiagnosisKeys.Enabled = true;

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

        private void InitializeDirs()
        {
            _teksDir = new File(FilesDir, TEKS_DIR);
            if (!_teksDir.Exists())
            {
                _teksDir.Mkdirs();
            }

            _exposureDetectionDir = new File(FilesDir, EXPOSURE_DETECTION);
            if (!_exposureDetectionDir.Exists())
            {
                _exposureDetectionDir.Mkdirs();
            }
        }

        private ExposureConfiguration _exposureConfiguration;

        private async Task InitializeExposureConfiguration()
        {
            var exposureConfigurationPath = new File(_exposureDetectionDir, EXPOSURE_CONFIGURATION_FILENAME);
            if (exposureConfigurationPath.Exists())
            {
                using BufferedReader br = new BufferedReader(new FileReader(exposureConfigurationPath));
                StringBuilder sb = new StringBuilder();
                string str = null;
                while ((str = await br.ReadLineAsync()) != null)
                {
                    sb.Append(str);
                }
                _exposureConfiguration = JsonConvert.DeserializeObject<ExposureConfiguration>(sb.ToString());
                return;
            }

            _exposureConfiguration = new ExposureConfiguration();
            var json = JsonConvert.SerializeObject(_exposureConfiguration, Formatting.Indented);

            using BufferedWriter bw = new BufferedWriter(new FileWriter(exposureConfigurationPath));
            await bw.WriteAsync(json);
            await bw.FlushAsync();
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

        private async Task ProvideDiagnosisKeys()
        {
            File[] diagnosisKeyFiles = await _exposureDetectionDir.ListFilesAsync();
            List<string> diagnosisKeyPaths = diagnosisKeyFiles.ToList()
                .FindAll(file => file.IsFile)
                .Select(file => file.AbsolutePath).ToList()
                .FindAll(file => file.EndsWith(".zip"));

            if(diagnosisKeyFiles.Length == 0)
            {
                status.Text = "No diagnosisKey file found";
                return;
            }

            try
            {
                await EnClient.ProvideDiagnosisKeysAsync(diagnosisKeyPaths, _exposureConfiguration);
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

        private void ShowTekHistory(List<ITemporaryExposureKey> temporaryExposureKeys)
        {
            if (buttonGetTekHistory == null)
            {
                return;
            }

            List<string> tekKeyData = temporaryExposureKeys.Select(tek => Convert.ToBase64String(tek.KeyData)).ToList();
            status.Text = string.Join("\n", tekKeyData);
        }

        private async Task SaveTekHistoryAsync(List<ITemporaryExposureKey> temporaryExposureKeys)
        {
            TemporaryExposureKeys teks = new TemporaryExposureKeys(temporaryExposureKeys, DateTime.Now)
            {
                Device = DeviceInfo.Model
            };

            string fileName = $"{teks.Id}.json";
            string json = teks.ToJsonString();
            Logger.D(json);

            File filePath = new File(_teksDir, fileName);
            using BufferedWriter bw = new BufferedWriter(new FileWriter(filePath));
            await bw.WriteAsync(json);
            await bw.FlushAsync();
        }

        private async Task<List<ITemporaryExposureKey>> GetTekHistory()
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

            return new List<ITemporaryExposureKey>();
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
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

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
                    List<ITemporaryExposureKey> teks = await GetTekHistory();
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