using System;
using ByteSmith.WindowsAzure.Messaging;
using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using Gcm.Client;
using System.Collections.Generic;
using Xamarin.Forms;

[assembly: Permission(Name = "pickUpApp.Android.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "pickUpApp.Android.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

namespace PickUpApp.droid
{
	[BroadcastReceiver(Permission=Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]

	[IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "pickUpApp.Android" })]
	[IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "pickUpApp.Android" })]
	[IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "pickUpApp.Android" })]
	[IntentFilter(new[] {Intent.ActionBootCompleted})]
	public class PickUpBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
	{
		public static string[] SENDER_IDS = new string[] { Constants.SenderID };

		public const string TAG = "PickUpBroadcastReceiver-GCM";

//		public override void OnReceive (Context context, Intent intent)
//		{
//			PickupIntentService.RunIntentInService(context, intent);
//
//			SetResult(Result.Ok, null, null);
//		}
	}

	[Service] //Must use the service tag
	public class PushHandlerService : GcmServiceBase
	{
		public static string RegistrationID { get; private set; }
		private NotificationHub Hub { get; set; }

		public PushHandlerService() : base(Constants.SenderID) 
		{
			//System.Diagnostics.Debug.WriteLine(PickUpBroadcastReceiver.TAG, "GcmService() constructor"); 
		}
		protected override void OnError (Context context, string errorId)
		{
			throw new NotImplementedException ();
		}
		protected override void OnUnRegistered (Context context, string registrationId)
		{
			System.Diagnostics.Debug.WriteLine ("GCMSERVICE UNREGISTERED");
			//throw new NotImplementedException ();
		}



		protected override void OnRegistered(Context context, string registrationId)
		{
			//System.Diagnostics.Debug.WriteLine(PickUpBroadcastReceiver.TAG, "GCM Registered: " + registrationId);
			RegistrationID = registrationId;

			//make sure we save the registrationID locally, so we don't keep asking for it!
			var preferences = GetSharedPreferences("AppData", FileCreationMode.Private);
			var deviceId = preferences.GetString("DeviceId","");
			if (string.IsNullOrEmpty (deviceId)) {
				var editor = preferences.Edit ();
				editor.PutString ("DeviceId", registrationId);
				editor.Commit ();
			}

			//createNotification("GcmService-GCM Registered...", "The device has been Registered, Tap to View!");
			App.myDevice.notificationid = registrationId;
			//only register when we know we have logged in and have sufficient tag info

			MessagingCenter.Subscribe<Account> (this, "loaded", async (s) => {
				Hub = new NotificationHub (Constants.NotificationHubPath, Constants.ConnectionString);

				try {
					//was awaited
					await Hub.UnregisterAllAsync (registrationId);
				} catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine ("Loaded " + ex.Message);
					//Debugger.Break();
				}
				finally
				{
					//want to clean up after ourselves, and only then register the new ones
					var tags = new List<string> () { App.myAccount.UserId, App.myAccount.Email, App.myAccount.id }; // create tags if you want

					try {
						//was awaited
						//var hubRegistration = Hub.RegisterNativeAsync (registrationId, tags).ConfigureAwait(false);

						string template = "{\"data\": {\"alert\": \"$(message)\", \"sound:\":\"$(sound)\", \"pickup\": \"$(pickup)\", \"invite\": \"$(invite)\",\"nobody\": \"$(nobody)\",\"confirm\":\"$(confirm)\", \"accepted\":\"$(accepted)\",\"notfirst\":\"$(notfirst)\",\"cancel\":\"$(cancel)\", \"uid\":\"$(uid)\" }}";
						//var expire = DateTime.Now.AddDays(90).ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
						await Hub.RegisterTemplateAsync(registrationId, template, "pickup2", tags);

						//we've registered, we're not gonna need it again!

						//issue with MessagingCenter that needs to enum the subscriptions, so give it a breath.
						//Device.BeginInvokeOnMainThread(()=>{
							//await System.Threading.Tasks.Task.Delay(25);
							//MessagingCenter.Unsubscribe<Account>(this, "loaded");
						//});

					} catch (Exception ex) {
						//System.Diagnostics.Debug.WriteLine ("HubReg " + ex.Message); 
						//Debugger.Break();
					}
				}
			});
		}


		//GCM is firing multiple times for the same message...so, only allow the first one through
		private static List<Guid> _Last10MessageIds;

		private bool IsRecentGuid(string checker)
		{
			if (string.IsNullOrEmpty (checker)) {
				return false;
			}

			Guid iChecker = Guid.Parse (checker);

			if (_Last10MessageIds.Contains (iChecker)) {
				return true;
			}

			//it's not there, so add it
			_Last10MessageIds.Insert(0, iChecker);
			//and now make sure the list is only 10 long!
			if (_Last10MessageIds.Count > 10) {
				_Last10MessageIds.RemoveRange (10, _Last10MessageIds.Count - 10);
			}
			return false;
		}

		protected override void OnHandleIntent (Intent intent)
		{
			OnMessage (this.BaseContext, intent);
		}

		protected override void OnMessage(Context context, Intent intent)
		{
			if (_Last10MessageIds == null)
			{
				_Last10MessageIds = new List<Guid>();
			}
			//System.Diagnostics.Debug.WriteLine(PickUpBroadcastReceiver.TAG, "GCM Message Received!");

			var msg = new StringBuilder();

			if (intent != null && intent.Extras != null)
			{
				foreach (var key in intent.Extras.KeySet())
					msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
			}


			//System.Diagnostics.Debug.WriteLine ("GCM: " + msg);

			//likely don't need this...had garbage registrations in Azure notification hub
			if (IsRecentGuid(intent.Extras.GetString("uid")))
			{
				return;
			}

			bool launchFromNotification = true;

			//I'm getting an invite
			if (intent.Extras.ContainsKey ("invite") && !string.IsNullOrEmpty(intent.Extras.GetString("invite"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("invite");
				MessagingCenter.Send <Invite> (i, "invite");
			}
			//my kids have been picked up
			if (intent.Extras.ContainsKey ("pickup")&& !string.IsNullOrEmpty(intent.Extras.GetString("pickup"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("pickup");
				MessagingCenter.Send <Invite>(i, "pickup");
			}
			//someone has accepted my invite
			if (intent.Extras.ContainsKey ("accepted")&& !string.IsNullOrEmpty(intent.Extras.GetString("accepted"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("accepted");
				launchFromNotification = false;
				Device.BeginInvokeOnMainThread (() => {
					MessagingCenter.Send <Invite> (i, "accepted");
				});
			}
			//nobody has accepted my invite
			if (intent.Extras.ContainsKey ("nobody")&& !string.IsNullOrEmpty(intent.Extras.GetString("nobody"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("nobody");
				Device.BeginInvokeOnMainThread (() => {
					MessagingCenter.Send <Invite> (i, "nobody");
				});
			}
			//I accepted and was the first to do so
			if (intent.Extras.ContainsKey ("confirm")&& !string.IsNullOrEmpty(intent.Extras.GetString("confirm"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("confirm");
				Device.BeginInvokeOnMainThread (() => {
					MessagingCenter.Send <Invite> (i, "confirm");
				});
			}
			//I accepted and was NOT the first, so I'm off the hook
			if (intent.Extras.ContainsKey ("notfirst")&& !string.IsNullOrEmpty(intent.Extras.GetString("notfirst"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("notfirst");
				launchFromNotification = false;
				Device.BeginInvokeOnMainThread (() => {
					MessagingCenter.Send <Invite> (i, "notfirst");
				});
			}
			//requestor canceled...I'm off the hoook
			if (intent.Extras.ContainsKey ("cancel")&& !string.IsNullOrEmpty(intent.Extras.GetString("cancel"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("cancel");
				MessagingCenter.Send <Invite>(i, "cancel");
			}

			string messageText = intent.Extras.GetString("alert");
			if (!string.IsNullOrEmpty(messageText))
			{
				createNotification("Pickup!", messageText, launchFromNotification);
				return;
			}

			//createNotification("Unknown message details", msg.ToString());
		}
		void createNotification(string title, string desc, bool launchFromNotification = true)
		{
			//Create notification
			var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

			//Create an intent to show ui
			var uiIntent = new Intent(this, typeof(MainActivity));

			//Create the notification
			var notification = new Notification(droid.Resource.Drawable.car_icon, title);

			//Auto cancel will remove the notification once the user touches it
			notification.Flags = NotificationFlags.AutoCancel;

			//Set the notification info
			//we use the pending intent, passing our ui intent over which will get called
			//when the notification is tapped.
			if (launchFromNotification) {
				notification.SetLatestEventInfo (this, title, desc, PendingIntent.GetActivity (this, 0, uiIntent, 0));
			} else {
				notification.SetLatestEventInfo (this, title, desc, null);
			}
			//Show the notification
			notificationManager.Notify(1, notification);
		}
	}
}

