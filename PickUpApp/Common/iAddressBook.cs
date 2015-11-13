using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PickUpApp
{
	public interface iAddressBook
	{
		//Task<List<LocalContact>> loadContacts();
		List<LocalContact> loadContacts(out string errorMessage);
		//void launch();
	}
}

