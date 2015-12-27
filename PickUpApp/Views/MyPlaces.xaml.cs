using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class MyPlaces : ContentPage
	{
		public MyPlaces ()
		{
			InitializeComponent ();
			this.ViewModel = new AccountPlaceViewModel (App.client);
			lstPlaces.ItemSelected += HandleItemSelected;

			this.ToolbarItems.Add (new ToolbarItem ("Add Place", "icn_new.png", async() => {
				await this.Navigation.PushAsync(new LocationSearch());
			}));

//			Button btnAdd = new Button ();
//			btnAdd.VerticalOptions = LayoutOptions.Center;
//			btnAdd.HorizontalOptions = LayoutOptions.Center;
//			btnAdd.HeightRequest = 50;
//			btnAdd.WidthRequest = (App.ScaledQuarterWidth/2) - 40;
//			btnAdd.FontAttributes = FontAttributes.Bold;
//			btnAdd.FontSize = 18;
//			btnAdd.BorderRadius = 8;
//			btnAdd.BackgroundColor = Color.FromRgb (73, 55, 109);
//			btnAdd.TextColor = Color.FromRgb (84, 210, 159);
//			btnAdd.Text = "ADD NEW PLACE";
//			btnAdd.TranslationY = - 10;
//			stacker.Children.Add (btnAdd);
//
//			btnAdd.Clicked += async delegate(object sender, EventArgs e) {
//				
//				await this.Navigation.PushAsync(new LocationSearch());
//			};

			//this.Padding = new Thickness(10, Device.OnPlatform(25, 0, 0), 10, 5);
			MessagingCenter.Subscribe<AccountPlace>(this, "PlaceAdded", (s) =>
				{
					 Navigation.PopAsync();
					 ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
				});

			MessagingCenter.Subscribe<EmptyClass> (this, "PlaceDeleted", (p) => {
				if (string.IsNullOrEmpty(p.Status))
				{
					//Navigation.PopAsync();
					ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
				}
				else{
					DisplayAlert("Could not delete", "This place is in use in the following activities: " + p.Status, "OK");
				}
			});
		}

//		void HandleClicked (object sender, EventArgs e)
//		{
//			AccountPlace ap = new AccountPlace ();
//			Navigation.PushModalAsync (new AddEditPlace (ap));
//		}

		void HandleItemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			//I guess we'd edit from here
			if (e.SelectedItem == null) return;
			Navigation.PushAsync (new LocationSearch (e.SelectedItem as AccountPlace));
			//Navigation.PushModalAsync(new AddEditPlace(e.SelectedItem as AccountPlace));
			lstPlaces.SelectedItem = null;

		}
		public void OnDelete (object sender, EventArgs e) {
			var mi = ((MenuItem)sender);
			AccountPlace ap = (AccountPlace)mi.CommandParameter;

			ViewModel.CurrentPlace = ap;
			ViewModel.ExecuteDeleteCommand ().ConfigureAwait(false);
		}

		protected AccountPlaceViewModel ViewModel
		{
			get { return this.BindingContext as AccountPlaceViewModel; }
			set { this.BindingContext = value; }
		}

	}
}

