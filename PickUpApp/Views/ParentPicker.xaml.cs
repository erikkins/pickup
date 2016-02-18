using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using System.Linq;

namespace PickUpApp
{
	public partial class ParentPicker : ContentPage
	{
		private PlaceType _placeType;
		public ParentPicker (Schedule currentSchedule, TrulyObservableCollection<KidSchedule> kidschedule, ObservableCollection<Kid> kids, PlaceType placeType)
		{
			InitializeComponent ();
			_placeType = placeType;

			this.ViewModel = new ActivityAddEditViewModel (App.client, currentSchedule, kidschedule, kids);

			NavigationPage.SetBackButtonTitle(this, "");
			this.ToolbarItems.Add (new ToolbarItem ("Done", null, () => {
				App.hudder.showHUD("Saving Parent");
				MessagingCenter.Send<Schedule>(currentSchedule, "UpdatePlease");
				//Navigation.PopAsync();
			}));

			foreach (AccountCircle ac in App.myCircle) {
				switch (_placeType) {
				case PlaceType.EndingPlace:
					if (currentSchedule.DefaultPickupAccount == ac.id) {
						ac.Selected = true;
					}
					break;
				case PlaceType.StartingPlace:
					if (currentSchedule.DefaultDropOffAccount == ac.id) {
						ac.Selected = true;
					}
					break;
				}
			}

			ExtendedListView lvParent = new ExtendedListView () {
				ItemsSource = App.myCoparents,
				ItemTemplate = new DataTemplate (typeof(CircleCellNoDelete)),  
				IsPullToRefreshEnabled = false,
				HasUnevenRows = false,
				BackgroundColor = Color.Transparent,
				RowHeight = 75,
				Header = null
			};

			lvParent.ItemAppearing  +=  delegate(object sender, ItemVisibilityEventArgs e) {
				System.Diagnostics.Debug.WriteLine("");
			};

			lvParent.ItemSelected += delegate(object sender, SelectedItemChangedEventArgs e) {

				if (e.SelectedItem == null)
				{
					return;
				}

				//ok, we need to differentiate between StartPlace and ActualLocation
				switch (_placeType)
				{
				case PlaceType.StartingPlace:
					currentSchedule.DefaultDropOffAccount = ((AccountCircle)e.SelectedItem).id;
					break;
				case PlaceType.EndingPlace:
					currentSchedule.DefaultPickupAccount = ((AccountCircle)e.SelectedItem).id;
					break;
				}
					
				//not sure we need this
				foreach (AccountCircle ac in App.myCoparents) {
					switch (_placeType)
					{
					case PlaceType.StartingPlace:
						if (ac.id == currentSchedule.DefaultDropOffAccount) {
							ac.Selected = true;
						} else {
							ac.Selected = false;
						}
						break;
					case PlaceType.EndingPlace:
						if (ac.id == currentSchedule.DefaultPickupAccount) {
							ac.Selected = true;
						} else {
							ac.Selected = false;
						}
						break;
					}

				}
				lvParent.ItemTemplate = new DataTemplate (typeof(CircleCellNoDelete));
			};

			stacker.Children.Add (lvParent);



		}
		protected ActivityAddEditViewModel ViewModel
		{
			get { return this.BindingContext as ActivityAddEditViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

