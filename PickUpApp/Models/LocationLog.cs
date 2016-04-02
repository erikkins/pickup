using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class LocationLog:BaseModel
	{
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

		private string _Latitude;
		[JsonProperty(PropertyName = "latitude")]
		public string Latitude { 
			get{
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
				return _Longitude; 
			} 
			set{if (value != _Longitude) {
					_Longitude = value; NotifyPropertyChanged ();
				} } 
		}

		private string _UserId;
		[JsonProperty(PropertyName = "userid")]
		public string UserId { get{return _UserId; } set{if (value != _UserId) {
					_UserId = value; NotifyPropertyChanged ();
				} } }

		private string _scheduleid;
		[JsonProperty(PropertyName = "scheduleid")]
		public string ScheduleID { get{ return _scheduleid; } set{if (value != _scheduleid) {
					_scheduleid = value; NotifyPropertyChanged ();} } }

		private string _messageID;
		[JsonProperty(PropertyName = "messageid")]
		public string MessageID { get{return _messageID; } set{ if (value != _messageID) {
					_messageID = value; NotifyPropertyChanged ();
				} } }

		private string _messageIDType;
		[JsonProperty(PropertyName = "messageidtype")]
		public string MessageIDType { get{return _messageIDType; } set{ if (value != _messageIDType) {
					_messageIDType = value; NotifyPropertyChanged ();
				} } }

		private string _logType;
		[JsonProperty(PropertyName = "logtype")]
		public string LogType { get{return _logType; } set{ if (value != _logType) {
					_logType = value; NotifyPropertyChanged ();
				} } }
		
		
	}
}

