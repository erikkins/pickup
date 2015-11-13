using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class Intro5 : ContentPage
	{
		public Intro5 ()
		{
			InitializeComponent ();


			FFSpline spline = new FFSpline ();
			spline.Color = Color.FromRgb (241, 179, 70);
			spline.WidthRequest = App.Device.Display.Width;
			spline.HeightRequest = App.Device.Display.Height;
			spline.StartPoint = new Point (0, 240);
			spline.EndPoint = new Point (App.ScaledWidth, 270);
			AbsoluteLayout.SetLayoutBounds (spline, new Rectangle (0, 0, App.ScaledWidth, App.ScaledHeight));
			abs.Children.Add (spline);

			Grid imageGrid = new Grid
			{
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = Color.Transparent,
				WidthRequest = (App.ScaledWidth)-100,
				RowSpacing = 0,
				//ColumnSpacing = 0,
				RowDefinitions = 
				{
					new RowDefinition { Height = new GridLength(100, GridUnitType.Absolute) },
					new RowDefinition { Height = new GridLength(150, GridUnitType.Absolute) },
					new RowDefinition {Height = new GridLength(100, GridUnitType.Absolute)}
				},
				ColumnDefinitions = 
				{
					new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) },
					new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) },
					new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) }
				}
				};	

			Image img = new Image ();
			//img.BackgroundColor = Color.FromRgb (73, 55, 109);
			img.Source = "intro_bubble_1.png";
			img.HorizontalOptions = LayoutOptions.Start;
			img.VerticalOptions = LayoutOptions.Center;
			imageGrid.Children.Add (img, 0, 2, 0, 2);

			Image img2 = new Image ();
			//img2.BackgroundColor = Color.FromRgb (73, 55, 109);
			img2.Source = "intro_bubble_2.png";
			img2.HorizontalOptions = LayoutOptions.Start;
			img2.VerticalOptions = LayoutOptions.Center;
			imageGrid.Children.Add (img2, 1, 3, 1, 3);

			stacker.Children.Add (imageGrid);


			Label l = new Label ();
			l.FontSize = 21;
			l.TextColor = Color.White;
			l.Text = "Now you can see your schedule\rand send fetch requests to\ryour circle when needed.";
			l.XAlign = TextAlignment.Center;
			l.HorizontalOptions = LayoutOptions.Center;
			stacker.Children.Add (l);


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

