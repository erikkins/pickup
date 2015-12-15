using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using XLabs.Forms.Controls;

//using System.Threading.Tasks;
//using PickUpApp.ViewModels;

namespace PickUpApp
{	
	public partial class Login : ContentPage
	{	
		public Login ()
		{
			InitializeComponent ();
			this.ViewModel = new SplashViewModel (App.client);
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			//btnFacebook.Command = ViewModel.LoginCommand;

			MessagingCenter.Subscribe<Exception> (this, "Error", (ex) => {
				DisplayAlert("Error", ex.Message, "OK");
			});


			MessagingCenter.Subscribe<string> (this, "LoginError", (ex) => {
				lblActivity.Text = ex;	
			});


			stacker.WidthRequest = (App.ScaledWidth) - 100;
			ExtendedEntry ee = new ExtendedEntry ();
			ee.VerticalOptions = LayoutOptions.Start;
			ee.WidthRequest = (App.ScaledWidth) - 50;
			ee.HasBorder = false;
			ee.Placeholder = "Email";
			ee.TextColor = Color.White;
			ee.Keyboard = Keyboard.Email;
			ee.PlaceholderTextColor = Color.Gray;
			Font f = Font.SystemFontOfSize (22);
			ee.Font = f;
			fields.Children.Add (ee);



			BoxView bv = new BoxView ();
			bv.HeightRequest = 1;
			bv.WidthRequest = (App.ScaledWidth) - 150;
			bv.Color = Color.White;
			fields.Children.Add (bv);

			ExtendedEntry eepw = new ExtendedEntry ();
			eepw.IsPassword = true;
			eepw.VerticalOptions = LayoutOptions.StartAndExpand;
			eepw.WidthRequest = (App.ScaledWidth) - 50;
			eepw.HasBorder = false;
			eepw.Placeholder = "Password";
			eepw.TextColor = Color.White;
			eepw.PlaceholderTextColor = Color.Gray;
			eepw.Font = f;
			fields.Children.Add (eepw);


			bv = new BoxView ();
			bv.HeightRequest = 1;
			bv.WidthRequest = (App.ScaledWidth) - 150;
			bv.Color = Color.White;
			fields.Children.Add (bv);

			//bottomstack.TranslationY = -20;

			StackLayout slRemember = new StackLayout ();
			slRemember.Orientation = StackOrientation.Horizontal;

			ImageButton ib = new ImageButton ();
			ib.TranslationX = 5;
			ib.ImageHeightRequest = 27;
			ib.ImageWidthRequest = 27;
			if (ViewModel.RememberPassword) {
				ib.Source = "ui_check_filled.png";
			} else {
				ib.Source = "ui_check_empty.png";
			}
			ib.VerticalOptions = LayoutOptions.Center;
			ib.HorizontalOptions = LayoutOptions.Center;
			ib.Clicked += delegate(object sender, EventArgs e) {
				if (ViewModel.RememberPassword)
				{
					ViewModel.RememberPassword = false;
					ib.Source = "ui_check_empty.png";
				}
				else{
					ViewModel.RememberPassword = true;
					ib.Source = "ui_check_filled.png";
				}
			};
			slRemember.Children.Add (ib);
			Label lRemember = new Label ();
			lRemember.TextColor = Color.White;
			lRemember.VerticalOptions = LayoutOptions.Center;
			lRemember.Text = "Keep me signed in";
			slRemember.Children.Add (lRemember);
			fields.Children.Add (slRemember);

			bv = new BoxView ();
			bv.HeightRequest = 10;
			bv.Color = Color.Transparent;
			fields.Children.Add (bv);

			Button b = new Button ();
			b.VerticalOptions = LayoutOptions.End;
			b.HorizontalOptions = LayoutOptions.Center;	
			b.HeightRequest = 55;
			b.WidthRequest = (App.ScaledWidth) - 50 ;
			b.FontAttributes = FontAttributes.Bold;
			b.FontSize = 18;
			b.Text = "Login";
			b.TextColor = Color.FromHex ("F6637F");
			b.BackgroundColor = Color.FromHex ("49376D");
			b.BorderColor = Color.FromHex ("54D29F");
			b.BorderRadius = 8;
			b.BorderWidth = 2;
			fields.Children.Add (b);
			b.Clicked += async delegate(object sender, EventArgs e) {
				//ok, let's log that badbear in
				this.ViewModel.AuthAccount = new AuthAccounts();
				this.ViewModel.AuthAccount.Email = ee.Text;
				this.ViewModel.AuthAccount.Username = ee.Text;
				this.ViewModel.AuthAccount.Password = eepw.Text;

				await this.ViewModel.ExecuteLoginCommand("Custom");
			};

	


			Button bForgot = new Button ();
			bForgot.Text = "Forgot password?";
			bForgot.TextColor = Color.White;
			bForgot.FontAttributes = FontAttributes.Bold | FontAttributes.Italic;
			fields.Children.Add (bForgot);


			MessagingCenter.Subscribe<MobileServiceClient>(this, "LoggedIn", (s) =>{
					ViewModel.Refresh();
					ViewModel.IsAuthenticated = true;
					//ok, so we logged in through the service...let's check and see if this account already exists
					ViewModel.ExecuteLoadItemsCommand(s.CurrentUser.UserId).ConfigureAwait(false);

					//mark the device setting so that we know we've logged in before
					Settings.HasLoggedIn = true;
					//Navigation.PushAsync(new TabbedMaster());
				});

			MessagingCenter.Subscribe<Account> (this, "loaded", async(s) => {

				//now we're fully loaded, account, logged in, but we need to preload the other pages (kids, circle, schedule, places)
				this.BindingContext = new KidsViewModel(App.client);
				//lblActivity.Text = "Loading Kids";
				App.hudder.showHUD("Loading Kids");
				await ((KidsViewModel)BindingContext).ExecuteLoadItemsCommand();

				this.BindingContext = new MyCircleViewModel(App.client);
				//lblActivity.Text = "Loading Circle";
				App.hudder.showHUD("Loading Circle");
				await ((MyCircleViewModel)BindingContext).ExecuteLoadItemsCommand();

				this.BindingContext = new AccountPlaceViewModel(App.client);
				//lblActivity.Text = "Loading Places";
				App.hudder.showHUD("Loading Places");
				await ((AccountPlaceViewModel)BindingContext).ExecuteLoadItemsCommand();
				App.hudder.hideHUD();
				this.BindingContext = new SplashViewModel(App.client);

				//really since we have a message waiting on our auth, we want to atleast load the
				//main page before popping the popover (else we won't see it!)

				//ok, don't need this anymore since we've already come in?
				//MessagingCenter.Unsubscribe<Account>(this, "loaded");

				//MessagingCenter.Send<Splash>(this, "auth");
				//MessagingCenter.Send<string>("splash", "NeedsRefresh");

				//take this back to the root...but then we need to launch the main page!

				MessagingCenter.Send<string>("login", "launch");
			});


			MessagingCenter.Subscribe<Account> (this, "Refresh", (s) => {
				//right now this is only triggered when the Account is Updated (e.g. from MyInfo)
				//Navigation.PushModalAsync (new TabbedMaster ()); 
			});



					
			//autologin
			//this.ViewModel.LoginCommand.Execute("Facebook");
			//this used to be in there
//			if (ViewModel.IsAuthenticated) {
//				//Navigation.PushModalAsync (new TabbedMaster ());
//				Navigation.PopModalAsync();
//			} else {
//				//this.ViewModel.ExecuteLoginCommand ("Facebook").ConfigureAwait (false);
//				Device.BeginInvokeOnMainThread (() => {
//					this.ViewModel.LoginCommand.Execute ("Facebook");
//				});
//			}

		}





		protected override void OnAppearing ()
		{
			
			base.OnAppearing ();



			//why did we remove this?
//			if (ViewModel.IsAuthenticated) {
//				//Navigation.PushModalAsync (new TabbedMaster ());
//				Navigation.PopModalAsync();
//			} else {
//				//this.ViewModel.ExecuteLoginCommand ("Facebook").ConfigureAwait (false);
//				Device.BeginInvokeOnMainThread (() => {
//					this.ViewModel.LoginCommand.Execute ("Facebook");
//				});
//			}

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

