using Chino;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace Sample.iOS
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            Debug.Print("ViewDidLoad");

            await ExposureNotificationClient.Shared.Init();
            await ShowStatusAsync();

            buttonEnableEn.TouchUpInside += async (sender, e) =>
            {
                Debug.Print("buttonEnableEn");
                await ExposureNotificationClient.Shared.Start();
                await ShowStatusAsync();
            };
            buttonShowTeksHistory.TouchUpInside += async (sender, e) =>
            {
                await ShowTeksAsync();
            };
        }

        private async Task ShowTeksAsync()
        {
            List<TemporaryExposureKey> teks = await ExposureNotificationClient.Shared.GetTemporaryExposureKeyHistory();
            List<string> tekKeyData = teks.Select(teks => Convert.ToBase64String(teks.KeyData)).ToList();
            var str = string.Join("\n", tekKeyData);
            buttonShowTeksHistory.SetTitle(str, UIControlState.Normal);
        }

        private async Task ShowStatusAsync()
        {
            await Task.Delay(1000);

            IExposureNotificationStatus status = await ExposureNotificationClient.Shared.GetStatus();

            switch (status.Status())
            {
                case Status.Active:
                    buttonEnableEn.SetTitle("EN is Active.", UIControlState.Normal);
                    break;
                case Status.NotActive:
                    buttonEnableEn.SetTitle("EN is NotActive.", UIControlState.Normal);
                    break;
                case Status.BluetoothOff:
                    buttonEnableEn.SetTitle("EN is BluetoothOff.", UIControlState.Normal);
                    break;
                case Status.Unauthorized:
                    buttonEnableEn.SetTitle("EN is Unauthorized.", UIControlState.Normal);
                    break;
                case Status.Unknown:
                    buttonEnableEn.SetTitle("EN is Unknown.", UIControlState.Normal);
                    break;
                case Status.Misc:
                    buttonEnableEn.SetTitle("EN is Misc.", UIControlState.Normal);
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