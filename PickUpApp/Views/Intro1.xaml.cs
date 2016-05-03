using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class Intro1 : ContentPage
	{
		public Intro1 ()
		{
			InitializeComponent ();


			//double deviceScale = App.Device.Display.Scale;

			StackLayout slButtons = new StackLayout ();
			slButtons.Orientation = StackOrientation.Horizontal;
			slButtons.HorizontalOptions = LayoutOptions.CenterAndExpand;
			slButtons.VerticalOptions = LayoutOptions.EndAndExpand;
			slButtons.WidthRequest = App.ScaledWidth - 50;

			slButtons.HeightRequest = 100;

			Button bReg = new Button ();
			bReg.VerticalOptions = LayoutOptions.Start;
			bReg.HorizontalOptions = LayoutOptions.StartAndExpand;	
			bReg.HeightRequest = 55;
			bReg.WidthRequest = App.ScaledQuarterWidth - 50 ;
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
			bLogin.WidthRequest = App.ScaledQuarterWidth - 50 ;
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


			//animate the arrow

			this.Appearing += async delegate(object sender, EventArgs e) {
				System.Diagnostics.Debug.WriteLine("Appearing");
				Rectangle oldBounds = orangearrow.Bounds;
				Rectangle newBounds = new Rectangle (oldBounds.X + 50, oldBounds.Y, oldBounds.Width, oldBounds.Height);
				for (int i=0; i < 5; i++)
				{
//					await orangearrow.TranslateTo(newBounds.X, newBounds.Y, 700, Easing.Linear);
//					await orangearrow.TranslateTo(oldBounds.X, oldBounds.Y, 700, Easing.Linear);
//					await System.Threading.Tasks.Task.Delay(300);

					//do it 5 times
					await orangearrow.LayoutTo (newBounds, 700, Easing.Linear);
					await orangearrow.LayoutTo (oldBounds, 700, Easing.Linear);
					await System.Threading.Tasks.Task.Delay(300);
				}
			};
		}
	}
}

