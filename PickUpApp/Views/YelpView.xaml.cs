using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class YelpView : ContentPage
	{
		public YelpView (double latitude, double longitude)
		{
			InitializeComponent ();
			this.ViewModel = new YelpViewModel (App.client, latitude, longitude);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			lstYelp.ItemSelected += async delegate(object sender, SelectedItemChangedEventArgs e) {
				await Navigation.PushModalAsync(new WebViewer(((YelpModel)e.SelectedItem).MobileURL));
			};
		}
		protected YelpViewModel ViewModel
		{
			get { return this.BindingContext as YelpViewModel; }
			set { this.BindingContext = value; }
		}
		void OnCloseClicked(object sender, EventArgs args)
		{
			Navigation.PopModalAsync ();
		}
	}
}

