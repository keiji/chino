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
using Java.IO;

namespace Sample.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", Icon = "@mipmap/ic_launcher", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int REQUEST_EN_START = 0x10;
        private const int REQUEST_GET_TEK_HISTORY = 0x11;

        private const string DIAGNOSIS_KEYS_DIR = "diagnosis_keys";

        private AbsExposureNotificationClient? EnClient = null;

        private Button? buttonEn = null;
        private Button? buttonGetTekHistory = null;
        private Button? buttonProvideDiagnosisKeys = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

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

                await ShowTekHistory();
            };

            buttonProvideDiagnosisKeys = FindViewById<Button>(Resource.Id.btn_provide_diagnosis_keys);
            buttonProvideDiagnosisKeys.Click += async delegate
            {
                Logger.D("buttonProvideDiagnosisKeys clicked");

                await ProvideDiagnosisKeys();
            };

        }

        private async Task ProvideDiagnosisKeys()
        {
            File diagnosisKeyDir = new File(FilesDir, DIAGNOSIS_KEYS_DIR);
            if (!diagnosisKeyDir.Exists())
            {
                diagnosisKeyDir.Mkdirs();
            }

            File[] diagnosisKeyFiles = await diagnosisKeyDir.ListFilesAsync();
            List<string> diagnosisKeyPaths = diagnosisKeyFiles.ToList()
                .FindAll(file => file.IsFile)
                .Select(file => file.AbsolutePath).ToList()
                .FindAll(file => file.EndsWith(".zip"));

            try
            {
                await EnClient.ProvideDiagnosisKeys(diagnosisKeyPaths);
            }
            catch (ApiException apiException)
            {
                Logger.D($"ProvideDiagnosisKeys ApiException {apiException.StatusCode}");
            }
        }

        private async Task ShowTekHistory()
        {
            if (buttonGetTekHistory == null)
            {
                return;
            }

            List<ITemporaryExposureKey> teks = await GetTekHistory();
            Logger.D(teks, RiskLevel.Highest.ToInt(), ReportType.ConfirmedTest);

            List<string> tekKeyData = teks.Select(tek => Convert.ToBase64String(tek.KeyData)).ToList();
            string str = string.Join("\n", tekKeyData);

            buttonGetTekHistory.Text = str;
        }

        private async Task<List<ITemporaryExposureKey>> GetTekHistory()
        {
            Logger.D("GetTekHistory");
            try
            {
                return await EnClient.GetTemporaryExposureKeyHistory();
            }
            catch (ApiException apiException)
            {
                Logger.D($"GetTekHistory ApiException {apiException.StatusCode}");

                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(this, REQUEST_GET_TEK_HISTORY);
                }
            }

            return new List<ITemporaryExposureKey>();
        }

        private async Task EnableEnAsync()
        {
            Logger.D("EnableEnAsync");
            try
            {
                await EnClient.Start();

                long version = await EnClient.GetVersion();
                Logger.D($"Version: {version}");
            }
            catch (ApiException apiException)
            {
                Logger.D($"EnableEnAsync ApiException {apiException.StatusCode}");

                if (apiException.StatusCode == CommonStatusCodes.ResolutionRequired)
                {
                    apiException.Status.StartResolutionForResult(this, REQUEST_EN_START);
                }
            }
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
                    DisableEnButton();
                    break;
                case REQUEST_GET_TEK_HISTORY:
                    await ShowTekHistory();
                    break;
            }
        }

        private void DisableEnButton()
        {
            if (buttonEn == null)
            {
                return;
            }

            buttonEn.Text = "Exposure Notification is Enabled";
            buttonEn.Enabled = false;
        }
    }
}