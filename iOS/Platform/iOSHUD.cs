using System;
using PickUpApp.iOS;
using BigTed;

[assembly: Xamarin.Forms.Dependency (typeof(iOSHUD))]
namespace PickUpApp.iOS
{
	public class iOSHUD: IHUD
	{
		public void showHUD(string what)
		{
			BTProgressHUD.Show (what);
		}

		public void hideHUD()
		{
			BTProgressHUD.Dismiss ();
		}

		public void showToast(string what)
		{
			BTProgressHUD.ShowToast (what, true, 1500);
		}
	}
}

