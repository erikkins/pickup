using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;

using Xamarin.Forms.Labs.iOS;

namespace PickUpApp.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate  //UIApplicationDelegate
	{
		UIWindow window;
		public static UIViewController MainView;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Forms.Init ();
			Xamarin.FormsMaps.Init ();
			CurrentPlatform.Init ();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			MainView =App.GetMainPage().CreateViewController();
			window.RootViewController = MainView; //App.GetMainPage ().CreateViewController ();
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

