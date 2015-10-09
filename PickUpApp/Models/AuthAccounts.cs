using System;
using Newtonsoft.Json;

namespace PickUpApp
{
	public class AuthAccounts:BaseModel
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

		private string _Username;
		[JsonProperty(PropertyName = "username")]
		public string Username { get{return _Username; } set{if (value != _Username) {
					_Username = value; NotifyPropertyChanged ();
				} } }


		private string _Password;
		[JsonProperty(PropertyName = "password")]
		public string Password { get{return _Password; } set{if (value != _Password) {
					_Password = value; NotifyPropertyChanged ();
				} } }

		private string _Email;
		[JsonProperty(PropertyName = "email")]
		public string Email { get{return _Email; } set{if (value != _Email) {
					_Email = value; NotifyPropertyChanged ();
				} } }

		private string _UserID;
		[JsonProperty(PropertyName = "userId")]
		public string UserID { get{return _UserID; } set{if (value != _UserID) {
					_UserID = value; NotifyPropertyChanged ();
				} } }

		private string _Token;
		[JsonProperty(PropertyName = "token")]
		public string Token { get{return _Token; } set{if (value != _Token) {
					_Token = value; NotifyPropertyChanged ();
				} } }
					
	}
}

