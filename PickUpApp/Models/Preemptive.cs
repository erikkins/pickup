using System;
using Newtonsoft.Json;
using System.Linq;

namespace PickUpApp
{
	
	public class Preemptive : BaseModel
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

		//this is the input param (e.g. MTuWThFSaSu)
		private string _days;
		[JsonProperty(PropertyName = "days")]
		public string Days { get{ return _days; } set{if (value != _days) {
					_days = value; NotifyPropertyChanged ();} } }



		public DateTime PickupDiff
		{
			get{
				return DateTime.Today.Add (TSPickup.Subtract (TimeSpan.FromMinutes (EndPlaceTravelTime)));
			}
		}

		public DateTime DropoffDiff
		{
			get{
				return DateTime.Today.Add (TSDropOff.Subtract (TimeSpan.FromMinutes (StartPlaceTravelTime)));
			}
		}


		private long _dropoffticks;
		[JsonProperty(PropertyName = "dropoffticks")]
		public long DropOffTicks
		{ get
			{ return _dropoffticks; } 
			set
			{
				if (value != _dropoffticks) {
					_dropoffticks = value;
					NotifyPropertyChanged ();
				}
			} 
		}

		[JsonIgnore]
		public TimeSpan TSDropOff
		{ 
			get
			{ 
				//convert from UTC
				DateTime dtUTC = new DateTime(_dropoffticks).AddDays(1);
				DateTime dt = dtUTC.ToLocalTime ();
				TimeSpan ts = dt.TimeOfDay;
				return ts;

				//orig
				//return TimeSpan.FromTicks(_dropoffticks); 
			} 
			set
			{
				if (value.Ticks != _dropoffticks) {	

					//convert to UTC
					DateTime dt = new DateTime(value.Ticks).AddDays(1);
					DateTime dtUTC = dt.ToUniversalTime ();
					TimeSpan tsUTC = dtUTC.TimeOfDay;
					_dropoffticks = tsUTC.Ticks;

					//ORIG
					//_dropoffticks = value.Ticks;
					NotifyPropertyChanged ();
				}
			} 
		}

		private long _pickupticks;
		[JsonProperty(PropertyName = "pickupticks")]
		public long PickupTicks
		{ get
			{ return _pickupticks; } 
			set
			{
				if (value != _pickupticks) {
					_pickupticks = value;
					NotifyPropertyChanged ();
				}
			} 
		}
			

		[JsonIgnore]
		public TimeSpan TSPickup
		{ 
			get
			{
				//convert from UTC
				DateTime dtUTC = new DateTime(_pickupticks).AddDays(1);
				DateTime dt = dtUTC.ToLocalTime ();
				TimeSpan ts = dt.TimeOfDay;
				return ts;

				//orig
				//return TimeSpan.FromTicks(_pickupticks); 
			} 
			set
			{
				if (value.Ticks != _pickupticks) {

					//convert to UTC
					DateTime dt = new DateTime(value.Ticks).AddDays(1);
					DateTime dtUTC = dt.ToUniversalTime ();
					TimeSpan tsUTC = dtUTC.TimeOfDay;
					_pickupticks = tsUTC.Ticks;

					//orig
					//_pickupticks = value.Ticks;
					NotifyPropertyChanged ();
				}
			} 
		}
			
		private string _Activity;
		[JsonProperty(PropertyName = "activity")]
		public string Activity { get{ return _Activity; } set{if (value != _Activity) {
					_Activity = value; NotifyPropertyChanged ();} } }

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

		private double _StartPlaceTravelTime;
		[JsonProperty(PropertyName = "startplacetraveltime")]
		public double StartPlaceTravelTime { get{return _StartPlaceTravelTime; } set{if (value != _StartPlaceTravelTime) {
					_StartPlaceTravelTime = value; NotifyPropertyChanged ();
				} } }


		private double _EndPlaceTravelTime;
		[JsonProperty(PropertyName = "endplacetraveltime")]
		public double EndPlaceTravelTime { get{return _EndPlaceTravelTime; } set{if (value != _EndPlaceTravelTime) {
					_EndPlaceTravelTime = value; NotifyPropertyChanged ();
				} } }
	}
}

