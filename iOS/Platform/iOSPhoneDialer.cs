using System;
using PickUpApp.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency (typeof(iOSPhoneDialer))]
namespace PickUpApp.iOS
{
	public class iOSPhoneDialer : IPhoneDialer
	{
		public iOSPhoneDialer(){
		}

		public void test()
		{
			DialPhone ("+17736191320");
		}

		public void DialPhone (string phoneNumber)
		{
			var url = new Uri ("tel:" + phoneNumber);

			if (UIApplication.SharedApplication.CanOpenUrl (url)) {
				UIApplication.SharedApplication.OpenUrl (url);
			} else {
				var av = new UIAlertView ("Not supported",
					"Scheme 'tel:' is not supported on this device",
					null,
					"OK",
					null);
				av.Show ();
			}
		}
	}
}

