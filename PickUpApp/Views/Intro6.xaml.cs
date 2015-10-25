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
			spline.EndPoint = new Point (App.Device.Display.Width / 3, 280);
			AbsoluteLayout.SetLayoutBounds (spline, new Rectangle (0, 0, App.Device.Display.Width/2, App.Device.Display.Height/2));
			abs.Children.Add (spline);


			Image img = new Image ();
			//img.BackgroundColor = Color.FromRgb (73, 55, 109);
			img.Source = "intro_carpool.png";
			img.HorizontalOptions = LayoutOptions.StartAndExpand;
			img.VerticalOptions = LayoutOptions.Center;
			stacker.Children.Add (img);


			Label l = new Label ();
			l.FontSize = 21;
			l.TextColor = Color.White;
			l.Text = "FamFetch puts you in the driver's\rseat when it comes to your kid's\rpickups and dropoffs.";
			l.XAlign = TextAlignment.Center;
			l.HorizontalOptions = LayoutOptions.Center;
			stacker.Children.Add (l);

			Button b = new Button ();
			b.Text = "close";
			b.Clicked += async delegate(object sender, EventArgs e) {
				await Navigation.PopModalAsync();
			};
			stacker.Children.Add (b);

			abs.RaiseChild (stacker);
		}
	}
}

