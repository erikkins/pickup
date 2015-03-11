using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class InviteRequest:BaseModel
	{

		private string _ScheduleID;
		[JsonProperty(PropertyName = "scheduleid")]
		public string ScheduleID { get{return _ScheduleID; } set{ if (value != _ScheduleID) {
					_ScheduleID = value; NotifyPropertyChanged ();
				} } }

		private string _circle;
		[JsonProperty(PropertyName = "circle")]
		public string circle { get{return _circle; } set{ if (value != _circle) {
					_circle = value; NotifyPropertyChanged ();
				} } }

		private string _note;
		[JsonProperty(PropertyName = "note")]
		public string note { get{return _note; } set{ if (value != _note) {
					_note = value; NotifyPropertyChanged ();
				} } }
	}
}

