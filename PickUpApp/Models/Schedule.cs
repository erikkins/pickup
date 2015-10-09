using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class Schedule:BaseModel
	{
		public const string ADDRESS_PLACEHOLDER = "Click to set address";

		public Schedule()
		{
			//set some defaults
			AtWhen = Util.RoundUp (DateTime.Now, TimeSpan.FromMinutes (30));
			StartTimeTicks = AtWhen.TimeOfDay.Ticks;
			AtWhenEnd = Util.RoundUp (DateTime.Now, TimeSpan.FromMinutes (30)).AddHours (1);
			EndTimeTicks = AtWhenEnd.TimeOfDay.Ticks;
			Frequency = "";
		}

		private string _Id;
		public string id 
		{ 
			get{ return _Id; }
			set
			{
				if (value != _Id) {
					_Id = value;
					NotifyPropertyChanged ();
				}
			}
		}


		private DateTime _AtWhen;
		[JsonProperty(PropertyName = "atwhen")]
		public DateTime AtWhen 
		{ get
			{ return _AtWhen; } 
			set
			{
				if (value != _AtWhen) {
					_AtWhen = value;
					NotifyPropertyChanged ();
				}
			} 
		}

		private DateTime _atwhenDropoff;
		[JsonProperty(PropertyName = "atwhendropoff")]
		public DateTime AtWhenDropOff
		{ get
			{ return _atwhenDropoff; } 
			set
			{
				if (value != _atwhenDropoff) {
					_atwhenDropoff = value;
					NotifyPropertyChanged ();
				}
			} 
		}

		private DateTime _atwhenPickup;
		[JsonProperty(PropertyName = "atwhenpickup")]
		public DateTime AtWhenPickup
		{ get
			{ return _atwhenPickup; } 
			set
			{
				if (value != _atwhenPickup) {
					_atwhenPickup = value;
					NotifyPropertyChanged ();
				}
			} 
		}

		private string _dropoffnotes;
		[JsonProperty(PropertyName = "dropoffnotes")]
		public string DropOffNotes { get{ return _dropoffnotes; } set{if (value != _dropoffnotes) {
					_dropoffnotes = value; NotifyPropertyChanged ();} } }

		private string _pickupnotes;
		[JsonProperty(PropertyName = "pickupnotes")]
		public string PickupNotes { get{ return _pickupnotes; } set{if (value != _pickupnotes) {
					_pickupnotes = value; NotifyPropertyChanged ();} } }

		private string _DefaultDropOffAccount;
		[JsonProperty(PropertyName = "defaultdropoffaccount")]
		public string DefaultDropOffAccount { get{return _DefaultDropOffAccount; } set{if (value != _DefaultDropOffAccount) {
					_DefaultDropOffAccount = value; NotifyPropertyChanged ();
				} } }
					
		private string _DefaultDropOffAccountFirstName;
		[JsonProperty(PropertyName = "defaultdropoffaccountfirstname")]

		public string DefaultDropOffAccountFirstName { get{return _DefaultDropOffAccountFirstName; } set{if (value != _DefaultDropOffAccountFirstName) {
					_DefaultDropOffAccountFirstName = value; NotifyPropertyChanged ();
				} } }
		public bool ShouldSerializeDefaultDropOffAccountFirstName()
		{
			return false;
		}
		public bool ShouldSerializeDefaultDropOffAccountLastName()
		{
			return false;
		}
		public bool ShouldSerializeDefaultDropOffAccountPhotoURL()
		{
			return false;
		}
		public bool ShouldSerializeDefaultDropOffAccountFullname()
		{
			return false;
		}
		private string _DefaultDropOffAccountLastName;
		[JsonProperty(PropertyName = "defaultdropoffaccountlastname")]
		public string DefaultDropOffAccountLastName { get{return _DefaultDropOffAccountLastName; } set{if (value != _DefaultDropOffAccountLastName) {
					_DefaultDropOffAccountLastName = value; NotifyPropertyChanged ();
				} } }

		public string DefaultDropOffAccountFullname
		{
			get{ return _DefaultDropOffAccountFirstName + " " + _DefaultDropOffAccountLastName; }
		}

		private string _DefaultDropOffAccountPhotoURL;
		[JsonProperty(PropertyName = "defaultdropoffaccountphotourl")]
		public string DefaultDropOffAccountPhotoURL { get{return _DefaultDropOffAccountPhotoURL; } set{if (value != _DefaultDropOffAccountPhotoURL) {
					_DefaultDropOffAccountPhotoURL = value; NotifyPropertyChanged ();
				} } }

		private string _DefaultPickupAccount;
		[JsonProperty(PropertyName = "defaultpickupaccount")]
		public string DefaultPickupAccount { get{return _DefaultPickupAccount; } set{if (value != _DefaultPickupAccount) {
					_DefaultPickupAccount = value; NotifyPropertyChanged ();
				} } }

		private string _DefaultPickupAccountFirstName;
		[JsonProperty(PropertyName = "defaultpickupaccountfirstname")]
		public string DefaultPickupAccountFirstName { get{return _DefaultPickupAccountFirstName; } set{if (value != _DefaultPickupAccountFirstName) {
					_DefaultPickupAccountFirstName = value; NotifyPropertyChanged ();
				} } }
		private string _DefaultPickupAccountLastName;
		[JsonProperty(PropertyName = "defaultpickupaccountlastname")]
		public string DefaultPickupAccountLastName { get{return _DefaultPickupAccountLastName; } set{if (value != _DefaultPickupAccountLastName) {
					_DefaultPickupAccountLastName = value; NotifyPropertyChanged ();
				} } }

		public string DefaultPickupAccountFullname
		{
			get{ return _DefaultPickupAccountFirstName + " " + _DefaultPickupAccountLastName; }
		}

		private string _DefaultPickupAccountPhotoURL;
		[JsonProperty(PropertyName = "defaultpickupaccountphotourl")]
		public string DefaultPickupAccountPhotoURL { get{return _DefaultPickupAccountPhotoURL; } set{if (value != _DefaultPickupAccountPhotoURL) {
					_DefaultPickupAccountPhotoURL = value; NotifyPropertyChanged ();
				} } }
		public bool ShouldSerializeDefaultPickupAccountFirstName()
		{
			return false;
		}
		public bool ShouldSerializeDefaultPickupAccountLastName()
		{
			return false;
		}
		public bool ShouldSerializeDefaultPickupAccountPhotoURL()
		{
			return false;
		}
		public bool ShouldSerializeDefaultPickupAccountFullname()
		{
			return false;
		}


		private string _Activity;
		[JsonProperty(PropertyName = "activity")]
		public string Activity { get{ return _Activity; } set{if (value != _Activity) {
					_Activity = value; NotifyPropertyChanged ();} } }

		private bool _Recurring;
		[JsonProperty(PropertyName = "recurring")]
		public bool Recurring { get{return _Recurring; } set{if (value != _Recurring) {
					_Recurring = value; NotifyPropertyChanged ();
				} } }

		private string _Frequency;
		[JsonProperty(PropertyName = "frequency")]
		public string Frequency { get{ return _Frequency;} set{if (value != _Frequency) {
					_Frequency= value; NotifyPropertyChanged ();} } }

		private string _Latitude;
		[JsonProperty(PropertyName = "latitude")]
		public string Latitude { get{return _Latitude; } set{if (value != _Latitude) {
					_Latitude = value; NotifyPropertyChanged ();
				} } }

		private string _Longitude;
		[JsonProperty(PropertyName = "longitude")]
		public string Longitude { get{return _Longitude; } set{if (value != _Longitude) {
					_Longitude = value; NotifyPropertyChanged ();
				} } }

		private string _Address;
		[JsonProperty(PropertyName = "address")]
		public string Address 
		{ 
			get
			{ 
				if (string.IsNullOrEmpty (_Address)) {
					return ADDRESS_PLACEHOLDER;
				} else {
					//strip out CRLF?
					return _Address.Replace (Environment.NewLine, "  ");
					//return _Address;
				}
			} 
			set
			{
				if (value != _Address)
				{
					_Address = value;
					NotifyPropertyChanged();
				}
			}
		}

		private string _LocationMessage;
		[JsonProperty(PropertyName = "locationmessage")]
		public string LocationMessage { get{return _LocationMessage; } set{if (value != _LocationMessage) {
					_LocationMessage = value; NotifyPropertyChanged ();
				} } }

		private DateTime _AtWhenEnd;
		[JsonProperty(PropertyName = "atwhenend")]
		public DateTime AtWhenEnd { get{ return _AtWhenEnd; } set
			{
				if (value != _AtWhenEnd) {
					_AtWhenEnd = value;
					NotifyPropertyChanged ();
				}
			} 
		}

		private string _Location;
		[JsonProperty(PropertyName = "location")]
		public string Location { get{return _Location; } set{if (value != _Location) {
					_Location = value; NotifyPropertyChanged ();
				} } }

		private string _UserId;
		[JsonProperty(PropertyName = "userId")]
		public string UserId { get{return _UserId; } set{if (value != _UserId) {
					_UserId = value; NotifyPropertyChanged ();
				} } }


		private long _StartTimeTicks;
		[JsonProperty(PropertyName = "startTimeTicks")]
		public long StartTimeTicks
		{
			get{
				return _StartTimeTicks;
			}
			set{
				if (value != _StartTimeTicks) {
					_StartTimeTicks = value;
					NotifyPropertyChanged ();
				}
			}
		}
		private long _EndTimeTicks;
		[JsonProperty(PropertyName = "endTimeTicks")]
		public long EndTimeTicks
		{
			get{
				return _EndTimeTicks;
			}
			set{
				if (value != _EndTimeTicks) {
					_EndTimeTicks = value;
					NotifyPropertyChanged ();
				}
			}
		}


		private string _StartPlaceID;
		[JsonProperty(PropertyName = "startplaceid")]
		public string StartPlaceID { get{return _StartPlaceID; } set{if (value != _StartPlaceID) {
					_StartPlaceID = value; NotifyPropertyChanged ();
				} } }


		private string _StartPlaceName;
		[JsonProperty(PropertyName = "startplacename")]
		public string StartPlaceName { get{return _StartPlaceName; } set{if (value != _StartPlaceName) {
					_StartPlaceName = value; NotifyPropertyChanged ();
				} } }
		private string _StartPlaceAddress;
		[JsonProperty(PropertyName = "startplaceaddress")]
		public string StartPlaceAddress { get{return _StartPlaceAddress; } set{if (value != _StartPlaceAddress) {
					_StartPlaceAddress = value; NotifyPropertyChanged ();
				} } }
		public bool ShouldSerializeStartPlaceName()
		{
			return false;
		}
		public bool ShouldSerializeStartPlaceAddress()
		{
			return false;
		}

		private int _StartPlaceDistance;
		[JsonProperty(PropertyName = "startplacedistance")]
		public int StartPlaceDistance { get{return _StartPlaceDistance; } set{if (value != _StartPlaceDistance) {
					_StartPlaceDistance = value; NotifyPropertyChanged ();
				} } }
		private int _StartPlaceTravelTime;
		[JsonProperty(PropertyName = "startplacetraveltime")]
		public int StartPlaceTravelTime { get{return _StartPlaceTravelTime; } set{if (value != _StartPlaceTravelTime) {
					_StartPlaceTravelTime = value; NotifyPropertyChanged ();
				} } }


		private string _EndPlaceID;
		[JsonProperty(PropertyName = "endplaceid")]
		public string EndPlaceID { get{return _EndPlaceID; } set{if (value != _EndPlaceID) {
					_EndPlaceID = value; NotifyPropertyChanged ();
				} } }
		private string _EndPlaceName;
		[JsonProperty(PropertyName = "endplacename")]
		public string EndPlaceName { get{return _EndPlaceName; } set{if (value != _EndPlaceName) {
					_EndPlaceName = value; NotifyPropertyChanged ();
				} } }
		private string _EndPlaceAddress;
		[JsonProperty(PropertyName = "endplaceaddress")]
		public string EndPlaceAddress { get{return _EndPlaceAddress; } set{if (value != _EndPlaceAddress) {
					_EndPlaceAddress = value; NotifyPropertyChanged ();
				} } }

		public bool ShouldSerializeEndPlaceName()
		{
			return false;
		}
		public bool ShouldSerializeEndPlaceAddress()
		{
			return false;
		}

		private int _EndPlaceDistance;
		[JsonProperty(PropertyName = "endplacedistance")]
		public int EndPlaceDistance { get{return _EndPlaceDistance; } set{if (value != _EndPlaceDistance) {
					_EndPlaceDistance = value; NotifyPropertyChanged ();
				} } }
		private int _EndPlaceTravelTime;
		[JsonProperty(PropertyName = "endplacetraveltime")]
		public int EndPlaceTravelTime { get{return _EndPlaceTravelTime; } set{if (value != _EndPlaceTravelTime) {
					_EndPlaceTravelTime = value; NotifyPropertyChanged ();
				} } }

		#region non-serializable
		[JsonIgnore]
		public TimeSpan StartTime 
		{ get
			{ return TimeSpan.FromTicks (StartTimeTicks);
			}
			set
			{
				if (value != TimeSpan.FromTicks(StartTimeTicks)) {
					//AtWhen += value;
					StartTimeTicks = value.Ticks;
					NotifyPropertyChanged ();
				}}

		}

		[JsonIgnore]
		public TimeSpan EndTime {
			get
			{ 
				return TimeSpan.FromTicks (EndTimeTicks);
			} 
			set
			{
				if (value != TimeSpan.FromTicks(EndTimeTicks)) {
					//AtWhen += value;
					EndTimeTicks = value.Ticks;
					NotifyPropertyChanged ();
				}
			}
		}
		[JsonIgnore]
		public bool Monday
		{
			get{
				if (Frequency.Contains ("M")) {
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
		[JsonIgnore]
		public bool Tuesday
		{
			get{
				if (Frequency.Contains ("Tu")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("Tu", value);
				NotifyPropertyChanged ();
			}
		}
		[JsonIgnore]
		public bool Wednesday
		{
			get{
				if (Frequency.Contains ("W")) {
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
		[JsonIgnore]
		public bool Thursday
		{
			get{
				if (Frequency.Contains ("Th")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("Th", value);
				NotifyPropertyChanged ();
			}
		}
		[JsonIgnore]
		public bool Friday
		{
			get{
				if (Frequency.Contains ("F")) {
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
		[JsonIgnore]
		public bool Saturday
		{
			get{
				if (Frequency.Contains ("Sa")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("Sa", value);
				NotifyPropertyChanged ();
			}
		}
		[JsonIgnore]
		public bool Sunday
		{
			get{
				if (Frequency.Contains ("Su")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("Su", value);
				NotifyPropertyChanged ();
			}
		}

		private void setFrequency (string day, bool include)
		{
			if (include)
			{
				//make sure the frequency has the value
				if (!Frequency.Contains(day))
				{
					Frequency += day;
				}
			}
			else{
				//make sure the frequency does NOT have the value
				if (Frequency.Contains(day))
				{
					Frequency = Frequency.Replace(day, "");
				}
			}
			Frequency = reorderFrequency ();
		}

		private string reorderFrequency()
		{
			//need to make sure it's in the correct MTWThFSaSu format
			string tempFreq = "";
			if (Monday) {
				tempFreq = "M";
			}
			if (Tuesday) {
				tempFreq += "Tu";
			}
			if (Wednesday) {
				tempFreq += "W";
			}
			if (Thursday) {
				tempFreq += "Th";
			}
			if (Friday) {
				tempFreq += "F";
			}
			if (Saturday) {
				tempFreq += "Sa";
			}
			if (Sunday) {
				tempFreq += "Su";
			}
			return tempFreq;
		}

		#endregion


	}
}

