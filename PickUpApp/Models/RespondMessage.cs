﻿using System;
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

		private string _comment;
		[JsonProperty(PropertyName = "comment")]
		public string Comment { get{return _comment; } set{ if (value != _comment) {
					_comment = value; NotifyPropertyChanged ();
				} } }
		
		private string _postUpdate;
		public string PostUpdate { get{return _postUpdate; } set{ if (value != _postUpdate) {
					_postUpdate = value; NotifyPropertyChanged ();
				} } }


		private string _conditional;
		[JsonProperty(PropertyName = "conditional")]
		public string Conditional { get{return _conditional; } set{ if (value != _conditional) {
					_conditional = value; NotifyPropertyChanged ();
				} } }
	}
}

