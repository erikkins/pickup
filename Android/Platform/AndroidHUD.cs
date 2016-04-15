using System;
using Android.App;
using PickUpApp.droid;
using Xamarin.Forms;
[assembly: Xamarin.Forms.Dependency(typeof(AndroidPickUpHUD))]
namespace PickUpApp.droid
{
	public class AndroidPickUpHUD:IHUD
	{
		public void showHUD(string what)
		{
	
			AndroidHUD.AndHUD.Shared.Show (Xamarin.Forms.Forms.Context, what);
		
		}

		public void hideHUD()
		{
			AndroidHUD.AndHUD.Shared.Dismiss (Xamarin.Forms.Forms.Context);
		}

		public void showToast(string what)
		{
			AndroidHUD.AndHUD.Shared.ShowToast (Xamarin.Forms.Forms.Context, what, AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(3));
		}
	}
}

