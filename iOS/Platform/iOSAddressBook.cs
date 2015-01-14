using System;
using PickUpApp.iOS;
using MonoTouch.AddressBook;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Collections;

[assembly: Xamarin.Forms.Dependency (typeof(iOSAddressBook))]
namespace PickUpApp.iOS
{
	public class iOSAddressBook:iAddressBook
	{
		public void launch()
		{
	
		}

		public async Task<List<LocalContact>> testIt ()
		{
			List<LocalContact> myContacts = new List<LocalContact> ();
			NSError err;
			var addressBook = ABAddressBook.Create (out err);		
			var authStatus = ABAddressBook.GetAuthorizationStatus ();
			if (authStatus == ABAuthorizationStatus.Authorized) {
				ABPerson[] allContacts = addressBook.GetPeople ();
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
			} else {
				addressBook.RequestAccess (delegate(bool granted, NSError error) {
					if (granted) {
						ABPerson[] allContacts = addressBook.GetPeople ();
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
					}
				});
			}

			return myContacts;
		}
	}
}