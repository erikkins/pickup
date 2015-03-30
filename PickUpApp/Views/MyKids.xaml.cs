using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PickUpApp
{	
	public partial class MyKids : ContentPage
	{	
		public MyKids ()
		{
			InitializeComponent ();
			this.ViewModel = new KidsViewModel (App.client);
			lstKids.ItemSelected += HandleItemSelected;
			btnAdd.Clicked += HandleClicked;
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			MessagingCenter.Subscribe<Kid>(this, "KidAdded", (s) =>
				{
					//ViewModel.Refresh();

					Navigation.PopModalAsync();
					ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
				});
		}

		void HandleClicked (object sender, EventArgs e)
		{
			Kid k = new Kid ();
			Navigation.PushModalAsync (new AddEditKid (k));
		}

		void HandleItemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			//I guess we'd edit from here
			if (e.SelectedItem == null) return;
			Navigation.PushModalAsync(new AddEditKid(e.SelectedItem as Kid));
			lstKids.SelectedItem = null;

		}
		public void OnDelete (object sender, EventArgs e) {
			var mi = ((MenuItem)sender);
			Kid k = (Kid)mi.CommandParameter;
			DisplayAlert("Delete Context Action", k.Id + " delete context action", "OK");
		}
		protected KidsViewModel ViewModel
		{
			get { return this.BindingContext as KidsViewModel; }
			set { this.BindingContext = value; }
		}

	}
}

