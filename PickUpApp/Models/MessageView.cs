using System;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PickUpApp
{
	public class MessageView : BaseModel
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
		private DateTimeOffset _createdat;
		[JsonProperty(PropertyName = "__createdAt")]
		public DateTimeOffset Created { get{return _createdat; } set{if (value != _createdat) {
					_createdat = value; NotifyPropertyChanged ();
				} } }
		
		private string _SenderID;
		[JsonProperty(PropertyName = "senderid")]
		public string SenderID { get{return _SenderID; } set{if (value != _SenderID) {
					_SenderID = value; NotifyPropertyChanged ();
				} } }

		private string _Sender;
		[JsonProperty(PropertyName = "sender")]
		public string Sender { get{return _Sender; } set{if (value != _Sender) {
					_Sender = value; NotifyPropertyChanged ();
				} } }

		private string _SenderPhotoURL;
		[JsonProperty(PropertyName = "senderphotourl")]
		public string SenderPhotoURL 
		{ get
			{
				if (!_SenderPhotoURL.ToLower ().StartsWith ("http")) {
					if (!App.Device.FileManager.FileExists (_SenderPhotoURL)) {
						return initialsPath ();
					}
				}
				return _SenderPhotoURL; 
			} 
			set{if (value != _SenderPhotoURL) {
					_SenderPhotoURL = value; NotifyPropertyChanged ();
				} } }

		private string initialsPath()
		{
			string initials = "";
			string filename = "";
			if (_Sender == null) {
				//sol
				return "";
			}

			string Firstname = "";
			string Lastname = "";

			if (_Sender.Contains (" ")) {

				string[] parts = _Sender.Split (' ');
				Firstname = parts [0];
				Lastname = parts [1];

			} else {
				Firstname = _Sender;
			}


			if (string.IsNullOrEmpty(Firstname)) {
				initials = Lastname.Substring (0, 1).ToUpper ();
			} else if (string.IsNullOrEmpty(Lastname)) {
				initials = Firstname.Substring (0, 1).ToUpper ();
			} else {
				initials = Firstname.Substring (0, 1).ToUpper () + Lastname.Substring (0, 1).ToUpper ();
			}

			var dep = DependencyService.Get<PickUpApp.ICircleText> ();
			filename = dep.CreateCircleText (initials, 50, 50);
			return filename;
		}


		private string _RecipientID;
		[JsonProperty(PropertyName = "recipientid")]
		public string RecipientID { get{return _RecipientID; } set{if (value != _RecipientID) {
					_RecipientID = value; NotifyPropertyChanged ();
				} } }

		private string _Recipient;
		[JsonProperty(PropertyName = "recipient")]
		public string Recipient { get{return _Recipient; } set{if (value != _Recipient) {
					_Recipient = value; NotifyPropertyChanged ();
				} } }

		private string _RecipientPhotoURL;
		[JsonProperty(PropertyName = "recipientphotourl")]
		public string RecipientPhotoURL { get{return _RecipientPhotoURL; } set{if (value != _RecipientPhotoURL) {
					_RecipientPhotoURL = value; NotifyPropertyChanged ();
				} } }

		private string _MessageType;
		[JsonProperty(PropertyName = "messagetype")]
		public string MessageType { get{return _MessageType; } set{if (value != _MessageType) {
					_MessageType = value; NotifyPropertyChanged ();
				} } }

		private string _Title;
		[JsonProperty(PropertyName = "title")]
		public string Title { get{return _Title; } set{if (value != _Title) {
					_Title = value; NotifyPropertyChanged ();
				} } }

		private string _RecipientString;
		[JsonProperty(PropertyName = "recipientstring")]
		public string RecipientString { get{return _RecipientString; } set{if (value != _RecipientString) {
					_RecipientString = value; NotifyPropertyChanged ();
				} } }

		private string _Message;
		[JsonProperty(PropertyName = "message")]
		public string Message { get{return _Message; } set{if (value != _Message) {
					_Message = value; NotifyPropertyChanged ();
				} } }

		private string _Route;
		[JsonProperty(PropertyName = "route")]
		public string Route { get{return _Route; } set{if (value != _Route) {
					_Route = value; NotifyPropertyChanged ();
				} } }

		private string _Status;
		[JsonProperty(PropertyName = "status")]
		public string Status { get{return _Status; } set{if (value != _Status) {
					_Status = value; NotifyPropertyChanged ();
				} } }

		private string _Link;
		[JsonProperty(PropertyName = "link")]
		public string Link { get{return _Link; } set{if (value != _Link) {
					_Link = value; NotifyPropertyChanged ();
				} } }


		private string _LinkDetail;
		[JsonProperty(PropertyName = "linkdetail")]
		public string LinkDetail { get{return _LinkDetail; } set{if (value != _LinkDetail) {
					_LinkDetail = value; NotifyPropertyChanged ();
				} } }

		private Today _messageToday;
		public Today MessageToday
		{get{ return _messageToday; }
			set{ 
				_messageToday = value; 

				//set the scheduledate to this date but UTC
				ScheduleDate = _messageToday.AtWhen;

				NotifyPropertyChanged (); 
			}
		}


		public DateTime ScheduleDate { 
			get 
			{
				//not clear why we shouldn't bring this back down to localtime
				return ScheduleDateUTC; 
			} 
			set
			{
				if (value != _scheduleDateUTC) {
					ScheduleDateUTC = value.ToUniversalTime();
					NotifyPropertyChanged ();
				}
			} 
		}

		private DateTime _scheduleDateUTC;
		[JsonProperty(PropertyName = "scheduledate")]
		public DateTime ScheduleDateUTC { 
			get 
			{
				return _scheduleDateUTC; 
			} 
			set
			{
				if (value != _scheduleDateUTC) {
					_scheduleDateUTC = value;
					NotifyPropertyChanged ();
				}
			} 
		}


		private bool _isActionable;
		public bool IsActionable
		{
			get {
				return _isActionable;
			}
			set{
				_isActionable = value;
				NotifyPropertyChanged ();
			}
		}


	}
}

