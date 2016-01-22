using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
//using Xamarin.Forms.Labs;
using XLabs.Forms;
using System.Threading.Tasks;

namespace PickUpApp
{
	public partial class InviteHUD : ContentPage
	{
		private InviteInfo _thisInviteInfo;


		public InviteHUD (InviteInfo thisInfo)
		{
			InitializeComponent ();
		

			//ok, this will show with clarity and urgency exactly WHO you are to pick up
			//WHERE you will picking them up and WHEN and HOW LONG it will take with traffic
			//ALSO it will show WHERE you can go afterwards using Yelp
			//and it will have a button that lets you share your current location
			//and it will have a button that says YES I PICKED UP
			//and we will use that to start keeping points on who's paying it forward
			//and we will work with retailers to determine coupons and other incentives
			//(perhaps post pickup?)
			//private messaging with the invitor? encrypted? whoa.
			_thisInviteInfo = thisInfo;
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			this.ViewModel = new InviteHUDViewModel (App.client, _thisInviteInfo);
			ViewModel.UpdateTrafficCommand.Execute (null);

			TableView tv = new TableView ();
			tv.HasUnevenRows = true;
			TableSection ts = new TableSection ();
			ts.Add (new TrafficTickerCell ());
			ActivityCell ac = new ActivityCell (ViewModel.ThisInvite.Kids, ViewModel.ThisInvite.AccountID);
			ac.Tapped +=  delegate(object sender, EventArgs e) {
				//ok, let's load up the allergen and kid info for these guys?
				DisplayAlert("Kid info", ac.KidInfo, "OK");
			};
			ts.Add (ac);

			TextCell tc = new TextCell ();
			if (!string.IsNullOrEmpty (ViewModel.ThisInvite.LocationMessage)) {
				tc.TextColor = Device.OnPlatform (Color.Black, Color.FromRgb(211,211,211), Color.Black);
				tc.Detail = ViewModel.ThisInvite.LocationMessage;

				ts.Add (tc);
			}

			MapCell mc = new MapCell (ViewModel.ThisInvite.Latitude, ViewModel.ThisInvite.Longitude, ViewModel.ThisInvite.Address, "",0);
			ts.Add (mc);
		
			mc.Tapped += Mc_Tapped;
//			mc.Tapped += async delegate(object sender, EventArgs e) {
//
//				Location loc = new Location();
//				loc.FullAddress = ViewModel.ThisInvite.Address;
//				loc.Latitude = ViewModel.ThisInvite.Latitude;
//				loc.Longitude = ViewModel.ThisInvite.Longitude;
//				loc.Name = ViewModel.ThisInvite.Activity;
//				LaunchMapApp(loc);
//					
//				//await Navigation.PushModalAsync(new TurnByTurnView(ViewModel.ThisInvite.Latitude, ViewModel.ThisInvite.Longitude, ViewModel.Itineraries));
//			};

			ImageCell messageCell = new ImageCell ();
			messageCell.TextColor = Device.OnPlatform (Color.Black, Color.FromRgb(211,211,211), Color.Black);
			messageCell.Text = "Send a message to " + _thisInviteInfo.Requestor;
			messageCell.ImageSource = "appbarmessage.png";
			messageCell.Height = 55;
			messageCell.Tapped += delegate(object sender, EventArgs e) {
				Navigation.PushModalAsync(new InviteMessageView(_thisInviteInfo));
			};

			ts.Add (messageCell);

			if (_thisInviteInfo.RequestorPhone != null) {

				ImageCell phoneCell = new ImageCell ();
				phoneCell.TextColor = Device.OnPlatform (Color.Black, Color.FromRgb(211,211,211), Color.Black);
				phoneCell.Text = "Call " + _thisInviteInfo.Requestor;
				phoneCell.ImageSource = "appbarphone.png";

//			beachImage.Source =  Device.OnPlatform(
//				iOS: ImageSource.FromFile("Images/waterfront.jpg"),
//				Android:  ImageSource.FromFile("waterfront.jpg"),
//				WinPhone: ImageSource.FromFile("Images/waterfront.png"));

				phoneCell.Height = 55;
				phoneCell.Tapped += delegate(object sender, EventArgs e) {
					var dep = DependencyService.Get<PickUpApp.IPhoneDialer> ();
					dep.DialPhone (_thisInviteInfo.RequestorPhone);
					//var device = Resolver.Resolve<IDevice>();
					// not all devices have phone service, f.e. iPod and Android tablets
					// so we need to check if phone service is available
					//if (device.PhoneService != null)
					//{
					//	device.PhoneService.DialNumber("+1 (773) 619-1320");
					//}
				};
				ts.Add (phoneCell);
			}

			ImageCell yelpCell = new ImageCell ();
			yelpCell.Text = "What to do after pickup?";
			yelpCell.Height = 55;
			yelpCell.TextColor = Device.OnPlatform (Color.Black, Color.FromRgb(211,211,211), Color.Black);
			yelpCell.ImageSource = "appbarsocialyelp.png"; //ImageSource.FromUri(new Uri("https://s3-media1.fl.yelpcdn.com/assets/2/www/img/55e2efe681ed/developers/yelp_logo_50x25.png"));
			yelpCell.Tapped += async delegate(object sender, EventArgs e) {
				//ok, pop the popover!
				await Navigation.PushModalAsync(new YelpView(_thisInviteInfo.Latitude, _thisInviteInfo.Longitude));
			};
			ts.Add (yelpCell);

			tv.Root.Add (ts);
			stacker.Children.Add (tv);

			this.Appearing += delegate {
				mc.Navigate ();
			};

			//we need to add the Yelp cells, but only once we get the data back, right?
//			ListView lv =new ListView();
//			DataTemplate dt = new DataTemplate (typeof(YelpCell));
//			lv.ItemTemplate = dt;
//			lv.ItemsSource = ViewModel.YelpBusinesses;
//			lv.HeightRequest = 50;
//			lv.VerticalOptions = LayoutOptions.End;
//			stacker.Children.Add (lv);

			//stacker.Children.Add (lv);



			Button btnCancel = new Button ();
			btnCancel.Text = "Close";
			btnCancel.TextColor = Color.Black;
			btnCancel.FontSize = 18;
			btnCancel.WidthRequest = 80;
			btnCancel.BackgroundColor = Color.Red;
			btnCancel.HorizontalOptions = LayoutOptions.Start;
			btnCancel.Clicked += async delegate(object sender, EventArgs e) {
				await Navigation.PopModalAsync();
			};


			Button btnGotIt = new Button ();
			btnGotIt.Text = "Picked Up";
			btnGotIt.TextColor = Color.Black;
			btnGotIt.FontSize = 18;
			btnGotIt.BackgroundColor = Color.Green;
			btnGotIt.HorizontalOptions = LayoutOptions.FillAndExpand;
			btnGotIt.Clicked += async delegate(object sender, EventArgs e) {
				//need to let the Requestor know that all is good

				await ViewModel.ExecuteAddEditCommand();

				//well, they've been picked up, but now we've gotta get them home!


				//await Navigation.PopModalAsync();
			};

			StackLayout slHoriz = new StackLayout ();
			slHoriz.Orientation = StackOrientation.Horizontal;
			slHoriz.VerticalOptions = LayoutOptions.End;
			slHoriz.HorizontalOptions = LayoutOptions.Fill;
			slHoriz.Children.Add (btnCancel);
			slHoriz.Children.Add (btnGotIt);

			stacker.Children.Add (slHoriz);


			MessagingCenter.Subscribe<Invite> (this, "Completed", async(s) => {


				//actually this only means we've picked them up...still need to return them home!
				ViewModel.OnReturn = true;
				ac = new ActivityCell (ViewModel.ThisInvite.Kids, ViewModel.ThisInvite.AccountID);
				//somehow need to refresh the ActivityCell
				ac.BindingContext = this.BindingContext;

				await ViewModel.UpdateTraffic();
				mc = new MapCell(ViewModel.ThisInvite.ReturnToLatitude, ViewModel.ThisInvite.ReturnToLongitude, ViewModel.ThisInvite.Address, "", 0);
				mc.BindingContext = this.BindingContext;
				//do we have to remove the old async delegate?
				//mc.Tapped -= Mc_Tapped;



				//tc pertains to location text on the way THERE
				tc.Text = "";
				//do we have to remote the old async delegate?
				yelpCell.Tapped += async delegate(object sender, EventArgs e) {
					//ok, pop the popover!
					await Navigation.PushModalAsync(new YelpView(_thisInviteInfo.ReturnToLatitude, _thisInviteInfo.ReturnToLongitude));
				};

				btnGotIt.Text = "Done";


				//				try{
				//					await Navigation.PopModalAsync();
				//				}
				//				catch(Exception ex)
				//				{
				//					System.Diagnostics.Debug.WriteLine("POP SUX:" + ex.Message + ex.StackTrace);
				//				}
				//				MessagingCenter.Send<string>("InviteHud", "NeedsRefresh");

			});

			MessagingCenter.Subscribe<Invite> (this, "Returned", async(sim) => {
				try{
					await Navigation.PopModalAsync();
				}
				catch(Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("POP SUX:" + ex.Message + ex.StackTrace);
				}
				MessagingCenter.Send<string>("InviteHud", "NeedsRefresh");
			});

			//let's see if we can yelp it up
			//ViewModel.LoadItemsCommand.Execute (null);


			Device.StartTimer (new TimeSpan (0, 0, 10), () => {
				if (ViewModel.OnReturn)
				{
					return false;
				}
				if (!ViewModel.IsLoading)
				{
					ViewModel.UpdateTrafficCommand.Execute(null);
				}
				return true;
			});

			Device.StartTimer (new TimeSpan (0, 0, 1), () => {
				if (ViewModel.OnReturn)
				{
					ViewModel.Countdown = "0:00";
					return false;
				}
				//Issue is that the endtime isn't adjusting for UTC
				DateTime endtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				DateTime now = DateTime.Now;
				TimeSpan tsEnd = TimeSpan.FromTicks(_thisInviteInfo.EndTimeTicks);
				endtime += tsEnd;

				if ( (endtime - now).Hours >= 1)
				{
					ViewModel.Countdown = (endtime - now).Hours.ToString() + ":" + (endtime - now).Minutes.ToString().PadLeft(2, '0');
				}
				else
				{
					ViewModel.Countdown = (endtime - now).Minutes.ToString() + ":" + (endtime - now).Seconds.ToString().PadLeft(2, '0');
				}
				return true;
			});
		}

