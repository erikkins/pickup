using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class BlackoutDate : BaseModel
	{
		private string _Id;
		[JsonProperty(PropertyName = "id")]
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

		private bool _selected;
		[JsonProperty(PropertyName = "selected")]
		public bool Selected { get{return _selected; } set{if (value != _selected) {
					_selected = value; NotifyPropertyChanged ();
				} } }

		private string _scheduleid;
		[JsonProperty(PropertyName = "scheduleid")]
		public string ScheduleID { get{return _scheduleid; } set{if (value != _scheduleid) {
					_scheduleid = value; NotifyPropertyChanged ();
				} } }

		private string _displayname;
		[JsonProperty(PropertyName = "displayname")]
		public string DisplayName { get{return _displayname; } set{if (value != _displayname) {
					_displayname = value; NotifyPropertyChanged ();
				} } }


//		private string _blackoutdatesid;
//		[JsonProperty(PropertyName = "blackoutdatesid")]
//		public string BlackoutDatesID { get{return _blackoutdatesid; } set{if (value != _blackoutdatesid) {
//					_blackoutdatesid = value; NotifyPropertyChanged ();
//				} } }
					
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
		

	}
}

