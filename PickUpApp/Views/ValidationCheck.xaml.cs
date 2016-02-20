using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class ValidationCheck : ContentPage
	{
		public ValidationCheck ()
		{
			InitializeComponent ();


			Button b = new Button ();
			b.VerticalOptions = LayoutOptions.End;
			b.HorizontalOptions = LayoutOptions.Center;	
			b.HeightRequest = 55;
			b.WidthRequest = (App.ScaledWidth) - 50 ;
			b.FontAttributes = FontAttributes.Bold;
			b.FontSize = 18;
			b.Text = "Verified";
			b.TextColor = Color.FromHex ("F6637F");
			b.BackgroundColor = Color.FromHex ("49376D");
			b.BorderColor = Color.FromHex ("54D29F");
			b.BorderRadius = 8;
			b.BorderWidth = 2;
			stacker.Children.Add (b);
			b.Clicked += async delegate(object sender, EventArgs e) {
				//ok, let's log that badbear in
				await Navigation.PopModalAsync();
				//try again
				MessagingCenter.Send<Microsoft.WindowsAzure.MobileServices.MobileServiceClient>(App.client, "LoggedIn");
			};

		}
	}
}

