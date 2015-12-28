using System;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PickUpApp
{
	public class Account:BaseModel
	{
		private string _Id;
		[JsonProperty(PropertyName = "id")]
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

		public string Fullname
		{
			get { return _Firstname + " " + _Lastname; }
		}
		private bool _selected;
		public bool Selected
		{
			get { return _selected; }
			set { _selected = value; }
		}
		private string _photoURL;
		[JsonProperty(PropertyName = "photourl")]
		public string PhotoURL
		{
			get{
				if (_photoURL == null) {
					if (Firstname == null && Lastname == null) {
						return null;
					}
					string initials = "";
					if (Firstname == null)
					{
						initials = Lastname.Substring(0,1).ToUpper();
					}
					else if (Lastname == null)
					{
						initials = Firstname.Substring(0,1).ToUpper();
					}
					else
					{
						initials = Firstname.Substring(0,1).ToUpper() + Lastname.Substring(0,1).ToUpper();
					}

					var dep = DependencyService.Get<PickUpApp.ICircleText>();
					string filename = dep.CreateCircleText(initials,50,50);
					return filename;
				} else {
					return _photoURL;
				}

			}
			set{
				if (value != _photoURL) {
					_photoURL = value; NotifyPropertyChanged ();
				}
			}
		}

		private bool _validated;
		[JsonProperty(PropertyName = "validated")]
		public bool Validated
		{
			get { return _validated; }
			set { _validated = value; NotifyPropertyChanged (); }
		}



	}
}

