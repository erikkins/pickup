using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class RespondMessage:BaseModel
	{
		private string _messageID;
		[JsonProperty(PropertyName = "messageid")]
		public string MessageID { get{return _messageID; } set{ if (value != _messageID) {
					_messageID = value; NotifyPropertyChanged ();
				} } }

		private string _status;
		[JsonProperty(PropertyName = "status")]
		public string Status { get{return _status; } set{ if (value != _status) {
					_status = value; NotifyPropertyChanged ();
				} } }

		private string _response;
		[JsonProperty(PropertyName = "response")]
		public string Response { get{return _response; } set{ if (value != _response) {
					_response = value; NotifyPropertyChanged ();
				} } }

	}
}

