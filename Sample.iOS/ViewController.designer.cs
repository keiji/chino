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
		UIKit.UIButton buttonEnableEn { get; set; }

		[Outlet]
		UIKit.UIButton buttonShowTeksHistory { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonEnableEn != null) {
				buttonEnableEn.Dispose ();
				buttonEnableEn = null;
			}

			if (buttonShowTeksHistory != null) {
				buttonShowTeksHistory.Dispose ();
				buttonShowTeksHistory = null;
			}
		}
	}
}
