using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class EmptyClass:BaseModel
	{
		private string _Status;
		[JsonProperty(PropertyName = "status")]
		public string Status { get{return _Status; } set{if (value != _Status) {
					_Status = value; NotifyPropertyChanged ();
				} } }		

	}
}

