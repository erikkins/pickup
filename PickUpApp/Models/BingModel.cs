using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;


namespace PickUpApp
{
	public class BingResponse: BaseModel
	{
		private ObservableCollection<BingResourceSet> _resourcesets;
		[JsonProperty(PropertyName = "resourceSets")]
		public ObservableCollection<BingResourceSet>ResourceSets
		{
			get { return _resourcesets; }
			set{
				_resourcesets = value;
				NotifyPropertyChanged ();
			}
		}
	}

	public class BingResourceSet : BaseModel
	{
		private int _estTotal;
		[JsonProperty(PropertyName="estimatedTotal")]
		public int EstimatedTotal
		{
			get { return _estTotal; }
			set{
				_estTotal = value;
				NotifyPropertyChanged ();
			}
		}

		private ObservableCollection<BingResource> _resources;
		[JsonProperty(PropertyName = "resources")]
		public ObservableCollection<BingResource> TripResources
		{
			get { return _resources; }
			set{
				_resources = value;
				NotifyPropertyChanged ();
			}
		}

	}

	public class BingResource: BaseModel
	{
		private string _distUnit;
		[JsonProperty(PropertyName = "distanceUnit")]
		public string DistanceUnit
		{
			get { return _distUnit; }
			set {
				_distUnit = value;
				NotifyPropertyChanged ();
			}
		}

		private string _durUnit;
		[JsonProperty(PropertyName = "durationUnit")]
		public string DurationUnit
		{
			get { return _durUnit; }
			set {
				_durUnit = value;
				NotifyPropertyChanged ();
			}
		}

		private string _trafficCongestion;
		[JsonProperty(PropertyName = "trafficCongestion")]
		public string TrafficCongestion
		{
			get { return _trafficCongestion; }
			set {
				_trafficCongestion = value;
				NotifyPropertyChanged ();
			}
		}

		private decimal _travelDistance;
		[JsonProperty(PropertyName = "travelDistance")]
		public decimal TravelDistance
		{
			get { return _travelDistance; }
			set {
				_travelDistance = value;
				NotifyPropertyChanged ();
			}
		}

		private decimal _travelDuration;
		[JsonProperty(PropertyName = "travelDuration")]
		public decimal TravelDuration
		{
			get { return _travelDuration; }
			set {
				_travelDuration = value;
				NotifyPropertyChanged ();
			}
		}

		private decimal _travelDurationTraffic;
		[JsonProperty(PropertyName = "travelDurationTraffic")]
		public decimal TravelDurationTraffic
		{
			get { return _travelDurationTraffic; }
			set {
				_travelDurationTraffic = value;
				NotifyPropertyChanged ();
			}
		}

		private ObservableCollection<BingRouteLeg> _routeLegs;
		[JsonProperty(PropertyName = "routeLegs")]
		public ObservableCollection<BingRouteLeg>RouteLegs
		{
			get { return _routeLegs; }
			set{
				_routeLegs = value;
				NotifyPropertyChanged ();
			}
		}

	}

	public class BingRouteLeg:BaseModel
	{
		private decimal _cost;
		[JsonProperty(PropertyName = "cost")]
		public decimal Cost
		{
			get { return _cost; }
			set {
				_cost = value;
				NotifyPropertyChanged ();
			}
		}

		private string _description;
		[JsonProperty(PropertyName = "description")]
		public string Description
		{
			get { return _description; }
			set {
				_description = value;
				NotifyPropertyChanged ();
			}
		}

		private decimal _travelDistance;
		[JsonProperty(PropertyName = "travelDistance")]
		public decimal TravelDistance
		{
			get { return _travelDistance; }
			set {
				_travelDistance = value;
				NotifyPropertyChanged ();
			}
		}

		private decimal _travelDuration;
		[JsonProperty(PropertyName = "travelDuration")]
		public decimal TravelDuration
		{
			get { return _travelDuration; }
			set {
				_travelDuration = value;
				NotifyPropertyChanged ();
			}
		}

		private ObservableCollection<BingItineraryItem> _itinItems;
		[JsonProperty(PropertyName = "itineraryItems")]
		public ObservableCollection<BingItineraryItem>ItineraryItems
		{
			get { return _itinItems; }
			set{
				_itinItems = value;
				NotifyPropertyChanged ();
			}
		}
	
	}

	public class BingItineraryItem:BaseModel
	{
		private string _compassDirection;
		[JsonProperty(PropertyName = "compassDirection")]
		public string CompassDirection
		{
			get { return _compassDirection; }
			set {
				_compassDirection = value;
				NotifyPropertyChanged ();
			}
		}

		private string _exit;
		[JsonProperty(PropertyName = "exit")]
		public string Exit
		{
			get { return _exit; }
			set {
				_exit = value;
				NotifyPropertyChanged ();
			}
		}

		private string _iconType;
		[JsonProperty(PropertyName = "iconType")]
		public string IconType
		{
			get { return _iconType; }
			set {
				_iconType = value;
				NotifyPropertyChanged ();
			}
		}

		private BingInstruction _instruction;
		[JsonProperty(PropertyName="instruction")]
		public BingInstruction Instruction
		{
			get { return _instruction; }
			set {
				_instruction = value;
				NotifyPropertyChanged ();
			}
		}

		private string _towardsRoadName;
		[JsonProperty(PropertyName = "towardsRoadName")]
		public string TowardsRoadName
		{
			get { return _towardsRoadName; }
			set {
				_towardsRoadName = value;
				NotifyPropertyChanged ();
			}
		}

		private decimal _travelDistance;
		[JsonProperty(PropertyName = "travelDistance")]
		public decimal TravelDistance
		{
			get { return _travelDistance; }
			set {
				_travelDistance = value;
				NotifyPropertyChanged ();
			}
		}

		private decimal _travelDuration;
		[JsonProperty(PropertyName = "travelDuration")]
		public decimal TravelDuration
		{
			get { return _travelDuration; }
			set {
				_travelDuration = value;
				NotifyPropertyChanged ();
			}
		}

		private string _travelMode;
		[JsonProperty(PropertyName = "travelMode")]
		public string TravelMode
		{
			get { return _travelMode; }
			set {
				_travelMode = value;
				NotifyPropertyChanged ();
			}
		}

	}

	public class BingInstruction:BaseModel
	{
		private string _formattedText;
		[JsonProperty(PropertyName = "formattedText")]
		public string FormattedText
		{
			get { return _formattedText; }
			set {
				_formattedText = value;
				NotifyPropertyChanged ();
			}
		}

		private string _maneuverType;
		[JsonProperty(PropertyName = "maneuverType")]
		public string ManeuverType
		{
			get { return _maneuverType; }
			set {
				_maneuverType = value;
				NotifyPropertyChanged ();
			}
		}

		private string _text;
		[JsonProperty(PropertyName = "text")]
		public string Text
		{
			get { return _text; }
			set {
				_text = value;
				NotifyPropertyChanged ();
			}
		}
	}
}

