using System;
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
			spline.EndPoint = new Point (App.Device.Display.Width / 2, 240);
			AbsoluteLayout.SetLayoutBounds (spline, new Rectangle (0, 0, App.Device.Display.Width/2, App.Device.Display.Height/2));
			abs.Children.Add (spline);



			Image img = new Image ();
			//img.BackgroundColor = Color.FromRgb (73, 55, 109);
			img.Source = "intro_places.png";
			img.HorizontalOptions = LayoutOptions.StartAndExpand;
			img.VerticalOptions = LayoutOptions.Center;
			stacker.Children.Add (img);

			Image img2 = new Image ();
			//img2.BackgroundColor = Color.FromRgb (73, 55, 109);
			img2.Source = "intro_cal.png";
			img2.HorizontalOptions = LayoutOptions.EndAndExpand;
			img2.VerticalOptions = LayoutOptions.Center;
			stacker.Children.Add (img2);


			Label l = new Label ();
			l.FontSize = 21;
			l.TextColor = Color.White;
			l.Text = "Finally, add the places and\ractivities that make up\ryour daily schedule.";
			l.XAlign = TextAlignment.Center;
			l.HorizontalOptions = LayoutOptions.Center;
			stacker.Children.Add (l);

			abs.RaiseChild (stacker);
		}
	}
}

