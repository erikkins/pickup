using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using System.Linq;


namespace PickUpApp
{


	public partial class PlaceSelector : ContentPage
	{
		private Schedule _currentSchedule;



		public PlaceSelector (Schedule currentSchedule, ObservableCollection<KidSchedule> kidschedule, ObservableCollection<Kid> kids)
		{
			InitializeComponent ();
			_currentSchedule = currentSchedule;
			this.ViewModel = new ActivityAddEditViewModel (App.client, currentSchedule, kidschedule, kids);

			this.ToolbarItems.Add (new ToolbarItem ("Done", null, async() => {

				//how do we set the selected item?
				//need to set the StartPlaceID based upon the selected value!

				//what we really need to be doing here is calculating the travel time!


				MessagingCenter.Send<Schedule>(_currentSchedule, "UpdatePlease");
				await Navigation.PopAsync();

			}));


			//gotta be a lambda for this, but...
//			foreach (AccountPlace ap in myPlaces) {
//				if (ap.id == currentSchedule.StartPlaceID) {
//					ap.Selected = true;
//				} else {
//					ap.Selected = false;
//				}	}
				
			this.BackgroundColor = Color.FromRgb (238, 236, 243);


			ListView lvKids = new ListView () {
				ItemsSource = ViewModel.AccountPlaces,
				ItemTemplate = new DataTemplate (typeof(PlaceCell)),
				IsPullToRefreshEnabled = false,
				HasUnevenRows = false,
				BackgroundColor = Color.Transparent,
				RowHeight = 75,
				Header = null
			};




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

			lvKids.ItemSelected += delegate(object sender, SelectedItemChangedEventArgs e) {

				if (e.SelectedItem == null)
				{
					return;
				}

				_currentSchedule.StartPlaceID = ((AccountPlace)e.SelectedItem).id;
				_currentSchedule.StartPlaceName = ((AccountPlace)e.SelectedItem).PlaceName;
				_currentSchedule.StartPlaceAddress = ((AccountPlace)e.SelectedItem).Address;

				foreach (AccountPlace ap in ViewModel.AccountPlaces) {
					if (ap.id == currentSchedule.StartPlaceID) {
						ap.Selected = true;
					} else {
						ap.Selected = false;
					}
				}
				//ViewModel.Refresh();
				lvKids.ItemTemplate = new DataTemplate (typeof(PlaceCell));

			};

			stacker.Children.Add (lvKids);


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

