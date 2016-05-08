using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

namespace PickUpApp
{
	public class DistanceService : BaseModel
	{
		private FormattedString _travelString;
		public FormattedString TravelString
		{
			get{
				if (_travelString == null) {
					_travelString = new FormattedString ();
				}
				_travelString.Spans.Clear ();

				if (TravelTime == -1975) {
					//secret code word for can't calculate
					_travelString.Spans.Add (new Span {
						Text = "Unknown",
						FontSize = 18,
						ForegroundColor = Color.FromRgb (241, 179, 70),
						FontAttributes = FontAttributes.Bold,

					});
					return _travelString;
				}

				//we revamped this to show what time you should be leaving
				TimeSpan tsTravel = new TimeSpan(0, (int)TravelTime, 0);

				try{

					DateTime dtFinal = new DateTime(EventTime.Subtract(tsTravel).Ticks);

				_travelString.Spans.Add (new Span {
					Text = dtFinal.ToString(@"h\:mm"),
					FontSize = 22,
					ForegroundColor = Color.FromRgb (241, 179, 70),
					FontAttributes = FontAttributes.Bold
				});
				_travelString.Spans.Add (new Span {
					Text = dtFinal.ToString(@"tt"),
					FontFamily = Device.OnPlatform ("HelveticaNeue-Light", "", ""),
					FontSize = 22,
					ForegroundColor = Color.FromRgb (241, 179, 70),
					FontAttributes = FontAttributes.None
				});
				}
				catch(Exception ex) {
					_travelString.Spans.Add (new Span {
						Text = "Unknown",
						FontSize = 18,
						ForegroundColor = Color.FromRgb (241, 179, 70),
						FontAttributes = FontAttributes.Bold,

					});
					return _travelString;
				}

//				if (TravelDifferential < 0) {
//					_travelString.Spans.Add (new Span {
//						Text = "On Time",
//						FontSize = 22,
//						ForegroundColor = Color.FromRgb (241, 179, 70),
//						FontAttributes = FontAttributes.Bold
//					});
//				} else {
//					_travelString.Spans.Add (new Span {
//						Text = "+" + Math.Round(TravelDifferential).ToString (),
//						FontSize = 22,
//						ForegroundColor = Color.FromRgb (241, 179, 70),
//						FontAttributes = FontAttributes.Bold
//					});
//					_travelString.Spans.Add (new Span {
//						Text = " min",
//						FontFamily = Device.OnPlatform ("HelveticaNeue-Light", "", ""),
//						FontSize = 22,
//						ForegroundColor = Color.FromRgb (241, 179, 70),
//						FontAttributes = FontAttributes.None
//					});
//				}
					return _travelString;
			}
		}


		/// <summary>
		/// this should come is a device local time
		/// </summary>
		private TimeSpan _eventTime;
		public TimeSpan EventTime
		{
			get{
				return _eventTime;
			}
			set{
				_eventTime = value;
				NotifyPropertyChanged ();
				NotifyPropertyChanged ("EventTime");
			}
		}

		private decimal _expectedTravelTime;
		public decimal ExpectedTravelTime
		{
			get {
				return _expectedTravelTime;
			}
			set{
				_expectedTravelTime = value;
				NotifyPropertyChanged ();
				NotifyPropertyChanged ("TravelDifferential");
			}
		}

		private double _travelTime;
		public double TravelTime
		{
			get{
				return _travelTime;
			}
			set{
				_travelTime = value;
				NotifyPropertyChanged ();
				NotifyPropertyChanged ("TravelDifferential");
				NotifyPropertyChanged ("TravelString");
			}
		}

