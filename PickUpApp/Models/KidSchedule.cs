using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class KidSchedule: BaseModel
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

		private string _KidID;
		[JsonProperty(PropertyName = "kidid")]
		public string KidID {get{return _KidID; }set{if (value != _KidID){
					_KidID = value; NotifyPropertyChanged ();
				}}}

		private string _ScheduleID;
		[JsonProperty(PropertyName = "scheduleid")]
		public string ScheduleID { get{return _ScheduleID; } set{if (value != _ScheduleID){
					_ScheduleID = value; NotifyPropertyChanged ();
				}} }

	}
}

