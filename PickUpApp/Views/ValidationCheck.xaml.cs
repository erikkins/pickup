using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace PickUpApp
{
	public partial class ValidationCheck : ContentPage
	{
		public ValidationCheck ()
		{
			InitializeComponent ();

			Label l = new Label ();
			l.TextColor = Color.White;
			l.VerticalOptions = LayoutOptions.End;
			l.HorizontalTextAlignment = TextAlignment.Center;
			l.HorizontalOptions = LayoutOptions.Center;
			l.FontSize = 12;
			l.Text = "By clicking Verified you accept our terms of service and data policy. For more information please visit:";

			Label hll = new Label ();
			hll.FormattedText = new FormattedString ();
			hll.FormattedText.Spans.Add (new Span { Text = "www.famfetch.com", FontSize= 14, ForegroundColor = Color.White, FontAttributes = FontAttributes.Bold });
			hll.HorizontalOptions = LayoutOptions.Center;

			var tap = new TapGestureRecognizer ();
			tap.Tapped += (sender, e) => {
				App.Device.LaunchUriAsync (new Uri("http://www.famfetch.com/governance"));
			};

			hll.GestureRecognizers.Add(tap);


		
			stacker.Children.Add (l);
			stacker.Children.Add (hll);
			BoxView spacer = new BoxView ();
			spacer.HeightRequest = 20;
			stacker.Children.Add (spacer);

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

