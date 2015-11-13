using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace PickUpApp
{	
	public partial class SelectContact : ContentPage
	{	
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

