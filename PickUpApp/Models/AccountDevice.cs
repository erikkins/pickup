using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class AccountDevice:BaseModel
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

		private string _userId;
		[JsonProperty(PropertyName = "userid")]
		public string userId
		{
			get{
				return _userId;
			}
			set{
				if (value != _userId) {
					_userId = value;
					NotifyPropertyChanged ();
				}
			}
		}

		private string _accountID;
		[JsonProperty(PropertyName = "accountid")]
		public string accountid
		{
			get{
				return _accountID;
			}
			set{
				if (value != _accountID) {
					_accountID = value;
					NotifyPropertyChanged ();
				}
			}
		}			

		private string _notificationID;
		[JsonProperty(PropertyName = "notificationid")]
		public string notificationid
		{
			get{
				return _notificationID;
			}
			set{
				if (value != _notificationID) {
					_notificationID = value;
					NotifyPropertyChanged ();
				}
			}
		}
	}
}

