using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using System.Linq;


namespace PickUpApp
{
	public enum PlaceType
	{
		ActivityPlace,
		StartingPlace,
		EndingPlace
	}

	public partial class PlaceSelector : ContentPage
	{
		private Schedule _currentSchedule;
		private PlaceType _placeType;

		private MapCell _mapCell;

		public PlaceSelector (Schedule currentSchedule, TrulyObservableCollection<KidSchedule> kidschedule, ObservableCollection<Kid> kids, AccountPlace selectedPlace, PlaceType placeType)
		{
			InitializeComponent ();
			_currentSchedule = currentSchedule;
			this.ViewModel = new ActivityAddEditViewModel (App.client, currentSchedule, kidschedule, kids);
			_placeType = placeType;
			NavigationPage.SetBackButtonTitle(this, "");
			this.ToolbarItems.Add (new ToolbarItem ("Done", null, () => {
				App.hudder.showHUD("Saving Place");
				MessagingCenter.Send<Schedule>(_currentSchedule, "UpdatePlease");
				//Navigation.PopAsync();
			}));


			//gotta be a lambda for this, but...this is just poor design! 
			foreach (AccountPlace ap in App.myPlaces) {
				switch (_placeType) {
				case PlaceType.ActivityPlace:
					if (ap.id == currentSchedule.AccountPlaceID) {
						ap.Selected = true;
					} else {
						ap.Selected = false;
					}	
					break;
				case PlaceType.StartingPlace:
					if (ap.id == currentSchedule.StartPlaceID) {
						ap.Selected = true;
					} else {
						ap.Selected = false;
					}	
					break;
				case PlaceType.EndingPlace:
					if (ap.id == currentSchedule.EndPlaceID) {
						ap.Selected = true;
					} else {
						ap.Selected = false;
					}
					break;
				}

			}
				
			this.BackgroundColor = Color.FromRgb (238, 236, 243);

			TableView tvMap = new TableView ();
			tvMap.Intent = TableIntent.Data;
			TableSection ts = new TableSection ();


			if (selectedPlace == null) {
				if (!string.IsNullOrEmpty (App.PositionLatitude) && !string.IsNullOrEmpty (App.PositionLongitude)) {
					_mapCell = new MapCell (double.Parse (App.PositionLatitude), double.Parse (App.PositionLongitude), "No address selected", "", 0);
				} else {
					_mapCell = new MapCell (41.8369, -87.6847, "No address selected", "", 0);
				}
				//_mapCell = new MapCell (double.Parse (currentSchedule.Latitude), double.Parse (currentSchedule.Longitude), currentSchedule.Address);
			} else {
				_mapCell = new MapCell (double.Parse(selectedPlace.Latitude), double.Parse(selectedPlace.Longitude), selectedPlace.Address, "", 0);
			}
			ts.Add (_mapCell);
			tvMap.Root.Add (ts);
			tvMap.HeightRequest = 202;
			tvMap.VerticalOptions = LayoutOptions.Start;
			stacker.Children.Add (tvMap);
			this.Appearing += delegate(object sender, EventArgs e) {
				_mapCell.Navigate();
			};


			ExtendedListView lvPlaces = new ExtendedListView () {
				ItemsSource = ViewModel.AccountPlaces,
				ItemTemplate = new DataTemplate (typeof(PlaceCell)),
				IsPullToRefreshEnabled = false,
				HasUnevenRows = false,
				BackgroundColor = Color.Transparent,
				RowHeight = 75,
				Header = null
			};
			stacker.Children.Add (lvPlaces);

			Button btnAdd = new Button ();
			btnAdd.VerticalOptions = LayoutOptions.Center;
			btnAdd.HorizontalOptions = LayoutOptions.Center;
			btnAdd.HeightRequest = 50;
			btnAdd.WidthRequest = App.ScaledWidth - 100;
			btnAdd.FontAttributes = FontAttributes.Bold;
			btnAdd.FontSize = 18;
			btnAdd.BorderRadius = 8;
			btnAdd.BackgroundColor = Color.FromRgb (73, 55, 109);
			btnAdd.TextColor = Color.FromRgb (84, 210, 159);
			btnAdd.Text = "ADD NEW PLACE";
			btnAdd.TranslationY = - 10;
			stacker.Children.Add (btnAdd);

			btnAdd.Clicked += async delegate(object sender, EventArgs e) {

				AccountPlace ap = new AccountPlace();
				if (!string.IsNullOrEmpty(App.PositionLongitude))
				{
					ap.Latitude = App.PositionLatitude;
					ap.Longitude = App.PositionLongitude;
				}

				await this.Navigation.PushAsync(new LocationSearch(ap));

			};

			MessagingCenter.Subscribe<AccountPlace>(this, "PlaceAdded", async (ap) =>
				{									
					//we actually need to reload places!
					//this is the loadEvent of AccountPlaceViewModel
					AccountPlaceViewModel apvm = new AccountPlaceViewModel(App.client);
					await apvm.ExecuteLoadItemsCommand();

					App.hudder.hideHUD();

					//but we want to mark this new place as the selected one!
					foreach(AccountPlace place in ViewModel.AccountPlaces)
					{
						if (ap.id == place.id)
						{
							place.Selected = true;
						}
					}
						
					try{
						await Navigation.PopAsync();
					}
					catch(Exception ex1)
					{
						System.Diagnostics.Debug.WriteLine("Could not pop window...probably already gone" + ex1);
					}
				});

//			lvKids.ItemTapped += delegate(object sender, ItemTappedEventArgs e) {
//
//				//((Xamarin.Forms.TemplatedItemsList<Xamarin.Forms.ItemsView<Xamarin.Forms.Cell>,Xamarin.Forms.Cell>)e.Group)
//
//				_currentSchedule.StartPlaceID = ((AccountPlace)e.Item).id;
//				_currentSchedule.StartPlaceName = ((AccountPlace)e.Item).PlaceName;
//				_currentSchedule.StartPlaceAddress = ((AccountPlace)e.Item).Address;
//
//				//this is insane...shouldn't have to rebuild the whole collection
//				ViewModel.AccountPlaces.Clear();
//				foreach (AccountPlace ap in App.myPlaces)
//				{
//					if (ap.id == currentSchedule.StartPlaceID)
//					{
//						ap.Selected = true;
//					}
//					else{
//						ap.Selected = false;
//					}
//					ViewModel.AccountPlaces.Add(ap);
//				}
//
//			};

			lvPlaces.ItemSelected += delegate(object sender, SelectedItemChangedEventArgs e) {

				if (e.SelectedItem == null)
				{
					return;
				}

				//ok, we need to differentiate between StartPlace and ActualLocation
				switch (_placeType)
				{
				case PlaceType.ActivityPlace:
					_currentSchedule.AccountPlaceID = ((AccountPlace)e.SelectedItem).id;
					break;
				case PlaceType.StartingPlace:
					_currentSchedule.StartPlaceID = ((AccountPlace)e.SelectedItem).id;
					break;
				case PlaceType.EndingPlace:
					_currentSchedule.EndPlaceID = ((AccountPlace)e.SelectedItem).id;
					break;
				}

				//_currentSchedule.StartPlaceName = ((AccountPlace)e.SelectedItem).PlaceName;
				//_currentSchedule.StartPlaceAddress = ((AccountPlace)e.SelectedItem).Address;

				_mapCell.Navigate(double.Parse(((AccountPlace)e.SelectedItem).Latitude), double.Parse(((AccountPlace)e.SelectedItem).Longitude), ((AccountPlace)e.SelectedItem).Address);

				foreach (AccountPlace ap in ViewModel.AccountPlaces) {
					switch (_placeType)
					{
					case PlaceType.ActivityPlace:
						if (ap.id == currentSchedule.AccountPlaceID) {
							ap.Selected = true;
						} else {
							ap.Selected = false;
						}
						break;
					case PlaceType.StartingPlace:
						if (ap.id == currentSchedule.StartPlaceID) {
							ap.Selected = true;
						} else {
							ap.Selected = false;
						}
						break;
					case PlaceType.EndingPlace:
						if (ap.id == currentSchedule.EndPlaceID) {
							ap.Selected = true;
						} else {
							ap.Selected = false;
						}
						break;
					}

				}
				//ViewModel.Refresh();
				lvPlaces.ItemTemplate = new DataTemplate (typeof(PlaceCell));

			};

			stacker.Children.Add (lvPlaces);


		}

