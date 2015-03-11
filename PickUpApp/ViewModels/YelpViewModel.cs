using System;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using PickUpApp.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using RestSharp.Portable;
using RestSharp.Portable.Authenticators;

namespace PickUpApp
{
	public class YelpViewModel:BaseViewModel
	{
		private ObservableCollection<YelpModel> _yelpers;
		public ObservableCollection<YelpModel>YelpBusinesses 
		{ 
			get{
				return _yelpers;
			}
			set
			{
				_yelpers = value;
				NotifyPropertyChanged ();
				}
		}
		private double _latitude, _longitude;

		public YelpViewModel()
		{
			//YelpBusinesses = new ObservableCollection<YelpModel> ();
		}

		public YelpViewModel (MobileServiceClient client, double latitude, double longitude) : this()
		{
			this.client = client;
			_latitude = latitude;
			_longitude = longitude;
			LoadItemsCommand.Execute (null);
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try {
				using (var client = new RestClient(new Uri("http://api.yelp.com/v2/")))
				{
					var request = new RestRequest("search", System.Net.Http.HttpMethod.Get);	
					client.Authenticator = OAuth1Authenticator.ForProtectedResource ("ntqQRQUmuzfyWIVma9vvSA", "Fdmh-EDE78avQAhUsbX8phjOYJE", "pBOuhp9FO6UFuQ_Pr4w9vfzx_M9kc_cS", "HGyA9x5e-CO1RjCcPjlMJW0oWiA");				
					request.AddQueryParameter ("term", "kids");
					request.AddQueryParameter("limit", "20");
					request.AddQueryParameter("category_filter", "parks,icecream,restaurants,kids_activities,zoos");
					//request.AddQueryParameter("location", ThisInvite.Address);
					request.AddQueryParameter("ll", _latitude + "," + _longitude);

					var result = await client.Execute(request);
					YelpResponse yr = Newtonsoft.Json.JsonConvert.DeserializeObject<YelpResponse>(System.Text.Encoding.UTF8.GetString(result.RawBytes, 0, result.RawBytes.Length));
					YelpBusinesses = yr.Businesses;

					//var yelpresponse = Newtonsoft.Json.Linq.JObject.Parse (System.Text.Encoding.UTF8.GetString(result.RawBytes, 0, result.RawBytes.Length));
					//System.Diagnostics.Debug.WriteLine(result);
				}

			} 
			catch (Exception ex) 
			{
				System.Diagnostics.Debug.WriteLine (ex.Message);	
			}
		
		}
	}
}

