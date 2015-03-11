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
			webviewer.Source = url;
		}
		void OnCloseClicked(object sender, EventArgs args)
		{
			Navigation.PopModalAsync ();
		}
	}
}

