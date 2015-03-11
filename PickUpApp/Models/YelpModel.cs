using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace PickUpApp
{

	public class YelpResponse: BaseModel
	{

		private ObservableCollection<YelpModel> _businesses;
		[JsonProperty(PropertyName = "businesses")]
		public ObservableCollection<YelpModel>Businesses
		{
			get { return _businesses; }
			set{
				_businesses = value;
				NotifyPropertyChanged ();
			}
		}

	}

	public class YelpModel : BaseModel
	{
		private string _id;
		[JsonProperty(PropertyName = "id")]
		public string ID { get{return _id; } set{if (value != _id) {
					_id = value; NotifyPropertyChanged ();
				} } }

		private string _name;
		[JsonProperty(PropertyName = "name")]
		public string Name { get{return _name; } set{if (value != _name) {
					_name = value; NotifyPropertyChanged ();
				} } }

		private string _snippet;
		[JsonProperty(PropertyName = "snippet_text")]
		public string Snippet { get{return _snippet; } set{if (value != _snippet) {
					_snippet = value; NotifyPropertyChanged ();
				} } }

		private bool _isClosed;
		[JsonProperty(PropertyName = "is_closed")]
		public bool IsClosed { get{return _isClosed; } set{if (value != _isClosed) {
					_isClosed = value; NotifyPropertyChanged ();
				} } }

		private string _phone;
		[JsonProperty(PropertyName = "display_phone")]
		public string Phone { get{return _phone; } set{if (value != _phone) {
					_phone = value; NotifyPropertyChanged ();
				} } }

		private double _distance;
		[JsonProperty(PropertyName = "distance")]
		public double Distance { 
			get{return _distance/ 1609.344; } //to get miles from meters
			set{if (value != _distance) {
					_distance = value; NotifyPropertyChanged ();
				} } }

		private decimal _rating;
		[JsonProperty(PropertyName = "rating")]
		public decimal Rating { get{return _rating; } set{if (value != _rating) {
					_rating = value; NotifyPropertyChanged ();
				} } }

		private string _starsURL;
		[JsonProperty(PropertyName = "rating_img_url_large")]
		public string StarsURL { get{return _starsURL; } set{if (value != _starsURL) {
					_starsURL = value; NotifyPropertyChanged ();
				} } }

		private string _mobileURL;
		[JsonProperty(PropertyName = "mobile_url")]
		public string MobileURL { get{return _mobileURL; } set{if (value != _mobileURL) {
					_mobileURL = value; NotifyPropertyChanged ();
				} } }
	}
}

