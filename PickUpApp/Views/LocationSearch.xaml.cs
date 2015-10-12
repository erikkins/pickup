using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Linq;
using XLabs.Platform.Services;
//using Xamarin.Forms.Labs.Services;
using System.Threading.Tasks;
//using Xamarin.Forms.Labs.Services.Geolocation;
using XLabs.Platform.Services.Geolocation;
using RestSharp.Portable;
using System.Collections.ObjectModel;
using XLabs.Forms.Controls;

namespace PickUpApp
{
	public partial class LocationSearch : ContentPage
	{

		SimpleTextCell stcName;
		PickUpApp.SimpleTextCell stcValue;
		ExtendedTableView tvLocation;

		private Location tempLocation;
		private ObservableCollection<GoogleResult> _places;
		public ObservableCollection<GoogleResult> Places 
		{
			get{ return _places; }
			set{ _places = value;  }
		}

		public LocationSearch (Schedule currentSchedule)
		{
			InitializeComponent ();
			tempLocation = new Location ();
			tempLocation.FullAddress = currentSchedule.Address;
			tempLocation.Name = currentSchedule.Location;
			//this.Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 5);

			searchBar.BackgroundColor = Color.FromRgb (238, 236, 243);

			this.ToolbarItems.Add (new ToolbarItem ("Done", null, async() => {
				//pop the calendar window
				currentSchedule.Location = tempLocation.Name;
				currentSchedule.Address = tempLocation.FullAddress;
				currentSchedule.Latitude = tempLocation.Latitude;
				currentSchedule.Longitude = tempLocation.Longitude;

				MessagingCenter.Send<Schedule>(currentSchedule, "UpdatePlease");
				await Navigation.PopAsync();
			}));


//			btnSave.Clicked += (object sender, EventArgs e) => {
//				currentSchedule.Address = tempLocation.FullAddress;
//				currentSchedule.Latitude = tempLocation.Latitude;
//				currentSchedule.Longitude = tempLocation.Longitude;
//
//				MessagingCenter.Send<Schedule> (currentSchedule, "LocationUpdated");
//			};


			tvLocation = new ExtendedTableView ();
			tvLocation.Intent = TableIntent.Form;
			tvLocation.BindingContext = tempLocation;
			tvLocation.HasUnevenRows = false;
			tvLocation.RowHeight = 80;
			tvLocation.BackgroundColor = Color.FromRgb (238, 236, 243);

			listStacker.Children.Add (tvLocation);
			TableSection ts = new TableSection ();

			stcName = new SimpleTextCell ("Name");

			ts.Add (stcName);
			stcValue = new SimpleTextCell ("Address");
			ts.Add (stcValue);
			tvLocation.Root.Add (ts);

//			btnCancel.Clicked += async (object sender, EventArgs e) => {
//				await Navigation.PopModalAsync ();
//			};
//
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
		//		var addressQuery = searchBar.Text;
		//		searchBar.Text = "";
		//		searchBar.Unfocus ();
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

				tempLocation.FullAddress = result.Address;
				tempLocation.Latitude = position.Latitude.ToString();
				tempLocation.Longitude = position.Longitude.ToString();
				tempLocation.Name = result.Name;

				//tvLocation.OnDataChanged();
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




//		private void SearchLocations (string what)
//		{
//
//
//			using (var client = new RestClient(new Uri("https://maps.googleapis.com/maps/api/place/")))
//			{
//				var request = new RestRequest("textsearch/json", System.Net.Http.HttpMethod.Get);	
//	
//				request.AddQueryParameter ("query", what);
//				request.AddQueryParameter("key", "AIzaSyDpVbafIazS-s6a82lp4fswviB_Kb0fbmQ");
//
//				var result =  client.Execute(request);
//				GoogleResponse yr = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleResponse>(System.Text.Encoding.UTF8.GetString(result.RawBytes, 0, result.RawBytes.Length));
//
//				Places = yr.Results;
//				lstSearch.ItemsSource = Places;
//
//				//var yelpresponse = Newtonsoft.Json.Linq.JObject.Parse (System.Text.Encoding.UTF8.GetString(result.RawBytes, 0, result.RawBytes.Length));
//				//System.Diagnostics.Debug.WriteLine(result);
//			}
//
//		}

		void CancelClicked (object sender, EventArgs e)
		{
			Navigation.PopAsync ();
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

	public class SimpleTextCell : ViewCell
	{
		private string _title;
		public SimpleTextCell(string title)
		{
			_title = title;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 80;

			Grid g = new Grid ();
			g.ColumnDefinitions = new ColumnDefinitionCollection ();
			ColumnDefinition cd = new ColumnDefinition ();
			cd.Width = 120;
			g.ColumnDefinitions.Add (cd);
			cd = new ColumnDefinition ();
			cd.Width = GridLength.Auto;
			g.ColumnDefinitions.Add (cd);

			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;
			sl.HorizontalOptions = LayoutOptions.Start;
			sl.VerticalOptions = LayoutOptions.Center;
			sl.BackgroundColor = Color.FromRgb (238, 236, 243);
			sl.HeightRequest = 80;
			//sl.WidthRequest = App.Device.Display.Width / 4;
			//sl.MinimumWidthRequest = App.Device.Display.Width / 4;

			BoxView bv = new BoxView ();
			bv.WidthRequest = 10;
			sl.Children.Add (bv);

			Label l = new Label ();

			l.Text = _title;

			l.TextColor = Color.FromRgb (246, 99, 127);
			l.HorizontalOptions = LayoutOptions.StartAndExpand;
			l.VerticalOptions = LayoutOptions.Center;
			l.FontAttributes = FontAttributes.Bold;
			l.LineBreakMode = LineBreakMode.TailTruncation;
			l.FontSize = 16;
			l.WidthRequest = 100;

			g.Children.Add (l, 0, 0);

			Label l2 = new Label();
			if (_title == "Name") {
				l2.SetBinding (Label.TextProperty, "Name");
			} else {
				l2.SetBinding (Label.TextProperty, "FullAddress");
			}
			//l2.Text = _value;
			l2.VerticalOptions = LayoutOptions.Center;
			l2.HorizontalOptions = LayoutOptions.StartAndExpand;
			l2.LineBreakMode = LineBreakMode.TailTruncation;

			g.Children.Add (l2, 1, 0);

			sl.Children.Add (g);

			View = sl;
		}
	}
}

