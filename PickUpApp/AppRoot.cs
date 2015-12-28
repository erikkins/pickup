using System;

using Xamarin.Forms;

namespace PickUpApp
{
	public class AppRoot : ContentPage
	{
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
				Device.BeginInvokeOnMainThread (() => {
					DisplayAlert ("Connection Error", "Uh oh! You must be connected to the internet for FamFetch to work!", "OK");
				});


				//MessagingCenter.Send<Exception> (newEx, "Error");
				return;
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

			App.GetPosition ().ConfigureAwait (false);

			var v  = DependencyService.Get<Plugin.Vibrate.Abstractions.IVibrate>();
			//I've received an invite
			MessagingCenter.Subscribe<Invite> (this, "invite", (i) => {
				v.Vibration (500);	
				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							Device.BeginInvokeOnMainThread(()=>{
								Navigation.PushModalAsync(new InviteView(i));
							});
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					Device.BeginInvokeOnMainThread(()=>{
						Navigation.PushModalAsync(new InviteView(i));
					});
				}
			});

			//someone has picked up my kids
			MessagingCenter.Subscribe<Invite> (this, "pickup", (i) => {
				v.Vibration (500);	
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
			});

			//someone has accepted my invite
			MessagingCenter.Subscribe<Invite> (this, "accepted", (i) => {
				v.Vibration (500);	
				DisplayAlert("Invite has been accepted!", i.Message, "OK");
				MessagingCenter.Send<string>("InviteAccepted", "NeedsRefresh");
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

			//nobody has accepted my invite
			MessagingCenter.Subscribe<Invite> (this, "nobody", (i) => {
				v.Vibration (500);
				DisplayAlert("Invite not accepted!", i.Message, "OK");
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
				v.Vibration (500);
				DisplayAlert("You are picking up!", i.Message, "OK");
				MessagingCenter.Send<string>("InviteIsMine", "NeedsRefresh");
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
				v.Vibration (500);	
				DisplayAlert("Thank you but you weren't first!", i.Message, "OK");
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
				v.Vibration (500);	

				//need to show a little more than just a dialog? showing them WHICH pickup is canceled?

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
			});


			MessagingCenter.Subscribe<AccountDevice>(this, "changed", (s) =>
				{

					PickupService.DefaultService.InsertAccountDeviceAsync(s).Wait(10000);
				});

		}
	}
}


