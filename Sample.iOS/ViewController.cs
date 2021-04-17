using Chino;
using System;
using System.Diagnostics;
using UIKit;

namespace Sample.iOS
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            buttonEnableEn.TouchUpInside += async (sender, e) =>
            {
                Debug.Print("buttonEnableEn");
                await ExposureNotificationClient.Shared.Start();
            };
            buttonShowTeksHistory.TouchUpInside += (sender, e) =>
            {
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}