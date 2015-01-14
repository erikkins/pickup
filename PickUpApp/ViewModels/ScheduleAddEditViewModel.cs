using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using PickUpApp.ViewModels;
using Xamarin.Forms;
using System.Collections.Generic;

namespace PickUpApp
{
	public class ScheduleAddEditViewModel:BaseViewModel
	{
		private string _tempLocation;
		private string _tempAddress;
		private string _tempLongitude;
		private string _tempLatitude;

		//public string Activity { get; set; }
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
		public ObservableCollection<KidSchedule> KidSchedules {get; set;}
		public ObservableCollection<Kid> Kids { get; set; }
		//public const string ADDRESS_PLACEHOLDER = "Click to add address";


		public ScheduleAddEditViewModel ()
		{

		}

		public ScheduleAddEditViewModel(MobileServiceClient client, Schedule existingSchedule, ObservableCollection<KidSchedule> kidSchedule, ObservableCollection<Kid> kids) : this()
		{
			this.client = client;

			Kids = new ObservableCollection<Kid> ();
			if (kids == null || kids.Count == 0) {
				LoadInitialCommand.Execute (null);
			} else {
				Kids = kids;
			}

			if (existingSchedule == null) {
				CurrentSchedule = new Schedule ();
				//set some defaults
				CurrentSchedule.AtWhen = Util.RoundUp (DateTime.Now, TimeSpan.FromMinutes (30));
				CurrentSchedule.AtWhenEnd = Util.RoundUp (DateTime.Now, TimeSpan.FromMinutes (30)).AddHours (1);
				CurrentSchedule.Frequency = "";

			} else {
				CurrentSchedule = existingSchedule;
			} 
			if (kidSchedule == null) {
				KidSchedules = new ObservableCollection<KidSchedule> ();
				LoadKidSchedulesCommand.Execute (null);
			} else {
				KidSchedules = kidSchedule;
				//something is weird in this situation where the atwhens were getting wiped out
				//reloading the schedule appeared to fix it

				//issue is that if we have an unsaved location, we need to persist that!  save first?
				_tempLocation = CurrentSchedule.Location;
				_tempLatitude = CurrentSchedule.Latitude;
				_tempLongitude = CurrentSchedule.Longitude;
				_tempAddress = CurrentSchedule.Address;
				ExecuteLoadItemsCommand ().ConfigureAwait (false);

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

				IsLoading = false; //allow future calls to work
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}

			IsLoading = false;
		}

		private Command loadInitialCommand;
		public Command LoadInitialCommand
		{
			get { return loadInitialCommand ?? (loadInitialCommand = new Command(async () => await LoadInitial())); }
		}

		public System.Threading.Tasks.Task LoadKids()
		{
			try{
				var kids = client.GetTable<Kid>().ToListAsync();
				kids.ConfigureAwait(false);
				Kids.Clear();
				foreach (var kid in kids.Result)
				{
					Kids.Add(kid);
				}
			}
			catch(Exception ex) {
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data kids. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
			return null;
		}

		public async System.Threading.Tasks.Task LoadInitial()
		{
			try{
				//seems silly to have to load this again...better way?
				var kids = await client.GetTable<Kid>().ToListAsync();

				Kids.Clear();
				foreach (var kid in kids)
				{
					Kids.Add(kid);
				}
			}
			catch(Exception ex) {
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data kids async. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
		}

		private Command loadKidSchedulesCommand;
		public Command LoadKidSchedulesCommand
		{
			get { return loadKidSchedulesCommand ?? (loadKidSchedulesCommand = new Command(async () => await LoadKidSchedules())); }
		}
		public async System.Threading.Tasks.Task LoadKidSchedules()
		{
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

		public override async System.Threading.Tasks.Task ExecuteLoadItemsCommand ()
		{
			try
			{
				if (CurrentSchedule.id != null)
				{
					var thisSched = await client.GetTable<Schedule>().LookupAsync(CurrentSchedule.id);
					CurrentSchedule = thisSched; //why?

					if (_tempAddress != null)
					{
						CurrentSchedule.Address = _tempAddress;
						_tempAddress = null;
					}
					if (_tempLatitude != null)
					{
						CurrentSchedule.Latitude = _tempLatitude;
						_tempLatitude = null;
					}
					if (_tempLocation != null)
					{
						CurrentSchedule.Location = _tempLocation;
						_tempLocation = null;
					}
					if (_tempLongitude != null)
					{
						CurrentSchedule.Longitude = _tempLongitude;
						_tempLongitude = null;
					}

				}
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Schedule. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
		}


		public TimeSpan StartTime 
		{ get
			{ 
				return CurrentSchedule.AtWhen.TimeOfDay;
			}
			set
			{
				if (value != CurrentSchedule.AtWhen.TimeOfDay) {
					CurrentSchedule.AtWhen += value;
					NotifyPropertyChanged ();
				}
			}

		}
		public TimeSpan EndTime {
			get{ return CurrentSchedule.AtWhenEnd.TimeOfDay; }
			set
			{
				//see trick pony above
				if (value != CurrentSchedule.AtWhenEnd.TimeOfDay) {
					CurrentSchedule.AtWhenEnd += value;
					NotifyPropertyChanged ();
				}
			}
		}

		public bool Monday
		{
			get{
				if (CurrentSchedule.Frequency.Contains ("M")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("M", value);
				NotifyPropertyChanged ();
			}
		}
		public bool Tuesday
		{
			get{
				if (CurrentSchedule.Frequency.Contains ("T")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("T", value);
				NotifyPropertyChanged ();
			}
		}
		public bool Wednesday
		{
			get{
				if (CurrentSchedule.Frequency.Contains ("W")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("W", value);
				NotifyPropertyChanged ();
			}
		}
		public bool Thursday
		{
			get{
				if (CurrentSchedule.Frequency.Contains ("R")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("R", value);
				NotifyPropertyChanged ();
			}
		}
		public bool Friday
		{
			get{
				if (CurrentSchedule.Frequency.Contains ("F")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("F", value);
				NotifyPropertyChanged ();
			}
		}
		public bool Saturday
		{
			get{
				if (CurrentSchedule.Frequency.Contains ("S")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("S", value);
				NotifyPropertyChanged ();
			}
		}
		public bool Sunday
		{
			get{
				if (CurrentSchedule.Frequency.Contains ("U")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("U", value);
				NotifyPropertyChanged ();
			}
		}

		private void setFrequency (string day, bool include)
		{
			if (include)
			{
				//make sure the frequency has the value
				if (!CurrentSchedule.Frequency.Contains(day))
				{
					CurrentSchedule.Frequency += day;
				}
			}
			else{
				//make sure the frequency does NOT have the value
				if (CurrentSchedule.Frequency.Contains(day))
				{
					CurrentSchedule.Frequency = CurrentSchedule.Frequency.Replace(day, "");
				}
			}
		}
	}
}

