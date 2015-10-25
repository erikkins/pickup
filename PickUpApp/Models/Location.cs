using System;

namespace PickUpApp
{
	public class Location:BaseModel
	{
		private string _address;
		public string FullAddress{
			get{ return _address; }
			set{ _address = value; NotifyPropertyChanged ("FullAddress"); }
		}

		private string _name;
		public string Name {
			get{ return _name; }
			set{ _name = value; NotifyPropertyChanged ("Name"); }
		}

		public string Latitude {get;set;}
		public string Longitude {get;set;}

		//this is for AccountPlace compat
		private string _phone;
		public string Phone {
			get{ return _phone; }
			set{ _phone = value; NotifyPropertyChanged ("Phone"); }
		}

		private string _notes;
		public string Notes {
			get{ return _notes; }
			set{ _notes = value; NotifyPropertyChanged ("Notes"); }
		}
	}
}

