﻿using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using PCLStorage;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace PickUpApp
{
	public class SplashViewModel:BaseViewModel
	{
		public SplashViewModel ()
		{

		}

		public SplashViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
			//SaveIt ();
			//WhackIt ();
			//ok, so first see if we have a local account already saved
			string account = GetLocalAccount ().Result;

			if (string.IsNullOrEmpty (account)) {
				//we don't have a saved account...require them to login!
				System.Diagnostics.Debug.WriteLine ("nothin");
				IsAuthenticated = false;

			} else {
				//load the existing account
				System.Diagnostics.Debug.WriteLine (account);
				IsAuthenticated = true;

			}
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
				}
				else{
					//hoping the only other alternative would be 0, not more than 1
					//I guess we need to add it
					//no API to get phone number, so we'll need them to add it at some point
					await ExecuteAddEditCommand();
				}

			}
			catch (Exception ex)
			{
				if (ex.Message == "Error: NameResolutionFailure") {
					//why are we getting this?
				} else {
					var page = new ContentPage ();
					var result = page.DisplayAlert ("Error", "Error loading data Splash. Please check connectivity and try again.", "OK", "Cancel");
					System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
				}
			}
			IsLoading = false;
		}

		public override async Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				var acct = client.GetTable<Account>();


				if (string.IsNullOrEmpty(App.myAccount.id))
					await acct.InsertAsync(App.myAccount);
				else
					await acct.UpdateAsync(App.myAccount);

				MessagingCenter.Send<MobileServiceClient>(client, "Refresh");
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}

			IsLoading = false;
		}

		public static async Task SaveIt()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder folder = await rootFolder.CreateFolderAsync("pickupSettings",
				CreationCollisionOption.OpenIfExists);
			IFile file = await folder.CreateFileAsync("account.txt",
				CreationCollisionOption.ReplaceExisting);
			await file.WriteAllTextAsync ("F722D8D8-3B5A-4C62-A22B-AA776841BB38");
		}
		public static async Task WhackIt()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder folder = await rootFolder.CreateFolderAsync("pickupSettings",
				CreationCollisionOption.OpenIfExists);

			IFile file = await folder.CreateFileAsync("account.txt",
				CreationCollisionOption.OpenIfExists);
			await file.DeleteAsync ();
		}

		public static async Task<string> GetLocalAccount()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder folder = await rootFolder.CreateFolderAsync("pickupSettings",
				CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
			IFile file = await folder.CreateFileAsync("account.txt",
				CreationCollisionOption.OpenIfExists);
			return await file.ReadAllTextAsync ();
		}

	}
}

