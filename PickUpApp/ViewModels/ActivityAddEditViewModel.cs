using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

namespace PickUpApp
{
	public class ActivityAddEditViewModel : BaseViewModel
	{

		bool _useBing = false;

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
				SaveLog ("Aborting CalculateDriveTime because something else isloading");
				return;
			}

			try{
				IsLoading = true;

				AccountPlace selectedPlace = new AccountPlace();
				AccountPlace endPlace = new AccountPlace();
				//grab the selected place
				foreach (AccountPlace ap in App.myPlaces)
				{
					if (ap.id == CurrentSchedule.StartPlaceID)
					{
						selectedPlace = ap;
						break;
					}
				}
				//a bit silly to spin through again, but the n will be small
				foreach (AccountPlace ap in App.myPlaces)
				{
					if (ap.id == CurrentSchedule.EndPlaceID)
					{
						endPlace = ap;
						break;
					}
				}

				if (selectedPlace == null || string.IsNullOrEmpty(selectedPlace.Latitude) || string.IsNullOrEmpty(selectedPlace.Longitude) || string.IsNullOrEmpty(CurrentSchedule.Latitude) || string.IsNullOrEmpty(CurrentSchedule.Longitude))
				{
					SaveLog("Bailing on DistanceCheck");
					System.Diagnostics.Debug.WriteLine ("Bailing on DistanceCheck ");
					return;
				}

				if (_useBing)
				{
					//right now this Bing lookup will only find travel time for the startplace
					PortableRest.RestRequest req = new PortableRest.RestRequest ("Routes", System.Net.Http.HttpMethod.Get);

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
					double min = br.ResourceSets[0].TripResources[0].TravelDurationTraffic/60;
					double distance = br.ResourceSets[0].TripResources[0].TravelDistance;
					string distanceUnit = br.ResourceSets[0].TripResources[0].DistanceUnit;
					//save the turn by tuSystem.Diagnostics.Debug.WriteLine ("BingError " + ex.Message);rn
					try{
						this.Itineraries = br.ResourceSets[0].TripResources[0].RouteLegs[0].ItineraryItems;
					}
					catch{}

					resp.Dispose();
					resp = null;
					req = null;

					CurrentSchedule.StartPlaceTravelTime = Math.Round((decimal)min, 2);
					CurrentSchedule.StartPlaceDistance = Math.Round((decimal)distance, 2);
					//for now, assume the pickup is departing from the same place
					CurrentSchedule.EndPlaceTravelTime = Math.Round((decimal)min,2);
					CurrentSchedule.EndPlaceDistance = Math.Round((decimal)distance, 2);
				}
				else{
					double tempmins=0, mins=0, tempdist=0, dist=0, tempminsend=0, tempdistend=0, minsend=0, distend=0;

					//use Google!
					using (var client = new RestClient(new Uri("https://maps.googleapis.com/maps/api/")))
					{
						var request = new RestRequest("distancematrix/json", Method.GET);	
						if (endPlace == null || endPlace.Latitude == null)
						{
							request.AddQueryParameter ("origins", selectedPlace.Latitude + "," + selectedPlace.Longitude);
							request.AddQueryParameter("destinations", CurrentSchedule.Latitude + "," + CurrentSchedule.Longitude);
						}
						else{
							request.AddQueryParameter ("origins", selectedPlace.Latitude + "," + selectedPlace.Longitude + "|" + endPlace.Latitude + "," + endPlace.Longitude);
							request.AddQueryParameter("destinations", CurrentSchedule.Latitude + "," + CurrentSchedule.Longitude + "|" + CurrentSchedule.Latitude + "," + CurrentSchedule.Longitude);
						}

						request.AddQueryParameter("mode", "driving");
						request.AddQueryParameter("units", "imperial");
						request.AddQueryParameter("key", "AIzaSyDpVbafIazS-s6a82lp4fswviB_Kb0fbmQ");

						var result = await client.Execute(request);
						GoogleDistanceResult yr = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleDistanceResult>(System.Text.Encoding.UTF8.GetString(result.RawBytes, 0, result.RawBytes.Length));
						System.Diagnostics.Debug.WriteLine("");
						if (yr.Rows.Count > 0)
						{
							
							tempmins = (double)yr.Rows[0].Elements[0].Duration.Value/(double)60;
							tempdist = (double)yr.Rows[0].Elements[0].Distance.Value/(double)1609.34; //meters in 1 mile
							
							if (yr.Rows.Count == 2)
							{
								tempminsend = (double)yr.Rows[1].Elements[0].Duration.Value/(double)60;
								tempdistend = (double)yr.Rows[1].Elements[0].Distance.Value/(double)1609.34; //meters in 1 mile	
							}
							else{
								tempminsend = tempmins;
								tempdistend = tempdist;
							}
						}
					}
					//turns out we have to do 2 calls...one to get the estimate and the second the see how far off the estimate is from real traffic...cha-ching
					using (var client2 = new RestClient(new Uri("https://maps.googleapis.com/maps/api/")))
					{
						var request2 = new RestRequest("distancematrix/json", Method.GET);	
						if (endPlace == null || endPlace.Latitude == null)
						{
							endPlace = selectedPlace;
						//request2.AddQueryParameter ("origins", selectedPlace.Latitude + "," + selectedPlace.Longitude);
						//request2.AddQueryParameter("destinations", CurrentSchedule.Latitude + "," + CurrentSchedule.Longitude);
						}
						//else
						{
							request2.AddQueryParameter ("origins", selectedPlace.Latitude + "," + selectedPlace.Longitude + "|" + endPlace.Latitude + "," + endPlace.Longitude);
							request2.AddQueryParameter("destinations", CurrentSchedule.Latitude + "," + CurrentSchedule.Longitude + "|" + CurrentSchedule.Latitude + "," + CurrentSchedule.Longitude);
						}

						//TODO: make the DAY of the departure time one of the days of the actual event (because traffic on different days will be different)
						DateTime estimatedDepartureTime = CurrentSchedule.DropoffDT.AddMinutes(-tempmins).AddDays(1); //the time MUST be in the future
						long depTime = estimatedDepartureTime.ToEpochTime();
						request2.AddQueryParameter("departure_time", depTime); //issue is that it will always use the start time to calculate traffic...crap...might need a third call to call pickup traffic
						request2.AddQueryParameter("traffic_model", "pessimistic");
						request2.AddQueryParameter("mode", "driving");
						request2.AddQueryParameter("units", "imperial");
						request2.AddQueryParameter("key", "AIzaSyDpVbafIazS-s6a82lp4fswviB_Kb0fbmQ");

						//https://maps.googleapis.com/maps/api/distancematrix/json?origins=41.8853372,-87.6337227&destinations=41.889526,-87.63843&units=imperial&mode=driving&departure_time=1455390847&key=AIzaSyDpVbafIazS-s6a82lp4fswviB_Kb0fbmQ

						var result2 = await client2.Execute(request2);
						GoogleDistanceResult yr2 = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleDistanceResult>(System.Text.Encoding.UTF8.GetString(result2.RawBytes, 0, result2.RawBytes.Length));
						System.Diagnostics.Debug.WriteLine("");
						if (yr2.Rows.Count > 0)
						{
							
							if (yr2.Rows[0].Elements[0].DurationInTraffic == null)
							{
								//this means the second Google was wasted! I want a refund.
								mins = tempmins;
								dist = tempdist;
							}
							else
							{
								mins = (double)yr2.Rows[0].Elements[0].DurationInTraffic.Value/(double)60;
								dist = (double)yr2.Rows[0].Elements[0].Distance.Value/(double)1609.34; //meters in 1 mile
								//ok, now we actually need to compare if the minswithtraffic is different than the original estimate, we need to adjust the departure time to arrive on time!
								//for now, just use the actual duration in traffic...
							}

							if (yr2.Rows.Count == 2)
							{
								if (yr2.Rows[1].Elements[0].DurationInTraffic == null)
								{
									//this means the second Google was wasted! I want a refund.
									minsend = tempminsend;
									distend = tempdistend;
								}
								else
								{
									minsend = (double)yr2.Rows[1].Elements[0].DurationInTraffic.Value/(double)60;
									distend = (double)yr2.Rows[1].Elements[0].Distance.Value/(double)1609.34; //meters in 1 mile
									//ok, now we actually need to compare if the minswithtraffic is different than the original estimate, we need to adjust the departure time to arrive on time!
									//for now, just use the actual duration in traffic...
								}
							}

							CurrentSchedule.StartPlaceTravelTime = Math.Round((decimal)mins, 2);
							CurrentSchedule.StartPlaceDistance = Math.Round((decimal)dist, 2);
							if (endPlace == null)
							{
								//for now, assume the pickup is departing from the same place
								CurrentSchedule.EndPlaceTravelTime = Math.Round((decimal)mins,2);
								CurrentSchedule.EndPlaceDistance = Math.Round((decimal)dist, 2);
							}
							else{
								CurrentSchedule.EndPlaceTravelTime = Math.Round((decimal)minsend,2);
								CurrentSchedule.EndPlaceDistance = Math.Round((decimal)distend, 2);
							}
						}
					}
				}


				IsLoading = false;

				//PickUpApp.fflog logentry = new fflog("BingLocation for " + selectedPlace.id + " from AddEditActivity");
				//this.ExecuteLogCommand(logentry);
			}
			catch(Exception ex)
			{
				IsLoading = false;
				SaveLog (ex.ToString());
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

				//moved this to the main UI
				//await CalculateDriveTime();

				IsLoading = true;
				if (string.IsNullOrEmpty(CurrentSchedule.id))
				{
					await sched.InsertAsync(CurrentSchedule);
					//Debug.WriteLine("ActivityAddEditVM -- Schedule Added");
				}
				else
				{
					await sched.UpdateAsync(CurrentSchedule);
					//Debug.WriteLine("ActivityAddEditVM -- Schedule Updated");
				}

				//but wait, there's more!
				//gotta add the kidids to the scheduleid (nest this somehow in a single call?)
				//whack 'em first
				EmptyClass res = await client.InvokeApiAsync<Schedule, EmptyClass>("deleteschedulekids", CurrentSchedule);
				//Debug.WriteLine("ActivityAddEditVM -- DeletedScheduleKids" + res.Status);
				var kidsched = client.GetTable<KidSchedule>();
				foreach (KidSchedule ks in KidSchedules)
				{
					if (ks.ScheduleID == null)
					{
						ks.ScheduleID = CurrentSchedule.id;
					}
					//Debug.WriteLine("Saving kid " + ks.KidID + " with id " + ks.id);
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
				//Debug.WriteLine("ActivityAddEditVM -- Added " + KidSchedules.Count.ToString() + " kids");


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

