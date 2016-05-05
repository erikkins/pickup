﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XLabs.Forms;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using FFImageLoading.Forms;

namespace PickUpApp
{
	public partial class RouteDetail : ContentPage
	{

		private System.Threading.CancellationTokenSource cancellationToken = new System.Threading.CancellationTokenSource ();
		DistanceService ds = new DistanceService ();

		public RouteDetail (Today currentToday, string PinString, int CurrentOrdinal)
		{
			InitializeComponent ();
			this.Padding = new Thickness(0, Device.OnPlatform(0, 0, 0), 0, 0);
			this.ViewModel = new RouteDetailViewModel (App.client);
			//NavigationPage.SetHasBackButton (this, true);


			this.Appearing += delegate(object sender, EventArgs e) {
				MessagingCenter.Subscribe<EmptyClass>(this, "launchchat", (ec)=>{
					Navigation.PushAsync(new ManageFetch(currentToday, false));
				});
			};
			this.Disappearing += delegate(object sender, EventArgs e) {
					MessagingCenter.Unsubscribe<EmptyClass>(this, "launchchat");
			};

			TableView tv = new TableView ();
			tv.BindingContext = currentToday;
			tv.HasUnevenRows = true;
			TableSection ts = new TableSection ();
		
			MapCell mc = new MapCell(double.Parse(currentToday.Latitude),double.Parse(currentToday.Longitude), currentToday.Address, PinString, CurrentOrdinal);
			ts.Add (mc);
			ts.Add (new RouteCell ());

			//deal is, we really want to load this cell with 0 delay, then run the traffic call to check realtime


			if (currentToday.IsPickup) {
				ds.ExpectedTravelTime = (decimal)currentToday.EndPlaceTravelTime;		
				ds.EventTime = currentToday.TSPickup;
			} else {
				ds.ExpectedTravelTime = (decimal)currentToday.StartPlaceTravelTime;
				ds.EventTime = currentToday.TSDropOff;
			}
			Location start = new Location ();
			start.Latitude = App.PositionLatitude;
			start.Longitude = App.PositionLongitude;
			Location end = new Location ();
			end.Latitude = currentToday.Latitude;
			end.Longitude = currentToday.Longitude;



			ds.StartingLocation = start;
			ds.EndingLocation = end;
			ds.CalculateDriveTime ().ConfigureAwait (true);
			//PickUpApp.fflog logentry = new fflog("BingLocation for " + currentToday.id + " from RouteDetail");
			//this.ViewModel.ExecuteLogCommand(logentry);


			//log them right now
			LocationLog ll = new LocationLog();
			ll.Latitude = App.PositionLatitude;
			ll.Longitude = App.PositionLongitude;
			ll.LogType = "routedetail";
			ll.ScheduleID = currentToday.id;
			if (currentToday.IsPickup)
			{
				//picking up
				if (!string.IsNullOrEmpty(currentToday.PickupMessageID))
				{
					ll.MessageID = currentToday.PickupMessageID;
					ll.MessageIDType = "pickup";
				}
			}
			else{
				if (!string.IsNullOrEmpty(currentToday.DropOffMessageID))
				{
					ll.MessageID = currentToday.DropOffMessageID;
					ll.MessageIDType = "dropoff";
				}
			}
			if (!string.IsNullOrEmpty(ll.Latitude) && !(string.IsNullOrEmpty(ll.Longitude)))
			{
				this.ViewModel.ExecuteLocationLogCommand(ll).ConfigureAwait(false);
			}

			//every 30 seconds we're on this screen, log the coordinates
			logLocationTimer (currentToday);		
			//make sure we stop the timer!
			this.Disappearing += delegate(object sender, EventArgs e) {
				cancellationToken.Cancel();
			};


			TrafficCell tc = new TrafficCell (0);
			tc.BindingContext = ds;
			ts.Add (tc);

//			if (currentToday.IsPickup) {
//				if (ds.TravelTime != (decimal)currentToday.EndPlaceTravelTime) {
//					//disparity
//					ts.Add (new TrafficCell (ds.TravelTime - (decimal)currentToday.EndPlaceTravelTime));
//				}
//			} else {
//				if (ds.TravelTime != (decimal)currentToday.StartPlaceTravelTime) {
//					//something interesting
//					ts.Add (new TrafficCell (ds.TravelTime - (decimal)currentToday.StartPlaceTravelTime));
//				}
//			}



			//really this should only be shown if this is a fetch request
			if (!string.IsNullOrEmpty(currentToday.DropOffMessageID) || !string.IsNullOrEmpty(currentToday.PickupMessageID)) {
				if (!string.IsNullOrEmpty (currentToday.ViaPhone)) {
					ts.Add (new ContactCell ());
				}
			}

			if (!string.IsNullOrEmpty (currentToday.LocationPhone)) {
				ts.Add (new LocationContactCell ());
			}

			ts.Add (new ButtonCell ());
			tv.Root.Add (ts);
			stacker.Children.Add (tv);
			this.Appearing += delegate(object sender, EventArgs e) {
				mc.Navigate();
			};

			MessagingCenter.Subscribe<Today> (this, "markcomplete", async (t) => {
				ScheduleAudit sa = new ScheduleAudit();
				if(t.IsPickup)
				{
					sa.PickupComplete = true;
					sa.PickupCompleteAtWhen = DateTime.UtcNow;
					sa.PickupUserID = App.myAccount.UserId;
				}
				else
				{
					sa.DropoffComplete = true;
					sa.DropoffCompleteAtWhen = DateTime.UtcNow;
					sa.DropoffUserID = App.myAccount.UserId;
				}
				sa.ScheduleDate = currentToday.AtWhen;
				sa.ScheduleID = currentToday.id;

				//since this isn't coming in via any messages we can't know that from here
				await ViewModel.ExecuteAuditCommand(sa);

				MessagingCenter.Send<string>("routedetail", "NeedsRefresh");

				//await Navigation.PopAsync();
			});

			MessagingCenter.Subscribe<PickUpApp.ViewModels.TodayViewModel> (this, "TodayLoaded", async(t) => {
				try{
				await Navigation.PopAsync ();
				}
				catch (Exception ex)
				{
					//why does this periodically fail?
				}
				MessagingCenter.Unsubscribe<PickUpApp.ViewModels.TodayViewModel>(this, "TodayLoaded");
			});




		}

