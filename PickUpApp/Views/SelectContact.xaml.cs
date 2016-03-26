using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using XLabs.Forms.Controls;

namespace PickUpApp
{	
	public partial class SelectContact : ContentPage
	{	
		ExtendedListView searchView;

		public SelectContact ()
		{
			InitializeComponent ();

			this.ViewModel = new SelectContactViewModel (App.client);
			lstContacts.ItemSelected += HandleItemSelected;
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);


			//TODO: figure out why this alert doesn't pop if there's a permissions error on device contacts
			MessagingCenter.Subscribe<string>(this, "ContactsError",  async(err) => {
				await DisplayAlert("Contacts Error!", err, "OK");
			});
		

			StackLayout slSearch = new StackLayout ();
			slSearch.Orientation = StackOrientation.Vertical;
			slSearch.BackgroundColor = AppColor.AppPurple;
			slSearch.HeightRequest = 40;

			BoxView bv = new BoxView ();
			bv.HeightRequest = 5;
			slSearch.Children.Add (bv);

			Label lSearch = new Label ();
			lSearch.TextColor = Color.White;
			lSearch.HeightRequest = 40;
			lSearch.Text = "Search Results";
			lSearch.FontAttributes = FontAttributes.Bold;
			lSearch.HorizontalOptions = LayoutOptions.CenterAndExpand;
			lSearch.VerticalOptions = LayoutOptions.CenterAndExpand;
			slSearch.Children.Add (lSearch);

			//add a separate listview with custom header for searched names
			searchView = new ExtendedListView();
			searchView.Header = slSearch;
			searchView.ItemTemplate = lstContacts.ItemTemplate;
			searchView.ItemSelected += HandleItemSelected;
			searchView.IsVisible = false;
			searchView.HasUnevenRows = false;
			searchView.RowHeight = 75;
			searchView.BackgroundColor = AppColor.AppGray;
			stacker.Children.Add (searchView);

			searchBar.TextChanged += delegate(object sender, TextChangedEventArgs e) {
				filterList(searchBar.Text);

			};

			searchBar.SearchButtonPressed += delegate(object sender, EventArgs e) {
				filterList(searchBar.Text);
			};

		}

		private void filterList(string filter)
		{
			
			if (string.IsNullOrWhiteSpace (searchBar.Text)) {
				lstContacts.IsVisible = true;
				searchView.IsVisible = false;

			}
			else {
				lstContacts.IsVisible = false;
				searchView.IsVisible = true;
				searchView.ItemsSource = ViewModel.Contacts.Where (c => c.DisplayName.ToLower ().Contains (filter.ToLower ()));
			}
//			if (string.IsNullOrWhiteSpace(searchBar.Text))
//			{
//				lstContacts.IsGroupingEnabled = true;
//				lstContacts.ItemsSource = ViewModel.ContactsSorted;
//			}
//			else
//			{
//				lstContacts.IsGroupingEnabled = false;
//				lstContacts.ItemsSource = ViewModel.Contacts.Where(x =>x.DisplayName.Contains(searchBar.Text));
//			}
			lstContacts.EndRefresh();
		}

		void HandleItemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			ViewModel.CurrentContact = e.SelectedItem as LocalContact;
			MessagingCenter.Send<LocalContact> (e.SelectedItem as LocalContact, "contactpicked");

			//don't save it yet...take it to the add/edit screen and save it from there!
			//ViewModel.ExecuteAddEditCommand ().ConfigureAwait (false);
			//ok, grab the selected contact and add them to my circle (queuing an invite blah blah blah)
			//MessagingCenter.Send<LocalContact> (e.SelectedItem as LocalContact, "ContactAdded");
		}

		protected SelectContactViewModel ViewModel
		{
			get { return this.BindingContext as SelectContactViewModel; }
			set { this.BindingContext = value; }
		}
		void OnButtonClicked(object sender, EventArgs args)
		{
			Navigation.PopAsync ();
		}
	}
}

