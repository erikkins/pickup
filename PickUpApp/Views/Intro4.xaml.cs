﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class Intro4 : ContentPage
	{
		public Intro4 ()
		{
			InitializeComponent ();


			FFSpline spline = new FFSpline ();
			spline.Color = Color.FromRgb (241, 179, 70);
			spline.WidthRequest = App.Device.Display.Width;
			spline.HeightRequest = App.Device.Display.Height;
			spline.StartPoint = new Point (0, 280);
			spline.EndPoint = new Point (App.ScaledWidth, 240);
			AbsoluteLayout.SetLayoutBounds (spline, new Rectangle (0, 0, App.ScaledWidth, App.ScaledHeight));
			abs.Children.Add (spline);

			int firstRowHeight = 50;
			if (App.ScaledHeight < 500)
			{
				firstRowHeight = 5;
			}

			Grid imageGrid = new Grid
			{
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent,
				WidthRequest = (App.ScaledWidth)-250,
				RowSpacing = 0,
				//ColumnSpacing = 1,

				RowDefinitions = 
				{
					new RowDefinition { Height = new GridLength(firstRowHeight, GridUnitType.Absolute) },
					new RowDefinition { Height = new GridLength(200, GridUnitType.Absolute) },
					new RowDefinition {Height = new GridLength(50, GridUnitType.Absolute)}
				},
				ColumnDefinitions = 
				{
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto }
				}
			};	


			Image img = new Image ();
			//img.BackgroundColor = Color.FromRgb (73, 55, 109);
			img.Source = "intro_places.png";
			img.HorizontalOptions = LayoutOptions.EndAndExpand;
			img.VerticalOptions = LayoutOptions.Start;
			imageGrid.Children.Add (img, 0, 1, 0, 2);

			Image img2 = new Image ();
			//img2.BackgroundColor = Color.FromRgb (73, 55, 109);
			img2.Source = "intro_cal.png";
			img2.HorizontalOptions = LayoutOptions.EndAndExpand;
			img2.VerticalOptions = LayoutOptions.End;
			imageGrid.Children.Add (img2, 1, 2, 1, 3);


			stacker.Children.Add (imageGrid);

			Label l = new Label ();
			l.FontSize = 21;
			l.TextColor = Color.White;
			l.Text = "Finally, add the places and\ractivities that make up\ryour daily schedule.";
			l.HorizontalTextAlignment = TextAlignment.Center;
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
