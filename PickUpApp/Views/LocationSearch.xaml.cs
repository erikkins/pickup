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
		PickUpApp.SimpleTextCell stcPhone;
		ExtendedTableView tvLocation;
		//Schedule _currentSchedule;

		//private Location tempLocation;
		private AccountPlace tempLocation;

		private ObservableCollection<GoogleResult> _places;
		public ObservableCollection<GoogleResult> Places 
		{
			get{ return _places; }
			set{ _places = value;  }
		}

		public LocationSearch()
		{
			//default constructor
			InitializeComponent();
			tempLocation = new AccountPlace ();
			this.ViewModel = new AccountPlaceViewModel (App.client);

			searchBar.BackgroundColor = Color.FromRgb (238, 236, 243);

			this.ToolbarItems.Add (new ToolbarItem ("Done", null, async() => {

				//this needs to save the AccountPlace...potentially adding it to the schedule
				//if it was added inline and return to where it started


					//we're just adding this place!
					tempLocation.accountid = App.myAccount.id;
				
					ViewModel.CurrentPlace = tempLocation;
					await ViewModel.ExecuteAddEditCommand();
					//reload the app cache
					await ViewModel.ExecuteLoadItemsCommand();
					await Navigation.PopAsync();


					//pop the calendar window
					//actually this shouldn't be saving the discrete elements anymore...
					//schedule should only reference the place IDs

//					_currentSchedule.Location = tempLocation.Name;
//					_currentSchedule.Address = tempLocation.FullAddress;
//					_currentSchedule.Latitude = tempLocation.Latitude;
//					_currentSchedule.Longitude = tempLocation.Longitude;



					//MessagingCenter.Send<Schedule>(_currentSchedule, "UpdatePlease");
					//await Navigation.PopAsync();
				
			}));


			//			btnSave.Clicked += (object sender, EventArgs e) => {
			//				currentSchedule.Address = tempLocation.FullAddress;
			//				currentSchedule.Latitude = tempLocation.Latitude;
			//				currentSchedule.Longitude = tempLocation.Longitude;
			//
			//				MessagingCenter.Send<Schedule> (currentSchedule, "LocationUpdated");
			//			};





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

				tempLocation.Address = result.Address;
				tempLocation.Latitude = position.Latitude.ToString();
				tempLocation.Longitude = position.Longitude.ToString();
				tempLocation.PlaceName = result.Name;

				//tvLocation.OnDataChanged();
			};
		}

		private void loadIt()
		{
			tvLocation = new ExtendedTableView ();
			tvLocation.Intent = TableIntent.Data;
			tvLocation.BindingContext = tempLocation;
			tvLocation.HasUnevenRows = false;
			tvLocation.RowHeight = 75;

			tvLocation.BackgroundColor = Color.FromRgb (238, 236, 243);

			listStacker.Children.Add (tvLocation);
			TableSection ts = new TableSection ();

			stcName = new SimpleTextCell ("Name");
			ts.Add (stcName);

			stcValue = new SimpleTextCell ("Address");
			ts.Add (stcValue);

			stcPhone = new SimpleTextCell ("Phone");
			ts.Add (stcPhone);


			tvLocation.Root.Add (ts);

			//			btnCancel.Clicked += async (object sender, EventArgs e) => {
			//				await Navigation.PopModalAsync ();
			//			};
			//


			if (tempLocation == null || tempLocation.Latitude == null) {
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
				Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (double.Parse (tempLocation.Latitude), double.Parse (tempLocation.Longitude));

				map.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
					Distance.FromMiles (0.1)));
				map.Pins.Add (new Pin {
					Label = tempLocation.Address,
					Position = thispos,
					Address = tempLocation.Address
				});


			}
		}


		public LocationSearch (AccountPlace accountPlace): this()
		{
			tempLocation = accountPlace;
			loadIt ();
			//kinda silly, no?

		}

		public LocationSearch (Schedule currentSchedule): this()
		{
			//constructor where we pass in a schedule to set a particular location within that item

			//InitializeComponent ();

			tempLocation.Address = currentSchedule.Address;
			tempLocation.PlaceName = currentSchedule.Location;
			loadIt ();
			//this.Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 5);



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

		protected AccountPlaceViewModel ViewModel
		{
			get { return this.BindingContext as AccountPlaceViewModel; }
			set { this.BindingContext = value; }
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
		private DateTime _date;

		public SimpleTextCell(string title)
		{
			_title = title;

		}


		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 75;

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
			l.FontAttributes = FontAttributes.Bold;
			l.FontSize = 16;
			l.TextColor = Color.FromRgb (246, 99, 127);
			l.HorizontalOptions = LayoutOptions.StartAndExpand;
			l.VerticalOptions = LayoutOptions.Center;
			l.WidthRequest = 100;

			g.Children.Add (l, 0, 0);

			ExtendedEntry l2 = new ExtendedEntry();
			l2.HasBorder = false;
			l2.BackgroundColor = Color.Transparent;
			switch (_title) {
			case "Name":
				l2.SetBinding (ExtendedEntry.TextProperty, "PlaceName");
				break;
			case "Address":
				l2.SetBinding (ExtendedEntry.TextProperty, "Address");
				break;
			case "Phone":
				l2.SetBinding (ExtendedEntry.TextProperty, "Phone");
				break;
			}

			//l2.Text = _value;
			l2.VerticalOptions = LayoutOptions.Center;
			l2.HorizontalOptions = LayoutOptions.StartAndExpand;
			//l2.LineBreakMode = LineBreakMode.TailTruncation;
			l2.WidthRequest = (App.Device.Display.Width / 2) - 150;

			g.Children.Add (l2, 1, 0);

			sl.Children.Add (g);

			View = sl;
		}
	}

	public class SimpleBoundTextCell : ViewCell
	{
		private string _title;
		private string _binding;

		public SimpleBoundTextCell(string title, string binding)
		{
			_title = title;
			_binding = binding;

		}


		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 75;

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
			l.FontAttributes = FontAttributes.Bold;
			l.FontSize = 16;
			l.TextColor = Color.FromRgb (246, 99, 127);
			l.HorizontalOptions = LayoutOptions.StartAndExpand;
			l.VerticalOptions = LayoutOptions.Center;
			l.WidthRequest = 100;

			g.Children.Add (l, 0, 0);

			ExtendedEntry l2 = new ExtendedEntry();
			l2.HasBorder = false;
			l2.BackgroundColor = Color.Transparent;
			l2.SetBinding (ExtendedEntry.TextProperty, _binding);


			//l2.Text = _value;
			l2.VerticalOptions = LayoutOptions.Center;
			l2.HorizontalOptions = LayoutOptions.StartAndExpand;
			//l2.LineBreakMode = LineBreakMode.TailTruncation;
			l2.WidthRequest = (App.Device.Display.Width / 2) - 150;

			g.Children.Add (l2, 1, 0);

			sl.Children.Add (g);

			View = sl;
		}
	}

	public class SimpleDateCell : ViewCell
	{
		private string _title;
		private DateTime _date;
		private string _binding;
		public SimpleDateCell(string title, DateTime defaultDate, string binding)
		{
			_title = title;
			_date = defaultDate;
			_binding = binding;
		}

		public DateTime SelectedDate
		{
			get{
				return _date;
			}
			set{
				_date = value;
				this.OnPropertyChanged ("SelectedDate");
			}
		}


		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 75;

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
			l.FontAttributes = FontAttributes.Bold;
			l.FontSize = 16;
			l.TextColor = Color.FromRgb (246, 99, 127);
			l.HorizontalOptions = LayoutOptions.StartAndExpand;
			l.VerticalOptions = LayoutOptions.Center;
			l.WidthRequest = 100;

			g.Children.Add (l, 0, 0);


			ExtendedDatePicker dp = new ExtendedDatePicker ();
			dp.HasBorder = false;
			dp.Date = _date;
			dp.VerticalOptions = LayoutOptions.Center;
			dp.HorizontalOptions = LayoutOptions.StartAndExpand;
			dp.WidthRequest = (App.Device.Display.Width / 2) - 150;
			if (!string.IsNullOrEmpty (_binding)) {
				dp.SetBinding (ExtendedDatePicker.DateProperty, _binding);
			}

			g.Children.Add (dp, 1, 0);



			sl.Children.Add (g);

			View = sl;
		}
	}

	public class SimplePickerCell : ViewCell
	{
		private string _title;
		private string _value;
		private List<string> _items;
		private int _selectedIndex;

		public SimplePickerCell(string title, string defaultValue, List<string> pickerItems)
		{
			_title = title;
			_value = defaultValue;
			_items = pickerItems;

			_selectedIndex = _items.IndexOf (_value);
		}

		public string SelectedValue
		{
			get{
				return _value;
			}
			set{
				_value = value;
				_selectedIndex = _items.IndexOf (_value);
				this.OnPropertyChanged ("SelectedValue");
			}
		}


		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 75;

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
			l.FontAttributes = FontAttributes.Bold;
			l.FontSize = 16;
			l.TextColor = Color.FromRgb (246, 99, 127);
			l.HorizontalOptions = LayoutOptions.StartAndExpand;
			l.VerticalOptions = LayoutOptions.Center;
			l.WidthRequest = 100;

			g.Children.Add (l, 0, 0);


			Picker p = new Picker ();

			p.WidthRequest = App.Device.Display.Width / 4;
			p.BackgroundColor = Color.FromRgb(238, 236, 243); 
			foreach (string s in _items) {
				p.Items.Add (s);
			}
			p.SelectedIndex = _items.IndexOf (_value);
			//p.SetBinding (Picker.SelectedIndexProperty, "_selectedIndex");

			
			g.Children.Add (p, 1, 0);



			sl.Children.Add (g);

			View = sl;
		}
	}

	public class SimpleImageCell : ViewCell
	{
		private string _imagePath;

		public SimpleImageCell(string imagePath)
		{
			_imagePath = imagePath;
		}

		public string ImagePath
		{
			get{
				return _imagePath;
			}
			set{
				_imagePath = value;

			}
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 120;

			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Vertical;
			sl.HorizontalOptions = LayoutOptions.Center;
			sl.VerticalOptions = LayoutOptions.Center;
			sl.BackgroundColor = Color.FromRgb (238, 236, 243);
			sl.HeightRequest = 100;
			sl.WidthRequest = App.Device.Display.Width / 2;
			//sl.MinimumWidthRequest = App.Device.Display.Width / 4;


			ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
				BorderColor = Color.Black,
				BorderThickness = 1,
				Aspect = Aspect.AspectFill,
				WidthRequest = 100,
				HeightRequest = 100,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Source= _imagePath
			};	

			sl.Children.Add (ci);

			View = sl;
		}
	}
}

