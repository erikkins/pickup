using System;
using Newtonsoft.Json;
using System.Linq;

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
			AtWhenEnd = Util.RoundUp (DateTime.Now, TimeSpan.FromMinutes (30)).AddHours (1).AddYears(1);
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


		private string _accountplaceid;
		[JsonProperty(PropertyName = "accountplaceid")]
		public string AccountPlaceID { get{ return _accountplaceid; } set{if (value != _accountplaceid) {
					_accountplaceid = value; NotifyPropertyChanged ();} } }

		private long _dropoffticks;
		[JsonProperty(PropertyName = "dropoffticks")]
		public long DropOffTicks
		{ get
			{ return _dropoffticks; } 
			set
			{
				if (value != _dropoffticks) {
					_dropoffticks = value;
					NotifyPropertyChanged ();
				}
			} 
		}


		public DateTime PickupDT
		{
			get{
				return DateTime.Today.Add (TSPickup);
			}
		}

		public DateTime PickupDiff
		{
			get{
				return DateTime.Today.Add (TSPickup.Subtract (TimeSpan.FromMinutes ((double)EndPlaceTravelTime)));
			}
		}

		public DateTime DropoffDT
		{
			get{
				return DateTime.Today.Add (TSDropOff);
			}
		}

		public DateTime DropoffDiff
		{
			get{
				return DateTime.Today.Add (TSDropOff.Subtract (TimeSpan.FromMinutes ((double)StartPlaceTravelTime)));
			}
		}


			
		[JsonIgnore]
		public TimeSpan TSDropOff
		{ 
			get
			{ 
				//convert from UTC
				DateTime dtUTC = new DateTime(_dropoffticks).AddDays(1);
				DateTime dt = dtUTC.ToLocalTime ();
				TimeSpan ts = dt.TimeOfDay;
				return ts;

				//orig
				//return TimeSpan.FromTicks(_dropoffticks); 
			} 
			set
			{
				if (value.Ticks != _dropoffticks) {	

					//convert to UTC
					DateTime dt = new DateTime(value.Ticks).AddDays(1);
					DateTime dtUTC = dt.ToUniversalTime ();
					TimeSpan tsUTC = dtUTC.TimeOfDay;
					_dropoffticks = tsUTC.Ticks;

					//ORIG
					//_dropoffticks = value.Ticks;
					NotifyPropertyChanged ();
				}
			} 
		}

		private long _pickupticks;
		[JsonProperty(PropertyName = "pickupticks")]
		public long PickupTicks
		{ get
			{ return _pickupticks; } 
			set
			{
				if (value != _pickupticks) {
					_pickupticks = value;
					NotifyPropertyChanged ();
				}
			} 
		}

		[JsonIgnore]
		public TimeSpan TSPickup
		{ 
			get
			{
				//convert from UTC
				DateTime dtUTC = new DateTime(_pickupticks).AddDays(1);
				DateTime dt = dtUTC.ToLocalTime ();
				TimeSpan ts = dt.TimeOfDay;
				return ts;

				//orig
				//return TimeSpan.FromTicks(_pickupticks); 
			} 
			set
			{
				if (value.Ticks != _pickupticks) {

					//convert to UTC
					DateTime dt = new DateTime(value.Ticks).AddDays(1);
					DateTime dtUTC = dt.ToUniversalTime ();
					TimeSpan tsUTC = dtUTC.TimeOfDay;
					_pickupticks = tsUTC.Ticks;

					//orig
					//_pickupticks = value.Ticks;
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
		public string Latitude { 
			get{
				//actually need to pull the latitude from the AccountPlace for the accountplaceid
				if (this.AccountPlaceID == null) {
					return null;
				} else {
					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
					                                                         where aps.id == this.AccountPlaceID
					                                                         select aps;
					if (ap.Count () > 0) {
						return ap.FirstOrDefault ().Latitude;
					} else {
						return null;
					}
				}
				//return _Latitude; 
			} 
			set{if (value != _Latitude) {
					_Latitude = value; NotifyPropertyChanged ();
				} } }

		private string _Longitude;
		[JsonProperty(PropertyName = "longitude")]
		public string Longitude 
		{ get
			{
				if (this.AccountPlaceID == null) {
					return null;
				} else {
					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
					                                                         where aps.id == this.AccountPlaceID
					                                                         select aps;
					if (ap.Count () > 0) {
						return ap.FirstOrDefault ().Longitude;
					} else {
						return null;
					}
				}
				//return _Longitude; 
			} 
			set{if (value != _Longitude) {
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
					if (this.AccountPlaceID == null) {
						return null;
					} else {
						System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
						                                                         where aps.id == this.AccountPlaceID
							                                                       select aps;

						if (ap.Count() == 0) {
							return null;
						} else {
							return ap.FirstOrDefault ().Address;
						}
					}
					//return _Address.Replace (Environment.NewLine, "  ");
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
		public string Location 
		{ get
			{
				if (this.AccountPlaceID == null) {
					return null;
				}
				else
				{
					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
					                                                         where aps.id == this.AccountPlaceID
					                                                         select aps;

					if (ap.Count() > 0) {
						return ap.FirstOrDefault ().PlaceName;
					} else {
						return null;
					}

				}
				//return _Location; 
			} 
			set{if (value != _Location) {
					_Location = value; NotifyPropertyChanged ();
				} } }



		[JsonIgnore]
		public string LocationPhone
		{
			get{
				if (this.AccountPlaceID == null) {
					return null;
				}
				else
				{
					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
							where aps.id == this.AccountPlaceID
						select aps;
					if (ap.Count () > 0) {
						return ap.FirstOrDefault ().Phone;
					} else {
						return null;
					}
				}
			}
		}

		[JsonIgnore]
		public string LocationNotes
		{
			get{
				if (this.AccountPlaceID == null) {
					return null;
				}
				else
				{
					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
							where aps.id == this.AccountPlaceID
						select aps;
					return ap.FirstOrDefault ().Notes;
				}
			}
		}

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
		public string StartPlaceName { 
			get{

				if (this.StartPlaceID == null) {
					return null;
				}
				else
				{
					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
							where aps.id == this.StartPlaceID
						select aps;

					if (ap.Count() > 0) {
						return ap.FirstOrDefault ().PlaceName;
					} else {
						return null;
					}

				}

				//return _StartPlaceName; 
			} 
			set{if (value != _StartPlaceName) {
					_StartPlaceName = value; NotifyPropertyChanged ();
				} } }
		private string _StartPlaceAddress;
		[JsonProperty(PropertyName = "startplaceaddress")]
		public string StartPlaceAddress { 
			get{
				if (_StartPlaceAddress == null) {
					return ADDRESS_PLACEHOLDER;
				}

				if (this.StartPlaceID == null) {
					return null;
				}
				else
				{
					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
							where aps.id == this.StartPlaceID
						select aps;

					if (ap.Count() > 0) {
						return ap.FirstOrDefault ().Address;
					} else {
						return null;
					}
				}
					
				//return _StartPlaceAddress; 
			} 
			set{if (value != _StartPlaceAddress) {
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

		private decimal _StartPlaceDistance;
		[JsonProperty(PropertyName = "startplacedistance")]
		public decimal StartPlaceDistance { get{return _StartPlaceDistance; } set{if (value != _StartPlaceDistance) {
					_StartPlaceDistance = value; NotifyPropertyChanged ();
				} } }
		private decimal _StartPlaceTravelTime;
		[JsonProperty(PropertyName = "startplacetraveltime")]
		public decimal StartPlaceTravelTime { get{return _StartPlaceTravelTime; } set{if (value != _StartPlaceTravelTime) {
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

		private decimal _EndPlaceDistance;
		[JsonProperty(PropertyName = "endplacedistance")]
		public decimal EndPlaceDistance { get{return _EndPlaceDistance; } set{if (value != _EndPlaceDistance) {
					_EndPlaceDistance = value; NotifyPropertyChanged ();
				} } }
		private decimal _EndPlaceTravelTime;
		[JsonProperty(PropertyName = "endplacetraveltime")]
		public decimal EndPlaceTravelTime { get{return _EndPlaceTravelTime; } set{if (value != _EndPlaceTravelTime) {
					_EndPlaceTravelTime = value; NotifyPropertyChanged ();
				} } }


		private string _via;
		[JsonProperty(PropertyName = "via")]
		public string Via { get{return _via; } set{if (value != _via) {
					_via = value; NotifyPropertyChanged ();
				} } }
		public bool ShouldSerializeVia()
		{
			return false;
		}

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

