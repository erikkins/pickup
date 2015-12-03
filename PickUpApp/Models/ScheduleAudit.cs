using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class ScheduleAudit:BaseModel
	{

		private string _scheduleid;
		[JsonProperty(PropertyName = "scheduleid")]
		public string ScheduleID { get{ return _scheduleid; } set{if (value != _scheduleid) {
					_scheduleid = value; NotifyPropertyChanged ();} } }


		private DateTimeOffset _scheduledate;
		[JsonProperty(PropertyName = "scheduledate")]
		public DateTimeOffset ScheduleDate { get{ return _scheduledate; } set{if (value != _scheduledate) {
					_scheduledate = value; NotifyPropertyChanged ();} } }

		private string _dropoffuserid;
		[JsonProperty(PropertyName = "dropoffuserid")]
		public string DropoffUserID { get{ return _dropoffuserid; } set{if (value != _dropoffuserid) {
					_dropoffuserid = value; NotifyPropertyChanged ();} } }
		
		private bool _dropoffcomplete;
		[JsonProperty(PropertyName = "dropoffcomplete")]
		public bool DropoffComplete { get{ return _dropoffcomplete; } set{if (value != _dropoffcomplete) {
					_dropoffcomplete = value; NotifyPropertyChanged ();} } }

		private DateTime _dropoffcompleteatwhen;
		[JsonProperty(PropertyName = "dropoffcompleteatwhen")]
		public DateTime DropoffCompleteAtWhen { get{ return _dropoffcompleteatwhen; } set{if (value != _dropoffcompleteatwhen) {
					_dropoffcompleteatwhen = value; NotifyPropertyChanged ();} } }

		private string _pickupuserid;
		[JsonProperty(PropertyName = "pickupuserid")]
		public string PickupUserID { get{ return _pickupuserid; } set{if (value != _pickupuserid) {
					_pickupuserid = value; NotifyPropertyChanged ();} } }

		private bool _pickupcomplete;
		[JsonProperty(PropertyName = "pickupcomplete")]
		public bool PickupComplete { get{ return _pickupcomplete; } set{if (value != _pickupcomplete) {
					_pickupcomplete = value; NotifyPropertyChanged ();} } }

		private DateTime _pickupcompleteatwhen;
		[JsonProperty(PropertyName = "pickupcompleteatwhen")]
		public DateTime PickupCompleteAtWhen { get{ return _pickupcompleteatwhen; } set{if (value != _pickupcompleteatwhen) {
					_pickupcompleteatwhen = value; NotifyPropertyChanged ();} } }

		private string _dropoffmessageid;
		[JsonProperty(PropertyName = "dropoffmessageid")]
		public string DropoffMessageID { get{ return _dropoffmessageid; } set{if (value != _dropoffmessageid) {
					_dropoffmessageid = value; NotifyPropertyChanged ();} } }

		private string _dropoffmessagestatus;
		[JsonProperty(PropertyName = "dropoffmessagestatus")]
		public string DropoffMessageStatus { get{ return _dropoffmessagestatus; } set{if (value != _dropoffmessagestatus) {
					_dropoffmessagestatus = value; NotifyPropertyChanged ();} } }

		private string _pickupmessageid;
		[JsonProperty(PropertyName = "pickupmessageid")]
		public string PickupMessageID { get{ return _pickupmessageid; } set{if (value != _pickupmessageid) {
					_pickupmessageid = value; NotifyPropertyChanged ();} } }

		private string _pickupmessagestatus;
		[JsonProperty(PropertyName = "pickupmessagestatus")]
		public string PickupMessageStatus { get{ return _pickupmessagestatus; } set{if (value != _pickupmessagestatus) {
					_pickupmessagestatus = value; NotifyPropertyChanged ();} } }

	}
}

