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
	public class PickUpBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
	{
		public static string[] SENDER_IDS = new string[] { Constants.SenderID };

		public const string TAG = "PickUpBroadcastReceiver-GCM";
	}

	[Service] //Must use the service tag
	public class PushHandlerService : GcmServiceBase
	{
		public static string RegistrationID { get; private set; }
		private NotificationHub Hub { get; set; }

		public PushHandlerService() : base(Constants.SenderID) 
		{
			System.Diagnostics.Debug.WriteLine(PickUpBroadcastReceiver.TAG, "GcmService() constructor"); 
		}
		protected override void OnError (Context context, string errorId)
		{
			throw new NotImplementedException ();
		}
		protected override void OnUnRegistered (Context context, string registrationId)
		{
			throw new NotImplementedException ();
		}

		protected override async void OnRegistered(Context context, string registrationId)
		{
			System.Diagnostics.Debug.WriteLine(PickUpBroadcastReceiver.TAG, "GCM Registered: " + registrationId);
			RegistrationID = registrationId;

			//createNotification("GcmService-GCM Registered...", "The device has been Registered, Tap to View!");

			//only register when we know we have logged in and have sufficient tag info

			MessagingCenter.Subscribe<Account> (this, "loaded", (s) => {
				Hub = new NotificationHub (Constants.NotificationHubPath, Constants.ConnectionString);
				try {
					//was awaited
					Hub.UnregisterAllAsync (registrationId);
				} catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine ("Loaded " + ex.Message);
					//Debugger.Break();
				}

				var tags = new List<string> () { App.myAccount.UserId, App.myAccount.Email, App.myAccount.id }; // create tags if you want

				try {
					//was awaited
					var hubRegistration = Hub.RegisterNativeAsync (registrationId, tags).ConfigureAwait(false);

				} catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine ("HubReg " + ex.Message); 
					//Debugger.Break();
				}
			});
		}
		protected override void OnMessage(Context context, Intent intent)
		{
			System.Diagnostics.Debug.WriteLine(PickUpBroadcastReceiver.TAG, "GCM Message Received!");

			var msg = new StringBuilder();

			if (intent != null && intent.Extras != null)
			{
				foreach (var key in intent.Extras.KeySet())
					msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
			}

			if (intent.Extras.ContainsKey ("invite")) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("invite");
				MessagingCenter.Send <Invite>(i, "invite");
			}
			if (intent.Extras.ContainsKey ("pickup")) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("pickup");
				MessagingCenter.Send <Invite>(i, "pickup");
			}


			string messageText = intent.Extras.GetString("msg");
			if (!string.IsNullOrEmpty(messageText))
			{
				createNotification("New hub message!", messageText);
				return;
			}

			//createNotification("Unknown message details", msg.ToString());
		}
		void createNotification(string title, string desc)
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

			notification.SetLatestEventInfo(this, title, desc, PendingIntent.GetActivity(this, 0, uiIntent, 0));

			//Show the notification
			notificationManager.Notify(1, notification);
		}
	}
}

