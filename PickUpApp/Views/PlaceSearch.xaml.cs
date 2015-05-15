using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Linq;
//using Xamarin.Forms.Labs.Services;
using XLabs.Platform.Services;
using System.Threading.Tasks;
//using Xamarin.Forms.Labs.Services.Geolocation;
using XLabs.Platform.Services.Geolocation;
using RestSharp.Portable;
using System.Collections.ObjectModel;

namespace PickUpApp
{
	public partial class PlaceSearch : ContentPage
	{
		private AccountPlace tempPlace;
		private ObservableCollection<GoogleResult> _places;
		public ObservableCollection<GoogleResult> Places 
		{
			get{ return _places; }
			set{ _places = value;  }
		}

		public PlaceSearch (AccountPlace currentPlace)
		{
			InitializeComponent ();
			tempPlace = new AccountPlace();
			this.Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 5);

			btnSave.Clicked += (object sender, EventArgs e) => {
				currentPlace.Address = tempPlace.Address;
				currentPlace.Latitude = tempPlace.Latitude;
				currentPlace.Longitude = tempPlace.Longitude;

				MessagingCenter.Send<AccountPlace> (currentPlace, "PlaceUpdated");
			};
				

			btnCancel.Clicked += async (object sender, EventArgs e) => {
				await Navigation.PopModalAsync ();
			};

			if (currentPlace.Latitude == null) {
				//nothing in there...we don't want Rome, so let's try to map using their current location?
				//Xamarin.Forms.Labs.Services.Geolocation.IGeolocator igeo = DependencyService.Get<Xamarin.Forms.Labs.Services.Geolocation.IGeolocator>();
				//System.Runtime.CompilerServices.ConfiguredTaskAwaitable<Xamarin.Forms.Labs.Services.Geolocation.Position> here = igeo.GetPositionAsync (6).ConfigureAwait (false);
				//Xamarin.Forms.Labs.Services.Geolocation.Position p2 = here.GetAwaiter ().GetResult ();
				GetPosition ().ConfigureAwait (false);
				//Position p = new Position (p2.Latitude, p2.Longitude);
				/*
				map.MoveToRegion (MapSpan.FromCenterAndRadius (p,
					Distance.FromMiles (0.1)));
				map.Pins.Add (new Pin {
					Label = "HERE",
					Position = p,
					Address = ""
				});
				*/
			} else {
				//preload with the last known somethin' or other
				map.Pins.Clear ();
				Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (double.Parse (currentPlace.Latitude), double.Parse (currentPlace.Longitude));

				map.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
					Distance.FromMiles (0.1)));
				map.Pins.Add (new Pin {
					Label = currentPlace.Address,
					Position = thispos,
					Address = currentPlace.Address
				});
			

			}


			searchBar.SearchButtonPressed += async (e, a) => {
				//we're actually going to do a location search and fill the listview
				lstSearch.IsVisible = true;
				LayoutRel.IsVisible = true;
				//need to feed it to some webservice (Google places or Yelp?)
				using (var client = new RestClient(new Uri("https://maps.googleapis.com/maps/api/place/")))
				{
					var request = new RestRequest("textsearch/json", System.Net.Http.HttpMethod.Get);	
		
					request.AddQueryParameter ("query", searchBar.Text);
					request.AddQueryParameter("key", "AIzaSyDpVbafIazS-s6a82lp4fswviB_Kb0fbmQ");

					var result = await client.Execute(request);
					GoogleResponse yr = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleResponse>(System.Text.Encoding.UTF8.GetString(result.RawBytes, 0, result.RawBytes.Length));

					Places = yr.Results;
					lstSearch.ItemsSource = Places;

					//var yelpresponse = Newtonsoft.Json.Linq.JObject.Parse (System.Text.Encoding.UTF8.GetString(result.RawBytes, 0, result.RawBytes.Length));
					//System.Diagnostics.Debug.WriteLine(result);
				}
			};

			lstSearch.ItemSelected += delegate(object sender, SelectedItemChangedEventArgs e) {
				lstSearch.IsVisible = false;
				LayoutRel.IsVisible = false;

				map.Pins.Clear ();
				var addressQuery = searchBar.Text;
				searchBar.Text = "";
				searchBar.Unfocus ();
				//await GetPosition ();
				//TODO: use Google's geocoder for all searches

				GoogleResult result = (GoogleResult)e.SelectedItem;

//				var positions = (await (new Geocoder ()).GetPositionsForAddressAsync (addressQuery)).ToList ();
//				if (!positions.Any ()) {
//					await DisplayAlert ("Could not find address", "Please be more specific with your search (add city, state, etc.)", "OK");
//					searchBar.Text = addressQuery;
//					return;
//				}
					
				//var position = positions.First ();
				Xamarin.Forms.Maps.Position position = new Xamarin.Forms.Maps.Position(result.Geometry.Location.Latitude, result.Geometry.Location.Longitude);

				//var addresses = (await (new Geocoder ()).GetAddressesForPositionAsync (position)).ToList ();

				map.MoveToRegion (MapSpan.FromCenterAndRadius (position,
					Distance.FromMiles (0.1)));
				map.Pins.Add (new Pin {
					Label = result.Name,
					Position = position,
					Address = result.Address
				});

				tempPlace.Address = result.Address;
				tempPlace.Latitude = position.Latitude.ToString();
				tempPlace.Longitude = position.Longitude.ToString();
			};

