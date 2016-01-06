using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

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


				if (TravelDifferential < 0) {
					_travelString.Spans.Add (new Span {
						Text = "On Time",
						FontSize = 22,
						ForegroundColor = Color.FromRgb (241, 179, 70),
						FontAttributes = FontAttributes.Bold
					});
				} else {
					_travelString.Spans.Add (new Span {
						Text = "+" + Math.Round(TravelDifferential).ToString (),
						FontSize = 22,
						ForegroundColor = Color.FromRgb (241, 179, 70),
						FontAttributes = FontAttributes.Bold
					});
					_travelString.Spans.Add (new Span {
						Text = " min",
						FontFamily = Device.OnPlatform ("HelveticaNeue-Light", "", ""),
						FontSize = 22,
						ForegroundColor = Color.FromRgb (241, 179, 70),
						FontAttributes = FontAttributes.None
					});
				}
					return _travelString;
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

		private decimal _travelTime;
		public decimal TravelTime
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

		public decimal TravelDifferential
		{
			get{
				return _travelTime - _expectedTravelTime;
			}
		}

		private decimal _distance;
		public decimal Distance
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

		private Command calculateTrafficCommand;
		public Command CalculateTrafficCommand
		{
			get { return calculateTrafficCommand ?? (calculateTrafficCommand = new Command(async () => await CalculateDriveTime())); }
		}
		public async  System.Threading.Tasks.Task CalculateDriveTime()
		{

			try{

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


				//Debug.WriteLine("ActivityAddEditVM -- CalculatedDriveTime");
				//ok, we get total seconds, so we need to divide by 60 to get minutes
				//decimal min = decimal.Parse(bingresponse ["resourceSets"] [0] ["resources"] [0] ["travelDurationTraffic"].ToString())/60;
				//bingresponse.RemoveAll();
				//bingresponse = null;

				TravelTime = Math.Round(min, 2);
				Distance = Math.Round(distance, 2);
					
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

