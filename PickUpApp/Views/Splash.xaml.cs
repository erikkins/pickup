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
					MessagingCenter.Send<Splash>(this, "auth");
					
				});

			MessagingCenter.Subscribe<Account> (this, "loaded", (s) => {
				Navigation.PushModalAsync (new TabbedMaster ()); 
			});

				
			MessagingCenter.Subscribe<Invite> (this, "invite", (i) => {

				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							var v  = DependencyService.Get<Refractored.Xam.Vibrate.Abstractions.IVibrate>();
							v.Vibration (500);
							Navigation.PushModalAsync(new InviteView(i));
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					var v  = DependencyService.Get<Refractored.Xam.Vibrate.Abstractions.IVibrate>();
					v.Vibration (500);
					App.Current.MainPage.Navigation.PushModalAsync(new InviteView(i));
					//Navigation.PushModalAsync(new InviteView(i));
				}
			});

			MessagingCenter.Subscribe<Invite> (this, "pickup", (i) => {
				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					//though this doesn't seem to be working very well
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							Navigation.PushModalAsync(new Confirmation(i));
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					Navigation.PushModalAsync(new Confirmation(i));
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

