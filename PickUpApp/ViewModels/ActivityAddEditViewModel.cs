using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PickUpApp
{
	public class ActivityAddEditViewModel : BaseViewModel
	{
		public ActivityAddEditViewModel (MobileServiceClient client, Schedule currentSchedule)
		{
			this.client = client;
			_currentSchedule = currentSchedule;
			//Kids = new ObservableCollection<Kid> ();
			KidSchedules = new TrulyObservableCollection<KidSchedule> ();
			BlackoutDates = new TrulyObservableCollection<BlackoutDate> ();
			Preemptives = new TrulyObservableCollection<Preemptive> ();
			LoadInitialCommand.Execute(null);						
		}



		public ActivityAddEditViewModel (MobileServiceClient client, Schedule currentSchedule, TrulyObservableCollection<KidSchedule> kidSchedule, ObservableCollection<Kid> kids)
		{
			this.client = client;
			_currentSchedule = currentSchedule;
			_kidschedules = kidSchedule;
			//Kids = kids;
		}

		public string ReturnVerb;

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
		public ObservableCollection<Kid> Kids { get { return App.myKids; }  }

		private TrulyObservableCollection<KidSchedule> _kidschedules;
		public TrulyObservableCollection<KidSchedule> KidSchedules 
		{get{ return _kidschedules; } 
			set{
				_kidschedules = value; 
				NotifyPropertyChanged ();
				NotifyPropertyChanged ("ViewName");
			}
		}

		private TrulyObservableCollection<BlackoutDate> _blackoutDates;
		public TrulyObservableCollection<BlackoutDate> BlackoutDates
		{
			get { return _blackoutDates;}
			set{
				_blackoutDates = value;
				NotifyPropertyChanged ();
				//NotifyPropertyChanged ("BlackoutDates");
				NotifyPropertyChanged ("Selected");
			}
		}

		private TrulyObservableCollection<Preemptive>_preemptives;
		public TrulyObservableCollection<Preemptive>Preemptives
		{
			get { return _preemptives; }
			set{
				_preemptives = value;
				NotifyPropertyChanged ();
			}
		}


		//private ObservableCollection<AccountPlace> _accountPlaces;
		public ObservableCollection<AccountPlace> AccountPlaces
		{
			get {
//				if (_accountPlaces == null) {
//					_accountPlaces = new ObservableCollection<AccountPlace> ();
//
//
//					foreach (AccountPlace ap in App.myPlaces) {
//						//this selection needs to be done a bit closer to the user!
////						if (ap.id == _currentSchedule.StartPlaceID) {
////							ap.Selected = true;
////						}
//						_accountPlaces.Add (ap);
//					}
//
//				}
//				return _accountPlaces;

				return App.myPlaces;
			}
			set{
				//if (_accountPlaces != value) {
					//_accountPlaces = value;
					App.myPlaces = value;
					NotifyPropertyChanged ();
					NotifyPropertyChanged ("AccountPlaces");
				//}
			}
		}


		private ObservableCollection<BingItineraryItem> _itins;
		public ObservableCollection<BingItineraryItem>Itineraries 
		{ 
			get{
				return _itins;
			}
			set
			{
				_itins = value;
				NotifyPropertyChanged ();
			}
		}

		private Command calculateTrafficCommand;
		public Command CalculateTrafficCommand
		{
			get { return calculateTrafficCommand ?? (calculateTrafficCommand = new Command(async () => await CalculateDriveTime())); }
		}
		public async  System.Threading.Tasks.Task CalculateDriveTime()
		{

			if (IsLoading) {
				return;
			}

			try{
				IsLoading = true;
				PortableRest.RestRequest req = new PortableRest.RestRequest ("Routes", System.Net.Http.HttpMethod.Get);

				AccountPlace selectedPlace = new AccountPlace();
				//grab the selected place
				foreach (AccountPlace ap in App.myPlaces)
				{
					if (ap.id == CurrentSchedule.StartPlaceID)
					{
						selectedPlace = ap;
						break;
					}
				}

				if (selectedPlace == null || string.IsNullOrEmpty(selectedPlace.Latitude) || string.IsNullOrEmpty(selectedPlace.Longitude) || string.IsNullOrEmpty(CurrentSchedule.Latitude) || string.IsNullOrEmpty(CurrentSchedule.Longitude))
				{
					System.Diagnostics.Debug.WriteLine ("Bailing on Bing ");
					return;
				}

				req.AddQueryString ("wp.0", selectedPlace.Latitude + "," + selectedPlace.Longitude);
				req.AddQueryString ("wp.1", CurrentSchedule.Latitude + "," + CurrentSchedule.Longitude);
				req.AddQueryString ("du", "mi");
				req.AddQueryString ("avoid", "minimizeTolls");
				req.AddQueryString ("key", "AiZzYU7682t3jrRWVPS6x139Nwpjxs0LXJy5QLweCP2-mLNPoHYWcTUREnntk_JA");


				PortableRest.RestClient rc = new PortableRest.RestClient ();			
				rc.UserAgent = "PickUp";
				rc.BaseUrl = "http://dev.virtualearth.net/REST/V1/";
				PortableRest.RestResponse<string> resp = await rc.SendAsync<string>(req, default(System.Threading.CancellationToken));
				//var bingresponse = Newtonsoft.Json.Linq.JObject.Parse (resp.Content);

				BingResponse br = Newtonsoft.Json.JsonConvert.DeserializeObject<BingResponse>(resp.Content);
				decimal min = br.ResourceSets[0].TripResources[0].TravelDurationTraffic/60;
				decimal distance = br.ResourceSets[0].TripResources[0].TravelDistance;
				string distanceUnit = br.ResourceSets[0].TripResources[0].DistanceUnit;
				//save the turn by tuSystem.Diagnostics.Debug.WriteLine ("BingError " + ex.Message);rn
				try{
					this.Itineraries = br.ResourceSets[0].TripResources[0].RouteLegs[0].ItineraryItems;
				}
				catch{}

				resp.Dispose();
				resp = null;
				req = null;


				//Debug.WriteLine("ActivityAddEditVM -- CalculatedDriveTime");
				//ok, we get total seconds, so we need to divide by 60 to get minutes
				//decimal min = decimal.Parse(bingresponse ["resourceSets"] [0] ["resources"] [0] ["travelDurationTraffic"].ToString())/60;
				//bingresponse.RemoveAll();
				//bingresponse = null;

				CurrentSchedule.StartPlaceTravelTime = Math.Round(min, 2);
				CurrentSchedule.StartPlaceDistance = Math.Round(distance, 2);
				//for now, assume the pickup is departing from the same place
				CurrentSchedule.EndPlaceTravelTime = Math.Round(min,2);
				CurrentSchedule.EndPlaceDistance = Math.Round(distance, 2);
				IsLoading = false;

				PickUpApp.fflog logentry = new fflog("BingLocation for " + selectedPlace.id + " from AddEditActivity");
				this.ExecuteLogCommand(logentry);
			}
			catch(Exception ex)
			{
				IsLoading = false;
				System.Diagnostics.Debug.WriteLine ("BingError " + ex.Message);
			}
			finally
			{
				IsLoading = false;
			
			}
		}


		public override async System.Threading.Tasks.Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			//IsLoading = true;

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

				await CalculateDriveTime();
				IsLoading = true;
				if (string.IsNullOrEmpty(CurrentSchedule.id))
				{
					await sched.InsertAsync(CurrentSchedule);
					//Debug.WriteLine("ActivityAddEditVM -- Schedule Added");
				}
				else
				{
					await sched.UpdateAsync(CurrentSchedule);
					Debug.WriteLine("ActivityAddEditVM -- Schedule Updated");
				}

				//but wait, there's more!
				//gotta add the kidids to the scheduleid (nest this somehow in a single call?)
				//whack 'em first
				EmptyClass res = await client.InvokeApiAsync<Schedule, EmptyClass>("deleteschedulekids", CurrentSchedule);
				Debug.WriteLine("ActivityAddEditVM -- DeletedScheduleKids" + res.Status);
				var kidsched = client.GetTable<KidSchedule>();
				foreach (KidSchedule ks in KidSchedules)
				{
					if (ks.ScheduleID == null)
					{
						ks.ScheduleID = CurrentSchedule.id;
					}
					Debug.WriteLine("Saving kid " + ks.KidID + " with id " + ks.id);
					try{
						await kidsched.InsertAsync(ks);
					}
					catch(Exception ex)
					{
						System.Diagnostics.Debug.WriteLine("Issue with KidSave(" + ks.id + "|" + ks.KidID + "): ");
						if (ex != null)
						{
							System.Diagnostics.Debug.WriteLine(ex);
						}
					}
				}
				Debug.WriteLine("ActivityAddEditVM -- Added " + KidSchedules.Count.ToString() + " kids");


				foreach (BlackoutDate bod in BlackoutDates)
				{
					EmptyClass resbod = await client.InvokeApiAsync<BlackoutDate, EmptyClass>("setblackoutdate", bod);
				}


				if (string.IsNullOrEmpty(ReturnVerb))
				{
					MessagingCenter.Send<Schedule>(CurrentSchedule, "ScheduleAdded");
				}
				else{
					MessagingCenter.Send<Schedule>(CurrentSchedule, ReturnVerb);
					ReturnVerb = null; //one time use
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine ("ActivityAddEditCommand " + ex);
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}
			finally{
				IsLoading = false;
			}

			IsLoading = false; //redundant
		}

		#region preemptive
		private Command checkPreemptiveCommand;
		public Command CheckPreemptiveCommand
		{
			get { return checkPreemptiveCommand ?? (checkPreemptiveCommand = new Command<string>(async (p) => await CheckPreemptive(p))); }
		}
		public async System.Threading.Tasks.Task CheckPreemptive(string days)
		{
			if (IsLoading) return;
			IsLoading = true;
			try{
				//seems silly to have to load this again...better way?
				Preemptive p = new Preemptive();
				p.Days = days;

				List<Preemptive> bPre = await client.InvokeApiAsync<Preemptive, List<Preemptive>>("checkpreemptive", p);
				Preemptives.Clear();
				foreach (Preemptive pe in bPre)
				{
					Preemptives.Add(pe);
				}

			}
			catch(Exception ex) {
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data preemptive async. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());

			}
			finally{
				IsLoading = false;
			}
			IsLoading = false;  //redundant
		}
		#endregion


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
				/*
				var kids = await client.GetTable<Kid>().ToListAsync();

				Kids.Clear();
				foreach (var kid in kids)
				{
					Kids.Add(kid);
				}
				*/

				var theseKids = await client.InvokeApiAsync<Schedule, List<KidSchedule>>("getschedulekids", CurrentSchedule);
				KidSchedules.Clear();
				//Debug.WriteLine("KidSchedules Clear from LoadInitial");
				foreach (KidSchedule ks in theseKids)
				{
					KidSchedules.Add(ks);
					//Debug.WriteLine("KidSchedule " + ks.KidID + " added from LoadInitial");
					foreach(Kid k in Kids)
					{
						if (k.Id == ks.KidID)
						{
							k.Selected = true;
						}
					}
				}

				BlackoutDate origin = new BlackoutDate();
				origin.ScheduleID = CurrentSchedule.id;
				var bods = await client.InvokeApiAsync<BlackoutDate, List<BlackoutDate>>("getblackoutdates", origin);
				BlackoutDates.Clear();
				foreach (BlackoutDate bod in bods)
				{
					bod.ScheduleID=CurrentSchedule.id;
					BlackoutDates.Add(bod);
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