		void Mc_Tapped (object sender, EventArgs e)
		{
			Location loc = new Location();
			loc.FullAddress = ViewModel.ThisInvite.Address;
			loc.Latitude = ViewModel.ThisInvite.Latitude.ToString();
			loc.Longitude = ViewModel.ThisInvite.Longitude.ToString();
			loc.Name = ViewModel.ThisInvite.Address;
			LaunchMapApp(loc);
		}
		protected InviteHUDViewModel ViewModel
		{
			get { return this.BindingContext as InviteHUDViewModel; }
			set { this.BindingContext = value; }
		}

		public void LaunchMapApp(Location loc) {
			// Windows Phone doesn't like ampersands in the names and the normal URI escaping doesn't help
			var name = loc.Name.Replace("&", "and"); // var name = Uri.EscapeUriString(place.Name);
			var location = string.Format("{0},{1}",loc.Latitude, loc.Longitude);
			var addr = Uri.EscapeUriString(loc.FullAddress);

			var request = Device.OnPlatform(
				// iOS doesn't like %s or spaces in their URLs, so manually replace spaces with +s
				string.Format("http://maps.apple.com/maps?q={0}&sll={1}", name.Replace(' ', '+'), location),

				// pass the address to Android if we have it
				string.Format("geo:0,0?q={0}({1})", string.IsNullOrWhiteSpace(addr) ? location : addr, name),

				// WinPhone
				string.Format("bingmaps:?cp={0}&q={1}", location, name)
			);

			Device.OpenUri(new Uri(request));
		}
	}

