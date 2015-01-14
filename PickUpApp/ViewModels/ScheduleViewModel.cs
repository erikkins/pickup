using System;
using PickUpApp.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace PickUpApp
{
	public class ScheduleViewModel:BaseViewModel
	{
		public ObservableCollection<Schedule> RecurringSchedule {get;set;}

		public ScheduleViewModel ()
		{
			RecurringSchedule = new ObservableCollection<Schedule> ();

		}
		public ScheduleViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
			LoadItemsCommand.Execute (null);
		}
		public override async System.Threading.Tasks.Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try
			{
				var recs = await client.InvokeApiAsync<List<Schedule>>("getmyrecurringschedule");
				RecurringSchedule.Clear();
				foreach (var sched in recs)
				{
					RecurringSchedule.Add(sched);
				}

			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Circle. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
			IsLoading = false;
		}
	}
}

