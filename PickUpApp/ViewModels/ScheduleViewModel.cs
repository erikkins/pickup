using System;
using PickUpApp.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using System.Linq;

namespace PickUpApp
{
	public class ScheduleViewModel:BaseViewModel
	{
		public ObservableCollection<Schedule> RecurringSchedule {get;set;}

		public Schedule CurrentSchedule{ get; set; }

		public ScheduleViewModel ()
		{
			RecurringSchedule = new ObservableCollection<Schedule> ();

			MessagingCenter.Subscribe<Schedule> (this, "RefreshSched", async(s) => {
				//System.Diagnostics.Debug.WriteLine("ScheduleViewModel -- RefreshSched Fired");
				//issue is, we're looking at a single schedule and trying to update it regardless of the others
				CurrentSchedule = s;
				await ExecuteLoadItemsCommand();
				Refresh();
			});

		}
		public ScheduleViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
			LoadItemsCommand.Execute (null);
		}
		public override async System.Threading.Tasks.Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			App.hudder.showHUD ("Loading Activities");
			try
			{
				var recs = await client.InvokeApiAsync<List<Schedule>>("getmyactivities");
				RecurringSchedule.Clear();
				foreach (var sched in recs)
				{
					RecurringSchedule.Add(sched);
					//this is so the selected schedule in AddEditActivity shows changes correctly.
					if (CurrentSchedule != null)
					{
						if (CurrentSchedule.id == sched.id)
						{
							CurrentSchedule = sched;
						}
					}
				}
				if (CurrentSchedule != null)
				{
					//MessagingCenter.Send<Schedule>(CurrentSchedule, "RefreshComplete");
					CurrentSchedule = null;
				}
				//System.Diagnostics.Debug.WriteLine("ScheduleViewModel -- Activities loaded");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("ScheduleViewModelEx -- " + ex);
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Circle. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
			finally{
				IsLoading = false;
				App.hudder.hideHUD ();
			}
			IsLoading = false;  //redundant
		}

		public override async System.Threading.Tasks.Task ExecuteDeleteCommand ()
		{
			try{
				List<EmptyClass> activitydelete = await client.InvokeApiAsync<Schedule,List<EmptyClass>>("deleteactivity", CurrentSchedule );

				if (activitydelete.Count == 0)
				{
					MessagingCenter.Send<EmptyClass>(new EmptyClass(), "ActivityDeleted");
				}
				else{
					MessagingCenter.Send<EmptyClass>(activitydelete.FirstOrDefault(), "ActivityDeleted");
				}
			}
			catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);		
			}
			finally{

			}
		}
	}
}

