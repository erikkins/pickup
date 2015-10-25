using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class Intro2 : ContentPage
	{
		public Intro2 ()
		{
			InitializeComponent ();


			FFSpline spline = new FFSpline ();
			spline.Color = Color.FromRgb (241, 179, 70);
			spline.WidthRequest = App.Device.Display.Width;
			spline.HeightRequest = App.Device.Display.Height;
			spline.StartPoint = new Point (230, 315);
			spline.EndPoint = new Point (App.Device.Display.Width / 2, 250);
			AbsoluteLayout.SetLayoutBounds (spline, new Rectangle (0, 0, App.Device.Display.Width/2, App.Device.Display.Height/2));
			abs.Children.Add (spline);


			Image img = new Image ();
			img.Source = "intro_kids.png";
			img.HorizontalOptions = LayoutOptions.Center;
			img.VerticalOptions = LayoutOptions.Center;
			//AbsoluteLayout.SetLayoutFlags (img, AbsoluteLayoutFlags.HeightProportional | AbsoluteLayoutFlags.WidthProportional);
			//AbsoluteLayout.SetLayoutBounds (img, new Rectangle (0, 0, App.Device.Display.Width / 2, App.Device.Display.Height / 2));

			stacker.Children.Add (img);

			Label l = new Label ();
			l.FontSize = 21;
			l.TextColor = Color.White;
			l.Text = "Start by adding\ryour kids.";
			l.XAlign = TextAlignment.Center;
			l.HorizontalOptions = LayoutOptions.Center;
			stacker.Children.Add (l);

		}
	}
}

