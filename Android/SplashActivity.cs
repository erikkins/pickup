
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PickUpApp.droid
{
	[Activity (Label = "FamFetch", Theme = "@style/FFTheme.Splash", MainLauncher=true, NoHistory=true )]			
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			System.Threading.Thread.Sleep(500); //Let's wait awhile...
			this.StartActivity(typeof(MainActivity));
		}
	}
}

