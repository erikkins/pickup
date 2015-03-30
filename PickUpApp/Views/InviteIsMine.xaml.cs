using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class InviteIsMine : ContentPage
	{
		public InviteIsMine ()
		{
			InitializeComponent ();
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			Button btnOK = new Button ();
			btnOK.Text = "Dismiss";
			btnOK.TextColor = Color.Black;
			btnOK.FontSize = 18;
			btnOK.BackgroundColor = Color.Green;
			btnOK.VerticalOptions = LayoutOptions.End;
			btnOK.Clicked += async delegate(object sender, EventArgs e) {
				await Navigation.PopModalAsync();
			};
			stacker.Children.Add (btnOK);
		}
	}
}

