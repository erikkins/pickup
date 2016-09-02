using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace PickUpApp.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			//Xamarin.Insights.Initialize (XamarinInsights.ApiKey);
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init ();
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
