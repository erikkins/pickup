using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using PickUpApp.ViewModels;
using Xamarin.Forms;
using System.Collections.Generic;

namespace PickUpApp
{
	public class Schedule1ViewModel:BaseViewModel
	{
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
		public ObservableCollection<KidSchedule> KidSchedules {get; set;}

		public Schedule1ViewModel()
		{
			Kids = new ObservableCollection<Kid> ();
			KidSchedules = new ObservableCollection<KidSchedule> ();
		}
		public Schedule1ViewModel (MobileServiceClient client, Schedule currentSchedule)
		{
			Kids = new ObservableCollection<Kid> ();
			KidSchedules = new ObservableCollection<KidSchedule> ();
			this.client = client;
			CurrentSchedule = currentSchedule;

			LoadInitialCommand.Execute (null);
			//LoadKidSchedulesCommand.Execute (null);
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

