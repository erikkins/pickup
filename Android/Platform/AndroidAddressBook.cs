using System;
using PickUpApp.Android;
using System.Diagnostics;
using Xamarin.Contacts;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidAddressBook))]

namespace PickUpApp.Android
{
	public class AndroidAddressBook:iAddressBook
	{

		public async Task<List<LocalContact>> testIt ()
		{
		
			List<LocalContact> myContacts = new List<LocalContact> ();

			var addressBook = new AddressBook (Xamarin.Forms.Forms.Context);
			var companyContacts = addressBook.ToList<Contact>() ;
			if (companyContacts.Any())
			{
				foreach (var contact in companyContacts)
				{
					LocalContact lc = new LocalContact ();
					lc.DisplayName = contact.DisplayName;

					var email = (from e in contact.Emails 
						select e.Address).FirstOrDefault();

					if (!string.IsNullOrEmpty(email)) {
						lc.Email = email;
					}

					lc.FirstName = contact.FirstName;
					lc.Id = contact.Id;
					lc.LastName = contact.LastName;

					var mobile = (from p in contact.Phones where
						p.Type == Xamarin.Contacts.PhoneType.Mobile
						select p.Number).FirstOrDefault();

					if (!string.IsNullOrEmpty(mobile)) {
						lc.Phone = mobile;
					}
					myContacts.Add (lc);

					var lastName = contact.LastName;
					var firstName = contact.FirstName;
					//var thumbnailBitmap = contact.GetThumbnail();
				}
			}
			
			return myContacts;
		}
	}
}

