using System;
using PickUpApp.iOS;
using System.IO;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency (typeof(iOSDeviceInfo))]
namespace PickUpApp.iOS
{
	public class iOSDeviceInfo :IDeviceInfo
	{
		public string AppVersion
		{
			get
			{
				return NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString ();
			}
		}

		public void LoadDeviceSettings()
		{			
			UIApplication.SharedApplication.OpenUrl (new NSUrl (UIApplication.OpenSettingsUrlString));
		}
	}
}

