using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gestures;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Labs.Droid;

namespace PickUpApp.Android
{
	[Activity (Label = "PickUpApp", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : XFormsApplicationDroid //AndroidActivity
	{
		private GestureDetector _gestureDetector;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);
			Xamarin.FormsMaps.Init (this, bundle);
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
			SetPage (App.GetMainPage ());

			var listener = new GestureListener();
			_gestureDetector = new GestureDetector(this, listener);
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			_gestureDetector.OnTouchEvent (e);
			return base.OnTouchEvent (e);
		}
			
		private class GestureListener : GestureDetector.SimpleOnGestureListener
		{
			public override void OnLongPress (MotionEvent e)
			{
				base.OnLongPress (e);
			}
			public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)	
			{
				return true;
			}
		}
	}


}

