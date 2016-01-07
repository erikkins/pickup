using System;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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
				dict.Add("deviceTime", App.CurrentToday.ToString());

				var today = await client.InvokeApiAsync<Dictionary<string,string>,List<Today>>("getmytoday", dict);

				//let's re-sort it by TSDropOff
				//today = today.OrderBy(o=>o.TSDropOff).ToList();
				today.Sort((x,y) => x.TSDropOff.CompareTo(y.TSDropOff));

				bool hasNext = false;

				Todays.Clear();
				foreach (var sched in today)
				{

					//well, we really need to split the today into two parts--dropoff and pickup...so need to create potentially 2 rows for each one

					if (sched.TSDropOff != TimeSpan.Zero)
					{
						sched.IsPickup = false; //means it's a dropoff
					}


					if (!hasNext && (!sched.DropOffComplete && !sched.PickupComplete))
					{
						sched.IsNext = true;
						hasNext = true;
					}
					else{
						sched.IsNext = false;
					}

					Todays.Add(sched);


					//now see if we need to add a pickup
					if (sched.TSPickup != TimeSpan.Zero)
					{
						//need a deep clone here
						Today pickup = sched.Clone();
						pickup.IsPickup = true;

						if (!hasNext && (!pickup.DropOffComplete || !pickup.PickupComplete))
						{
							pickup.IsNext = true;
							hasNext = true;
						}
						else{
							pickup.IsNext = false;
						}

						Todays.Add(pickup);
					}



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

