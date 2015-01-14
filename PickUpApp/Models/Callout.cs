using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class Callout:BaseModel
	{
		private string _Id;
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

		private string _ScheduleID;
		[JsonProperty(PropertyName = "scheduleid")]
		public string ScheduleID { get{return _ScheduleID; } set{ if (value != _ScheduleID) {
					_ScheduleID = value; NotifyPropertyChanged ();
				} } }

		private DateTime _ActualAtWhen;
		[JsonProperty(PropertyName = "actualatwhen")]
		public DateTime ActualAtWhen { get{return _ActualAtWhen; } set{if (value != _ActualAtWhen) {
					_ActualAtWhen = value; NotifyPropertyChanged ();
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
	}
}

