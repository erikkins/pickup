using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace PickUpApp
{
	public class SelectContactViewModel:BaseViewModel
	{
		public ObservableCollection<LocalContact> Contacts { get; set; }
		private ObservableCollection<Grouping<string, LocalContact>> _contactsSorted;
		public ObservableCollection<Grouping<string, LocalContact>> ContactsSorted { get{ return _contactsSorted; } set{ _contactsSorted = value; NotifyPropertyChanged (); } }
		private LocalContact _localContact;
		public LocalContact CurrentContact{
			get{ return _localContact; } 
			set{ _localContact = value; NotifyPropertyChanged (); }
		}

		public SelectContactViewModel ()
		{
			Contacts = new ObservableCollection<LocalContact> ();
		}
		public SelectContactViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
			LoadItemsCommand.Execute (null);
		}
		public void search(string what)
		{
			var sorted = from lc in Contacts where lc.DisplayName.Contains(what)
				orderby lc.DisplayName
				group lc by lc.NameSort into contactGroup
				select new Grouping<string, LocalContact>(contactGroup.Key, contactGroup);

			//create a new collection of groups
			ContactsSorted = new ObservableCollection<Grouping<string, LocalContact>>(sorted);

		}
		public override async Task ExecuteAddEditCommand ()
		{
			IsLoading = true;

			try{

				AccountCircle tempAccount = new AccountCircle();
				tempAccount.Email = CurrentContact.Email;
				tempAccount.Firstname = CurrentContact.FirstName;
				tempAccount.Lastname = CurrentContact.LastName;
				tempAccount.Phone = CurrentContact.Phone;
				tempAccount.Accepted = CurrentContact.Accepted;
				tempAccount.Coparent = CurrentContact.Coparent;
				tempAccount.id = CurrentContact.Id;

				//anyway to pull the accountcircle id instead of the account id?

				var contact = await client.InvokeApiAsync<AccountCircle, List<AccountCircle>>("savecircle", tempAccount);
				//System.Diagnostics.Debug.WriteLine(contact[0].Fullname);
			}
			catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine ("savecontact " + ex);
			}
			finally{
				IsLoading = false;
			}


			MessagingCenter.Send<LocalContact> (CurrentContact, "ContactAdded");
			IsLoading = false;
		}
		public override async Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try
			{
				string err = "";
				var contacts =  DependencyService.Get<iAddressBook> ().loadContacts (out err);
				if (!string.IsNullOrEmpty(err))
				{
					MessagingCenter.Send<string>(err, "ContactsError");
				}
				if (contacts == null)
				{
					//something happened in there
					return;
				}
				Contacts.Clear();
				foreach (var c in contacts)
				{
					Contacts.Add(c);
				}
				//this.Contacts = contacts.ToArray();
				var sorted = from lc in Contacts where lc.FirstName != null && !lc.FirstName.StartsWith("*") && !lc.FirstName.StartsWith("<")
					orderby lc.DisplayName
					group lc by lc.NameSort into contactGroup
					select new Grouping<string, LocalContact>(contactGroup.Key, contactGroup);

				//create a new collection of groups
				ContactsSorted = new ObservableCollection<Grouping<string, LocalContact>>(sorted);
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Contact. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
			finally{
				IsLoading = false;
			}
			IsLoading = false; //redundant
		}
	}
}

