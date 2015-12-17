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

			this.ToolbarItems.Add (new ToolbarItem ("Add Kid", "icn_new.png", async() => {
				Kid k = new Kid ();
				//should this be modal?
				await Navigation.PushAsync (new AddEditKid (k));
			}));

			lstKids.ItemSelected += HandleItemSelected;
			//btnAdd.Clicked += HandleClicked;
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			MessagingCenter.Subscribe<Kid>(this, "KidAdded",  async(s) =>
				{
					//ViewModel.Refresh();
					try{
						await Navigation.PopAsync();
					}
					catch(Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex);		
					}
					 await ViewModel.ExecuteLoadItemsCommand();
				});
		}

		public void OnDelete (object sender, EventArgs e) {
			var mi = ((MenuItem)sender);
			Kid k = (Kid)mi.CommandParameter;
			DisplayAlert("Delete Context Action", k.Fullname + " delete context action", "OK");
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
			Navigation.PushAsync(new AddEditKid(e.SelectedItem as Kid));
			lstKids.SelectedItem = null;

		}

		protected KidsViewModel ViewModel
		{
			get { return this.BindingContext as KidsViewModel; }
			set { this.BindingContext = value; }
		}

	}
}

