using System;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PickUpApp
{
	public class Kid: BaseModel
	{
		private string _Id;
		[JsonProperty(PropertyName = "id")]
		public string Id 
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

		public string Fullname {
			get{ return _Firstname + " " + _Lastname; }		
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

		private bool _selected;
		public bool Selected {get { return _selected; }
			set {
				if (value != _selected) {
					_selected = value;
					NotifyPropertyChanged ();
				}
			}
		}

		private string _photoURL;
		[JsonProperty(PropertyName = "photourl")]
		public string PhotoURL
		{
			get{

				if (_photoURL == null) {
					//generate one on the fly...
					string initials = "";
					if (Firstname == null && Lastname == null) {
						//sol
						return "";
					}
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
				}

				return _photoURL;
			}
			set{
				if (value != _photoURL) {
					_photoURL = value; NotifyPropertyChanged ();
				}
			}
		}


		public string Age
		{
			get{
				DateTime today = DateTime.Now;

				return (((today.Year - _dob.Year) * 372 + (today.Month - _dob.Month) * 31 + (today.Day - _dob.Day)) / 372).ToString();
			}
		}

		private DateTime _dob;
		[JsonProperty(PropertyName = "dob")]
		public DateTime DateOfBirth
		{
			get{
				if (_dob == DateTime.MinValue) {
					_dob = new DateTime (DateTime.Now.AddYears(-5).Year, 6, 15);
				}
				return _dob;
			}
			set{
				if (value != _dob) {
					_dob = value; NotifyPropertyChanged ();
				}
			}
		}

		private string _allergies;
		[JsonProperty(PropertyName = "allergies")]
		public string Allergies
		{
			get{
				return _allergies;
			}
			set{
				if (value != _allergies) {
					_allergies = value; NotifyPropertyChanged ();
				}
			}
		}

		private string _gender;
		[JsonProperty(PropertyName = "gender")]
		public string Gender
		{
			get{
				if (string.IsNullOrEmpty (_gender)) {
					_gender = "Unknown";
				}
				return _gender;
			}
			set{
				if (value != _gender) {
					_gender = value; NotifyPropertyChanged ();
				}
			}
		}

		private string _secretword;
		[JsonProperty(PropertyName = "secretword")]
		public string SecretWord
		{
			get{
				if (string.IsNullOrEmpty (_secretword)) {
					_secretword = "";
				}
				return _secretword;
			}
			set{
				if (value != _secretword) {
					_secretword = value; NotifyPropertyChanged ();
				}
			}
		}



		private string _via;
		[JsonProperty(PropertyName = "via")]
		public string Via
		{
			get{
				return _via;
			}
			set{
				if (value != _via) {
					_via = value; NotifyPropertyChanged ();
				}
			}
		}
		public bool ShouldSerializeVia()
		{
			return false;
		}

		private bool _mine;
		public bool Mine {get { return _mine; }
			set {
				if (value != _mine) {
					_mine = value;
					NotifyPropertyChanged ();
				}
			}
		}

	}
}

//account F722D8D8-3B5A-4C62-A22B-AA776841BB38
//kid BFC0E504-A2A4-41F6-8899-2CCE71F04238