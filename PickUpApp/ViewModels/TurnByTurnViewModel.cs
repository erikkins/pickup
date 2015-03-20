using System;
using System.ComponentModel;
using PickUpApp.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PickUpApp
{
	public class TurnByTurnViewModel : BaseViewModel
	{
		public TurnByTurnViewModel ()
		{
			
		}

		private ObservableCollection<BingItineraryItem> _itins;
		public ObservableCollection<BingItineraryItem>Itineraries 
		{ 
			get{
				return _itins;
			}
			set
			{
				_itins = value;
				NotifyPropertyChanged ();
			}
		}

		private double _latitude, _longitude;

		public TurnByTurnViewModel (double destinationLatitude, double destinationLongitude) : this()
		{
			this.client = client;
			_latitude = destinationLatitude;
			_longitude = destinationLongitude;
			LoadItemsCommand.Execute (null);
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try {
				await App.GetPosition();
				IsLoading = true;
				try{

					PortableRest.RestRequest req = new PortableRest.RestRequest ("Routes", System.Net.Http.HttpMethod.Get);
					req.AddQueryString ("wp.0", App.PositionLatitude + "," + App.PositionLongitude);
					req.AddQueryString ("wp.1", _latitude + "," + _longitude);
					req.AddQueryString ("du", "mi");
					req.AddQueryString ("avoid", "minimizeTolls");
					req.AddQueryString ("key", "AqXf-x5KdOluBQB35EjKT3owEzBLbfUqetvc0rPZ7xAbW_EKMsZ0RB0IYWkypdwH");


					PortableRest.RestClient rc = new PortableRest.RestClient ();			
					rc.UserAgent = "PickUp";
					rc.BaseUrl = "http://dev.virtualearth.net/REST/V1/";
					PortableRest.RestResponse<string> resp = await rc.SendAsync<string>(req, default(System.Threading.CancellationToken));

					BingResponse br = Newtonsoft.Json.JsonConvert.DeserializeObject<BingResponse>(resp.Content);
					Itineraries = br.ResourceSets[0].TripResources[0].RouteLegs[0].ItineraryItems;

					resp.Dispose();
					resp = null;
					req = null;
				} 
			catch (Exception ex) 
			{
				System.Diagnostics.Debug.WriteLine ("TurnByTurn Error " + ex.Message);	
			}
			finally{
				IsLoading = false;
			}
			}
			catch(Exception ex) {
				
			}

		}
	}
}
