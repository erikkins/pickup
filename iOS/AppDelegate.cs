using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using WindowsAzure.Messaging;
using Xamarin.Forms.Labs;
using Xamarin.Forms.Labs.iOS;
using Xamarin.Forms.Labs.Services;

namespace PickUpApp.iOS
{

	[Register ("AppDelegate")]
	public partial class AppDelegate : 
	global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate // superclass new in 1.3
	{
		//UIWindow window;
		//public static UIKit.UIViewController  MainView;
		NSData deviceNotificationToken;

		public override bool FinishedLaunching (UIKit.UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();
			Xamarin.FormsMaps.Init ();
			ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init ();
			Refractored.Xam.Forms.Vibrate.iOS.Vibrate.Init ();

			// Register for Notifications
			if (Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0].ToString()) < 8) {
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
			} else {
				UIUserNotificationType notificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
				var settings = UIUserNotificationSettings.GetSettingsForTypes(notificationTypes, new NSSet (new string[] {}));
				UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications ();
			}
				

			// Process any potential notification data from launch
			ProcessNotification (options, true);

			MessagingCenter.Subscribe<Account> (this, "loaded", (s) => {
				//can I do the NotificationHub stuff here?
				// Register our info with Azure
				// Connection string from your azure dashboard
				var cs = SBConnectionString.CreateListenAccess(
					new NSUrl("sb://pickuphub-ns.servicebus.windows.net/"),
					"RwsApEzeS+I08PaJJOtAvnbpVTzEJaTyi/R73XbXDyg=");

				var hub = new SBNotificationHub (cs, "pickuphub");

				NSArray tagArray = NSArray.FromObjects(App.myAccount.UserId,App.myAccount.Email, App.myAccount.id);

				NSSet tagSet = new NSSet(tagArray);

//				hub.RegisterNativeAsync (deviceNotificationToken, tagSet, err => {
//					if (err != null)
//						Console.WriteLine("Error: " + err.Description);
//					else
//						Console.WriteLine("Success");
//				});

				string template = "{\"aps\": {\"alert\": \"$(message)\", \"sound:\":\"$(sound)\", \"pickup\": \"$(pickup)\", \"invite\": \"$(invite)\",\"nobody\": \"$(nobody)\" }}";
				var expire = DateTime.Now.AddDays(90).ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
				hub.RegisterTemplateAsync(deviceNotificationToken, "pickupTemplate", template, expire, tagSet, err=>{
					if (err != null)
						Console.WriteLine("Error: " + err.Description);
					else
						Console.WriteLine("Success");
				});


			});
				
			LoadApplication (new App ());

			//window = new UIWindow (UIScreen.MainScreen.Bounds);
			//MainView = App.Current.MainPage.CreateViewController ();
			//MainView =App.GetMainPage().CreateViewController();
			//window.RootViewController = MainView;
			//window.MakeKeyAndVisible ();

			return base.FinishedLaunching (app, options);
		}
			
		public override void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings)
		{
			application.RegisterForRemoteNotifications ();
		}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			Console.WriteLine("Failed to Register for Remote Notifications: {0}", error.LocalizedDescription);
		}

		public override void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			switch (application.ApplicationState) {
			case UIApplicationState.Active:
				ProcessNotification (userInfo, true);
				completionHandler (UIBackgroundFetchResult.NewData);
				break;
			case UIApplicationState.Background:
				ProcessNotification (userInfo, true);
				completionHandler (UIBackgroundFetchResult.NewData);
				break;
			case UIApplicationState.Inactive:
				ProcessNotification (userInfo, false);
				completionHandler (UIBackgroundFetchResult.NewData);
				break;
			}

		}

		public override void RegisteredForRemoteNotifications (UIApplication app, NSData deviceToken)
		{
			if (deviceToken != null) {
				deviceNotificationToken = deviceToken;

				// Connection string from your azure dashboard
//			var cs = SBConnectionString.CreateListenAccess(
//				new NSUrl("sb://pickuphub-ns.servicebus.windows.net/"),
//				"RwsApEzeS+I08PaJJOtAvnbpVTzEJaTyi/R73XbXDyg=");

				string szToken = deviceToken.Description;

				if (!string.IsNullOrWhiteSpace (szToken)) {
					szToken = szToken.Trim ('<');
					szToken = szToken.Trim ('>');
					szToken = szToken.Replace (" ", "");
				}
				//do we need the sanitized token or keep the "taggy" one?
				App.myDevice.notificationid = deviceToken.Description;
			}
			//token ir now wrapped in <> in iOS8...is that ok?
			//we now have to save this thing to the AccountDevice table (do we? let's just use tags!)

			// Register our info with Azure
//			var hub = new SBNotificationHub (cs, "pickuphub");
//
//			NSArray tagArray = NSArray.FromObjects(App.myAccount.UserId,App.myAccount.Email,"AllUsers");
//		
//			NSSet tagSet = new NSSet(tagArray);
//
//			hub.RegisterNativeAsync (deviceToken, tagSet, err => {
//				if (err != null)
//					Console.WriteLine("Error: " + err.Description);
//				else
//					Console.WriteLine("Success");
//			});
		}

//		public override void ReceivedRemoteNotification (UIApplication app, NSDictionary userInfo)
//		{
//			// Process a notification received while the app was already open
//			ProcessNotification (userInfo, false);
//		}
		public void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			// Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey(new NSString("aps")))
			{
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

				string alert = string.Empty;

				//sweet, brilliant, actually. we'll pass in a msgtype value

				//if msgtype="invite", then we will pop the invite view with salient information (scheduleID will get passed in)

				if (aps.ContainsKey(new NSString("invite")) && !string.IsNullOrEmpty(aps ["invite"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["invite"].ToString ();
					MessagingCenter.Send <Invite>(i, "invite");
				}

				//if msgtype="pickup", this means someone has accepted your invite (you'll get an accountid for this person)
				if (aps.ContainsKey (new NSString("pickup")) && !string.IsNullOrEmpty(aps ["pickup"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["pickup"].ToString ();
					MessagingCenter.Send<Invite> (i, "pickup");
				}

				//Extract the alert text
				// NOTE: If you're using the simple alert by just specifying 
				// "  aps:{alert:"alert msg here"}  " this will work fine.
				// But if you're using a complex alert with Localization keys, etc., 
				// your "alert" object from the aps dictionary will be another NSDictionary. 
				// Basically the json gets dumped right into a NSDictionary, 
				// so keep that in mind.
				//if (aps.ContainsKey(new NSString("alert")))
				//	alert = (aps [new NSString("alert")] as NSString).ToString();

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching)
				{
					//Manually show an alert
					if (!string.IsNullOrEmpty(alert))
					{
						UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
						avAlert.Show();
					}
				}           
			}
		}
	}

	/*
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		public static UIViewController MainView;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{

			Forms.Init ();
			Xamarin.FormsMaps.Init ();
			CurrentPlatform.Init ();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			//MainView = Xamarin.Forms.Application.Current.MainPage.CreateViewController ();
			MainView =App.GetMainPage().CreateViewController();
			window.RootViewController = MainView; //App.GetMainPage ().CreateViewController ();
			window.MakeKeyAndVisible ();
			
			return true;

		}
	}
	*/
}

