using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class FetchRequest1 : ContentPage
	{
		public FetchRequest1 ()
		{
			InitializeComponent ();
			ViewModel = new MyCircleViewModel (App.client);

			this.ToolbarItems.Add (new ToolbarItem ("Next", null, async() => {
				//pop the calendar window
				//await DisplayAlert("CAL!", "show the calendar", "Cancel");
				await Navigation.PushAsync(new FetchRequest2());
			}));
		}

		protected MyCircleViewModel ViewModel
		{
			get { return this.BindingContext as MyCircleViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