		public double TravelDifferential
		{
			get{

				DateTime now = DateTime.Now;
				TimeSpan diff = EventTime - now.TimeOfDay;
				double minsuntil = diff.TotalMinutes;			

				return minsuntil - TravelTime;

				//if I'm in the position where I'm supposed to start from, we can calculate the differential based upon the expected travel time
//				if (App.PositionLatitude == _startLoc.Latitude && App.PositionLongitude == _startLoc.Longitude) {					
//					return _travelTime - _expectedTravelTime;
//				} else {
//					//ok, we're somewhere else (presumably on the road)...how should we show this now?
//					//probably how many minutes are we away from the location and what time the thing's supposed to start
//				}
			}
		}

		private double _distance;
		public double Distance
		{
			get{
				return _distance;
			}
			set{
				_distance = value;
				NotifyPropertyChanged ();
			}
		}


		private Location _startLoc;
		public Location StartingLocation
		{
			get{
				return _startLoc;
			}
			set{
				_startLoc = value;
				NotifyPropertyChanged ();
			}
		}

		private Location _endLoc;
		public Location EndingLocation
		{
			get{
				return _endLoc;
			}
			set{
				_endLoc = value;
				NotifyPropertyChanged ();
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

		private bool _useBing = false;

		private Command calculateTrafficCommand;
		public Command CalculateTrafficCommand
		{
			get { return calculateTrafficCommand ?? (calculateTrafficCommand = new Command(async () => await CalculateDriveTime())); }
		}
		public async  System.Threading.Tasks.Task CalculateDriveTime()
		{

			if (_startLoc == null || string.IsNullOrEmpty (_startLoc.Latitude) || string.IsNullOrEmpty (_startLoc.Longitude)) {
				return;
			}
			if (_endLoc == null || string.IsNullOrEmpty (_endLoc.Latitude) || string.IsNullOrEmpty (_endLoc.Longitude)) {
				return;
			}

			try{

				if (_useBing)
				{
					PortableRest.RestRequest req = new PortableRest.RestRequest ("Routes", System.Net.Http.HttpMethod.Get);

					req.AddQueryString ("wp.0", _startLoc.Latitude + "," + _startLoc.Longitude);
					req.AddQueryString ("wp.1", _endLoc.Latitude + "," + _endLoc.Longitude);
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
					//save the turn by turn
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

					TravelTime = Math.Round(min, 2);
					Distance = Math.Round(distance, 2);
				}
				else{
					double tempmins=0, tempdist=0;

					//use Google!
					using (var client = new RestClient(new Uri("https://maps.googleapis.com/maps/api/")))
					{
						var request = new RestRequest("distancematrix/json", Method.GET);	
						request.AddQueryParameter ("origins", _startLoc.Latitude + "," + _startLoc.Longitude);
						request.AddQueryParameter("destinations", _endLoc.Latitude + "," + _endLoc.Longitude);
						request.AddQueryParameter("departure_time", "now"); 
						request.AddQueryParameter("traffic_model", "pessimistic");
						request.AddQueryParameter("mode", "driving");
						request.AddQueryParameter("units", "imperial");
						request.AddQueryParameter("key", "AIzaSyDpVbafIazS-s6a82lp4fswviB_Kb0fbmQ");

						var result = await client.Execute(request);
						GoogleDistanceResult yr = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleDistanceResult>(System.Text.Encoding.UTF8.GetString(result.RawBytes, 0, result.RawBytes.Length));
						System.Diagnostics.Debug.WriteLine("");
						if (yr.Rows.Count > 0 && yr.Rows[0].Elements[0].Duration != null)
						{

							tempmins = (double)yr.Rows[0].Elements[0].Duration.Value/(double)60;
							tempdist = (double)yr.Rows[0].Elements[0].Distance.Value/(double)1609.34; //meters in 1 mile

							TravelTime = Math.Round(tempmins,2);
							Distance = Math.Round(tempdist, 2);
						}
					}

				}
			}
			catch(Exception ex)
			{
				TravelTime = -1975;
				System.Diagnostics.Debug.WriteLine ("BingError " + ex.Message);
			}
			finally
			{

			}
		}
	}
}

