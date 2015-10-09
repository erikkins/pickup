using System;

using Xamarin.Forms;

namespace PickUpApp
{
	public class HomePage : MasterDetailPage
	{
		public HomePage ()
		{
			Title = "FamFetch";

			Detail = new NavigationPage(new TodayView ()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };

			if (!((TodayView)((NavigationPage)Detail).CurrentPage).ViewModel.IsAuthenticated)
			{
				Navigation.PushModalAsync (new Splash ());
			}
			//need the menu to load AFTER we've downloaded all prelim data
			Master = new NavigationPage(new HomeMenu ()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127), Title = "MY SETTINGS", Icon="icn_settings.png" };//new HomeMenu ();


			//before the thing loads, we've got make sure we're logged in!

//			Content = new StackLayout { 
//				Children = {
//					new Label { Text = "Hello ContentPage" }
//				}
//			};
			var v  = DependencyService.Get<Refractored.Xam.Vibrate.Abstractions.IVibrate>();
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
				Device.BeginInvokeOnMainThread(()=>{
				DisplayAlert("Invite has been accepted!", i.Message, "OK");
				});
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
				Device.BeginInvokeOnMainThread(()=>{
				DisplayAlert("Invite not accepted!", i.Message, "OK");
				});
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
				Device.BeginInvokeOnMainThread(()=>{
				DisplayAlert("You are picking up!", i.Message, "OK");
				});
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
				Device.BeginInvokeOnMainThread(()=>{
				DisplayAlert("Thank you but you weren't first!", i.Message, "OK");
				});
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

//			MessagingCenter.Subscribe<InviteMessage>(this, "arrived", (s) =>
//				{
//
//				});

		}



	}
}


