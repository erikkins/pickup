using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
//using Xamarin.Forms;
using PickUpApp.droid;
using Xamarin.Auth;
using Newtonsoft.Json.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidLoginClient))]

namespace PickUpApp.droid
{

	public class AndroidLoginClient : IMobileClient
	{

		public AndroidLoginClient() {}

		public async Task<MobileServiceUser> Authorize(MobileServiceAuthenticationProvider provider)
		{

			//var accountStore = AccountStore.Create (Application.Context);
			var accountStore = AccountStore.Create(Application.Context);

			var accounts = accountStore.FindAccountsForService ("Facebook").ToArray ();

			if (accounts.Count() != 0) {
				//login from cache
				//var token = new JObject ();
				//token = new JObject (accounts [0].Properties ["token"]);
				//var token = JObject.FromObject(new { access_token = accounts[0].Properties["token"]});
				string realUsername = accounts [0].Username; //accounts [0].Username.Substring (accounts [0].Username.IndexOf (":") + 1);
				MobileServiceUser msu = new MobileServiceUser (realUsername);
				msu.MobileServiceAuthenticationToken = accounts [0].Properties ["token"];
				App.client.CurrentUser = msu;
				/*
				try{
				msu = await App.client.LoginAsync( MobileServiceAuthenticationProvider.Facebook, token);
				}
				catch (Exception ex) {
					Console.Write (ex.Message);
				}
				*/


				return msu;

			} else {
				//normal login
				MobileServiceUser tmsu = await App.client.LoginAsync (Xamarin.Forms.Forms.Context, provider);

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

			CookieSyncManager.CreateInstance(Application.Context);
			CookieManager.Instance.RemoveAllCookie();

			App.client.Logout();
			var accountStore = AccountStore.Create (Application.Context);
			var myaccounts = accountStore.FindAccountsForService ("Facebook");
			if (myaccounts.Count() > 0)
			{
				accountStore.Delete (myaccounts.First(), "Facebook");
			}

		}
	}
	/* ORIGINAL
	 * public class AndroidLoginClient : IMobileClient
	{
		public AndroidLoginClient() {}

		public async Task<MobileServiceUser> Authorize(MobileServiceAuthenticationProvider provider)
		{
			return await App.client.LoginAsync(Xamarin.Forms.Forms.Context, provider);
		}

		public void Logout()
		{

			CookieSyncManager.CreateInstance(Application.Context);
			CookieManager.Instance.RemoveAllCookie();

			App.client.Logout();
		}
	}
	*/
}