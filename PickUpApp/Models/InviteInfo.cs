using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class InviteInfo:BaseModel
	{
		private string _Id;
		[JsonProperty(PropertyName="id")]
		public string Id 
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

		private string _Activity;
		[JsonProperty(PropertyName = "activity")]
		public string Activity { get{ return _Activity; } set{if (value != _Activity) {
					_Activity = value; NotifyPropertyChanged ();} } }

		private double _Latitude;
		[JsonProperty(PropertyName = "latitude")]
		public double Latitude { get{return _Latitude; } set{if (value != _Latitude) {
					_Latitude = value; NotifyPropertyChanged ();
				} } }

		private double _Longitude;
		[JsonProperty(PropertyName = "longitude")]
		public double Longitude { get{return _Longitude; } set{if (value != _Longitude) {
					_Longitude = value; NotifyPropertyChanged ();
				} } }

		private string _Address;
		[JsonProperty(PropertyName = "address")]
		public string Address 
		{ 
			get
			{ 
				if (string.IsNullOrEmpty (_Address)) {
					return "No known address";
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

		private string _Location;
		[JsonProperty(PropertyName = "location")]
		public string Location { get{ return _Location; } set{if (value != _Location) {
					_Location = value; NotifyPropertyChanged ();} } }

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

		private bool _Solved;
		[JsonProperty(PropertyName = "solved")]
		public bool Solved { get{return _Solved; } set{if (value != _Solved) {
					_Solved = value; NotifyPropertyChanged ();
				} } }

		private string _SolvedBy;
		[JsonProperty(PropertyName = "solvedby")]
		public string SolvedBy { get{return  _SolvedBy; } set{if (value != _SolvedBy) {
					_SolvedBy = value; NotifyPropertyChanged ();
				} } }

		private bool _Complete;
		[JsonProperty(PropertyName = "complete")]
		public bool Complete { get{return _Complete; } set{if (value != _Complete) {
					_Complete = value; NotifyPropertyChanged ();
				} } }

		private DateTime _CompleteAtWhen;
		[JsonProperty(PropertyName = "completeatwhen")]
		public DateTime CompleteAtWhen { get{return  _CompleteAtWhen; } set{if (value != _CompleteAtWhen) {
					_CompleteAtWhen = value; NotifyPropertyChanged ();
				} } }

	}
}

