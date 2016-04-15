using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Controls;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.MobileServices;

namespace PickUpApp
{


	public partial class Register : ContentPage
	{
		const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
			@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
		

		public Register ()
		{
			InitializeComponent ();
			this.ViewModel = new SplashViewModel (App.client);

			MessagingCenter.Subscribe<string> (this, "RegisterError", (ex) => {
				lblActivity.Text = ex;	
			});

			MessagingCenter.Subscribe<MobileServiceClient>(this, "Registered", (s) =>{
				MessagingCenter.Send("register", "registrationcomplete");
//				ViewModel.Refresh();
//				ViewModel.IsAuthenticated = true;
//				Settings.HasLoggedIn = true;
//				ViewModel.ExecuteLoadItemsCommand(s.CurrentUser.UserId).ConfigureAwait(false);
			});

//			MessagingCenter.Subscribe<Account> (this, "loaded", async(s) => {
//
//				//now we're fully loaded, account, logged in, but we need to preload the other pages (kids, circle, schedule, places)
//				this.BindingContext = new KidsViewModel(App.client);
//				lblActivity.Text = "Loading Kids";
//				await ((KidsViewModel)BindingContext).ExecuteLoadItemsCommand();
//
//				this.BindingContext = new MyCircleViewModel(App.client);
//				lblActivity.Text = "Loading Circle";
//				await ((MyCircleViewModel)BindingContext).ExecuteLoadItemsCommand();
//
//				this.BindingContext = new AccountPlaceViewModel(App.client);
//				lblActivity.Text = "Loading Places";
//				await ((AccountPlaceViewModel)BindingContext).ExecuteLoadItemsCommand();
//
//				this.BindingContext = new SplashViewModel(App.client);
//			
//				//let the parent know to launch the homepage
//				MessagingCenter.Send<string>("register", "launch");
//			});

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

		
			ExtendedEntry eepw2 = new ExtendedEntry ();
			eepw2.IsPassword = true;
			eepw2.VerticalOptions = LayoutOptions.StartAndExpand;
			eepw2.WidthRequest = (App.ScaledWidth) - 50;
			eepw2.HasBorder = false;
			eepw2.Placeholder = "Repeat Password";
			eepw2.TextColor = Color.White;
			eepw2.PlaceholderTextColor = Color.Gray;
			eepw2.Font = f;
			fields.Children.Add (eepw2);

			bv = new BoxView ();
			bv.HeightRequest = 1;
			bv.WidthRequest = (App.ScaledWidth) - 150;
			bv.Color = Color.White;
			fields.Children.Add (bv);


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
			b.Text = "Register";
			b.TextColor = Color.FromHex ("F6637F");
			b.BackgroundColor = Color.FromHex ("49376D");
			b.BorderColor = Color.FromHex ("54D29F");
			b.BorderRadius = 8;
			b.BorderWidth = 2;
			fields.Children.Add (b);
			b.Clicked += async delegate(object sender, EventArgs e) {
				//ok, let's log that badbear in
				if (eepw.Text.Length < 6)
				{
					lblActivity.Text= "Password must be atleast 6 characters!";
					return;
				}
				if (eepw.Text != eepw2.Text)
				{
					lblActivity.Text = "Passwords do not match!";
					return;
				}
				if (!Regex.IsMatch(ee.Text, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
				{
					lblActivity.Text = "Email not valid!";
					return;
				}

				this.ViewModel.AuthAccount = new AuthAccounts();
				this.ViewModel.AuthAccount.Email = ee.Text;
				this.ViewModel.AuthAccount.Username = ee.Text;
				this.ViewModel.AuthAccount.Password = eepw.Text;
				await this.ViewModel.ExecuteRegisterCommand();
			};


			bv = new BoxView ();
			bv.HeightRequest = 15;
			fields.Children.Add (bv);

			//add a login button (for those who somehow got stuck here)
			Label hll = new Label ();
			hll.FormattedText = new FormattedString ();
			hll.FormattedText.Spans.Add (new Span { Text = "If you already registered, click here", FontSize= 14, ForegroundColor = Color.White, FontAttributes = FontAttributes.Bold });
			hll.HorizontalOptions = LayoutOptions.Center;

			var tap = new TapGestureRecognizer ();
			tap.Tapped += (sender, e) => {
				MessagingCenter.Send<string>("intro", "login");
			};

			hll.GestureRecognizers.Add(tap);
			fields.Children.Add (hll);

		}

		protected SplashViewModel ViewModel
		{
			get { return this.BindingContext as SplashViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

