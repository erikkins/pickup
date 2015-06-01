using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class InviteMessage:BaseModel
	{
	
		private string _Id;
		[JsonProperty(PropertyName="id")]
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

		private string _InviteID;
		[JsonProperty(PropertyName = "inviteid")]
		public string InviteID { get{return _InviteID; } set{ if (value != _InviteID) {
					_InviteID = value; NotifyPropertyChanged ();
				} } }
		

		private string _AccountID;
		[JsonProperty(PropertyName="accountid")]
		public string AccountID
		{ 
			get{ return _AccountID; }
			set
			{
				if (value != _AccountID) {
					_AccountID = value;
					NotifyPropertyChanged ();
				}
			}
		}

		private string _Message;
		[JsonProperty(PropertyName = "message")]
		public string Message { get{return _Message; } set{if (value != _Message) {
					_Message = value; NotifyPropertyChanged ();
				} } }


	}
}

