using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Linq;
using Xamarin.Forms.Labs.Services;
using System.Threading.Tasks;
using Xamarin.Forms.Labs.Services.Geolocation;

namespace PickUpApp
{
	public partial class LocationSearch : ContentPage
	{
		private Location tempLocation;

		public LocationSearch (Schedule currentSchedule)
		{
			InitializeComponent ();
			tempLocation = new Location ();
			this.Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 5);

			btnSave.Clicked += (object sender, EventArgs e) => {
				currentSchedule.Address = tempLocation.FullAddress;
				currentSchedule.Latitude = tempLocation.Latitude;
				currentSchedule.Longitude = tempLocation.Longitude;

				MessagingCenter.Send<Schedule> (currentSchedule, "LocationUpdated");
			};



			btnCancel.Clicked += async (object sender, EventArgs e) => {
				await Navigation.PopModalAsync ();
			};

			if (currentSchedule.Latitude == null) {
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
				Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (double.Parse (currentSchedule.Latitude), double.Parse (currentSchedule.Longitude));

				map.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
					Distance.FromMiles (0.1)));
				map.Pins.Add (new Pin {
					Label = currentSchedule.Address,
					Position = thispos,
					Address = currentSchedule.Address
				});
			

			}

			searchBar.SearchButtonPressed += async (e, a) => {
				map.Pins.Clear ();
				var addressQuery = searchBar.Text;
				searchBar.Text = "";
				searchBar.Unfocus ();
				//await GetPosition ();
				//TODO: use Google's geocoder for all searches

				var positions = (await (new Geocoder ()).GetPositionsForAddressAsync (addressQuery)).ToList ();
				if (!positions.Any ()) {
					await DisplayAlert ("Could not find address", "Please be more specific with your search (add city, state, etc.)", "OK");
					searchBar.Text = addressQuery;
					return;
				}
					
				var position = positions.First ();

				var addresses = (await (new Geocoder ()).GetAddressesForPositionAsync (position)).ToList ();

				map.MoveToRegion (MapSpan.FromCenterAndRadius (position,
					Distance.FromMiles (0.1)));
				map.Pins.Add (new Pin {
					Label = addressQuery,
					Position = position,
					Address = addresses [0]
				});

				tempLocation.FullAddress = addresses[0];
				tempLocation.Latitude = position.Latitude.ToString();
				tempLocation.Longitude = position.Longitude.ToString();

			};
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
							tempLocation.Latitude = t.Result.Latitude.ToString("N4");
							tempLocation.Longitude = t.Result.Longitude.ToString("N4");

							Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (double.Parse (tempLocation.Latitude), double.Parse (tempLocation.Longitude));

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
			System.Diagnostics.Debug.WriteLine (e.Error.ToString ());
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

