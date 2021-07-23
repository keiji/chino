// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Sample.iOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton buttonDetectExposure { get; set; }

		[Outlet]
		UIKit.UIButton buttonDownloadDiagnosisKeys { get; set; }

		[Outlet]
		UIKit.UIButton buttonEnableEn { get; set; }

		[Outlet]
		UIKit.UIButton buttonRequestPreauthorizedKeys { get; set; }

		[Outlet]
		UIKit.UIButton buttonRequestReleaseKeys { get; set; }

		[Outlet]
		UIKit.UIButton buttonShowTeksHistory { get; set; }

		[Outlet]
		UIKit.UIButton buttonUploadDiagnosisKeys { get; set; }

		[Outlet]
		UIKit.UILabel serverInfo { get; set; }

		[Outlet]
		UIKit.UILabel status { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonDetectExposure != null) {
				buttonDetectExposure.Dispose ();
				buttonDetectExposure = null;
			}

			if (buttonDownloadDiagnosisKeys != null) {
				buttonDownloadDiagnosisKeys.Dispose ();
				buttonDownloadDiagnosisKeys = null;
			}

			if (buttonEnableEn != null) {
				buttonEnableEn.Dispose ();
				buttonEnableEn = null;
			}

			if (buttonRequestPreauthorizedKeys != null) {
				buttonRequestPreauthorizedKeys.Dispose ();
				buttonRequestPreauthorizedKeys = null;
			}

			if (buttonRequestReleaseKeys != null) {
				buttonRequestReleaseKeys.Dispose ();
				buttonRequestReleaseKeys = null;
			}

			if (buttonShowTeksHistory != null) {
				buttonShowTeksHistory.Dispose ();
				buttonShowTeksHistory = null;
			}

			if (buttonUploadDiagnosisKeys != null) {
				buttonUploadDiagnosisKeys.Dispose ();
				buttonUploadDiagnosisKeys = null;
			}

			if (serverInfo != null) {
				serverInfo.Dispose ();
				serverInfo = null;
			}

			if (status != null) {
				status.Dispose ();
				status = null;
			}
		}
	}
}