	//Android only tap workaround
	public class ListMapHUD : Map { }

	public class YelpCell : ViewCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();
		
			dynamic c = BindingContext;

			StackLayout sl = new StackLayout ();
			sl.Padding = new Thickness (0, Device.OnPlatform (20, 0, 0), 0, 0);
			sl.Orientation = StackOrientation.Horizontal;
			Label l = new Label ();
			l.SetBinding (Label.TextProperty, "YelpBusinesses.Name");
			sl.Children.Add (l);

			View = sl;

		}
	}

	public class MapCellHUD : ViewCell
	{
		private double _latitude, _longitude;
		private ListMap _theMap;

		public MapCellHUD (double latitude, double longitude)
		{
			_latitude = latitude;
			_longitude = longitude;

		}



		public void Navigate()
		{
			//MEGA KLUDGE FOR ANDROID! Map somehow loads twice...correctly, then Africa.  Gotta load it again!
			if (Device.OS == TargetPlatform.Android) {
				Device.StartTimer (new TimeSpan (0, 0, 2), () => {

					Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (_latitude, _longitude);

					Pin p = new Pin ();
					p.SetBinding (Pin.AddressProperty, "ThisInvite.Address");
					p.Position = thispos;
					p.Label = "Hey";

					_theMap.Pins.Add (p);

					_theMap.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
						Distance.FromMiles (0.1)));

					return false;
				});
			} else {
				Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (_latitude, _longitude);

				Pin p = new Pin ();
				p.SetBinding (Pin.AddressProperty, "ThisInvite.Address");
				p.Position = thispos;
				p.Label = "Hey";

				_theMap.Pins.Add (p);

				_theMap.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
					Distance.FromMiles (0.1)));
			}


		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 150;
			this.IsEnabled = false;
			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;		
			sl.VerticalOptions = LayoutOptions.Start;

			StackLayout slAddress = new StackLayout ();
			slAddress.Orientation = StackOrientation.Vertical;

			Label locationLabel = new Label ();
			//locationLabel.Text = "Home!";
			locationLabel.SetBinding (Label.TextProperty, "ThisInvite.Location");
			slAddress.Children.Add (locationLabel);

			Label addresslabel = new Label ();
			addresslabel.HorizontalOptions = LayoutOptions.Center;
			addresslabel.SetBinding (Label.TextProperty, "ThisInvite.Address");
			slAddress.Children.Add (addresslabel);
			sl.Children.Add(slAddress);

			_theMap = new ListMap ();
			_theMap.WidthRequest = 200;
			_theMap.HeightRequest = 200;
			_theMap.MinimumHeightRequest = 100;
			_theMap.MinimumWidthRequest = 100;
			_theMap.HorizontalOptions = LayoutOptions.EndAndExpand;
			_theMap.MapType = MapType.Street;
			_theMap.IsShowingUser = false;
			_theMap.HasScrollEnabled = false;
			_theMap.HasZoomEnabled = false;

			sl.Children.Add (_theMap);
				
			View = sl;
		
		
		}
	}

	public class ActivityCell: ViewCell
	{
		private string _kids;
		private string _accountID;

		public string KidInfo {
			get;
			set;
		}

		public ActivityCell (string Kids, string AccountID)
		{
			_kids = Kids;
			_accountID = AccountID;
		}
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			dynamic c = BindingContext;
			this.Height = 150;
			StackLayout sl = new StackLayout ();
			sl.Padding = new Thickness (0, Device.OnPlatform (20, 0, 0), 0, 0);
			sl.Orientation = StackOrientation.Vertical;
			sl.HorizontalOptions = LayoutOptions.Center;
			sl.VerticalOptions = LayoutOptions.Center;
		
			StackLayout slKids = new StackLayout ();
			slKids.Orientation = StackOrientation.Horizontal;

			string[] kiddos = _kids.Split ('^');
			foreach (string k in kiddos) {
				StackLayout slThis = new StackLayout ();
				slThis.Orientation = StackOrientation.Vertical;
				slThis.HorizontalOptions = LayoutOptions.Center;

				var uis = new UriImageSource ();
				uis.CacheValidity = new TimeSpan (0, 5, 0);
				uis.CachingEnabled = true;

				//ok the new kid is name|id so we need to parse that bad bear
				string[] kidparts = k.Split('|');
				string kidname = kidparts [0];
				string kidid = kidparts [1].ToLower();
				string kiddob = kidparts [2];
				string kidallergies = kidparts [3];
				string kidgender = kidparts [4];

				KidInfo += kidname + Environment.NewLine + "=========" + Environment.NewLine;
				KidInfo += "DOB: " + kiddob + Environment.NewLine;
				KidInfo += "Gender: " + kidgender + Environment.NewLine;
				if (kidallergies.Length > 0) {
					KidInfo += "Allergies: " + kidallergies + Environment.NewLine;
				}
				KidInfo += Environment.NewLine;

				//wow, this cannot by MY account number..it's got to be the Invitor's account id
				//string azureURL = AzureStorageConstants.BlobEndPoint + App.myAccount.id.ToLower () + "/" + k.Trim ().ToLower () + ".jpg";
				string azureURL = AzureStorageConstants.BlobEndPoint + _accountID.ToLower() + "/" + kidid.Trim ().ToLower () + ".jpg";
				uis.Uri = new Uri (azureURL);

				ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
					BorderColor = Color.Black,
					BorderThickness = 1,
					Aspect = Aspect.AspectFill,
					WidthRequest = 100,
					HeightRequest = 100,
					HorizontalOptions = LayoutOptions.Center,
					//AzureStorageConstants.BlobEndPoint + App.myAccount.id.ToLower() + "/" + ViewModel.CurrentKid.Firstname.ToLower() + ".jpg";
					Source = uis
				};					
	
				slThis.Children.Add (ci);
				ci = null;
				//GC.Collect ();

				Label namelabel = new Label ();
				namelabel.HorizontalOptions = LayoutOptions.Center;
				//namelabel.SetBinding (Label.TextProperty, "Kids");
				namelabel.Text = kidname;
				slThis.Children.Add (namelabel);
				slKids.Children.Add (slThis);
			}

			sl.Children.Add (slKids);


			StackLayout slFull = new StackLayout ();
			if (kiddos.Length > 2) {
				slFull.Orientation = StackOrientation.Vertical;
				this.Height += 45;
			} else {
				slFull.Orientation = StackOrientation.Horizontal;
			}
			slFull.VerticalOptions = LayoutOptions.FillAndExpand;
			slFull.Children.Add (sl);

			Label activityLabel = new Label ();
			activityLabel.FontSize = 32;
			activityLabel.SetBinding (Label.TextProperty, "ThisInvite.Activity");
			activityLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
			activityLabel.VerticalOptions = LayoutOptions.Center;
			slFull.Children.Add (activityLabel);

