﻿using System;

using Xamarin.Forms;

namespace PickUpApp
{
	public class AppRoot : ContentPage
	{
		DateTime lastLocationLog = new DateTime (1900, 1, 1);
		Location lastLocation = new Location();

		public AppRoot ()
		{			
			BackgroundColor = Color.FromRgb (73, 55, 109);
			Content = new StackLayout { 
				BackgroundColor = Color.FromRgb (73, 55, 109),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				Children = {
					new Image {Source="Icon.png", HorizontalOptions=LayoutOptions.Center, VerticalOptions=LayoutOptions.Center}
					//new Label { Text = "Welcome to FamFetch!", HorizontalOptions= LayoutOptions.Center, VerticalOptions=LayoutOptions.Center, TextColor=Color.White }
				}
			};


			if (App.Device.Network.InternetConnectionStatus () == XLabs.Platform.Services.NetworkStatus.NotReachable) {
				//uh oh
				Device.BeginInvokeOnMainThread (async() => {
					bool shouldRetry = await DisplayAlert ("Connection Error", "Uh oh! You must be connected to the internet for FamFetch to work! Please connect and tap Retry", "Retry", "Cancel");
					if (shouldRetry)
					{
						TestConnection();

					}
					else{
						//this means we're SOL...not sure what to do here
						return;
					}
				});			
				//MessagingCenter.Send<Exception> (newEx, "Error");
				//return;
			}


			MessagingCenter.Subscribe<string> (this, "launch", async(s) => {
				if (Navigation.ModalStack.Count > 0)
				{
					await Navigation.PopModalAsync();
				}
				await Navigation.PushModalAsync(new HomePage(), false);
			});
			MessagingCenter.Subscribe<string> (this, "login", async(s) => {
				await Navigation.PopModalAsync(false);
				await Navigation.PushModalAsync(new Login(), false);
			});
			MessagingCenter.Subscribe<string> (this, "register", async(s) => {
				await Navigation.PopModalAsync(false);
				await Navigation.PushModalAsync(new Register(), false);
			});
			MessagingCenter.Subscribe<string> (this, "registrationcomplete", async(s) => {
				await Navigation.PopModalAsync(false);
				await Navigation.PushModalAsync (new Splash (), false);
				MessagingCenter.Send<Microsoft.WindowsAzure.MobileServices.MobileServiceClient>(App.client, "LoggedIn");
			});

			MessagingCenter.Subscribe<string> (this, "geodenied", async(s) => {
				bool selection = await DisplayAlert("Please enable Location Services", "FamFetch uses your location to calculate drive times with traffic.  Tap Settings to Allow Location Access.", "Settings", "OK");
				if (selection)
				{					
					DependencyService.Get<IDeviceInfo> ().LoadDeviceSettings();
				}
			});

			Settings.FirstTime = true;

			//this is really just the underlying root page that sits under the current modal navigation
			if (string.IsNullOrEmpty (Settings.CachedAuthToken)) {
				if (Settings.FirstTime) {
					//first things first, show them the intro pages!
					Navigation.PushModalAsync (new CarouselMaster ());
					Settings.FirstTime = false;
				} else {
					//ok, we've been here before...but have we logged in?
					if (Settings.HasLoggedIn) {
						//just push the login/splash screen
						Navigation.PushModalAsync (new Login (), false);
					} else {
						//I think we need to push the register screen
						Navigation.PushModalAsync (new Register (), false);
					}
				}
			} else {
				//we have a cached auth token...use it
				Microsoft.WindowsAzure.MobileServices.MobileServiceUser msu = new Microsoft.WindowsAzure.MobileServices.MobileServiceUser(Settings.CachedUserName);
				msu.MobileServiceAuthenticationToken = Settings.CachedAuthToken;
				App.client.CurrentUser = msu;

				//deal is that we need to load all the stuff!
				Navigation.PushModalAsync (new Splash ());
				MessagingCenter.Send<Microsoft.WindowsAzure.MobileServices.MobileServiceClient>(App.client, "LoggedIn");
			}



			MessagingCenter.Subscribe<LocationLog> (this, "BackgroundLocationUpdated", async(loc) => {
				//System.Diagnostics.Debug.WriteLine("Got Update");
				//don't log it if it's been less than a minute
				if (lastLocationLog > DateTime.UtcNow.AddSeconds(-60))
				{
					return;
				}
				//don't log it if they haven't moved
				if (loc.Latitude == lastLocation.Latitude && loc.Longitude == lastLocation.Longitude)
				{
					return;
				}

				PickupService.DefaultService.client.CurrentUser = App.client.CurrentUser;

				await PickupService.DefaultService.InsertLocationLogAsync(loc);
				lastLocationLog = DateTime.UtcNow;
				lastLocation.Latitude = loc.Latitude;
				lastLocation.Longitude = loc.Longitude;
			});
				
			App.GetPosition ().ConfigureAwait (false);

			//var v  = DependencyService.Get<Plugin.Vibrate.Abstractions.IVibrate>();
			//I've received an invite
			MessagingCenter.Subscribe<Invite> (this, "invite", (i) => {
				Plugin.Vibrate.CrossVibrate.Current.Vibration(500);	
				MessageView mv = new MessageView();
				mv.Id = i.Id;
				MessagingCenter.Send<MessageView>(mv, "LoadMessages"); 

//				if (App.client.CurrentUser == null)
//				{
//					//we've encountered a race condition where we got a notification,
//					//we want to launch the invite, but we're not yet logged in...
//					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
//						{
//							Device.BeginInvokeOnMainThread(()=>{
//								Navigation.PushModalAsync(new InviteView(i));
//							});
//							MessagingCenter.Unsubscribe<Splash>(this, "auth");
//						});
//				}
//				else{
//					Device.BeginInvokeOnMainThread(()=>{
//						Navigation.PushModalAsync(new InviteView(i));
//					});
//				}

			});

			//someone has picked up my kids
			MessagingCenter.Subscribe<Invite> (this, "pickup", (i) => {
				Plugin.Vibrate.CrossVibrate.Current.Vibration(500);	

				RespondMessage rm = new RespondMessage ();
				rm.MessageID = i.Id;
				rm.Response = "1";
				rm.Status = "read";
				rm.PostUpdate = "today";
				MessagingCenter.Send<RespondMessage> (rm, "messagesupdated");
				/*
				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					//though this doesn't seem to be working very well
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							Device.BeginInvokeOnMainThread(()=>{
								Navigation.PushModalAsync(new Confirmation(i));
							});
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					Device.BeginInvokeOnMainThread(()=>{
						Navigation.PushModalAsync(new Confirmation(i));
					});
				}
				*/
			});

			//someone has accepted my invite
			MessagingCenter.Subscribe<Invite> (this, "accepted", (i) => {
				Plugin.Vibrate.CrossVibrate.Current.Vibration(500);	
				DisplayAlert("Fetch request has been accepted!", i.Message, "OK");
				RespondMessage rm = new RespondMessage ();
				rm.MessageID = i.Id;
				rm.Response = "1";
				rm.Status = "read";
				rm.PostUpdate = "today";
				MessagingCenter.Send<RespondMessage> (rm, "messagesupdated");
				//MessagingCenter.Send<string>("InviteAccepted", "NeedsRefresh");
				//let's just make this an alert
				//				if (App.client.CurrentUser == null)
				//				{
				//					//we've encountered a race condition where we got a notification,
				//					//we want to launch the invite, but we're not yet logged in...
				//					//though this doesn't seem to be working very well
				//					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
				//						{
				//							Device.BeginInvokeOnMainThread(()=>{
				//								Navigation.PushModalAsync(new InviteAccept());
				//							});
				//							MessagingCenter.Unsubscribe<Splash>(this, "auth");
				//						});
				//				}
				//				else{
				//					Device.BeginInvokeOnMainThread(()=>{
				//						Navigation.PushModalAsync(new InviteAccept());
				//					});
				//				}
			});

			//someone has NOT accepted my invite
			MessagingCenter.Subscribe<Invite> (this, "declined", (i) => {
				Plugin.Vibrate.CrossVibrate.Current.Vibration(500);	

				DisplayAlert("Fetch request has NOT been accepted!", i.Message, "OK");
				RespondMessage rm = new RespondMessage ();
				rm.MessageID = i.Id;
				rm.Response = "1";
				rm.Status = "read";
				rm.PostUpdate = "today";
				MessagingCenter.Send<RespondMessage> (rm, "messagesupdated");
			});

			//nobody has accepted my invite
			MessagingCenter.Subscribe<Invite> (this, "nobody", (i) => {
				Plugin.Vibrate.CrossVibrate.Current.Vibration(500);	
//				Device.BeginInvokeOnMainThread (() => {
//					DisplayAlert("Fetch request not accepted!", i.Message, "OK");
//				});
				AlertInfo ai = new AlertInfo();
				ai.Message = i.Message;
				ai.Title = "Fetch request not accepted!";
				MessagingCenter.Send<AlertInfo>(ai, "ShowAlert");

				RespondMessage rm = new RespondMessage ();
				rm.MessageID = i.Id;
				rm.Response = "1";
				rm.Status = "read";
				rm.PostUpdate = "today";
				MessagingCenter.Send<RespondMessage> (rm, "messagesupdated");


				//				if (App.client.CurrentUser == null)
				//				{
				//					//we've encountered a race condition where we got a notification,
				//					//we want to launch the invite, but we're not yet logged in...
				//					//though this doesn't seem to be working very well
				//					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
				//						{
				//							Device.BeginInvokeOnMainThread(()=>{
				//								Navigation.PushModalAsync(new InviteNotAccepted());
				//							});
				//							MessagingCenter.Unsubscribe<Splash>(this, "auth");
				//						});
				//				}
				//				else{
				//					Device.BeginInvokeOnMainThread(()=>{
				//						Navigation.PushModalAsync(new InviteNotAccepted());
				//					});
				//				}
			});

			//I accepted and was the first to do so
			MessagingCenter.Subscribe<Invite> (this, "confirm", (i) => {
				Plugin.Vibrate.CrossVibrate.Current.Vibration(500);	
				DisplayAlert("You are picking up!", i.Message, "OK");

				//need to refresh today
				RespondMessage rm = new RespondMessage ();
				rm.MessageID = i.Id;
				rm.Response = "1";
				rm.Status = "read";
				rm.PostUpdate = "today";
				MessagingCenter.Send<RespondMessage> (rm, "messagesupdated");
				//MessagingCenter.Send<string>("InviteIsMine", "NeedsRefresh");
				//				if (App.client.CurrentUser == null)
				//				{
				//					//we've encountered a race condition where we got a notification,
				//					//we want to launch the invite, but we're not yet logged in...
				//					//though this doesn't seem to be working very well
				//					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
				//						{
				//							Device.BeginInvokeOnMainThread(()=>{
				//								Navigation.PushModalAsync(new InviteIsMine());
				//							});
				//							MessagingCenter.Unsubscribe<Splash>(this, "auth");
				//						});
				//				}
				//				else{
				//					Device.BeginInvokeOnMainThread(()=>{
				//						Navigation.PushModalAsync(new InviteIsMine());
				//					});
				//				}
			});


			MessagingCenter.Subscribe<Invite> (this, "notfirst", (i) => {
				Plugin.Vibrate.CrossVibrate.Current.Vibration(500);	
				DisplayAlert("Thank you but you weren't first!", i.Message, "OK");
				RespondMessage rm = new RespondMessage ();
				rm.MessageID = i.Id;
				rm.Response = "1";
				rm.Status = "read";
				rm.PostUpdate = "today";
				MessagingCenter.Send<RespondMessage> (rm, "messagesupdated");
				//				if (App.client.CurrentUser == null)
				//				{
				//					//we've encountered a race condition where we got a notification,
				//					//we want to launch the invite, but we're not yet logged in...
				//					//though this doesn't seem to be working very well
				//					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
				//						{
				//							Device.BeginInvokeOnMainThread(()=>{
				//								Navigation.PushModalAsync(new InviteIsMine());
				//							});
				//							MessagingCenter.Unsubscribe<Splash>(this, "auth");
				//						});
				//				}
				//				else{
				//					Device.BeginInvokeOnMainThread(()=>{
				//						Navigation.PushModalAsync(new InviteIsMine());
				//					});
				//				}
			});


			MessagingCenter.Subscribe<Invite> (this, "cancel", (i) => {
				Plugin.Vibrate.CrossVibrate.Current.Vibration(500);	

				//need to show a little more than just a dialog? showing them WHICH pickup is canceled?
				RespondMessage rm = new RespondMessage ();
				rm.MessageID = i.Id;
				rm.Response = "1";
				rm.Status = "read";
				rm.PostUpdate = "today";
				MessagingCenter.Send<RespondMessage> (rm, "messagesupdated");
				/*
				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					//though this doesn't seem to be working very well
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							Device.BeginInvokeOnMainThread(()=>{
								Navigation.PushModalAsync(new InviteIsMine());
							});
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					Device.BeginInvokeOnMainThread(()=>{
						Navigation.PushModalAsync(new InviteIsMine());
					});
				}
				*/
			});

			MessagingCenter.Subscribe<MessageView>(this, "launchchat", async(mv)=>{
				//this is a MessageView with ONLY a messageid in it...we need to get the other stuff to know which conversation we're in

				MessageViewModel mvm = new MessageViewModel(App.client, null);
				//we really need to load the Today
				Today t =  await mvm.ExecuteLoadTodayCommandFromMessage(mv);
				if (Navigation.ModalStack.Count > 0)
				{
					HomePage hp = (HomePage)Navigation.ModalStack[0];
					NavigationPage np = (NavigationPage)hp.Detail;

					await np.PushAsync(new ManageFetch(t, false));
				}
				//now trip the message
				MessagingCenter.Send<MessageView> (mv, "chatreceived");
			});

			MessagingCenter.Subscribe<AccountDevice>(this, "changed", (s) =>
				{

					PickupService.DefaultService.InsertAccountDeviceAsync(s).Wait(10000);
				});

		}

		private bool TestConnection()
		{			
			if (App.Device.Network.InternetConnectionStatus () == XLabs.Platform.Services.NetworkStatus.NotReachable) {
				//uh oh
				Device.BeginInvokeOnMainThread (async() => {
					bool shouldRetry = await DisplayAlert ("Connection Error", "Uh oh! You must be connected to the internet for FamFetch to work! Please connect and tap Retry", "Retry", "Cancel");
					if (!shouldRetry) {
						//this means we're SOL...not sure what to do here

					} else {
						TestConnection ();
					}
				});	
			} else {
				return true;
			}
			return false;
		}

	}
}