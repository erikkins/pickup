using System;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace PickUpApp.ViewModels
{
	public class TodayViewModel: BaseViewModel
	{
		public ObservableCollection<Account> Accounts { get; set; }
		public TodayViewModel ()
		{
			this.Accounts = new ObservableCollection<Account> ();
		}

		public TodayViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
			LoadItemsCommand.Execute(null);
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try
			{
				var accounts = await client.GetTable<Account>().ToListAsync();

				Accounts.Clear();
				foreach (var acct in accounts)
				{
					Accounts.Add(acct);
				}
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Today. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
			IsLoading = false;
		}
	}
}

