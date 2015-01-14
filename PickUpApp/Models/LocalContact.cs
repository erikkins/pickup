using System;

namespace PickUpApp
{
	public class LocalContact:BaseModel
	{
		public string Id { get; set; }
		public string DisplayName{ get; set; }
		public string PhotoId { get; set; }
		public string FirstName {get;set;}
		public string LastName {get;set;}
		public string Email {get;set;}
		public string Phone {get;set;} 
		public string NameSort
		{
			get
			{
				if (string.IsNullOrWhiteSpace(DisplayName) || DisplayName.Length == 0)
					return "?";

				return DisplayName[0].ToString().ToUpper();
			}
		}

	}
}