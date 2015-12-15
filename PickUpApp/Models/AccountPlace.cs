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

		private string _phone;
		[JsonProperty(PropertyName = "phone")]
		public string Phone
		{
			get{
				return _phone;
			}
			set{
				if (value != _phone) {
					_phone = value;
					NotifyPropertyChanged ();
				}
			}
		}

		private string _notes;
		[JsonProperty(PropertyName = "notes")]
		public string Notes
		{
			get{
				return _notes;
			}
			set{
				if (value != _notes) {
					_notes = value;
					NotifyPropertyChanged ();
				}
			}
		}

		private string _via;
		[JsonProperty(PropertyName = "via")]
		public string Via
		{
			get{
				return _via;
			}
			set{
				if (value != _via) {
					_via = value;
					NotifyPropertyChanged ();
				}
			}
		}
		public bool ShouldSerializeVia()
		{
			return false;
		}

		[JsonIgnore]
		private bool _selected;
		public bool Selected
		{
			get{
				return _selected;
			}
			set{
				_selected = value;
				NotifyPropertyChanged ("Selected");
			}
		}

	}
}

