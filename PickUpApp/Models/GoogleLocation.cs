using System;
using Newtonsoft.Json;

namespace PickUpApp
{

	public class GoogleLocation: BaseModel
	{
		private GoogleLatLong _latlong;
		[JsonProperty(PropertyName = "location")]
		public GoogleLatLong Location { get{return _latlong; } set{if (value != _latlong) {
					_latlong = value; NotifyPropertyChanged ();
				} } }
	}
	public class GoogleLatLong:BaseModel
	{
		private double _lat;
		[JsonProperty(PropertyName = "lat")]
		public double Latitude { get{return _lat; } set{if (value != _lat) {
					_lat = value; NotifyPropertyChanged ();
				} } }

		private double _lng;
		[JsonProperty(PropertyName = "lng")]
		public double Longitude { get{return _lng; } set{if (value != _lng) {
					_lng = value; NotifyPropertyChanged ();
				} } }
		

	}
}

