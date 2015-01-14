using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using System.Threading.Tasks;
using Xamarin.Forms;
using PickUpApp.iOS;
using Xamarin.Auth;

[assembly: Xamarin.Forms.Dependency(typeof(iOSMobileClient))]

namespace PickUpApp.iOS
{
	public class iOSMobileClient : IMobileClient
	{
		public async Task<MobileServiceUser> Authorize(Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider provider)
		{

			var accountStore = AccountStore.Create ();

			var accounts = accountStore.FindAccountsForService ("Facebook").ToArray ();

			if (accounts.Count() != 0) {
			
				string realUsername = accounts [0].Username; //accounts [0].Username.Substring (accounts [0].Username.IndexOf (":") + 1);
				MobileServiceUser msu = new MobileServiceUser (realUsername);
				msu.MobileServiceAuthenticationToken = accounts [0].Properties ["token"];
				App.client.CurrentUser = msu;
				return msu;

			} else {
				//normal login
				MobileServiceUser tmsu = await App.client.LoginAsync (AppDelegate.MainView, provider);

				var account = new Xamarin.Auth.Account (tmsu.UserId, new Dictionary<string,string> { {
						"token",
						tmsu.MobileServiceAuthenticationToken
					}
				});
				accountStore.Save (account, "Facebook");

				return tmsu;
			}
		}

		public void Logout()
		{
			foreach (var cookie in NSHttpCookieStorage.SharedStorage.Cookies)
				NSHttpCookieStorage.SharedStorage.DeleteCookie(cookie);

			App.client.Logout();
			var accountStore = AccountStore.Create ();
			var myaccounts = accountStore.FindAccountsForService ("Facebook");
			if (myaccounts.Count() > 0) {
				accountStore.Delete (myaccounts.First (), "Facebook");
			}
		}
	}

	/* ORIGINAL
	 * public class iOSMobileClient : IMobileClient
	{
		public async Task<MobileServiceUser> Authorize(Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider provider)
		{
			return await App.client.LoginAsync(AppDelegate.MainView, provider);
		}

		public void Logout()
		{
			foreach (var cookie in NSHttpCookieStorage.SharedStorage.Cookies)
				NSHttpCookieStorage.SharedStorage.DeleteCookie(cookie);

			App.client.Logout();
		}
	}
	 * 
	 * */
}