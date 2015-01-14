using System;

namespace PickUpApp
{
	public class Location:BaseModel
	{
		public string FullAddress{get;set;}
		public string Name {get;set;}
		public string Latitude {get;set;}
		public string Longitude {get;set;}
	}
}

