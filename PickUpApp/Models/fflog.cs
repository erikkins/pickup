using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class fflog:BaseModel
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

		public fflog(string who, string what)
		{
			Who = who;
			What = what;
		}
		public fflog (string what)
		{
			Who = App.myAccount.id;
			What = what;
		}

		private string _who;
		[JsonProperty(PropertyName = "who")]
		public string Who { get{return _who; } set{ if (value != _who) {
					_who = value; NotifyPropertyChanged ();
				} } }
	
		private string _what;
		[JsonProperty(PropertyName = "what")]
		public string What { get{return _what; } set{ if (value != _what) {
					_what = value; NotifyPropertyChanged ();
				} } }
	
	}
}

