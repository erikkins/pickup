using System;
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
					NotifyPropertyChanged ("AccountPlaces");
				}
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
			try{
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
				//save the turn by turn
				try{
					this.Itineraries = br.ResourceSets[0].TripResources[0].RouteLegs[0].ItineraryItems;
				}
				catch{}

				resp.Dispose();
				resp = null;
				req = null;



				//ok, we get total seconds, so we need to divide by 60 to get minutes
				//decimal min = decimal.Parse(bingresponse ["resourceSets"] [0] ["resources"] [0] ["travelDurationTraffic"].ToString())/60;
				//bingresponse.RemoveAll();
				//bingresponse = null;

				CurrentSchedule.StartPlaceTravelTime = min;
				CurrentSchedule.StartPlaceDistance = distance;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("BingError " + ex.Message);
			}
			finally
			{

			
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

				await CalculateDriveTime();

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