		protected RouteDetailViewModel ViewModel
		{
			get { return this.BindingContext as RouteDetailViewModel; }
			set { this.BindingContext = value; }
		}

		Location lastLocation = new Location();
		private void logLocationTimer(Today currentToday)
		{
			Device.StartTimer(TimeSpan.FromSeconds(30), () =>
				{

					if (cancellationToken.IsCancellationRequested)
					{
						return false;
					}

					System.Threading.Tasks.Task.Factory.StartNew(async () =>
						{
							LocationLog ll = new LocationLog();
							ll.Latitude = App.PositionLatitude;
							ll.Longitude = App.PositionLongitude;
							ll.LogType = "routedetail";
							ll.ScheduleID = currentToday.id;
							if (currentToday.IsPickup)
							{
								//picking up
								if (!string.IsNullOrEmpty(currentToday.PickupMessageID))
								{
									ll.MessageID = currentToday.PickupMessageID;
									ll.MessageIDType = "pickup";
								}
							}
							else{
								if (!string.IsNullOrEmpty(currentToday.DropOffMessageID))
								{
									ll.MessageID = currentToday.DropOffMessageID;
									ll.MessageIDType = "dropoff";
								}
							}
							if (!string.IsNullOrEmpty(ll.Latitude) && !(string.IsNullOrEmpty(ll.Longitude)))
							{
								if (ll.Latitude != lastLocation.Latitude && ll.Longitude != lastLocation.Longitude)
								{
									await this.ViewModel.ExecuteLocationLogCommand(ll);
									lastLocation.Latitude = ll.Latitude;
									lastLocation.Longitude = ll.Longitude;
								}
							}

							//don't forget to refresh the DistanceService!
							Location start = new Location ();
							start.Latitude = App.PositionLatitude;
							start.Longitude = App.PositionLongitude;
							Location end = new Location ();
							end.Latitude = currentToday.Latitude;
							end.Longitude = currentToday.Longitude;

							ds.StartingLocation = start;
							ds.EndingLocation = end;
							await ds.CalculateDriveTime ();


							// Switch back to the UI thread to update the UI
							Device.BeginInvokeOnMainThread(() =>
								{
									// Update the UI
									// ...
									// Now repeat by scheduling a new request
									logLocationTimer(currentToday);
								});
						});

					// Don't repeat the timer (we will start a new timer when the request is finished)
					return false;
				});
		}
	}


	public class ButtonCell : ViewCell
	{
		public ButtonCell()
		{
			//init
		}
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 75;

			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Vertical;
			sl.HorizontalOptions = LayoutOptions.Center;
			sl.VerticalOptions = LayoutOptions.Center;
			sl.BackgroundColor = Color.FromRgb (238, 236, 243);
			sl.HeightRequest = 75;
			sl.WidthRequest = App.ScaledWidth;
			sl.MinimumWidthRequest = App.ScaledWidth;

