using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using Xamarin.Forms;
using PickUpApp.iOS;
using Xamarin.Auth;
using WindowsAzure.Messaging;

[assembly: Xamarin.Forms.Dependency(typeof(iOSMobileClient))]

namespace PickUpApp.iOS
{
	public class iOSMobileClient : IMobileClient
	{
		public async Task<MobileServiceUser> Authorize(Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider provider)
		{
			var accountStore = AccountStore.Create ();

			//right now let's support Facebook, Google and our own
			switch (provider) {
			case Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.Facebook:
				App.ServiceProvider = "Facebook";
				break;
			case Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.Google:
				App.ServiceProvider = "Google";
				break;
			case Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory:
				App.ServiceProvider = "AD";
				break;
			}

			var accounts = accountStore.FindAccountsForService (App.ServiceProvider).ToArray ();
//			if (accounts.Count() > 0) {
//				accountStore.Delete (accounts.First (), App.ServiceProvider);
//			}
			if (accounts.Count() != 0) {
			
				string realUsername = accounts [0].Username; //accounts [0].Username.Substring (accounts [0].Username.IndexOf (":") + 1);
				MobileServiceUser msu = new MobileServiceUser (realUsername);
				msu.MobileServiceAuthenticationToken = accounts [0].Properties ["token"];
				App.client.CurrentUser = msu;

				return msu;

			} else {
				//normal login
				try{
					//MobileServiceUser tmsu = await App.client.LoginAsync (UIApplication.SharedApplication.KeyWindow.RootViewController, provider);

					MobileServiceUser tmsu = await App.client.LoginAsync (UIApplication.SharedApplication.KeyWindow.RootViewController.PresentedViewController, provider);
					var account = new Xamarin.Auth.Account (tmsu.UserId, new Dictionary<string,string> { {
							"token",
							tmsu.MobileServiceAuthenticationToken
						}
					});

					accountStore.Save (account, App.ServiceProvider);			

					return tmsu;
				}
				catch 	(Exception ex) {
					System.Diagnostics.Debug.WriteLine (ex.ToString ());
				}
				return null;
			}
		}

		public void Logout()
		{
			foreach (var cookie in NSHttpCookieStorage.SharedStorage.Cookies)
				NSHttpCookieStorage.SharedStorage.DeleteCookie(cookie);

			App.client.Logout();
			var accountStore = AccountStore.Create ();
			var myaccounts = accountStore.FindAccountsForService (App.ServiceProvider);
			if (myaccounts.Count() > 0) {
				accountStore.Delete (myaccounts.First (), App.ServiceProvider);
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