		protected ActivityAddEditViewModel ViewModel
		{
			get { return this.BindingContext as ActivityAddEditViewModel; }
			set { this.BindingContext = value; }
		}
	}

	public class PlaceCell : ViewCell
	{

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 75;
			AccountPlace ap = (AccountPlace)c;

			if (ap == null) {
				return;
			}

			StackLayout slHoriz = new StackLayout ();
			slHoriz.Orientation = StackOrientation.Horizontal;

			BoxView bv = new BoxView ();
			bv.WidthRequest = 10;

			slHoriz.Children.Add (bv);

			Image greenpin = new Image ();
			greenpin.Source = "icn_pin.png";
			greenpin.HorizontalOptions = LayoutOptions.Start;
			greenpin.VerticalOptions = LayoutOptions.Center;

			slHoriz.Children.Add (greenpin);

			Label l = new Label ();
			l.Text = ap.PlaceName;
			l.VerticalOptions = LayoutOptions.Center;
			l.HorizontalOptions = LayoutOptions.Start;

			slHoriz.Children.Add (l);

			ImageButton ib = new ImageButton ();
			ib.HorizontalOptions = LayoutOptions.EndAndExpand;
			ib.VerticalOptions = LayoutOptions.Center;
			ib.ImageHeightRequest = 27;
			ib.ImageWidthRequest = 27;
			if (ap.Selected) {
				
				ib.Source = "ui_check_filled.png";
			} else {
				ib.Source = "ui_check_empty.png";
			}
			ib.SetBinding (ImageButton.CommandParameterProperty, new Binding ("."));

		
			ib.Clicked += delegate(object sender, EventArgs e) {

				var b = (ImageButton)sender;
				var t = b.CommandParameter;

				((ListView)((StackLayout)b.ParentView).ParentView).SelectedItem = t;
				if (ap.Selected)
				{
					//ap.Selected = false;
					ib.Source = "ui_check_filled.png";
					//base.View.Unfocus();
				}
				else{
					//ap.Selected = true;
					ib.Source = "ui_check_empty.png";
					//base.View.Focus();
				}
				//((ContentPage)((StackLayout)((ListView)((StackLayout)b.ParentView).ParentView).ParentView).ParentView).DisplayAlert("test", "Clicked " + t, "Cancel");


			};

			slHoriz.Children.Add (ib);


			View = slHoriz;

		}
	}
}

