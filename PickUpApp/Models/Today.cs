using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class Today : BaseModel
	{
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
					return "WHAT?";
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

		private string _Requestor;
		[JsonProperty(PropertyName = "requestor")]
		public string Requestor { get{ return _Requestor; } set{if (value != _Requestor) {
					_Requestor = value; NotifyPropertyChanged ();} } }

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


	}
}

