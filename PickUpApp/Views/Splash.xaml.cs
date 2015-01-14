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

			MessagingCenter.Subscribe<MobileServiceClient>(this, "LoggedIn", (s) =>
				{
					ViewModel.IsAuthenticated = true;
					ViewModel.Refresh();

					//ok, so we logged in through the service...let's check and see if this account already exists
					ViewModel.ExecuteLoadItemsCommand(s.CurrentUser.UserId).ConfigureAwait(false);
					//Navigation.PushAsync(new TabbedMaster());
					Navigation.PushModalAsync(new TabbedMaster());
				});
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			if (ViewModel.IsAuthenticated) {
				Navigation.PushModalAsync (new TabbedMaster ());
			}
			else{
				this.ViewModel.ExecuteLoginCommand ("Facebook").ConfigureAwait (false);
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

