using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class InviteResponse:BaseModel
	{

		private string _Recipients;
		[JsonProperty(PropertyName = "recipients")]
		public string Recipients { get{return _Recipients; } set{ if (value != _Recipients) {
					_Recipients = value; NotifyPropertyChanged ();
				} } }

		private string _returncode;
		[JsonProperty(PropertyName = "returncode")]
		public string ReturnCode { get{return _returncode; } set{ if (value != _returncode) {
					_returncode = value; NotifyPropertyChanged ();
				} } }
					
	}
}
