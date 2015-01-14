using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class Account:BaseModel
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

		private string _Firstname;
		[JsonProperty(PropertyName = "firstname")]
		public string Firstname { get{return _Firstname; } set{if (value != _Firstname) {
					_Firstname = value; NotifyPropertyChanged ();
				} } }

		private string _Lastname;
		[JsonProperty(PropertyName = "lastname")]
		public string Lastname { get{return _Lastname; } set{if (value != _Lastname) {
					_Lastname = value; NotifyPropertyChanged ();
				} } }

		private string _Phone;
		[JsonProperty(PropertyName = "phone")]
		public string Phone { get{return _Phone; } set{if (value != _Phone) {
					_Phone = value; NotifyPropertyChanged ();
				} } }

		private string _Email;
		[JsonProperty(PropertyName = "email")]
		public string Email { get{return _Email; } set{if (value != _Email) {
					_Email = value; NotifyPropertyChanged ();
				} } }

		private string _UserId;
		[JsonProperty(PropertyName = "userId")]
		public string UserId { get{return _UserId; } set{if (value != _UserId) {
					_UserId = value; NotifyPropertyChanged ();
				} } }

		private string _Gender;
		[JsonProperty(PropertyName = "gender")]
		public string Gender { get{return _Gender; } set{if (value != _Gender) {
					_Gender = value; NotifyPropertyChanged ();
				} } }

		private string _Locale;
		[JsonProperty(PropertyName = "locale")]
		public string Locale { get{return _Locale; } set{if (value != _Locale) {
					_Locale = value; NotifyPropertyChanged ();
				} } }

		private string _Timezone;
		[JsonProperty(PropertyName = "timezone")]
		public string Timezone { get{return _Timezone; } set{if (value != _Timezone) {
					_Timezone = value; NotifyPropertyChanged ();
				} } }
	}
}

