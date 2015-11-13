using System;
using PickUpApp.iOS;
using AddressBook;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Foundation;
using System.Collections.Generic;
using System.Collections;
using PickUpApp;


[assembly: Xamarin.Forms.Dependency (typeof(iOSAddressBook))]
namespace PickUpApp.iOS
{
	public class iOSAddressBook:iAddressBook
	{
		public void launch()
		{
	
		}

		private List<LocalContact> loadThem(ABAddressBook theBook)
		{
			List<LocalContact> myContacts = new List<LocalContact> ();
			ABPerson[] allContacts = theBook.GetPeople ();
			foreach (ABPerson abp in allContacts) {
				LocalContact lc = new LocalContact ();
				lc.DisplayName = abp.FirstName + " " + abp.LastName;
				if (abp.GetEmails ().Count > 0) {
					lc.Email = abp.GetEmails ().FirstOrDefault ().Value;
				}
				lc.FirstName = abp.FirstName;
				lc.Id = abp.Id.ToString();
				lc.LastName = abp.LastName;
				if (abp.GetPhones ().Count > 0) {
					lc.Phone = abp.GetPhones ().FirstOrDefault ().Value;
				}
				myContacts.Add (lc);
			}
			return myContacts;
		}

		//public async Task<List<LocalContact>> loadContacts ()
		public  List<LocalContact> loadContacts (out string errorMessage)
		{
			errorMessage = "";
			List<LocalContact> myContacts = new List<LocalContact> ();
			try{
			NSError err;
			var addressBook = ABAddressBook.Create (out err);		

			if (err != null && err.Domain == "ABAddressBookErrorDomain")
			{
				//we have a permissions error here
					errorMessage = "You must enable access to Contacts!";
					return myContacts;
			}

			var authStatus = ABAddressBook.GetAuthorizationStatus ();
			if (authStatus == ABAuthorizationStatus.Authorized) {
				myContacts = loadThem (addressBook);
			} else {
				 addressBook.RequestAccess (delegate(bool granted, NSError error) {
					if (granted) {
						myContacts = loadThem(addressBook);
					}
					else{
						//denied access!
						LocalContact lc = new LocalContact();
						lc.DisplayName = "ACCESS DENIED!";
						myContacts.Add(lc);
					}
				});
			}
			}
			catch(Exception ex) {
				Debug.Write (ex);
			}

			return myContacts;
		}
	}
}