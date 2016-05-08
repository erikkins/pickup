using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using WindowsAzure.Messaging;
//using Xamarin.Forms.Labs;
//using Xamarin.Forms.Labs.iOS;
using XLabs.Forms;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;
using XLabs.Ioc;
using XLabs.Platform.Services;
using XLabs.Platform.Services.IO;
//using Xamarin.Social;
//using Xamarin.Social.Services;
//using Xamarin.Forms.Labs.Services;
using Facebook.CoreKit;
using MTiRate;
using HockeyApp;
using System.Threading.Tasks;

namespace PickUpApp.iOS
{

	[Register ("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate // superclass new in 1.3 (was global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate)
	{
		//UIWindow window;
		//public static UIKit.UIViewController  MainView;
		NSData deviceNotificationToken;

//		private static FacebookService mFacebook;
//		public static FacebookService Facebook
//		{
//			get
//			{
//				if (mFacebook == null)
//				{
//					mFacebook = new FacebookService() {
//						ClientId = "445633295574438",
//						RedirectUrl = new Uri ("Redirect URL from https://developers.facebook.com/apps")
//					};
//				}
//
//				return mFacebook;
//			}
//		}



		//public override UIWindow Window { get; set; }
		public override void DidEnterBackground (UIApplication application)
		{
			base.DidEnterBackground (application);

					
		}



//		private void scheduleBackgroundTimer()
//		{
//			System.Console.WriteLine ("ScheduleBackgroundTimer");
//
//				Device.StartTimer (new TimeSpan (0, 0, 10), () => {
//					System.Console.WriteLine ("BackgroundTimerFired");
//
//					System.Threading.Tasks.Task.Factory.StartNew (async() => {
//						System.Console.WriteLine ("BackgroundGetPosition");
//						await App.GetPosition ();
//						System.Console.WriteLine ("BackgroundGotPosition");
//						//don't log it if it's the same location as before
//						//if (App.PositionLatitude != lastLocationLog.Latitude || App.PositionLongitude != lastLocationLog.Longitude) {
//							if (!string.IsNullOrEmpty (App.PositionLatitude) && !(string.IsNullOrEmpty (App.PositionLongitude))) {	
//								ll.Latitude = App.PositionLatitude;
//								ll.Longitude = App.PositionLongitude;
//								ll.LogType = "background";
//								System.Console.WriteLine ("BackgroundSaving");
//								await bvm.ExecuteLocationLogCommand (ll);
//							}
//						//}
//						Device.BeginInvokeOnMainThread (() => {
//							scheduleBackgroundTimer ();
//						});
//						
//					});						
//
//					return false;
//				});		
//		}

		public override void WillEnterForeground (UIApplication application)
		{
			base.WillEnterForeground (application);
		}

		public override bool FinishedLaunching (UIKit.UIApplication app, NSDictionary options)
		{

			global::Xamarin.Forms.Forms.Init ();
			Xamarin.FormsMaps.Init ();
			ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init ();
			FFImageLoading.Forms.Touch.CachedImageRenderer.Init ();
			//Insights.Initialize("882979cfc38cb829ecfaf090e99781b90980c55a");



			HockeyApp.Setup.EnableCustomCrashReporting (() => {

				//Get the shared instance

				var manager = BITHockeyManager.SharedHockeyManager;

				//Configure it to use our APP_ID
				manager.Configure ("ed87bdba01244d8b82ed8df1ec5a7690");

				//Start the manager
				manager.StartManager ();

				//Authenticate (there are other authentication options)
				manager.Authenticator.AuthenticateInstallation ();

				//Rethrow any unhandled .NET exceptions as native iOS 
				// exceptions so the stack traces appear nicely in HockeyApp
				AppDomain.CurrentDomain.UnhandledException += (sender, e) => 
					Setup.ThrowExceptionAsNative(e.ExceptionObject);

				TaskScheduler.UnobservedTaskException += (sender, e) => 
					Setup.ThrowExceptionAsNative(e.Exception);
			});

			//Window = new UIWindow (UIScreen.MainScreen.Bounds);


			//Plugin.Vibrate.iOS.Vibrate.Init ();
			var container = new SimpleContainer ();
			container.Register<IDevice> (t => AppleDevice.CurrentDevice);
			container.Register<IDisplay> (t => t.Resolve<IDevice> ().Display);
			container.Register<INetwork>(t=> t.Resolve<IDevice>().Network);
			container.Register<IFileManager>(t=>t.Resolve<IDevice>().FileManager);

			Resolver.SetResolver (container.GetResolver ());



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

			MessagingCenter.Subscribe<Account> (this, "loaded", async(s) => {
				//can I do the NotificationHub stuff here?
				// Register our info with Azure
				// Connection string from your azure dashboard

				string hubEndpoint = "sb://pickuphub-ns.servicebus.windows.net/";
				string hubSecret = "RwsApEzeS+I08PaJJOtAvnbpVTzEJaTyi/R73XbXDyg=";
				string hubPath = "pickuphub";


				#if DEBUG
				hubEndpoint = "sb://pickuphubdebug-ns.servicebus.windows.net/";
				hubSecret = "nsg+MhEyf9/KrelyeQbHsTAbBeEuH0e4sTqf1qPt3tU=";
				hubPath = "pickupHubDebug";
				#endif

				var cs = SBConnectionString.CreateListenAccess(new NSUrl(hubEndpoint),hubSecret);

				var hub = new SBNotificationHub (cs, hubPath);

				NSArray tagArray = NSArray.FromObjects(App.myAccount.UserId,App.myAccount.Email, App.myAccount.id);

				NSSet tagSet = new NSSet(tagArray);

//				hub.RegisterNativeAsync (deviceNotificationToken, tagSet, err => {
//					if (err != null)
//						Console.WriteLine("Error: " + err.Description);
//					else
//						Console.WriteLine("Success");
//				});

				try{
					string template = "{\"aps\": {\"alert\": \"$(message)\", \"sound:\":\"$(sound)\", \"pickup\": \"$(pickup)\", \"invite\": \"$(invite)\",\"nobody\": \"$(nobody)\",\"confirm\":\"$(confirm)\", \"accepted\":\"$(accepted)\",\"notfirst\":\"$(notfirst)\",\"cancel\":\"$(cancel)\", \"uid\":\"$(uid)\",\"invmsg\":\"$(invmsg)\",\"circle\":\"$(circle)\",\"declined\":\"$(declined)\",\"chat\":\"$(chat)\" }}";
					var expire = DateTime.Now.AddDays(90).ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
					Console.WriteLine(template);
					hub.UnregisterAllAsync(deviceNotificationToken, err=>{
						if (err == null)
						{
							hub.RegisterTemplateAsync(deviceNotificationToken, "pickupTemplate2", template, expire, tagSet, errreg=>{
								if (errreg != null)
									Console.WriteLine("HubRegistrationError: " + err.Description);
								else
									Console.WriteLine("HubRegistrationSuccess--" + tagSet.ToString());
							});
						}
					});



					//ok, we're registered...stop listening
					await System.Threading.Tasks.Task.Delay(25);
					MessagingCenter.Unsubscribe<Account>(this, "loaded");
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
				}


			});
				

			Profile.EnableUpdatesOnAccessTokenChange (true);
			Facebook.CoreKit.Settings.AppID = "445633295574438";
			Facebook.CoreKit.Settings.DisplayName = "FamFetch";
			Facebook.CoreKit.AppEvents.ActivateApp ();




			LoadApplication (new App ());


//			iRate.SharedInstance.PreviewMode = true;
//			iRate.SharedInstance.DaysUntilPrompt = 5;
//			iRate.SharedInstance.UsesUntilPrompt = 30;
//
//			iRate.SharedInstance.UserDidAttemptToRateApp += (sender, e) => {
//				Console.WriteLine ("User is rating app now!");
//			};
//
//			iRate.SharedInstance.UserDidDeclineToRateApp += (sender, e) => {
//				Console.WriteLine ("User does not want to rate app");
//			};
//
//			iRate.SharedInstance.UserDidRequestReminderToRateApp += (sender, e) => {
//				Console.WriteLine ("User will rate app later");
//			};





			//window = new UIWindow (UIScreen.MainScreen.Bounds);
			//MainView = App.Current.MainPage.CreateViewController ();
			//MainView =App.GetMainPage().CreateViewController();
			//Window.RootViewController = MainView;
			//Window.MakeKeyAndVisible ();


			Facebook.CoreKit.ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);
			//return Facebook.CoreKit.ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);

//			long startTicks, endTicks;
//			startTicks = DateTime.Now.Ticks;
//			endTicks = DateTime.Now.Ticks;
//			long ms1 = (endTicks - startTicks) / TimeSpan.TicksPerMillisecond;
//			System.Diagnostics.Debug.WriteLine ("FullLoad: " + ms1.ToString ());

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

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{			

			//var rurl = new Rivets.AppLinkUrl (url.ToString ());
			Rivets.AppLinks.DefaultResolver = new Rivets.FacebookIndexAppLinkResolver ("445633295574438", "bc150affdef11fe0f473c4ce01b380b7");

			//BFURL *parsedUrl = [BFURL URLWithInboundURL:url sourceApplication:sourceApplication];
//			if ([parsedUrl appLinkData]) {
//				// this is an applink url, handle it here
//				NSURL *targetUrl = [parsedUrl targetURL];
//				[[[UIAlertView alloc] initWithTitle:@"Received link:"
//                                    message:[targetUrl absoluteString]
//					delegate:nil
//					cancelButtonTitle:@"OK"
//                          otherButtonTitles:nil] show];
//			}

			return Facebook.CoreKit.ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
			//return ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
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
//
//		public override void ReceivedRemoteNotification (UIApplication app, NSDictionary userInfo)
//		{
//			// Process a notification received while the app was already open
//			ProcessNotification (userInfo, false);
//		}
		public void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			// Check to see if the dictionary has the aps key.  This is the notification payload you would have se
			if (null != options && options.ContainsKey(new NSString("aps")))
			{
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

				string alert = string.Empty;

				//sweet, brilliant, actually. we'll pass in a msgtype value

				//if msgtype="invite", then we will pop the invite view with salient information (scheduleID will get passed in)

				//I've received an invite
				if (aps.ContainsKey(new NSString("invite")) && !string.IsNullOrEmpty(aps ["invite"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["invite"].ToString ();
					MessagingCenter.Send <Invite>(i, "invite");
				}

				//my kids have been picked up
				if (aps.ContainsKey (new NSString("pickup")) && !string.IsNullOrEmpty(aps ["pickup"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["pickup"].ToString ();
					MessagingCenter.Send<Invite> (i, "pickup");
				}

				//someone has accepted my invite
				if (aps.ContainsKey (new NSString("accepted")) && !string.IsNullOrEmpty(aps ["accepted"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["accepted"].ToString ();
					i.Message = aps ["alert"].ToString ();
					MessagingCenter.Send<Invite> (i, "accepted");
				}

				//someone has NOT accepted my invite
				if (aps.ContainsKey (new NSString("declined")) && !string.IsNullOrEmpty(aps ["declined"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["declined"].ToString ();
					i.Message = aps ["alert"].ToString ();
					MessagingCenter.Send<Invite> (i, "declined");
				}

				//nobody has accepted my invite
				if (aps.ContainsKey (new NSString("nobody")) && !string.IsNullOrEmpty(aps ["nobody"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["nobody"].ToString ();
					MessagingCenter.Send<Invite> (i, "nobody");
				}

				//I accepted and was the first to do so
				if (aps.ContainsKey (new NSString("confirm")) && !string.IsNullOrEmpty(aps ["confirm"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["confirm"].ToString ();
					MessagingCenter.Send<Invite> (i, "confirm");
				}

				//I accepted and was NOT the first to do so
				if (aps.ContainsKey (new NSString("notfirst")) && !string.IsNullOrEmpty(aps ["notfirst"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["notfirst"].ToString ();
					MessagingCenter.Send<Invite> (i, "notfirst");
				}

				//I accepted but the requestor has canceled so I'm off the hook
				if (aps.ContainsKey (new NSString("cancel")) && !string.IsNullOrEmpty(aps ["cancel"].ToString ())) {
					Invite i = new Invite ();
					i.Id = aps ["cancel"].ToString ();
					MessagingCenter.Send<Invite> (i, "cancel");
				}

				if (aps.ContainsKey (new NSString("invmsg")) && !string.IsNullOrEmpty(aps ["invmsg"].ToString ())) {
					InviteMessage im = new InviteMessage ();
					string[] parts = aps["invmsg"].ToString ().Split ('|');
					im.Id = parts [0];
					im.AccountID = parts [1];
					MessagingCenter.Send<InviteMessage> (im, "arrived");
				}
				if (aps.ContainsKey (new NSString("circle")) && !string.IsNullOrEmpty(aps ["circle"].ToString ())) {
					//refresh circle and kids
					EmptyClass ec = new EmptyClass();
					//this will trick the app into reloading kids and circle
					MessagingCenter.Send<EmptyClass> (ec, "CircleChanged");
				}
				if (aps.ContainsKey (new NSString("chat")) && !string.IsNullOrEmpty(aps ["chat"].ToString ())) {
					//we will get the messageID of the last chat entry which we'll pass to getchat and pull the whole conversation
					MessageView mv = new MessageView();
					mv.Id = aps ["chat"].ToString ();
					mv.Link = aps ["chat"].ToString ();
					MessagingCenter.Send<MessageView> (mv, "chatreceived");
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

//				if (aps.ContainsKey (new NSString("alert"))) {
//					alert = aps ["alert"].ToString ();
//				}

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

