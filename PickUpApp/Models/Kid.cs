using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class Kid: BaseModel
	{
		private string _Id;
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
	}
}

//account F722D8D8-3B5A-4C62-A22B-AA776841BB38
//kid BFC0E504-A2A4-41F6-8899-2CCE71F04238