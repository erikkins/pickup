using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace PickUpApp
{
	public class SelectContactViewModel:BaseViewModel
	{
		public ObservableCollection<LocalContact> Contacts { get; set; }
		public ObservableCollection<Grouping<string, LocalContact>> ContactsSorted { get; set; }
		public LocalContact CurrentContact{ get; set;}

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

				Account tempAccount = new Account();
				tempAccount.Email = CurrentContact.Email;
				tempAccount.Firstname = CurrentContact.FirstName;
				tempAccount.Lastname = CurrentContact.LastName;
				tempAccount.Phone = CurrentContact.Phone;

				var contact = await client.InvokeApiAsync<Account, Account>("checkandregisteraccount", tempAccount);
				System.Diagnostics.Debug.WriteLine(contact.Fullname);
			}
			catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine ("checkregex " + ex.Message);
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
				var contacts = await DependencyService.Get<iAddressBook> ().loadContacts ();
				Contacts.Clear();
				foreach (var c in contacts)
				{
					Contacts.Add(c);
				}
				//this.Contacts = contacts.ToArray();
				var sorted = from lc in Contacts
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

