using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class Feedback:BaseModel
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

		private string _content;
		[JsonProperty(PropertyName = "content")]
		public string Content { get{return _content; } set{if (value != _content) {
					_content = value; NotifyPropertyChanged ();
				} } }	

	}
}

