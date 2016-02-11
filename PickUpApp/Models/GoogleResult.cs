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

	public class GoogleDistanceResult:BaseModel
	{
		private string _status;
		[JsonProperty(PropertyName = "status")]
		public string Status { get{return _status; } set{if (value != _status) {
					_status = value; NotifyPropertyChanged ();
				} } }

		private List<string> _origin_addresses;
		[JsonProperty(PropertyName = "origin_addresses")]
		public List<string> OriginAddresses { get{return _origin_addresses; } set{if (value != _origin_addresses) {
					_origin_addresses = value; NotifyPropertyChanged ();
				} } }

		private List<string> _dest_addresses;
		[JsonProperty(PropertyName = "destination_addresses")]
		public List<string> DestinationAddresses { get{return _dest_addresses; } set{if (value != _dest_addresses) {
					_dest_addresses = value; NotifyPropertyChanged ();
				} } }

		private List<GoogleElements> _rows;
		[JsonProperty(PropertyName = "rows")]
		public List<GoogleElements> Rows
		{
			get{
				return _rows;
			}
			set{
				if (value != _rows) {
					_rows = value;
					NotifyPropertyChanged ();
				}
			}
		}


	}
		

	public class GoogleElements:BaseModel
	{
		private List<GoogleElement> _elements;
		[JsonProperty(PropertyName = "elements")]
		public List<GoogleElement>Elements
		{
			get{
				return _elements;
			}
			set{
				if (value != _elements)
				{
					_elements = value;
					NotifyPropertyChanged();
				}
			}
		}
	}

	public class GoogleElement: BaseModel
	{
		private string _status;
		[JsonProperty(PropertyName = "status")]
		public string Status { get{return _status; } set{if (value != _status) {
					_status = value; NotifyPropertyChanged ();
				} } }

		private GoogleDuration _duration;
		[JsonProperty(PropertyName = "duration")]
		public GoogleDuration Duration
		{
			get { return _duration; }
			set{
				if (value != _duration) {
					_duration = value;
					NotifyPropertyChanged ();
				}
			}
		}

		private GoogleDistance _distance;
		[JsonProperty(PropertyName = "distance")]
		public GoogleDistance Distance
		{
			get { return _distance; }
			set{
				if (value != _distance) {
					_distance = value;
					NotifyPropertyChanged ();
				}
			}
		}

	}

	public class GoogleDistance: BaseModel
	{
		private int _value;
		[JsonProperty(PropertyName = "value")]
		public int Value { get{return _value; } set{if (value != _value) {
					_value = value; NotifyPropertyChanged ();
				} } }

		private string _text;
		[JsonProperty(PropertyName = "text")]
		public string Text { get{return _text; } set{if (value != _text) {
					_text = value; NotifyPropertyChanged ();
				} } }
	}

	public class GoogleDuration: BaseModel
	{
		private int _value;
		[JsonProperty(PropertyName = "value")]
		public int Value { get{return _value; } set{if (value != _value) {
					_value = value; NotifyPropertyChanged ();
				} } }

		private string _text;
		[JsonProperty(PropertyName = "text")]
		public string Text { get{return _text; } set{if (value != _text) {
					_text = value; NotifyPropertyChanged ();
				} } }
	}
}

