using System;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PickUpApp.ViewModels
{
	public class TodayViewModel: BaseViewModel
	{
		public ObservableCollection<Today> Todays { get; set; }
		public TodayViewModel ()
		{
			this.Todays = new ObservableCollection<Today> ();
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

				Dictionary<string,string> dict = new Dictionary<string, string>();
				dict.Add("deviceTime", DateTime.Now.ToString());

				var today = await client.InvokeApiAsync<Dictionary<string,string>,List<Today>>("getmytoday", dict);

				Todays.Clear();
				foreach (var sched in today)
				{
					Todays.Add(sched);
				}
				//sweet, we now have our today list!
				IsLoading = false;

				MessagingCenter.Send<TodayViewModel>(this, "TodayLoaded");

				/*
				var accounts = await client.GetTable<Account>().ToListAsync();

				Accounts.Clear();
				foreach (var acct in accounts)
				{
					Accounts.Add(acct);
				}
				*/
			}
			catch (Exception ex)
			{
	 		var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Today. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine ("TodayEx " + ex.Message + result.Status.ToString ());
			}
			finally {
				IsLoading = false;
			}
		}
	}
}

