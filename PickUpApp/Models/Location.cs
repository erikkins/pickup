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
	}
}

