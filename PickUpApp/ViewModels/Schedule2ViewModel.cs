using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using PickUpApp.ViewModels;
using Xamarin.Forms;
using System.Collections.Generic;

namespace PickUpApp
{
	public class Schedule2ViewModel:BaseViewModel
	{
		private Schedule _currentSchedule;
		public Schedule CurrentSchedule {
			get { return _currentSchedule; }
			set{
				_currentSchedule = value;
				NotifyPropertyChanged ();
			}
		}
		private ObservableCollection<Kid> _kids;
		public ObservableCollection<Kid> Kids
		{
			get { return _kids; }
			set {
				_kids = value;
				NotifyPropertyChanged ();
			}
		}

		private ObservableCollection<KidSchedule> _kidSchedules;
		public ObservableCollection<KidSchedule>KidSchedules
		{
			get { return _kidSchedules; }
			set {
				_kidSchedules = value;
				NotifyPropertyChanged ();
			}
		}


		public Schedule2ViewModel ()
		{
		}
		public Schedule2ViewModel (MobileServiceClient client, Schedule currentSchedule, ObservableCollection<Kid>currentKids, ObservableCollection<KidSchedule> currentKidSchedule)
		{
			this.client = client;
			Kids = currentKids;
			KidSchedules = currentKidSchedule;
			CurrentSchedule = currentSchedule;

		}

		public override async System.Threading.Tasks.Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				var sched = client.GetTable<Schedule>();

				if (CurrentSchedule.Address == Schedule.ADDRESS_PLACEHOLDER)
				{
					//clear it, since no address was provided
					CurrentSchedule.Address = "";
				}

				//I think we should be converting the Local datetime to UTC prior to saving
				//CurrentSchedule.AtWhen = TimeZoneInfo.ConvertTime(CurrentSchedule.AtWhen, TimeZoneInfo.Utc);

				if (string.IsNullOrEmpty(CurrentSchedule.id))
					await sched.InsertAsync(CurrentSchedule);
				else
					await sched.UpdateAsync(CurrentSchedule);

				//but wait, there's more!
				//gotta add the kidids to the scheduleid (nest this somehow in a single call?)
				//whack 'em first
				var dels = await client.InvokeApiAsync<Schedule, EmptyClass>("deleteschedulekids", CurrentSchedule);

				var kidsched = client.GetTable<KidSchedule>();
				foreach (KidSchedule ks in KidSchedules)
				{
					if (ks.ScheduleID == null)
					{
						ks.ScheduleID = CurrentSchedule.id;
					}
					await kidsched.InsertAsync(ks);
				}


				MessagingCenter.Send<Schedule>(CurrentSchedule, "ScheduleAdded");
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}
			finally{
				IsLoading = false;
			}

			IsLoading = false; //redundant
		}
	}
}