//			searchBar.SearchButtonPressed += async (e, a) => {
//				map.Pins.Clear ();
//				var addressQuery = searchBar.Text;
//				searchBar.Text = "";
//				searchBar.Unfocus ();
//				//await GetPosition ();
//				//TODO: use Google's geocoder for all searches
//
//				var positions = (await (new Geocoder ()).GetPositionsForAddressAsync (addressQuery)).ToList ();
//				if (!positions.Any ()) {
//					await DisplayAlert ("Could not find address", "Please be more specific with your search (add city, state, etc.)", "OK");
//					searchBar.Text = addressQuery;
//					return;
//				}
//					
//				var position = positions.First ();
//
//				var addresses = (await (new Geocoder ()).GetAddressesForPositionAsync (position)).ToList ();
//
//				map.MoveToRegion (MapSpan.FromCenterAndRadius (position,
//					Distance.FromMiles (0.1)));
//				map.Pins.Add (new Pin {
//					Label = addressQuery,
//					Position = position,
//					Address = addresses [0]
//				});
//
//				tempLocation.FullAddress = addresses[0];
//				tempLocation.Latitude = position.Latitude.ToString();
//				tempLocation.Longitude = position.Longitude.ToString();
//
//			};
		}

		void CancelClicked (object sender, EventArgs e)
		{
			Navigation.PopModalAsync ();
		}

		private readonly TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		private IGeolocator geolocator;
		private System.Threading.CancellationTokenSource cancelSource;
		string PositionStatus = "";

		void Setup()
		{
			if (this.geolocator != null)
				return;
			this.geolocator = DependencyService.Get<IGeolocator> ();
			this.geolocator.PositionError += OnListeningError;
			this.geolocator.PositionChanged += OnPositionChanged;
		}
		async Task GetPosition ()
		{
			Setup();
			if (!geolocator.IsGeolocationEnabled) {

			}

			this.cancelSource = new System.Threading.CancellationTokenSource();


			IsBusy = true;
			await this.geolocator.GetPositionAsync (timeout: 10000, cancelToken: this.cancelSource.Token, includeHeading: true)
				.ContinueWith (t =>
					{
						IsBusy = false;
						if (t.IsFaulted)
							PositionStatus = ((GeolocationException)t.Exception.InnerException).Error.ToString();
						else if (t.IsCanceled)
							PositionStatus = "Canceled";
						else
						{
							PositionStatus = t.Result.Timestamp.ToString("G");
							tempPlace.Latitude = t.Result.Latitude.ToString("N4");
							tempPlace.Longitude = t.Result.Longitude.ToString("N4");

							Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (double.Parse (tempPlace.Latitude), double.Parse (tempPlace.Longitude));

							map.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
								Distance.FromMiles (0.1)));
							map.Pins.Add (new Pin {
								Label = PositionStatus,
								Position = thispos,
								Address = ""
							});

						}

					}, scheduler);
		}
		private void OnListeningError(object sender, PositionErrorEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Listening Error" + e.Error.ToString ());
//						BeginInvokeOnMainThread (() => {
//							//ListenStatus.Text = e.Error.ToString();
//						});
		}

		private void OnPositionChanged(object sender, PositionEventArgs e)
		{
			Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (e.Position.Latitude, e.Position.Longitude);

			map.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
				Distance.FromMiles (0.1)));
			map.Pins.Add (new Pin {
				Label = "you are here",
				Position = thispos,
				Address = ""
			});
			//			BeginInvokeOnMainThread (() => {
			//				ListenStatus.Text = e.Position.Timestamp.ToString("G");
			//				ListenLatitude.Text = "La: " + e.Position.Latitude.ToString("N4");
			//				ListenLongitude.Text = "Lo: " + e.Position.Longitude.ToString("N4");
			//			});
		}

	}
}

