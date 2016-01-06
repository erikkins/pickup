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
				k.Mine = true;
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

			MessagingCenter.Subscribe<EmptyClass> (this, "KidDeleted", (p) => {
				if (string.IsNullOrEmpty(p.Status))
				{
					//Navigation.PopAsync();
					ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
				}
				else{
					DisplayAlert("Could not delete", "This kid is in use in the following activities: " + p.Status, "OK");
				}
			});

		}

		public void OnDelete (object sender, EventArgs e) {
			var mi = ((MenuItem)sender);
			Kid k = (Kid)mi.CommandParameter;
			if (k.Mine) {
				ViewModel.SelectedKid = k;
				ViewModel.ExecuteDeleteCommand ().ConfigureAwait (false);
			} else {
				DisplayAlert ("Uh oh", "You cannot delete someone else's kid!", "OK");
			}
			//DisplayAlert("Delete Context Action", k.Fullname + " delete context action", "OK");
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

