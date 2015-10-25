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
			spline.EndPoint = new Point (App.Device.Display.Width / 2, 270);
			AbsoluteLayout.SetLayoutBounds (spline, new Rectangle (0, 0, App.Device.Display.Width/2, App.Device.Display.Height/2));
			abs.Children.Add (spline);


			Image img = new Image ();
			//img.BackgroundColor = Color.FromRgb (73, 55, 109);
			img.Source = "intro_bubble_1.png";
			img.HorizontalOptions = LayoutOptions.StartAndExpand;
			img.VerticalOptions = LayoutOptions.Center;
			stacker.Children.Add (img);

			Image img2 = new Image ();
			//img2.BackgroundColor = Color.FromRgb (73, 55, 109);
			img2.Source = "intro_bubble_2.png";
			img2.HorizontalOptions = LayoutOptions.EndAndExpand;
			img2.VerticalOptions = LayoutOptions.Center;
			stacker.Children.Add (img2);


			Label l = new Label ();
			l.FontSize = 21;
			l.TextColor = Color.White;
			l.Text = "Now you can see your schedule\rand send fetch requests to\ryour circle when needed.";
			l.XAlign = TextAlignment.Center;
			l.HorizontalOptions = LayoutOptions.Center;
			stacker.Children.Add (l);

			abs.RaiseChild (stacker);
		}
	}
}

