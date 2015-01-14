using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace PickUpApp
{
	public interface IMobileClient
	{
		Task<MobileServiceUser> Authorize(MobileServiceAuthenticationProvider provider);
		void Logout();
	}
}

