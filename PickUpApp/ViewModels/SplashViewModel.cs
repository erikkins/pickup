using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;

using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using PickUpApp;

namespace PickUpApp
{
	public class SplashViewModel:BaseViewModel
	{

		public bool RememberPassword
		{
			get{
				return Settings.RememberPassword;
			}
			set{
				Settings.RememberPassword = value;
				NotifyPropertyChanged ("RememberPassword");
			}
		}

		public string Firstname
		{
			get{
				return App.myAccount.Firstname;
			}
			set{
				App.myAccount.Firstname = value;
				NotifyPropertyChanged ();
			}
		}
		public string Lastname
		{
			get{
				return App.myAccount.Lastname;
			}
			set{
				App.myAccount.Lastname = value;
				NotifyPropertyChanged ();
			}
		}
		public string Phone
		{
			get{
				return App.myAccount.Phone;
			}
			set{
				App.myAccount.Phone = value;
				NotifyPropertyChanged ();
			}
		}
		public string Email
		{
			get{
				return App.myAccount.Email;
			}
			set{
				App.myAccount.Email = value;
				NotifyPropertyChanged ();
			}
		}
		public string PhotoURL
		{
			get{
				return App.myAccount.PhotoURL;
			}
			set{
				App.myAccount.PhotoURL = value;
				NotifyPropertyChanged ();
			}
		}

		public SplashViewModel ()
		{

		}

		public SplashViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
			//SaveIt ();
			//WhackIt ();
			//ok, so first see if we have a local account already saved
			//string account = GetLocalAccount ().Result;

//			if (string.IsNullOrEmpty (account)) {
//				//we don't have a saved account...require them to login!
//				//System.Diagnostics.Debug.WriteLine ("nothin");
//				IsAuthenticated = false;
//
//			} else {
//				//load the existing account
//				//System.Diagnostics.Debug.WriteLine (account);
//			//	IsAuthenticated = true;
//
//			}
		}

		public async Task ExecuteLoadItemsCommand (string Id)
		{
			IsLoading = true;
			try
			{

				var account = await client.GetTable<Account>().ToListAsync();
				if (account.Count == 1)
				{
					//we have our account
					App.myAccount = account[0];
					App.myDevice.accountid = App.myAccount.id;
					App.myDevice.userId = App.myAccount.UserId;

					MessagingCenter.Send<Account>(App.myAccount, "loaded");
				}
				else{
					//hoping the only other alternative would be 0, not more than 1
					//I guess we need to add it
					//no API to get phone number, so we'll need them to add it at some point
					IsLoading = false;
					Logout();
					//what do we do here? we didn't get one account back???
					//await ExecuteAddEditCommand();
				}

			}
			catch (Exception ex)
			{
 				if (ex.Message == "Error: NameResolutionFailure") {
					//why are we getting this?
				} else if (ex.Message == "Error: The authentication token has expired.") {
					//force them to login!
					IsAuthenticated = false;
					Logout ();
				}
				else {
					
					var page = new ContentPage ();
					var result = page.DisplayAlert ("Error", "Error loading data Splash. Please check connectivity and try again.", "OK", "Cancel");
					System.Diagnostics.Debug.WriteLine ("SplashEx " + ex.Message + result.Status.ToString ());
					if (ex.GetType ().Name == "MobileServiceInvalidOperationException") {
						Exception e = new Exception(((Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException)ex).Response.ReasonPhrase);
						MessagingCenter.Send<Exception> (e, "Error");
					} else {
						MessagingCenter.Send<Exception> (ex, "Error");
					}
				}
			}
			finally{
				IsLoading = false;
			}
			IsLoading = false; //redundant
		}

		public override async Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				var acct = client.GetTable<Account>();


				if (string.IsNullOrEmpty(App.myAccount.id))
				{
					await acct.InsertAsync(App.myAccount);
					MessagingCenter.Send<Account>(App.myAccount, "loaded");
				}
				else
				{
					await acct.UpdateAsync(App.myAccount);
					MessagingCenter.Send<Account>(App.myAccount, "Refresh");
				}
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}
			finally{
				IsLoading = false;
			}

			IsLoading = false;  //redundant
		}

		private Command forgotCommand;
		public Command ForgotCommand
		{
			get { return forgotCommand ?? (forgotCommand = new Command<Account>(async (a) => await ExecuteForgotCommand(a))); }
		}

		public virtual async Task ExecuteForgotCommand(Account AccountEntry)
		{
			try{
				var logger =  await client.InvokeApiAsync<Account, EmptyClass>("forgotpassword", AccountEntry);
				System.Diagnostics.Debug.WriteLine(logger.Status);
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}


//		public static async Task SaveIt()
//		{
//			IFolder rootFolder = FileSystem.Current.LocalStorage;
//			IFolder folder = await rootFolder.CreateFolderAsync("pickupSettings",
//				CreationCollisionOption.OpenIfExists);
//			IFile file = await folder.CreateFileAsync("account.txt",
//				CreationCollisionOption.ReplaceExisting);
//			await file.WriteAllTextAsync ("F722D8D8-3B5A-4C62-A22B-AA776841BB38");
//		}
//		public static async Task WhackIt()
//		{
//			IFolder rootFolder = FileSystem.Current.LocalStorage;
//			IFolder folder = await rootFolder.CreateFolderAsync("pickupSettings",
//				CreationCollisionOption.OpenIfExists);
//
//			IFile file = await folder.CreateFileAsync("account.txt",
//				CreationCollisionOption.OpenIfExists);
//			await file.DeleteAsync ();
//		}
//
//		public static async Task<string> GetLocalAccount()
//		{
//			IFolder rootFolder = FileSystem.Current.LocalStorage;
//			IFolder folder = await rootFolder.CreateFolderAsync("pickupSettings",
//				CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
//			IFile file = await folder.CreateFileAsync("account.txt",
//				CreationCollisionOption.OpenIfExists);
//			return await file.ReadAllTextAsync ();
//		}

	}
}