			Button btnComplete = new Button ();
			btnComplete.VerticalOptions = LayoutOptions.Center;
			btnComplete.HorizontalOptions = LayoutOptions.Center;
			btnComplete.HeightRequest = 50;
			btnComplete.WidthRequest = (App.ScaledWidth) - 50;
			btnComplete.FontAttributes = FontAttributes.Bold;
			btnComplete.FontSize = 18;
			btnComplete.BorderRadius = 8;
			btnComplete.BackgroundColor = Color.FromRgb (73, 55, 109);
			btnComplete.TextColor = Color.FromRgb (84, 210, 159);
			btnComplete.Text = "Mark as Complete";
			sl.Children.Add (btnComplete);

			btnComplete.Clicked += delegate(object sender, EventArgs e) {
				//await ((RouteDetail)this.ParentView.Parent.Parent).DisplayAlert ("DONE!", "Complete", "Cancel");
				MessagingCenter.Send<Today>((Today)c, "markcomplete");
			};

			View = sl;

		}
	}

	public class RouteCell : ViewCell
	{
		public RouteCell()
		{
			//initializer
		}
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;

			Today t = (Today)c;
			this.Height = 130;
			this.IsEnabled = false;

			//this is the cell that shows the pickup detail

			StackLayout outer = new StackLayout ();
			outer.Orientation = StackOrientation.Horizontal;
			outer.HorizontalOptions = LayoutOptions.StartAndExpand;
			outer.WidthRequest = App.ScaledWidth;
			outer.Padding = 10;
			outer.BackgroundColor = Color.FromRgb (238, 236, 243);

			StackLayout left = new StackLayout ();
			left.Orientation = StackOrientation.Vertical;
			left.HorizontalOptions = LayoutOptions.StartAndExpand;
			left.WidthRequest = App.ScaledQuarterWidth;

			StackLayout right = new StackLayout ();
			right.Orientation = StackOrientation.Vertical;
			right.HorizontalOptions = LayoutOptions.EndAndExpand;
			right.WidthRequest = App.ScaledQuarterWidth;

			StackLayout detail = new StackLayout ();
			detail.Orientation = StackOrientation.Horizontal;
			detail.HorizontalOptions = LayoutOptions.Start;

			Image icon = new Image ();
			if (t.IsNext) {
				if (t.IsPickup) {
					icon.Source = "icn_pin_up_pink.png";
				} else {
					icon.Source = "icn_pin_dwn_pink.png";
				}
			} else {
				if (t.IsPickup) {
					icon.Source = "icn_pin_up_grey.png";
				} else {
					icon.Source = "icn_pin_dwn_grey.png";
				}
			}
			//icon.Source = "icn_pin_up_pink.png";
			icon.HorizontalOptions = LayoutOptions.Start;
			detail.Children.Add (icon);

			StackLayout detail2 = new StackLayout ();
			detail2.Orientation = StackOrientation.Vertical;

			Label time = new Label ();
			time.FormattedText = new FormattedString ();
			if (t.IsPickup) {
				DateTime intermediate = DateTime.Today.Add (t.TSPickup);
				time.FormattedText.Spans.Add (new Span { Text = intermediate.ToString(@"h\:mm"), FontSize= 24, ForegroundColor = Color.Black, FontAttributes = FontAttributes.Bold });
				time.FormattedText.Spans.Add (new Span { Text = " " + intermediate.ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower(), FontFamily=Device.OnPlatform("HelveticaNeue-Light", "", ""), FontSize = 24, ForegroundColor = Color.Black, FontAttributes = FontAttributes.None });
			} 
			else {
				DateTime intermediate = DateTime.Today.Add (t.TSDropOff);
				time.FormattedText.Spans.Add (new Span { Text = intermediate.ToString(@"h\:mm"), FontSize= 24, ForegroundColor = Color.Black, FontAttributes = FontAttributes.Bold });
				time.FormattedText.Spans.Add (new Span { Text = " " + intermediate.ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower(), FontFamily=Device.OnPlatform("HelveticaNeue-Light", "", ""), FontSize = 24, ForegroundColor = Color.Black, FontAttributes = FontAttributes.None });
			}



			time.VerticalOptions = LayoutOptions.Start;
			detail2.Children.Add (time);

			Label activity = new Label ();
			if (t.IsPickup) {
				activity.Text = t.Activity +  " Pickup";
			} else {
				activity.Text= t.Activity + " Dropoff";
			}
			activity.FontAttributes = FontAttributes.Bold;
			activity.FontSize = 16;
			activity.TextColor = Color.Black;
			activity.VerticalOptions = LayoutOptions.End;

			detail2.Children.Add (activity);

			detail.Children.Add (detail2);

			left.Children.Add (detail);

			Label instructions = new Label ();
			if (t.IsPickup) {
				instructions.Text = t.PickupNotes;
			} else {
				instructions.Text = t.DropOffNotes;
			}
			instructions.TextColor = Color.Gray;
			instructions.FontSize = 14;
			instructions.FontAttributes = FontAttributes.Italic;
			left.Children.Add (instructions);

			if (!string.IsNullOrEmpty (t.Kids)) {
				string[] kids = t.Kids.Split ('^');
				this.Height += 60;
				foreach (string s in kids) {

					string[] parts = s.Split ('|');
					string azureURL = AzureStorageConstants.BlobEndPoint + t.AccountID.ToLower () + "/" + parts [1].Trim ().ToLower () + ".jpg";
					if (!string.IsNullOrEmpty (parts [5])) {
						azureURL = parts [5].Trim ();
					}
					foreach (Kid k in App.myKids) {
						if (k.Id.ToLower () == parts [1].ToLower ()) {
							azureURL = k.PhotoURL;
							break;
						}
					}

					CachedImage cachedimg = new CachedImage ();
					cachedimg.Source = azureURL;
					cachedimg.CacheDuration = TimeSpan.FromDays (30);
					cachedimg.DownsampleToViewSize = true;
					cachedimg.TransparencyEnabled = false;
					cachedimg.Aspect = Aspect.AspectFill;
					cachedimg.HeightRequest = 50;
					cachedimg.WidthRequest = 50;
					cachedimg.HorizontalOptions = LayoutOptions.Center;
					cachedimg.VerticalOptions = LayoutOptions.Center;
					cachedimg.Transformations.Add (new FFImageLoading.Transformations.CircleTransformation (1, "0x000000"));


					//Uri auri = new Uri (azureURL);
					/*
					ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
						BorderColor = Color.Black,
						BorderThickness = 1,
						Aspect = Aspect.AspectFill,
						WidthRequest = 50,
						HeightRequest = 50,
						HorizontalOptions = LayoutOptions.Center,
						Source = azureURL
					};	
					*/

					StackLayout kid = new StackLayout ();
					kid.Orientation = StackOrientation.Horizontal;
					kid.VerticalOptions = LayoutOptions.Center;
					//kid.Children.Add (ci);
					kid.Children.Add(cachedimg);
					Label kidname = new Label ();
					kidname.Text = parts [0];
					kidname.VerticalOptions = LayoutOptions.Center;
					kidname.FontSize = 14;
					kidname.TextColor = Color.Black;
					kid.Children.Add (kidname);

					right.Children.Add (kid);
				}

			}

			outer.Children.Add (left);
			outer.Children.Add (right);

			View = outer;
		}

	}



	//Android only tap workaround
	public class ListMap : Map { }

	public class MapCell : ViewCell
	{
		private double _latitude, _longitude;
		private ListMap _theMap;
		string _address;
		private Label whiteaddress;
		private string _pinstring;
		private int _currentOrdinal;

		public MapCell (double latitude, double longitude, string address, string Pinstring, int CurrentOrdinal)
		{
			_latitude = latitude;
			_longitude = longitude;
			_address = address;
			_pinstring = Pinstring;
			_currentOrdinal = CurrentOrdinal;
		}

		public void Navigate(double latitude, double longitude, string address)
		{
			_latitude = latitude;
			_longitude = longitude;
			_address = address;
			whiteaddress.Text = _address;
			this.Navigate ();
		}

		public void Navigate()
		{
			Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (_latitude, _longitude);
			Pin p = new Pin ();


			//MEGA KLUDGE FOR ANDROID! Map somehow loads twice...correctly, then Africa.  Gotta load it again!
			if (Device.OS == TargetPlatform.Android) {
				Device.StartTimer (new TimeSpan (0, 0, 2), () => {
					//p.SetBinding (Pin.AddressProperty, "427 Illinois Rd  Wilmette, IL  60091");
					p.Position = thispos;

					p.Label = "Hey";

					_theMap.Pins.Add (p);

					_theMap.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
						Distance.FromMiles (0.1)));

					return false;
				});
			}

			if (Device.OS == TargetPlatform.iOS) {
				p.Address = _address;
				p.SetBinding (Pin.AddressProperty, _address);
				p.Position = thispos;
				p.Label = "Hey";
				_theMap.Pins.Add (p);

				_theMap.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
					Distance.FromMiles (0.1)));
			}


		}


		private Rectangle getOrdinalRect(int myordinal, int selectedOrdinal, int totalCount)
		{
			double pinStart = (App.ScaledWidth/2) - 10.5;
			Rectangle rectZero = new Rectangle(pinStart, 10, 21, 27);
			Rectangle rectMinusOne = new Rectangle(pinStart - 30, 10, 21, 27);
			Rectangle rectMinusTwo = new Rectangle(pinStart - 60, 10, 21, 27);
			Rectangle rectMinusThree = new Rectangle(pinStart - 90, 10, 21, 27);
			Rectangle rectMinusFour = new Rectangle(pinStart - 120, 10, 21, 27);
			Rectangle rectMinusFive = new Rectangle(pinStart - 150, 10, 21, 27);

			Rectangle rectPlusOne = new Rectangle(pinStart + 30, 10, 21, 27);
			Rectangle rectPlusTwo = new Rectangle(pinStart + 60, 10, 21, 27);
			Rectangle rectPlusThree = new Rectangle(pinStart + 90, 10, 21, 27);
			Rectangle rectPlusFour = new Rectangle(pinStart + 120, 10, 21, 27);
			Rectangle rectPlusFive = new Rectangle(pinStart + 150, 10, 21, 27);

			if (myordinal == selectedOrdinal) {
				return rectZero;
			}

			if (myordinal < selectedOrdinal) {
				//this is the minusrects
				switch (selectedOrdinal - myordinal) {
				case 1:
					return rectMinusOne;
					//break;
				case 2:
					return rectMinusTwo;
					//break;
				case 3:
					return rectMinusThree;
					//break;
				case 4:
					return rectMinusFour;
					//break;
				case 5:
					return rectMinusFive;
					//break;
				default:
					return Rectangle.Zero;
					//break;
				}
			}
			else{
				//these are the pluses
				switch (myordinal -selectedOrdinal)
				{
				case 1:
					return rectPlusOne;
					//break;
				case 2:
					return rectPlusTwo;
					//break;
				case 3:
					return rectPlusThree;
					//break;
				case 4:
					return rectPlusFour;
					//break;
				case 5:
					return rectPlusFive;
					//break;
				default:
					return Rectangle.Zero;
					//break;
				}
			}

			//return Rectangle.Zero;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 202;//162;
			this.IsEnabled = false;
//			StackLayout sl = new StackLayout ();
//			sl.Orientation = StackOrientation.Horizontal;		
//			sl.VerticalOptions = LayoutOptions.Start;
//
//			StackLayout slAddress = new StackLayout ();
//			slAddress.Orientation = StackOrientation.Vertical;
//
//			Label locationLabel = new Label ();
//			locationLabel.Text = "Home!";
//			//locationLabel.SetBinding (Label.TextProperty, "Home");
//			slAddress.Children.Add (locationLabel);
//
//			Label addresslabel = new Label ();
//			addresslabel.HorizontalOptions = LayoutOptions.Center;
//			addresslabel.Text = "427 Illinois Rd.  Wilmette, IL  60091";
//			//addresslabel.SetBinding (Label.TextProperty, "427 Illinois Rd  Wilmette, IL  60091");
//			slAddress.Children.Add (addresslabel);
//			sl.Children.Add(slAddress);



			AbsoluteLayout sl = new AbsoluteLayout ();
		

			_theMap = new ListMap ();
			//XLabs.Platform.Device.IDisplay disp;

			_theMap.WidthRequest = App.ScaledWidth;
			_theMap.HeightRequest = 202;
			_theMap.MinimumHeightRequest = 100;
			_theMap.MinimumWidthRequest = 200;
			_theMap.HorizontalOptions = LayoutOptions.CenterAndExpand;
			_theMap.MapType = MapType.Street;
			_theMap.IsShowingUser = false;
			_theMap.HasScrollEnabled = false;
			_theMap.HasZoomEnabled = false;


			sl.Children.Add (_theMap, new Point (0, 0));

			StackLayout header = new StackLayout ();
			header.BackgroundColor = Color.FromRgb (73,55,109);
			header.Orientation = StackOrientation.Horizontal;
			header.VerticalOptions = LayoutOptions.Start;
			header.HorizontalOptions = LayoutOptions.StartAndExpand;
			header.HeightRequest = 45;

			sl.Children.Add (header, new Rectangle(0,0,App.ScaledWidth, 45), AbsoluteLayoutFlags.None);



			//reality is that we need some cleansing here...at most we have 11 positions...the current one and 5 to either side
			//if the pinstring has more than 11, we need to find where our 11 should be

			if (_pinstring.Length > 11) {
				//where in the longer string does our selected ordinal sit?  how many should be behind it, how many in front of it?
				if (_currentOrdinal < 11) {
					if (_currentOrdinal > 5) {
						_pinstring = _pinstring.Substring (_currentOrdinal - 5, 11);
						_currentOrdinal = 5;
						//_currentOrdinal -= 5;
					} else {
						_pinstring = _pinstring.Substring (0, 11);
					}

				} else {

					//first get our startpoint (this is easy, since it's always 5 back
					int start = _currentOrdinal - 5;			

					//now see how far forward we can go
					int end = start + 11;
					if ((start + end) > _pinstring.Length ) {
						end = _pinstring.Length - start;
					}
					_pinstring = _pinstring.Substring (start, end);
					_currentOrdinal = 5;
				}
			}
				
			for (int i = 0; i < _pinstring.Length; i++) {

				string cs = _pinstring.Substring(i, 1);
				string imgsrc = "";
				switch (cs)
				{
					case "1":
					imgsrc = "icn_pin_check_sm.png";
						break;
					case "2":
					imgsrc = "icn_pin_up_pink_sm.png";
						break;
					case "3":
					imgsrc = "icn_pin_dwn_pink_sm.png";
						break;
					case "4":
					imgsrc = "icn_pin_up_grey_sm.png";
						break;
					case "5":
					imgsrc = "icn_pin_dwn_grey_sm.png";
						break;
				}
				Image img = new Image ();
				img.Source = imgsrc;
				Rectangle thisRect = getOrdinalRect (i, _currentOrdinal, _pinstring.Length);
				if (thisRect != Rectangle.Zero) {
					sl.Children.Add (img, thisRect, AbsoluteLayoutFlags.None);
				}
			}



//			Image imgPin = new Image ();
//			imgPin.Source = "icn_pin_check_sm.png";
//			sl.Children.Add(imgPin, rectMinusOne, AbsoluteLayoutFlags.None);
//
//			Image imgUp = new Image ();
//			imgUp.Source = "icn_pin_dwn_grey_sm.png";
//			sl.Children.Add (imgUp, rectPlusTwo, AbsoluteLayoutFlags.None);
//
//			Image imgDown = new Image ();
//			imgDown.Source = "icn_pin_up_grey_sm.png";
//			sl.Children.Add (imgDown, rectPlusOne, AbsoluteLayoutFlags.None);
//
//			Image imgPink = new Image ();
//			imgPink.Source = "icn_pin_dwn_pink_sm.png";
//			sl.Children.Add (imgPink, rectZero, AbsoluteLayoutFlags.None);



			Image imgPurple = new Image ();
			imgPurple.Source = "ui_tri_purple.png";
			imgPurple.HorizontalOptions = LayoutOptions.Center;

			double startPoint = (App.ScaledWidth/ 2) - 10;
			sl.Children.Add (imgPurple, new Rectangle (startPoint, 45, 20, 10), AbsoluteLayoutFlags.None);

			//need to create the gradient and the address text
			Image gradient = new Image();
			gradient.Source = "gradient.png";
			gradient.Aspect = Aspect.AspectFill;
			gradient.Opacity = 0.4;
			gradient.WidthRequest = App.ScaledWidth/ 2;
			sl.Children.Add(gradient, new Rectangle(0, 142, App.ScaledWidth, 60),AbsoluteLayoutFlags.None);

			whiteaddress = new Label ();
			whiteaddress.TextColor = Color.White;
			whiteaddress.FontSize = 14;
			whiteaddress.Text = _address;
			whiteaddress.LineBreakMode = LineBreakMode.WordWrap;
			whiteaddress.VerticalTextAlignment = TextAlignment.Center;
			whiteaddress.HorizontalTextAlignment = TextAlignment.Center;
			sl.Children.Add (whiteaddress, new Rectangle (0, 142, App.ScaledWidth, 60), AbsoluteLayoutFlags.None);

			View = sl;

		}
	}


	public class TrafficCell : ViewCell
	{
		decimal _diff;

		public TrafficCell(decimal diff)
		{
			_diff = diff;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;

			this.Height = 66;
			this.IsEnabled = false;

			Label est = new Label ();
			est.Text = "Leave by";
			est.FontSize = 16;
			est.TextColor = Color.Black;
			est.HorizontalOptions = LayoutOptions.StartAndExpand;
			est.VerticalOptions = LayoutOptions.Center;
			est.VerticalTextAlignment = TextAlignment.Center;
			est.TranslationX = 40;
			est.FontAttributes = FontAttributes.Bold;

			StackLayout slTraffic = new StackLayout ();
			slTraffic.Orientation = StackOrientation.Horizontal;
			slTraffic.WidthRequest = App.ScaledWidth;
			slTraffic.Padding = 20;	
			slTraffic.BackgroundColor = Color.FromRgb (238, 236, 243);
			slTraffic.Children.Add (est);

			Label traffic = new Label ();
			traffic.HorizontalOptions = LayoutOptions.EndAndExpand;
			traffic.VerticalOptions = LayoutOptions.Center;
			traffic.VerticalTextAlignment = TextAlignment.Center;
			traffic.FormattedText = new FormattedString ();
			traffic.WidthRequest = App.ScaledQuarterWidth - 40;
			//traffic.TranslationX = -10;
			//traffic.TranslationY = -2;
			//traffic.FormattedText.Spans.Add (new Span { Text = _diff.ToString(), FontSize= 24, ForegroundColor = Color.FromRgb(241, 179, 70), FontAttributes = FontAttributes.Bold });
			//traffic.FormattedText.Spans.Add (new Span { Text = " min", FontFamily=Device.OnPlatform("HelveticaNeue-Light", "", ""), FontSize = 24, ForegroundColor = Color.FromRgb(241, 179, 70), FontAttributes = FontAttributes.None });
			traffic.SetBinding (Label.FormattedTextProperty, "TravelString");

			slTraffic.Children.Add (traffic);


			View = slTraffic;

		}
	}


	public class ContactCell : ViewCell
	{
		public ContactCell()
		{
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			Today t = (Today)c;
			this.Height = 80;
			//this.IsEnabled = false;

			Label cntct = new Label ();
			cntct.Text = "CONTACT";
			cntct.FontSize = 14;
			cntct.TextColor = Color.Black;
			cntct.HorizontalOptions = LayoutOptions.StartAndExpand;
			cntct.VerticalOptions = LayoutOptions.Start;
			cntct.TranslationX = 5;
			cntct.FontAttributes = FontAttributes.Bold;

			StackLayout slContact = new StackLayout ();
			slContact.Orientation = StackOrientation.Vertical;
			slContact.WidthRequest = App.ScaledWidth;
			slContact.Padding =	20;
			slContact.BackgroundColor = Color.FromRgb (238, 236, 243);
			slContact.Children.Add (cntct);

			Label contactName = new Label ();
			contactName.FontFamily = Device.OnPlatform ("HelveticaNeue-Light", "", "");
			contactName.FontSize = 22;
			contactName.HorizontalOptions = LayoutOptions.StartAndExpand;
			contactName.Text = t.Via;
			contactName.TranslationX = 5;
			slContact.Children.Add (contactName);

			StackLayout slMain = new StackLayout ();

			slMain.Orientation = StackOrientation.Horizontal;
			slMain.BackgroundColor = Color.FromRgb (238, 236, 243);

			slMain.Children.Add (slContact);


			//now the contact buttons
//			ImageCircle.Forms.Plugin.Abstractions.CircleImage phoneButton = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
//				BorderColor = Color.FromRgb(241, 179, 70),
//				BorderThickness = 2,
//				Aspect = Aspect.AspectFill,
//				WidthRequest = 50,
//				HeightRequest = 50,
//				HorizontalOptions = LayoutOptions.Center,
//				Source = "icn_phone.png"
//			};	

			Button btnPhone = new Button ();
			btnPhone.Image = "icn_phone.png";
			btnPhone.BorderColor = Color.FromRgb (241, 179, 70);
			btnPhone.HorizontalOptions = LayoutOptions.Center;
			btnPhone.VerticalOptions = LayoutOptions.Center;
			btnPhone.BorderRadius = 25;
			btnPhone.BorderWidth = 4;
			btnPhone.HeightRequest = 50;
			btnPhone.MinimumHeightRequest = 50;
			btnPhone.WidthRequest = 50;
			btnPhone.MinimumWidthRequest = 50;
			btnPhone.BackgroundColor = Color.FromRgb (238, 236, 243);
			btnPhone.TranslationX = -30;
			slMain.Children.Add (btnPhone);
			btnPhone.Clicked += delegate(object sender, EventArgs e) {
				//await ((RouteDetail)this.ParentView.Parent.Parent).DisplayAlert ("Fetch!", "Call", "Cancel");
				if (App.Device.PhoneService == null)
				{
					App.hudder.showToast("Device does not support Phone calling");
					return;
				}
				App.Device.PhoneService.DialNumber(t.ViaPhone);

			};
				
			//don't need to message yourself...although
			if (t.AccountID != App.myAccount.id) {
				Button btnMessage = new Button ();
				btnMessage.Image = "icn_text.png";
				btnMessage.BorderColor = Color.FromRgb (241, 179, 70);
				btnMessage.HorizontalOptions = LayoutOptions.Center;
				btnMessage.VerticalOptions = LayoutOptions.Center;
				btnMessage.BorderRadius = 25;
				btnMessage.BorderWidth = 4;
				btnMessage.HeightRequest = 50;
				btnMessage.MinimumHeightRequest = 50;
				btnMessage.WidthRequest = 50;
				btnMessage.MinimumWidthRequest = 50;
				btnMessage.BackgroundColor = Color.FromRgb (238, 236, 243);
				btnMessage.TranslationX = -20;
				slMain.Children.Add (btnMessage);
				btnMessage.Clicked += async delegate(object sender, EventArgs e) {
					//load the ManageFetch screen...but without certain abilities
					MessagingCenter.Send<EmptyClass> (new EmptyClass (), "launchchat");
					//await ((RouteDetail)this.ParentView.Parent.Parent).DisplayAlert ("Fetch!", "Message", "Cancel");
				};
			}
			

			View = slMain;

		}
	}

	public class LocationContactCell : ViewCell
	{
		public LocationContactCell()
		{
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 80;
			//this.IsEnabled = false;
			Today t = (Today)c;


			Label cntct = new Label ();
			cntct.Text = "LOCATION CONTACT";
			cntct.FontSize = 14;
			cntct.TextColor = Color.Black;
			cntct.HorizontalOptions = LayoutOptions.StartAndExpand;
			cntct.VerticalOptions = LayoutOptions.Start;
			cntct.TranslationX = 5;
			cntct.FontAttributes = FontAttributes.Bold;

			StackLayout slContact = new StackLayout ();
			slContact.Orientation = StackOrientation.Vertical;
			slContact.WidthRequest = App.ScaledWidth;
			slContact.Padding =	20;
			slContact.BackgroundColor = Color.FromRgb (238, 236, 243);
			slContact.Children.Add (cntct);

			Label contactName = new Label ();
			contactName.FontFamily = Device.OnPlatform ("HelveticaNeue-Light", "", "");
			contactName.FontSize = 22;
			contactName.HorizontalOptions = LayoutOptions.StartAndExpand;
			contactName.Text = t.Location;
			contactName.TranslationX = 5;
			contactName.LineBreakMode = LineBreakMode.TailTruncation;
			slContact.Children.Add (contactName);

			StackLayout slMain = new StackLayout ();

			slMain.Orientation = StackOrientation.Horizontal;
			slMain.BackgroundColor = Color.FromRgb (238, 236, 243);

			slMain.Children.Add (slContact);


			Button btnPhone = new Button ();
			btnPhone.Image = "icn_phone.png";
			btnPhone.BorderColor = Color.FromRgb (241, 179, 70);
			btnPhone.HorizontalOptions = LayoutOptions.Center;
			btnPhone.VerticalOptions = LayoutOptions.Center;
			btnPhone.BorderRadius = 25;
			btnPhone.BorderWidth = 4;
			btnPhone.HeightRequest = 50;
			btnPhone.MinimumHeightRequest = 50;
			btnPhone.WidthRequest = 50;
			btnPhone.MinimumWidthRequest = 50;
			btnPhone.BackgroundColor = Color.FromRgb (238, 236, 243);
			btnPhone.TranslationX = -20;
			slMain.Children.Add (btnPhone);
			btnPhone.Clicked +=  delegate(object sender, EventArgs e) {
				//await ((RouteDetail)this.ParentView.Parent.Parent).DisplayAlert ("Fetch!", "Call", "Cancel");
				try{
				App.Device.PhoneService.DialNumber(t.LocationPhone);
				}
				catch (Exception ex)
				{
					App.hudder.showToast("Cannot access telephone");
				}
			};


			View = slMain;

		}
	}
}

