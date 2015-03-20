using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace PickUpApp
{
	public class GoogleResponse :BaseModel
	{
		private ObservableCollection<GoogleResult> _results;
		[JsonProperty(PropertyName = "results")]
		public ObservableCollection<GoogleResult>Results
		{
			get { return _results; }
			set{
				_results = value;
				NotifyPropertyChanged ();
			}
		}

	}

	public class GoogleResult :BaseModel
	{
		private GoogleLocation _geometry;
		[JsonProperty(PropertyName = "geometry")]
		public GoogleLocation Geometry { get{return _geometry; } set{if (value != _geometry) {
					_geometry = value; NotifyPropertyChanged ();
				} } }

		private string _name;
		[JsonProperty(PropertyName = "name")]
		public string Name { get{return _name; } set{if (value != _name) {
					_name = value; NotifyPropertyChanged ();
				} } }

		private string _placeid;
		[JsonProperty(PropertyName = "place_id")]
		public string PlaceID { get{return _placeid; } set{if (value != _placeid) {
					_placeid = value; NotifyPropertyChanged ();
				} } }

		private string _formaddress;
		[JsonProperty(PropertyName = "formatted_address")]
		public string Address { get{return _formaddress; } set{if (value != _formaddress) {
					_formaddress = value; NotifyPropertyChanged ();
				} } }

	}
}

