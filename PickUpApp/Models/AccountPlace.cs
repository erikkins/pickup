using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class AccountPlace:BaseModel
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

		private string _placename;
		[JsonProperty(PropertyName = "placename")]
		public string PlaceName
		{
			get{
				return _placename;
			}
			set{
				if (value != _placename) {
					_placename = value;
					NotifyPropertyChanged ();
				}
			}
		}
		private string _latitude;
		[JsonProperty(PropertyName = "latitude")]
		public string Latitude
		{
			get{
				return _latitude;
			}
			set{
				if (value != _latitude) {
					_latitude = value;
					NotifyPropertyChanged ();
				}
			}
		}
		private string _longitude;
		[JsonProperty(PropertyName = "longitude")]
		public string Longitude
		{
			get{
				return _longitude;
			}
			set{
				if (value != _longitude) {
					_longitude = value;
					NotifyPropertyChanged ();
				}
			}
		}
		private string _address;
		[JsonProperty(PropertyName = "address")]
		public string Address
		{
			get{
				return _address;
			}
			set{
				if (value != _address) {
					_address = value;
					NotifyPropertyChanged ();
				}
			}
		}
	}
}

