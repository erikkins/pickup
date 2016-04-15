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
using ByteSmith.WindowsAzure.Messaging;
using Gcm.Client;
using Xamarin;
using XLabs.Ioc;
using XLabs.Platform.Services;
using XLabs.Platform.Services.IO;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;
using HockeyApp;
using System.Threading.Tasks;


namespace PickUpApp.droid
{
	[Activity (Label = "FamFetch", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity //XFormsApplicationDroid //AndroidActivity
	{
		private GestureDetector _gestureDetector;
		public const string HOCKEYAPP_APPID = "ed87bdba01244d8b82ed8df1ec5a7690";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);
			Xamarin.FormsMaps.Init (this, bundle);
			ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init ();
			//Refractored.Xam.Forms.Vibrate.Droid.Vibrate.Init ();

			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
			//Insights.Initialize("882979cfc38cb829ecfaf090e99781b90980c55a", this);

			//start hockeyapp
			// Register the crash manager before Initializing the trace writer
			HockeyApp.CrashManager.Register (this, HOCKEYAPP_APPID); 

			//Register to with the Update Manager
			HockeyApp.UpdateManager.Register (this, HOCKEYAPP_APPID);

			// Initialize the Trace Writer
			HockeyApp.TraceWriter.Initialize ();

			// Wire up Unhandled Expcetion handler from Android
			AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) => 
			{
				// Use the trace writer to log exceptions so HockeyApp finds them
				HockeyApp.TraceWriter.WriteTrace(args.Exception);
				args.Handled = true;
			};

			// Wire up the .NET Unhandled Exception handler
			AppDomain.CurrentDomain.UnhandledException +=
				(sender, args) => HockeyApp.TraceWriter.WriteTrace(args.ExceptionObject);

			// Wire up the unobserved task exception handler
			TaskScheduler.UnobservedTaskException += 
				(sender, args) => HockeyApp.TraceWriter.WriteTrace(args.Exception);
			//end hockeyapp



			//NOTE:  VERY IMPORTANT
			//DO NOT FORGET TO ADD THE INIT STUFF FOR IDEVICE, etc. HERE
			//TODO:THIS
			var container = new SimpleContainer ();
			container.Register<IDevice> (t => AndroidDevice.CurrentDevice);
			container.Register<IDisplay> (t => t.Resolve<IDevice> ().Display);
			container.Register<INetwork>(t=> t.Resolve<IDevice>().Network);
			Resolver.SetResolver (container.GetResolver ());


			//SetPage (App.Current.MainPage);

			LoadApplication (new App ());

			ActionBar.SetIcon (new Android.Graphics.Drawables.ColorDrawable (Android.Graphics.Color.Transparent));

			RegisterWithGCM ();
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

		private void RegisterWithGCM()
		{
			// Check to ensure everything's setup right
			GcmClient.CheckDevice(this);
			GcmClient.CheckManifest(this);

			// Register for push notifications
			//System.Diagnostics.Debug.WriteLine("Registering...");

			var preferences = GetSharedPreferences("AppData", FileCreationMode.Private);
			var deviceId = preferences.GetString("DeviceId","");

			if (string.IsNullOrEmpty (deviceId)) {
				GcmClient.Register(this, Constants.SenderID);
			}
		}
	}


}

