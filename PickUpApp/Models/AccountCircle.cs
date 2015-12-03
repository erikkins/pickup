using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class AccountCircle : Account
	{

		private string _circleid;
		[JsonProperty(PropertyName = "circleid")]
		public string CircleID
		{
			get { return _circleid; }
			set { _circleid = value; NotifyPropertyChanged (); }
		}

		private bool _accepted;
		[JsonProperty(PropertyName = "accepted")]
		public bool Accepted
		{
			get { return _accepted; }
			set { _accepted = value; NotifyPropertyChanged (); }
		}

		private bool _coparent;
		[JsonProperty(PropertyName = "coparent")]
		public bool Coparent
		{
			get { return _coparent; }
			set { _coparent = value; NotifyPropertyChanged (); }
		}

		private string _via;
		[JsonProperty(PropertyName = "via")]
		public string Via
		{
			get { return _via; }
			set { _via = value; NotifyPropertyChanged (); }
		}
	}
}

