using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class FetchRequest2 : ContentPage
	{
		public FetchRequest2 ()
		{
			InitializeComponent ();
			this.ToolbarItems.Add (new ToolbarItem ("Send", null, async() => {
				//pop the calendar window
				//await DisplayAlert("CAL!", "show the calendar", "Cancel");
				await Navigation.PopToRootAsync();
			}));
		}
	}
}

