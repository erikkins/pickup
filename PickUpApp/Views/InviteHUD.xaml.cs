using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Labs;
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
			ts.Add (new ActivityCell (ViewModel.ThisInvite.Kids));
			ts.Add (new MapCell (ViewModel.ThisInvite.Latitude, ViewModel.ThisInvite.Longitude));

			ImageCell messageCell = new ImageCell ();
			messageCell.Text = "Send a message to " + _thisInviteInfo.Requestor;
			messageCell.ImageSource = "messageicongraysmall.jpg";
			messageCell.Height = 55;
			ts.Add (messageCell);

			ImageCell phoneCell = new ImageCell ();
			phoneCell.Text = "Call " + _thisInviteInfo.Requestor;
			phoneCell.ImageSource = "icon-phone.png";
			phoneCell.Height = 55;
			phoneCell.Tapped += async delegate(object sender, EventArgs e) {

				//var device = Resolver.Resolve<IDevice>();
				// not all devices have phone service, f.e. iPod and Android tablets
				// so we need to check if phone service is available
				//if (device.PhoneService != null)
				//{
				//	device.PhoneService.DialNumber("+1 (773) 619-1320");
				//}
			};
			ts.Add (phoneCell);



			ImageCell yelpCell = new ImageCell ();
			yelpCell.Text = "What to do?";
			yelpCell.Height = 55;
			yelpCell.ImageSource = ImageSource.FromUri(new Uri("https://s3-media1.fl.yelpcdn.com/assets/2/www/img/55e2efe681ed/developers/yelp_logo_50x25.png"));
			yelpCell.Tapped += async delegate(object sender, EventArgs e) {
				//ok, pop the popover!
				await Navigation.PushModalAsync(new YelpView(_thisInviteInfo.Latitude, _thisInviteInfo.Longitude));
			};
			ts.Add (yelpCell);

			tv.Root.Add (ts);
			stacker.Children.Add (tv);

			//we need to add the Yelp cells, but only once we get the data back, right?
//			ListView lv =new ListView();
//			DataTemplate dt = new DataTemplate (typeof(YelpCell));
//			lv.ItemTemplate = dt;
//			lv.ItemsSource = ViewModel.YelpBusinesses;
//			lv.HeightRequest = 50;
//			lv.VerticalOptions = LayoutOptions.End;
//			stacker.Children.Add (lv);

			//stacker.Children.Add (lv);

			Button btnGotIt = new Button ();
			btnGotIt.Text = "Picked Up";
			btnGotIt.TextColor = Color.Black;
			btnGotIt.FontSize = 18;
			btnGotIt.BackgroundColor = Color.Green;
			btnGotIt.VerticalOptions = LayoutOptions.End;
			btnGotIt.Clicked += async delegate(object sender, EventArgs e) {
				//need to let the Requestor know that all is good

				await Navigation.PopModalAsync();
			};
			stacker.Children.Add (btnGotIt);

			//let's see if we can yelp it up
			ViewModel.LoadItemsCommand.Execute (null);


			Device.StartTimer (new TimeSpan (0, 0, 10), () => {
				if (!ViewModel.IsLoading)
				{
					ViewModel.UpdateTrafficCommand.Execute(null);
				}
				return true;
			});

			Device.StartTimer (new TimeSpan (0, 0, 1), () => {
				//Issue is that the endtime isn't adjusting for UTC
				DateTime now = DateTime.Now;
				DateTime endtime = Util.RoundUp(now, TimeSpan.FromTicks(_thisInviteInfo.EndTimeTicks));
				if ( (endtime - now).Hours > 1)
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
		protected InviteHUDViewModel ViewModel
		{
			get { return this.BindingContext as InviteHUDViewModel; }
			set { this.BindingContext = value; }
		}
	}

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

	public class MapCell : ViewCell
	{
		private double _latitude, _longitude;

		public MapCell (double latitude, double longitude)
		{
			_latitude = latitude;
			_longitude = longitude;
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
			locationLabel.Text = "Home!";
			//locationLabel.SetBinding (Label.TextProperty, "ThisInvite.Location");
			slAddress.Children.Add (locationLabel);

			Label addresslabel = new Label ();
			addresslabel.HorizontalOptions = LayoutOptions.Center;
			addresslabel.SetBinding (Label.TextProperty, "ThisInvite.Address");
			slAddress.Children.Add (addresslabel);
			sl.Children.Add(slAddress);

			Xamarin.Forms.Maps.Map map = new Xamarin.Forms.Maps.Map ();
			map.WidthRequest = 200;
			map.HeightRequest = 200;
			map.MinimumHeightRequest = 100;
			map.MinimumWidthRequest = 100;
			map.HorizontalOptions = LayoutOptions.EndAndExpand;


			Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (_latitude, _longitude);
			map.Pins.Add (new Pin {
				Label = "",
				Position = thispos,
				Address = ""
			});

			map.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
				Distance.FromMiles (0.1)));

			sl.Children.Add (map);
			View = sl;
		
		
		}
	}

	public class ActivityCell: ViewCell
	{
		private string _kids;

		public ActivityCell (string Kids)
		{
			_kids = Kids;
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
			sl.VerticalOptions = LayoutOptions.Start;
		
			StackLayout slKids = new StackLayout ();
			slKids.Orientation = StackOrientation.Horizontal;

			string[] kiddos = _kids.Split (',');
			foreach (string k in kiddos) {
				StackLayout slThis = new StackLayout ();
				slThis.Orientation = StackOrientation.Vertical;
				slThis.HorizontalOptions = LayoutOptions.Center;
				ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
					BorderColor = Color.Black,
					BorderThickness = 1,
					Aspect = Aspect.AspectFill,
					WidthRequest = 100,
					HeightRequest = 100,
					HorizontalOptions = LayoutOptions.Center,
					Source = UriImageSource.FromUri (new Uri ("https://pickupapp.blob.core.windows.net/" + App.myAccount.id.ToLower() + "/" + k.Trim().ToLower() + ".jpg"))
				};

				slThis.Children.Add (ci);

				Label namelabel = new Label ();
				namelabel.HorizontalOptions = LayoutOptions.Center;
				//namelabel.SetBinding (Label.TextProperty, "Kids");
				namelabel.Text = k;
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
			slFull.Children.Add (activityLabel);

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

			Label trafficMinutesLabel = new Label ();
			trafficMinutesLabel.Text = "traffic\nminutes";
			trafficMinutesLabel.HorizontalOptions = LayoutOptions.End;
			sl.Children.Add (trafficMinutesLabel);

			View = sl;
		}
	}
}

