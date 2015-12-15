using System;
using Newtonsoft.Json;
using System.Linq;

namespace PickUpApp
{
	
	public class Today : BaseModel
	{

		public const string ADDRESS_PLACEHOLDER = "Click to set address";
		//ok this is a merged class between Schedule and Invite
		//seems stupid to copy the fields directly
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

		private string _rowType;
		[JsonProperty(PropertyName = "rowtype")]
		public string RowType
		{
			get{
				return _rowType;
			}
			set
			{
				if (value != _rowType) {
					_rowType = value;
					NotifyPropertyChanged ();
				}
			}
		}

		private string _accountplaceid;
		[JsonProperty(PropertyName = "accountplaceid")]
		public string AccountPlaceID { get{ return _accountplaceid; } set{if (value != _accountplaceid) {
					_accountplaceid = value; NotifyPropertyChanged ();} } }


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

		[JsonIgnore]
		public TimeSpan TSDropOff
		{ get
			{ return TimeSpan.FromTicks(_dropoffticks); } 
			set
			{
				if (value.Ticks != _dropoffticks) {
					_dropoffticks = value.Ticks;
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
		{ get
			{ return TimeSpan.FromTicks(_pickupticks); } 
			set
			{
				if (value.Ticks != _pickupticks) {
					_pickupticks = value.Ticks;
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
//				if (this.AccountPlaceID == null) {
//					return null;
//				} else {
//					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
//							where aps.id == this.AccountPlaceID
//						select aps;
//					return ap.FirstOrDefault ().Latitude;
//				}
				return _Latitude; 
			} 
			set{if (value != _Latitude) {
					_Latitude = value; NotifyPropertyChanged ();
				} } }

		private string _Longitude;
		[JsonProperty(PropertyName = "longitude")]
		public string Longitude 
		{ get
			{
//				if (this.AccountPlaceID == null) {
//					return null;
//				} else {
//					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
//							where aps.id == this.AccountPlaceID
//						select aps;
//					return ap.FirstOrDefault ().Longitude;
//				}
				return _Longitude; 
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
				return _Address.Replace (Environment.NewLine, "  ");
//				if (string.IsNullOrEmpty (_Address)) {
//					return ADDRESS_PLACEHOLDER;
//				} else {
//					//strip out CRLF?
//					if (this.AccountPlaceID == null) {
//						return null;
//					} else {
//						System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
//								where aps.id == this.AccountPlaceID
//							select aps;
//						return ap.FirstOrDefault ().Address;
//					}
//					//return _Address.Replace (Environment.NewLine, "  ");
//					//return _Address;
//				}
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
//				if (this.AccountPlaceID == null) {
//					return null;
//				}
//				else
//				{
//					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
//							where aps.id == this.AccountPlaceID
//						select aps;
//					return ap.FirstOrDefault ().PlaceName;
//				}
				return _Location; 
			} 
			set{if (value != _Location) {
					_Location = value; NotifyPropertyChanged ();
				} } }
		
		//[JsonIgnore]
		private string _locationPhone;
		[JsonProperty(PropertyName = "locationphone")]
		public string LocationPhone
		{
			get{
				return _locationPhone;
//				if (this.AccountPlaceID == null) {
//					return null;
//				}
//				else
//				{
//					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
//							where aps.id == this.AccountPlaceID
//						select aps;
//					return ap.FirstOrDefault ().Phone;
//				}
			}
			set{
				_locationPhone = value;
				NotifyPropertyChanged ();
			}
		}
			

		//[JsonIgnore]
		private string _locationNotes;
		[JsonProperty(PropertyName = "locationnotes")]
		public string LocationNotes
		{
			get{
				return _locationNotes;
//				if (this.AccountPlaceID == null) {
//					return null;
//				}
//				else
//				{
//					System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in App.myPlaces
//							where aps.id == this.AccountPlaceID
//						select aps;
//					return ap.FirstOrDefault ().Notes;
//				}
			}
			set{
				_locationNotes = value;
				NotifyPropertyChanged();
			}
		}

		//unused?
//		private string _LocationMessage;
//		[JsonProperty(PropertyName = "locationmessage")]
//		public string LocationMessage { get{return _LocationMessage; } set{if (value != _LocationMessage) {
//					_LocationMessage = value; NotifyPropertyChanged ();
//				} } }
		
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


		private string _AccountID;
		[JsonProperty(PropertyName="accountid")]
		public string AccountID
		{ 
			get{ return _AccountID; }
			set
			{
				if (value != _AccountID) {
					_AccountID = value;
					NotifyPropertyChanged ();
				}
			}
		}
		private string _Requestor;
		[JsonProperty(PropertyName = "requestor")]
		public string Requestor { get{ return _Requestor; } set{if (value != _Requestor) {
					_Requestor = value; NotifyPropertyChanged ();} } }

		private string _RequestorPhone;
		[JsonProperty(PropertyName = "requestorphone")]
		public string RequestorPhone { get{ return _RequestorPhone; } set{if (value != _RequestorPhone) {
					_RequestorPhone = value; NotifyPropertyChanged ();} } }

		private string _Kids;
		[JsonProperty(PropertyName = "kids")]
		public string Kids { get{ return _Kids; } set{if (value != _Kids) {
					_Kids = value; NotifyPropertyChanged ();} } }

		private string _pickupDate;
		[JsonProperty(PropertyName = "pickupdate")]
		public string PickupDate { get{return _pickupDate; } set{if (value != _pickupDate) {
					_pickupDate = value; NotifyPropertyChanged ();
				} } }
					
		private string _Message;
		[JsonProperty(PropertyName = "message")]
		public string Message { get{return _Message; } set{if (value != _Message) {
					_Message = value; NotifyPropertyChanged ();
				} } }

		private string _ConfirmedBy;
		[JsonProperty(PropertyName = "confirmedby")]
		public string ConfirmedBy 
		{ 
			get{
				return _ConfirmedBy;
			} 
			set{if (value != _ConfirmedBy) {
					_ConfirmedBy = value; NotifyPropertyChanged ();
				} } }

		private string _ReturnTo;
		[JsonProperty(PropertyName = "returnto")]
		public string ReturnTo { get{return _ReturnTo; } set{if (value != _ReturnTo) {
					_ReturnTo = value; NotifyPropertyChanged ();
				} } }

		private double _ReturnToLatitude;
		[JsonProperty(PropertyName = "returntolatitude")]
		public double ReturnToLatitude { get{return _ReturnToLatitude; } set{if (value != _ReturnToLatitude) {
					_ReturnToLatitude = value; NotifyPropertyChanged ();
				} } }

		private double _ReturnToLongitude;
		[JsonProperty(PropertyName = "returntolongitude")]
		public double ReturnToLongitude { get{return _ReturnToLongitude; } set{if (value != _ReturnToLongitude) {
					_ReturnToLongitude = value; NotifyPropertyChanged ();
				} } }

		private string _ReturnToAddress;
		[JsonProperty(PropertyName = "returntoaddress")]
		public string ReturnToAddress { get{return _ReturnToAddress; } set{if (value != _ReturnToAddress) {
					_ReturnToAddress = value; NotifyPropertyChanged ();
				} } }
		

		public string ActualAtWhen
		{
			get{
				
				//calculate it from the AtWhen + StartTimeTicks
				DateTime starttime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				DateTime now = DateTime.Now;
				TimeSpan tsEnd = TimeSpan.FromTicks(StartTimeTicks);
				starttime += tsEnd;

				return string.Format ("{0:hh:mm tt}", starttime);
			}
		}
		public string ActualAtWhenEnd
		{
			get{

				//calculate it from the AtWhen + StartTimeTicks
				DateTime endtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				DateTime now = DateTime.Now;
				TimeSpan tsEnd = TimeSpan.FromTicks(EndTimeTicks);
				endtime += tsEnd;

				return string.Format ("{0:hh:mm tt}", endtime);
				}
		}

		public string TodayDescriptor
		{
			get
			{
				string ret = "";
				switch (this.RowType) {
				case "schedule":
					if (!string.IsNullOrEmpty (this.ConfirmedBy)) {
						ret = this.ConfirmedBy + " will be picking up";
					}
					break;
				case "invite":
					if (!string.IsNullOrEmpty (this.Requestor)) {
						ret = "You are picking up for " + this.Requestor;
					}
					break;
				}
				return ret;
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

		private double _StartPlaceDistance;
		[JsonProperty(PropertyName = "startplacedistance")]
		public double StartPlaceDistance { get{return _StartPlaceDistance; } set{if (value != _StartPlaceDistance) {
					_StartPlaceDistance = value; NotifyPropertyChanged ();
				} } }
		private double _StartPlaceTravelTime;
		[JsonProperty(PropertyName = "startplacetraveltime")]
		public double StartPlaceTravelTime { get{return _StartPlaceTravelTime; } set{if (value != _StartPlaceTravelTime) {
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
		private double _EndPlaceDistance;
		[JsonProperty(PropertyName = "endplacedistance")]
		public double EndPlaceDistance { get{return _EndPlaceDistance; } set{if (value != _EndPlaceDistance) {
					_EndPlaceDistance = value; NotifyPropertyChanged ();
				} } }
		private double _EndPlaceTravelTime;
		[JsonProperty(PropertyName = "endplacetraveltime")]
		public double EndPlaceTravelTime { get{return _EndPlaceTravelTime; } set{if (value != _EndPlaceTravelTime) {
					_EndPlaceTravelTime = value; NotifyPropertyChanged ();
				} } }

		private bool _TrafficWarning;
		[JsonProperty(PropertyName = "trafficwarning")]
		public bool TrafficWarning { get{return _TrafficWarning; } set{if (value != _TrafficWarning) {
					_TrafficWarning = value; NotifyPropertyChanged ();
				} } }
		
		private bool _DropOffComplete;
		[JsonProperty(PropertyName = "dropoffcomplete")]
		public bool DropOffComplete { get{return _DropOffComplete; } set{if (value != _DropOffComplete) {
					_DropOffComplete = value; NotifyPropertyChanged ();
				} } }

		private bool _PickupComplete;
		[JsonProperty(PropertyName = "pickupcomplete")]
		public bool PickupComplete { get{return _PickupComplete; } set{if (value != _PickupComplete) {
					_PickupComplete = value; NotifyPropertyChanged ();
				} } }


		private string _DropOffMessageID;
		[JsonProperty(PropertyName = "dropoffmessageid")]
		public string DropOffMessageID { get{return _DropOffMessageID; } set{if (value != _DropOffMessageID) {
					_DropOffMessageID = value; NotifyPropertyChanged ();
				} } }

		private string _DropOffMessageStatus;
		[JsonProperty(PropertyName = "dropoffmessagestatus")]
		public string DropOffMessageStatus { get{return _DropOffMessageStatus; } set{if (value != _DropOffMessageStatus) {
					_DropOffMessageStatus = value; NotifyPropertyChanged ();
				} } }

		private string _PickupMessageID;
		[JsonProperty(PropertyName = "pickupmessageid")]
		public string PickupMessageID { get{return _PickupMessageID; } set{if (value != _PickupMessageID) {
					_PickupMessageID = value; NotifyPropertyChanged ();
				} } }

		private string _PickupMessageStatus;
		[JsonProperty(PropertyName = "pickupmessagestatus")]
		public string PickupMessageStatus { get{return _PickupMessageStatus; } set{if (value != _PickupMessageStatus) {
					_PickupMessageStatus = value; NotifyPropertyChanged ();
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

		//not serialized
		private bool _IsNext;
		public bool IsNext { get{return _IsNext; } set{if (value != _IsNext) {
					_IsNext = value; NotifyPropertyChanged ();
				} } }


		//not serialized
		private bool _IsPickup;
		public bool IsPickup { get{return _IsPickup; } set{if (value != _IsPickup) {
					_IsPickup = value; NotifyPropertyChanged ();
				} } }
	}
}

