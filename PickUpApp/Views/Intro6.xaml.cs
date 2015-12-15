using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class Intro6 : ContentPage
	{
		public Intro6 ()
		{
			InitializeComponent ();


			FFSpline spline = new FFSpline ();
			spline.Color = Color.FromRgb (241, 179, 70);
			spline.WidthRequest = App.Device.Display.Width;
			spline.HeightRequest = App.Device.Display.Height;
			spline.StartPoint = new Point (0, 270);
			spline.EndPoint = new Point (App.ScaledWidth , 280);
			AbsoluteLayout.SetLayoutBounds (spline, new Rectangle (0, 0, App.ScaledWidth, App.ScaledHeight));
			abs.Children.Add (spline);


			Image img = new Image ();
			//img.BackgroundColor = Color.FromRgb (73, 55, 109);
			img.Source = "intro_carpool.png";
			img.HorizontalOptions = LayoutOptions.Center;
			img.VerticalOptions = LayoutOptions.Start;
			stacker.Children.Add (img);


			BoxView spacer = new BoxView ();
			spacer.HeightRequest = 50;
			stacker.Children.Add (spacer);

			Label l = new Label ();
			l.FontSize = 21;
			l.TextColor = Color.White;
			l.Text = "FamFetch puts you in the driver's\rseat when it comes to your kid's\rpickups and dropoffs.";
			l.HorizontalTextAlignment = TextAlignment.Center;
			l.HorizontalOptions = LayoutOptions.Center;
			l.VerticalOptions = LayoutOptions.End;
			stacker.Children.Add (l);

//			Button b = new Button ();
//			b.Text = "close";
//			b.Clicked += async delegate(object sender, EventArgs e) {
//				await Navigation.PopModalAsync();
//			};
//			stacker.Children.Add (b);


			StackLayout slButtons = new StackLayout ();
			slButtons.Orientation = StackOrientation.Horizontal;
			slButtons.HorizontalOptions = LayoutOptions.CenterAndExpand;
			slButtons.VerticalOptions = LayoutOptions.EndAndExpand;
			slButtons.WidthRequest = (App.ScaledWidth) - 50;
			slButtons.HeightRequest = 100;

			Button bReg = new Button ();
			bReg.VerticalOptions = LayoutOptions.Start;
			bReg.HorizontalOptions = LayoutOptions.StartAndExpand;	
			bReg.HeightRequest = 55;
			bReg.WidthRequest = (App.ScaledQuarterWidth) - 50 ;
			bReg.FontAttributes = FontAttributes.Bold;
			bReg.FontSize = 18;
			bReg.Text = "Register";
			bReg.TextColor = Color.FromHex ("F6637F");
			bReg.BackgroundColor = Color.FromHex ("49376D");
			bReg.BorderColor = Color.FromHex ("54D29F");
			bReg.BorderRadius = 8;
			bReg.BorderWidth = 2;
			slButtons.Children.Add (bReg);
			bReg.Clicked +=  delegate(object sender, EventArgs e) {
				//register this guy
				MessagingCenter.Send<string>("intro", "register");
			};

			Button bLogin = new Button ();
			bLogin.VerticalOptions = LayoutOptions.Start;
			bLogin.HorizontalOptions = LayoutOptions.End;	
			bLogin.HeightRequest = 55;
			bLogin.WidthRequest = (App.ScaledQuarterWidth) - 50 ;
			bLogin.FontAttributes = FontAttributes.Bold;
			bLogin.FontSize = 18;
			bLogin.Text = "Login";
			bLogin.TextColor = Color.FromHex ("F6637F");
			bLogin.BackgroundColor = Color.FromHex ("49376D");
			bLogin.BorderColor = Color.FromHex ("54D29F");
			bLogin.BorderRadius = 8;
			bLogin.BorderWidth = 2;
			slButtons.Children.Add (bLogin);
			bLogin.Clicked += delegate(object sender, EventArgs e) {
				//login this guy
				MessagingCenter.Send<string>("intro", "login");
			};

			stacker.Children.Add (slButtons);

			abs.RaiseChild (stacker);
		}
	}
}

