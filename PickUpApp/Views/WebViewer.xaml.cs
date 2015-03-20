using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class WebViewer : ContentPage
	{
		public WebViewer (string url)
		{
			InitializeComponent ();
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			webviewer.Source = url;
		}
		void OnCloseClicked(object sender, EventArgs args)
		{
			Navigation.PopModalAsync ();
		}
	}
}

