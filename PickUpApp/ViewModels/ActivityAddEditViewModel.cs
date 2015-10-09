﻿using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickUpApp
{
	public class ActivityAddEditViewModel : BaseViewModel
	{
		public ActivityAddEditViewModel (MobileServiceClient client, Schedule currentSchedule)
		{
			this.client = client;
			_currentSchedule = currentSchedule;
			Kids = new ObservableCollection<Kid> ();
			KidSchedules = new ObservableCollection<KidSchedule> ();

			LoadInitialCommand.Execute(null);

		}



		public ActivityAddEditViewModel (MobileServiceClient client, Schedule currentSchedule, ObservableCollection<KidSchedule> kidSchedule, ObservableCollection<Kid> kids)
		{
			this.client = client;
			_currentSchedule = currentSchedule;
			_kidschedules = kidSchedule;
			Kids = kids;
		}

		private Schedule _currentSchedule;
		public Schedule CurrentSchedule 
		{ get { return _currentSchedule; }
			set
			{
				_currentSchedule = value;
				NotifyPropertyChanged ();
				NotifyPropertyChanged ("ViewName");
				NotifyPropertyChanged ("StartTime");
				NotifyPropertyChanged ("EndTime");
			}
		}
		public ObservableCollection<Kid> Kids { get; set; }

		private ObservableCollection<KidSchedule> _kidschedules;
		public ObservableCollection<KidSchedule> KidSchedules 
		{get{ return _kidschedules; } 
			set{
				_kidschedules = value; 
				NotifyPropertyChanged ();
				NotifyPropertyChanged ("ViewName");
			}
		}


		private ObservableCollection<AccountPlace> _accountPlaces;
		public ObservableCollection<AccountPlace> AccountPlaces
		{
			get {
				if (_accountPlaces == null) {
					_accountPlaces = new ObservableCollection<AccountPlace> ();


					foreach (AccountPlace ap in App.myPlaces) {
						if (ap.id == _currentSchedule.StartPlaceID) {
							ap.Selected = true;

						}
						_accountPlaces.Add (ap);
					}

				}
				return _accountPlaces;
			}
			set{
				if (_accountPlaces != value) {
					_accountPlaces = value;
					NotifyPropertyChanged ();
					NotifyPropertyChanged ("ViewName");
				}
			}
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

				//need to set some defaults here
				if (CurrentSchedule.DefaultDropOffAccount == null)
				{
					
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
				await client.InvokeApiAsync<Schedule, EmptyClass>("deleteschedulekids", CurrentSchedule);

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



		#region loadKids
		private Command loadInitialCommand;
		public Command LoadInitialCommand
		{
			get { return loadInitialCommand ?? (loadInitialCommand = new Command(async () => await LoadInitial())); }
		}
		public async System.Threading.Tasks.Task LoadInitial()
		{
			if (IsLoading) return;
			IsLoading = true;
			try{
				//seems silly to have to load this again...better way?
				var kids = await client.GetTable<Kid>().ToListAsync();

				Kids.Clear();
				foreach (var kid in kids)
				{
					Kids.Add(kid);
				}

				var theseKids = await client.InvokeApiAsync<Schedule, List<KidSchedule>>("getschedulekids", CurrentSchedule);
				KidSchedules.Clear();
				foreach (KidSchedule ks in theseKids)
				{
					KidSchedules.Add(ks);

					foreach(Kid k in Kids)
					{
						if (k.Id == ks.KidID)
						{
							k.Selected = true;
						}
					}
				}

			}
			catch(Exception ex) {
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data kids async. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());

			}
			finally{
				IsLoading = false;
			}
			IsLoading = false;  //redundant
		}
		#endregion
	}
}

