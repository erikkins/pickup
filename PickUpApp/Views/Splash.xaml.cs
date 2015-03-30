using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;

//using System.Threading.Tasks;
//using PickUpApp.ViewModels;

namespace PickUpApp
{	
	public partial class Splash : ContentPage
	{	
		public Splash ()
		{
			InitializeComponent ();
			this.ViewModel = new SplashViewModel (App.client);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			btnFacebook.Command = ViewModel.LoginCommand;
			btnLogout.Clicked += btnLogout_Clicked;

			MessagingCenter.Subscribe<MobileServiceClient>(this, "LoggedIn", (s) =>{
					ViewModel.Refresh();
					ViewModel.IsAuthenticated = true;
					//ok, so we logged in through the service...let's check and see if this account already exists
					ViewModel.ExecuteLoadItemsCommand(s.CurrentUser.UserId).ConfigureAwait(false);
					//Navigation.PushAsync(new TabbedMaster());
				});

			MessagingCenter.Subscribe<Account> (this, "loaded", (s) => {

				//if we're launching out to the TabbedMaster, unsubscribe to the invite and pickup events
				//MessagingCenter.Unsubscribe<Invite>(this, "invite");
				//MessagingCenter.Unsubscribe<Invite>(this, "pickup");

				//if we don't have sufficient user information, pop the MyInfo page!
				if (string.IsNullOrEmpty(App.myAccount.Email) || string.IsNullOrEmpty(App.myAccount.Phone))
				{
					Navigation.PushModalAsync(new MyInfo());
				}
				else{
					Navigation.PushModalAsync (new TabbedMaster ()); 
				}
				//really since we have a message waiting on our auth, we want to atleast load the
				//main page before popping the popover (else we won't see it!)

				MessagingCenter.Send<Splash>(this, "auth");
			});

			MessagingCenter.Subscribe<Account> (this, "Refresh", (s) => {
				//right now this is only triggered when the Account is Updated (e.g. from MyInfo)
				Navigation.PushModalAsync (new TabbedMaster ()); 
			});


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
				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					//though this doesn't seem to be working very well
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							Device.BeginInvokeOnMainThread(()=>{
								Navigation.PushModalAsync(new InviteAccept());
							});
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					Device.BeginInvokeOnMainThread(()=>{
						Navigation.PushModalAsync(new InviteAccept());
					});
				}
			});

			//nobody has accepted my invite
			MessagingCenter.Subscribe<Invite> (this, "nobody", (i) => {
				v.Vibration (500);	
				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					//though this doesn't seem to be working very well
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							Device.BeginInvokeOnMainThread(()=>{
								Navigation.PushModalAsync(new InviteNotAccepted());
							});
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					Device.BeginInvokeOnMainThread(()=>{
						Navigation.PushModalAsync(new InviteNotAccepted());
					});
				}
			});

			//I accepted and was the first to do so
			MessagingCenter.Subscribe<Invite> (this, "confirm", (i) => {
				v.Vibration (500);	
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


			MessagingCenter.Subscribe<Invite> (this, "notfirst", (i) => {
				v.Vibration (500);	
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


			MessagingCenter.Subscribe<Invite> (this, "cancel", (i) => {
				v.Vibration (500);	
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



		protected override void OnAppearing ()
		{
			
			base.OnAppearing ();

			if (ViewModel.IsAuthenticated) {
				Navigation.PushModalAsync (new TabbedMaster ());
	
			} else {
				//this.ViewModel.ExecuteLoginCommand ("Facebook").ConfigureAwait (false);
				this.ViewModel.LoginCommand.Execute("Facebook");
			}

		}
		void btnLogout_Clicked(object sender, EventArgs e)
		{
			ViewModel.Logout();
		}

		protected SplashViewModel ViewModel
		{
			get { return this.BindingContext as SplashViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

