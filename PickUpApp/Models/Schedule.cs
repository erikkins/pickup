using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class Schedule:BaseModel
	{
		public const string ADDRESS_PLACEHOLDER = "Click to set address";

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


		private DateTime _AtWhen;
		[JsonProperty(PropertyName = "atwhen")]
		public DateTime AtWhen 
		{ get
			{ return _AtWhen; } 
			set
			{
				if (value != _AtWhen) {
					_AtWhen = value;
					NotifyPropertyChanged ();
				}
			} 
		}



		private string _Activity;
		[JsonProperty(PropertyName = "activity")]
		public string Activity { get{ return _Activity; } set{if (value != _Activity) {
					_Activity = value; NotifyPropertyChanged ();} } }

		private bool _Recurring;
		[JsonProperty(PropertyName = "recurring")]
		public bool Recurring { get{return _Recurring; } set{if (value != _Recurring) {
					_Recurring = value; NotifyPropertyChanged ();
				} } }

		private string _Frequency;
		[JsonProperty(PropertyName = "frequency")]
		public string Frequency { get{ return _Frequency;} set{if (value != _Frequency) {
					_Frequency= value; NotifyPropertyChanged ();} } }

		private string _Latitude;
		[JsonProperty(PropertyName = "latitude")]
		public string Latitude { get{return _Latitude; } set{if (value != _Latitude) {
					_Latitude = value; NotifyPropertyChanged ();
				} } }

		private string _Longitude;
		[JsonProperty(PropertyName = "longitude")]
		public string Longitude { get{return _Longitude; } set{if (value != _Longitude) {
					_Longitude = value; NotifyPropertyChanged ();
				} } }

		private string _Address;
		[JsonProperty(PropertyName = "address")]
		public string Address 
		{ 
			get
			{ 
				if (string.IsNullOrEmpty (_Address)) {
					return ADDRESS_PLACEHOLDER;
				} else {
					//strip out CRLF?
					return _Address.Replace (Environment.NewLine, "  ");
					//return _Address;
				}
			} 
			set
			{
				if (value != _Address)
				{
					_Address = value;
					NotifyPropertyChanged();
				}
			}
		}

		private DateTime _AtWhenEnd;
		[JsonProperty(PropertyName = "atwhenend")]
		public DateTime AtWhenEnd { get{ return _AtWhenEnd; } set
			{
				if (value != _AtWhenEnd) {
					_AtWhenEnd = value;
					NotifyPropertyChanged ();
				}
			} 
		}

		private string _Location;
		[JsonProperty(PropertyName = "location")]
		public string Location { get{return _Location; } set{if (value != _Location) {
					_Location = value; NotifyPropertyChanged ();
				} } }

		private string _UserId;
		[JsonProperty(PropertyName = "userId")]
		public string UserId { get{return _UserId; } set{if (value != _UserId) {
					_UserId = value; NotifyPropertyChanged ();
				} } }


		private long _StartTimeTicks;
		[JsonProperty(PropertyName = "startTimeTicks")]
		public long StartTimeTicks
		{
			get{
				return _StartTimeTicks;
			}
			set{
				if (value != _StartTimeTicks) {
					_StartTimeTicks = value;
					NotifyPropertyChanged ();
				}
			}
		}
		private long _EndTimeTicks;
		[JsonProperty(PropertyName = "endTimeTicks")]
		public long EndTimeTicks
		{
			get{
				return _EndTimeTicks;
			}
			set{
				if (value != _EndTimeTicks) {
					_EndTimeTicks = value;
					NotifyPropertyChanged ();
				}
			}
		}

		#region non-serializable
		[JsonIgnore]
		public TimeSpan StartTime 
		{ get
			{ return TimeSpan.FromTicks (StartTimeTicks);
			}
			set
			{
				if (value != TimeSpan.FromTicks(StartTimeTicks)) {
					//AtWhen += value;
					StartTimeTicks = value.Ticks;
					NotifyPropertyChanged ();
				}}

		}

		[JsonIgnore]
		public TimeSpan EndTime {
			get
			{ 
				return TimeSpan.FromTicks (EndTimeTicks);
			} 
			set
			{
				if (value != TimeSpan.FromTicks(EndTimeTicks)) {
					//AtWhen += value;
					EndTimeTicks = value.Ticks;
					NotifyPropertyChanged ();
				}
			}
		}
		[JsonIgnore]
		public bool Monday
		{
			get{
				if (Frequency.Contains ("M")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("M", value);
				NotifyPropertyChanged ();
			}
		}
		[JsonIgnore]
		public bool Tuesday
		{
			get{
				if (Frequency.Contains ("T")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("T", value);
				NotifyPropertyChanged ();
			}
		}
		[JsonIgnore]
		public bool Wednesday
		{
			get{
				if (Frequency.Contains ("W")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("W", value);
				NotifyPropertyChanged ();
			}
		}
		[JsonIgnore]
		public bool Thursday
		{
			get{
				if (Frequency.Contains ("Th")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("Th", value);
				NotifyPropertyChanged ();
			}
		}
		[JsonIgnore]
		public bool Friday
		{
			get{
				if (Frequency.Contains ("F")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("F", value);
				NotifyPropertyChanged ();
			}
		}
		[JsonIgnore]
		public bool Saturday
		{
			get{
				if (Frequency.Contains ("Sa")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("Sa", value);
				NotifyPropertyChanged ();
			}
		}
		[JsonIgnore]
		public bool Sunday
		{
			get{
				if (Frequency.Contains ("Su")) {
					return true;
				} else {
					return false;
				}
			}
			set{
				setFrequency ("Su", value);
				NotifyPropertyChanged ();
			}
		}

		private void setFrequency (string day, bool include)
		{
			if (include)
			{
				//make sure the frequency has the value
				if (!Frequency.Contains(day))
				{
					Frequency += day;
				}
			}
			else{
				//make sure the frequency does NOT have the value
				if (Frequency.Contains(day))
				{
					Frequency = Frequency.Replace(day, "");
				}
			}
			Frequency = reorderFrequency ();
		}

		private string reorderFrequency()
		{
			//need to make sure it's in the correct MTWThFSaSu format
			string tempFreq = "";
			if (Monday) {
				tempFreq = "M";
			}
			if (Tuesday) {
				tempFreq += "T";
			}
			if (Wednesday) {
				tempFreq += "W";
			}
			if (Thursday) {
				tempFreq += "Th";
			}
			if (Friday) {
				tempFreq += "F";
			}
			if (Saturday) {
				tempFreq += "Sa";
			}
			if (Sunday) {
				tempFreq += "Su";
			}
			return tempFreq;
		}

		#endregion


	}
}

