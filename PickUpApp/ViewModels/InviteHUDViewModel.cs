using System;
using System.ComponentModel;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;



namespace PickUpApp
{
	public class InviteHUDViewModel:BaseViewModel
	{
		

		private string _countdown;
		public string Countdown
		{
			get{ return _countdown; }set{ _countdown = value; NotifyPropertyChanged ();}
		}

		private string _trafficTime;
		public string TrafficTime
		{
			get{ return _trafficTime; }
			set{ _trafficTime = value; NotifyPropertyChanged ();}
		}

		private InviteInfo _thisInvite;
		public InviteInfo ThisInvite
		{
			get{
				return _thisInvite;
			}
			set{
				if (value != _thisInvite) {
					_thisInvite = value;
					NotifyPropertyChanged ();
				}
			}
		}

		public InviteHUDViewModel ()
		{

		}
		public InviteHUDViewModel(MobileServiceClient client, InviteInfo invite):this(){
			this.client = client;
			ThisInvite = invite;
		}


		private Command updateTrafficCommand;
		public Command UpdateTrafficCommand
		{
			get { return updateTrafficCommand ?? (updateTrafficCommand = new Command(async () => await UpdateTraffic())); }
		}
		public async Task UpdateTraffic()
		{
			
			await App.GetPosition();
			IsLoading = true;
			try{
			PortableRest.RestRequest req = new PortableRest.RestRequest ("Routes", System.Net.Http.HttpMethod.Get);
			req.AddQueryString ("wp.0", App.PositionLatitude + "," + App.PositionLongitude);
			req.AddQueryString ("wp.1", _thisInvite.Latitude + "," + _thisInvite.Longitude);
			req.AddQueryString ("du", "mi");
			req.AddQueryString ("avoid", "minimizeTolls");
			req.AddQueryString ("key", "AqXf-x5KdOluBQB35EjKT3owEzBLbfUqetvc0rPZ7xAbW_EKMsZ0RB0IYWkypdwH");


			PortableRest.RestClient rc = new PortableRest.RestClient ();			
			rc.UserAgent = "PickUp";
			rc.BaseUrl = "http://dev.virtualearth.net/REST/V1/";
			PortableRest.RestResponse<string> resp = await rc.SendAsync<string>(req, default(System.Threading.CancellationToken));
			//var bingresponse = Newtonsoft.Json.Linq.JObject.Parse (resp.Content);

			BingResponse br = Newtonsoft.Json.JsonConvert.DeserializeObject<BingResponse>(resp.Content);
			decimal min = br.ResourceSets[0].TripResources[0].TravelDurationTraffic/60;

			resp.Dispose();
			resp = null;
			req = null;



			//ok, we get total seconds, so we need to divide by 60 to get minutes
			//decimal min = decimal.Parse(bingresponse ["resourceSets"] [0] ["resources"] [0] ["travelDurationTraffic"].ToString())/60;
			//bingresponse.RemoveAll();
			//bingresponse = null;
			

			TrafficTime = string.Format("{0:N1}", min);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("BingError " + ex.Message);
			}
			finally
			{
			
			IsLoading = false;
			}
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			try
			{

//				var request = WebRequest.Create(uriBuilder.ToString());
//				request.Method = "GET";
//
//				request.SignRequest(
//					new Tokens {
//						ConsumerKey = CONSUMER_KEY,
//						ConsumerSecret = CONSUMER_SECRET,
//						AccessToken = TOKEN,
//						AccessTokenSecret = TOKEN_SECRET
//					}
//				).WithEncryption(EncryptionMethod.HMACSHA1).InHeader();
//
//				HttpWebResponse response = (HttpWebResponse)request.GetResponse();



				//RestRequest req = new RestRequest ("Search", System.Net.Http.HttpMethod.Get);

				//req.AddQueryParameter ("location", "evanston il");
				//req.AddQueryParameter ("term", "food");



				//
				//auth.Authenticate (rc, req);

				//IRestResponse resp = rc.Execute (req).ConfigureAwait(false).GetAwaiter().GetResult();


				//not sure we need this...hopefully this will be preloaded in the Today dropdown.
				//can we mix metaphors???

				//var confirmData = await client.GetTable<Schedule>().ToListAsync();
				//				var kids = await client.GetTable<Kid>().ToListAsync();
				//
				//				App.myKids.Clear();
				//				foreach (var kid in kids)
				//				{
				//					App.myKids.Add(kid);
				//				}

			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Kids. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine ("WEIRD" + ex.Message + result.Status.ToString ());
			}
		}

		public override async Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				Invite i = new Invite()
				{
					Id = _thisInvite.Id
				};
				var completedata = await client.InvokeApiAsync<Invite, EmptyClass>("completepickup",i);
				//System.Diagnostics.Debug.WriteLine("");
				MessagingCenter.Send<Invite>(i, "Completed");

			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}
			finally{
				IsLoading = false;
			}

			IsLoading = false;  //redundant
		}

	}
}

