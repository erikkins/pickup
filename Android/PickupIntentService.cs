using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using Gcm.Client;
using System.Collections.Generic;
using Xamarin.Forms;
using Android.Runtime;
using Android.Views;
using Android.OS;
using ByteSmith.WindowsAzure.Messaging;

namespace PickUpApp.droid
{
	public class PickupIntentService : IntentService
	{
		static PowerManager.WakeLock sWakeLock;
		static object LOCK = new object();
		private NotificationHub Hub { get; set; }

		public PickupIntentService()
		{
			
		}

		public static void RunIntentInService(Context context, Intent intent)
		{
			lock (LOCK)
			{
				if (sWakeLock == null)
				{
					// This is called from BroadcastReceiver, there is no init.
					var pm = PowerManager.FromContext(context);
					sWakeLock = pm.NewWakeLock(
						WakeLockFlags.Partial, "My WakeLock Tag");
				}
			}

			sWakeLock.Acquire();
			intent.SetClass(context, typeof(PickupIntentService));
			context.StartService(intent);
			sWakeLock.Release ();

		}

		protected override void OnHandleIntent (Intent intent)
		{
			
			try
			{
				Context context = ApplicationContext;
				string action = intent.Action;

				if (action.Equals("com.google.android.c2dm.intent.REGISTRATION"))
				{
					HandleRegistration(context, intent);
				}
				else if (action.Equals("com.google.android.c2dm.intent.RECEIVE"))
				{
					HandleMessage(context, intent);
				}
			}
			finally
			{
				lock (LOCK)
				{
					//Sanity check for null as this is a public method
					if (sWakeLock != null)
						sWakeLock.Release();
				}
			}
		}


		protected void HandleRegistration(Context context, Intent intent)
		{
			string registrationId = intent.GetStringExtra("registration_id");
			string error = intent.GetStringExtra("error");
			string unregistration = intent.GetStringExtra("unregistered");

			//System.Diagnostics.Debug.WriteLine(PickUpBroadcastReceiver.TAG, "GCM Registered: " + registrationId);
			//RegistrationID = registrationId;

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
					//really don't need to be unregistering each time...only if unregistration comes across
					//but we were seeing some funkiness with multiple reg's so leave it in...for now.
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

						string template = "{\"data\": {\"alert\": \"$(message)\", \"sound:\":\"$(sound)\", \"pickup\": \"$(pickup)\", \"invite\": \"$(invite)\",\"nobody\": \"$(nobody)\",\"confirm\":\"$(confirm)\", \"accepted\":\"$(accepted)\",\"notfirst\":\"$(notfirst)\",\"cancel\":\"$(cancel)\", \"uid\":\"$(uid)\",\"invmsg\":\"$(invmsg)\",\"circle\":\"$(circle)\" }}";
						//var expire = DateTime.Now.AddDays(90).ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
						await Hub.RegisterTemplateAsync(registrationId, template, "pickup3", tags);

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
		protected void HandleMessage(Context context, Intent intent)
		{
			var msg = new StringBuilder();

			if (intent != null && intent.Extras != null)
			{
				foreach (var key in intent.Extras.KeySet())
					msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
			}

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
				MessagingCenter.Send <Invite>(i, "accepted");
			}
			//nobody has accepted my invite
			if (intent.Extras.ContainsKey ("nobody")&& !string.IsNullOrEmpty(intent.Extras.GetString("nobody"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("nobody");
				MessagingCenter.Send <Invite>(i, "nobody");
			}
			//I accepted and was the first to do so
			if (intent.Extras.ContainsKey ("confirm")&& !string.IsNullOrEmpty(intent.Extras.GetString("confirm"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("confirm");
				MessagingCenter.Send <Invite>(i, "confirm");
			}
			//I accepted and was NOT the first, so I'm off the hook
			if (intent.Extras.ContainsKey ("notfirst")&& !string.IsNullOrEmpty(intent.Extras.GetString("notfirst"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("notfirst");
				MessagingCenter.Send <Invite>(i, "notfirst");
			}
			//requestor canceled...I'm off the hoook
			if (intent.Extras.ContainsKey ("cancel")&& !string.IsNullOrEmpty(intent.Extras.GetString("cancel"))) {
				Invite i = new Invite ();
				i.Id = intent.Extras.GetString ("cancel");
				MessagingCenter.Send <Invite>(i, "cancel");
			}

			if (intent.Extras.ContainsKey ("invmsg")&& !string.IsNullOrEmpty(intent.Extras.GetString("invmsg"))) {
				InviteMessage im = new InviteMessage ();
				string[] parts = intent.Extras.GetString ("invmsg").Split ('|');
				im.Id = parts [0];
				im.AccountID = parts [1];
				MessagingCenter.Send <InviteMessage>(im, "arrived");
				//intent.Extras.Remove ("alert");
			}

			string messageText = intent.Extras.GetString("alert");
			if (!string.IsNullOrEmpty(messageText))
			{
				createNotification("Pickup!", messageText);
				return;
			}


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