//			Label locationMessageLabel = new Label ();
//			activityLabel.FontSize = 14;
//			activityLabel.SetBinding (Label.TextProperty, "ThisInvite.LocationMessage");
//			activityLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
//			activityLabel.VerticalOptions = LayoutOptions.End;
//			slFull.Children.Add (locationMessageLabel);

			View = slFull;
		}
	}

	public class TrafficTickerCell : ViewCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			dynamic c = BindingContext;
			this.Height = 50;
			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;
			sl.VerticalOptions = LayoutOptions.Center;
			Label namelabel = new Label ();
			namelabel.FontSize = 32;
			namelabel.HorizontalOptions = LayoutOptions.Start;
			namelabel.SetBinding (Label.TextProperty, "Countdown");
			//namelabel.Text = "TIMETIL";
			sl.Children.Add (namelabel);

			Label timetilLabel = new Label ();
			timetilLabel.Text = "until\npickup";
			namelabel.HorizontalOptions = LayoutOptions.Start;
			sl.Children.Add (timetilLabel);

			BoxView bv = new BoxView ();
			bv.Color = Color.Gray;
			bv.HeightRequest = this.Height;
			bv.WidthRequest = 2;
			bv.HorizontalOptions = LayoutOptions.CenterAndExpand;
			sl.Children.Add (bv);

			Label startlabel = new Label();
			startlabel.SetBinding (Label.TextProperty, "TrafficTime");
			startlabel.HorizontalOptions = LayoutOptions.End;
			startlabel.FontSize = 32;
			//startlabel.Text = "26";
			sl.Children.Add (startlabel);

			ActivityIndicator trafficLoading = new ActivityIndicator ();
			trafficLoading.SetBinding (ActivityIndicator.IsRunningProperty, "LocationUpdating");

			Label trafficMinutesLabel = new Label ();
			trafficMinutesLabel.Text = "traffic\nminutes";
			trafficMinutesLabel.HorizontalOptions = LayoutOptions.End;
			sl.Children.Add (trafficMinutesLabel);

			View = sl;
		}
	}
}

