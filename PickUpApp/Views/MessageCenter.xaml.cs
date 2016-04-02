using System;
using System.Collections.Generic;
using XLabs.Forms.Controls;
using PickUpApp.ViewModels;
using Xamarin.Forms;

namespace PickUpApp
{
	public partial class MessageCenter : ContentPage
	{
		public MessageCenter ()
		{
			InitializeComponent ();
			this.ViewModel = new MessageViewModel (App.client, null);		
			this.Icon = "icn_back.png";

			ExtendedListView elv = new ExtendedListView ();
			elv.BackgroundColor = AppColor.AppGray;
			elv.HasUnevenRows = true;
			elv.RefreshCommand = ViewModel.LoadItemsCommand;
			elv.ItemsSource = App.myMessages;
			elv.ItemTemplateSelector = new MessageTemplateSelector ();
			elv.SeparatorVisibility = SeparatorVisibility.None;
			elv.Header = null;
			elv.IsPullToRefreshEnabled = true;

			elv.BindingContextChanged += delegate(object sender, EventArgs e) {
				//System.Diagnostics.Debug.WriteLine("BIGBINDING CHANGED");
			};
			stacker.Children.Add (elv);

			//this.ViewModel.ExecuteLoadItemsCommand ().ConfigureAwait(false);
			MessagingCenter.Unsubscribe<string>(this, "messagesloaded");
			MessagingCenter.Subscribe<string> (this, "messagesloaded", (s) => {
				//elv.IsRefreshing = false;
				//Device.BeginInvokeOnMainThread(async()=>{
					//System.Diagnostics.Debug.WriteLine("MESSAGES LOADED");
					App.hudder.hideHUD();
					if (App.myMessages.Count == 0)
					{
						//System.Diagnostics.Debug.WriteLine ("PopAsync from MessageCenter after messagesloaded");
						//try{
						MessagingCenter.Unsubscribe<RespondMessage> (this, "messageresponse");
						MessagingCenter.Unsubscribe<RespondMessage> (this, "messagesupdated");
						MessagingCenter.Unsubscribe<string>(this, "messagesloaded");
						Navigation.PopAsync();
						
						//}
						//catch{}

					}
				//});
			});

			MessagingCenter.Unsubscribe<RespondMessage> (this, "messagesupdated");
			MessagingCenter.Subscribe<RespondMessage>(this, "messagesupdated", (rm) =>
			{
					if (rm==null)
					{
						return;
					}
					Device.BeginInvokeOnMainThread(async()=>{
						//we need to know if there are any other collections
						//that need updating as a result of this...
						//TODO: add support for reloading different sections based upon the response type
						switch (rm.PostUpdate)
						{
						case "today":
							App.hudder.showHUD("Refreshing Today");
							//we're doing this basically anyway at the bottom of this
							//MessagingCenter.Send<string>("msgupdate", "NeedsRefresh");
							/*
							this.BindingContext = new TodayViewModel(App.client);
							App.hudder.showHUD("Loading Today");
							await ((TodayViewModel)BindingContext).ExecuteLoadItemsCommand();
							App.hudder.hideHUD();
							*/
							break;
						case "circlekids":

							MessageViewModel bc = this.BindingContext as MessageViewModel;

							this.BindingContext = new KidsViewModel(App.client);
							//lblActivity.Text = "Loading Kids";
							App.hudder.showHUD("Loading Kids");
							await ((KidsViewModel)BindingContext).ExecuteLoadItemsCommand();

							this.BindingContext = new MyCircleViewModel(App.client);
							//lblActivity.Text = "Loading Circle";
							App.hudder.showHUD("Loading Circle");
							await ((MyCircleViewModel)BindingContext).ExecuteLoadItemsCommand();
							App.hudder.hideHUD();

							this.BindingContext = bc;
							break;
						}


						if (this.ViewModel == null)
						{
							System.Diagnostics.Debug.WriteLine ("VIEWMODEL WAS NULL!");
							this.ViewModel = new MessageViewModel(App.client, null);
						}

						//System.Diagnostics.Debug.WriteLine ("LOADING ITEMS FROM MESSAGESUPDATED");
						await this.ViewModel.ExecuteLoadItemsCommand();


					});
					//elv.IsRefreshing = false;

			});

			MessagingCenter.Unsubscribe<RespondMessage> (this, "messageresponse");
			MessagingCenter.Subscribe<RespondMessage> (this, "messageresponse", (mr) => {
				Device.BeginInvokeOnMainThread(async()=>{

					if (!string.IsNullOrEmpty(mr.Conditional))
					{
						bool ret = DisplayAlert("Preemptive Check", mr.Conditional, "Continue", "Cancel");
						if (!ret)
						{
							return;
						}
					}
						
					App.hudder.showHUD("Saving Message");
					this.ViewModel.CurrentMessageResponse = mr;
					//System.Diagnostics.Debug.WriteLine ("LOADING ITEMS AFTER MESSAGERESPONSE");
				    await this.ViewModel.ExecuteAddEditCommand();
				});
			});

//			this.Disappearing += delegate(object sender, EventArgs e) {
//				MessagingCenter.Unsubscribe<string> (this, "messagesloaded");
//				MessagingCenter.Unsubscribe<RespondMessage> (this, "messagesupdated");
//			};

		}

		protected MessageViewModel ViewModel
		{
			get { return this.BindingContext as MessageViewModel; }
			set { this.BindingContext = value; }
		}
	}

	public class MessageTemplateSelector : XLabs.Forms.Controls.DataTemplateSelector
	{
		public DataTemplate FetchRequest = new DataTemplate (typeof(PickupRequestCell));
		public DataTemplate CircleRequest = new DataTemplate (typeof(CircleRequestCell));
		public DataTemplate MessageTemplate = new DataTemplate (typeof(MessageCell));


		public override DataTemplate SelectTemplate (object item, BindableObject container)
		{
			System.Diagnostics.Debug.WriteLine ("TEMPLATE REQ");
			var msg = (MessageView)item;
			switch (msg.MessageType) {
			case "pickup":				
				return FetchRequest;
				//break;
			case "circle":
				return CircleRequest;
				//break;
			case "message":
				return MessageTemplate;
				//break;
			default:
				return null;
				//break;
			}
			//return base.SelectTemplate (item, container);
		}
	}

	public class PickupRequestCell : ViewCell
	{
		private static int CountOfNewlines(string s)
		{
			if (string.IsNullOrEmpty (s)) {
				return 0;
			}
			try{
				int n = 0;
				foreach( var c in s )
				{
					if ( c == '\n' ) n++;
					if (c == ',')
						n++;
				}
				return n+1;
			}
			catch {
				return 0;
			}
		}
		private bool _isActionable = true;
		public bool IsActionable
		{
			get { return _isActionable; }
			set { _isActionable = value; }
		}
//		public PickupRequestCell (bool IsActionable = false)
//		{
//			_isActionable = IsActionable;
//		}

		protected override void OnBindingContextChanged()
		{
			System.Diagnostics.Debug.WriteLine ("BINDING CONTEXT");
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;

			MessageView mv = (MessageView)c;
			if (mv == null) {
				return;
			}

			_isActionable = mv.IsActionable;

			StackLayout slMain = new StackLayout ();

			slMain.Orientation = StackOrientation.Vertical;
			slMain.WidthRequest = App.ScaledWidth - 20;
			slMain.HorizontalOptions = LayoutOptions.FillAndExpand;
			slMain.Spacing = 0;

			//add some space
			BoxView bv = new BoxView();
			bv.HeightRequest = 10;
			slMain.Children.Add (bv);

			//top box
			StackLayout sl = new StackLayout ();
			sl.Padding = new Thickness (10, 10, 10, 10);
			sl.Orientation = StackOrientation.Horizontal;
			sl.BackgroundColor = Color.White;
			sl.HorizontalOptions = LayoutOptions.FillAndExpand;

			StackLayout slVert = new StackLayout ();
			slVert.Orientation = StackOrientation.Vertical;

			Label l = new Label ();
			l.Text = "Request for Pickup";
			l.FontFamily = "HelveticaNeue-Light";
			l.FontSize = 24;
			l.HorizontalOptions = LayoutOptions.StartAndExpand;
			slVert.Children.Add (l);

			Label lFrom = new Label();
			if (_isActionable) {
				lFrom.Text = "from " + mv.Sender + " • " + DateTimeExtensions.GetTimeSpan (mv.Created.ToLocalTime ().DateTime);
			} else {
				lFrom.Text = "from " + App.myAccount.Fullname + " • " + DateTimeExtensions.GetTimeSpan (DateTime.Now.ToLocalTime ());

			}
			lFrom.TextColor = Color.FromRgb (157, 157, 157);
			slVert.Children.Add (lFrom);

			//we need a label saying the actual DATE when this will occur
			bool isToday = false;
			Label lActivityDate = new Label ();

			if (DateTime.Now.Date == mv.ScheduleDate.Date) {
			//if (DateTime.Now.Date == mv.ScheduleDate.Date) {
			//if (DateTime.UtcNow.Date.ToLocalTime().Date == mv.ScheduleDate.ToUniversalTime().Date){
				lActivityDate.Text = "for TODAY";
				isToday = true;
	
			} else {
				lActivityDate.Text = "for " + mv.ScheduleDate.ToString ("dddd") + " " + mv.ScheduleDate.ToString ("d");
			}
			lActivityDate.TextColor = Color.FromRgb (157, 157, 157);
			slVert.Children.Add (lActivityDate);

			if (!string.IsNullOrEmpty (mv.Message)) {
				Label lMessage = new Label ();
				lMessage.Text = mv.Message;
				slVert.Children.Add (lMessage);
			}


			sl.Children.Add (slVert);

			ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
				BorderColor = Color.Black,
				BorderThickness = 1,
				Aspect = Aspect.AspectFill,
				WidthRequest = 50,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Source= mv.SenderPhotoURL
			};	
			sl.Children.Add (ci);

			StackLayout slBorder = new StackLayout ();
			slBorder.WidthRequest = App.ScaledWidth - 20;
			slBorder.BackgroundColor = Color.FromRgb (157, 157, 157);
			slBorder.Padding = new Thickness (0.5);
			slBorder.Children.Add (sl);
			slBorder.HorizontalOptions = LayoutOptions.Center;
			slMain.Children.Add (slBorder);

			//now add the pickup detail (ripped this straight out of the TodayCell

			Today t = mv.MessageToday;
			StackLayout mainlayout = new StackLayout ();
			mainlayout.Spacing = 0;
			mainlayout.Orientation = StackOrientation.Vertical;
			mainlayout.VerticalOptions = LayoutOptions.StartAndExpand;
			mainlayout.BackgroundColor = Color.FromRgb (73, 55, 55);//109);
			mainlayout.HeightRequest = 155;

			Color bgColor = Color.White;

			//add a spacer for the grid...20px or so
			BoxView bv2 = new BoxView();
			bv2.BackgroundColor = bgColor;

			bv2.HeightRequest = 20;
			mainlayout.Children.Add (bv2);

			//ok, now add the details

			Grid detailGrid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = bgColor,
				RowSpacing = 0,
				//ColumnSpacing = 0,
				RowDefinitions = 
				{
					new RowDefinition { Height = new GridLength(25, GridUnitType.Absolute) },
					new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)},
					new RowDefinition {Height = new GridLength(48, GridUnitType.Absolute) }
				},
				ColumnDefinitions = 
				{
					new ColumnDefinition { Width = new GridLength(58, GridUnitType.Absolute) },
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(46, GridUnitType.Absolute) }
				}
				};		//start nav images
			//there are 3 phases...
			// 1) complete (gray background with green pin check) [autocomplete after 15 minutes?]
			// 2) coming up next (white background with pink pin check)
			// 3) future (grab background with gray pin check) [both up and down]

			const string trigreen = "ui_tri_green.png";
			const string linegreen = "ui_line_green.png";
			const string pingreen = "icn_pin_check.png";
			const string tripink = "ui_tri_pink.png";
			const string linepink = "ui_line_dash_pink.png";
			const string pinuppink = "icn_pin_up_pink.png";
			const string pindownpink = "icn_pin_dwn_pink";
			const string trigray = "ui_tri_grey.png";
			const string linegray = "ui_line_dash_grey.png";
			const string pindowngray = "icn_pin_dwn_grey.png";
			const string pinupgray = "icn_pin_up_grey";

			const string arrowpink = "icn_arrow_pink.png";
			const string arrowgray = "icn_arrow_grey.png";


			Image triangle = new Image ();
			triangle.Source = tripink;
			triangle.Aspect = Aspect.AspectFit;
			triangle.HorizontalOptions = LayoutOptions.Center;
			triangle.VerticalOptions = LayoutOptions.End;
			detailGrid.Children.Add (triangle, 0, 1, 0, 1);


			//line.HeightRequest = 55;
			//detailGrid.Children.Add (line, 0, 1, 1, 2);

			StackLayout slLine = new StackLayout ();
			slLine.HeightRequest = 55;
			slLine.Orientation = StackOrientation.Vertical;
			slLine.Spacing = 0;
			slLine.Padding = new Thickness (0);
			slLine.VerticalOptions = LayoutOptions.StartAndExpand;
			slLine.HorizontalOptions = LayoutOptions.Center;
			//we're now going to add 10 line images into it to make the dashed lines dashed
			for (int i = 0; i < 10; i++) {
				Image line = new Image ();
				line.Aspect = Aspect.AspectFill;
				line.VerticalOptions = LayoutOptions.CenterAndExpand;
				line.HorizontalOptions = LayoutOptions.Center;
				line.Source = linepink;
				slLine.Children.Add (line);
			}
			detailGrid.Children.Add (slLine, 0, 1, 1, 2);


			Image pin = new Image ();
		
			if (t.IsPickup) {
				pin.Source = pinuppink;
			} else {
				pin.Source = pindownpink;
			}
				
			pin.HorizontalOptions = LayoutOptions.Center;
			pin.VerticalOptions = LayoutOptions.Start;
			detailGrid.Children.Add (pin, 0, 1, 2, 3);

			//end nav images


			//if today, do a traffic calculation and say "From current location"
			//otherwise, hide this altogether

			double travelMinutes = 0;
			double travelDistance = 0;
			if (isToday) {
				DistanceService ds = new DistanceService ();
				Location startLoc = new Location ();
				startLoc.Latitude = App.PositionLatitude;
				startLoc.Longitude = App.PositionLongitude;

				Location endLoc = new Location ();
				endLoc.Latitude = t.Latitude;
				endLoc.Longitude = t.Longitude;
				ds.StartingLocation = startLoc;
				ds.EndingLocation = endLoc;
				//ds.CalculateDriveTime ();
				var task = System.Threading.Tasks.Task.Run(async () => await ds.CalculateDriveTime());
				task.Wait();
				travelMinutes = ds.TravelTime;
				travelDistance = ds.Distance;
			}


			Label l0 = new Label ();
			if (mv.MessageToday.IsPickup) {
				DateTime intermediate = DateTime.Today.Add (t.TSPickup.Subtract (TimeSpan.FromMinutes (travelMinutes)));
				l0.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture);
				//l.Text = DateTime.Parse (t.TSPickup).AddMinutes (-t.EndPlaceTravelTime).ToLocalTime ().ToString ("t");
			} else {
				DateTime intermediate = DateTime.Today.Add (t.TSDropOff.Subtract (TimeSpan.FromMinutes (travelMinutes)));
				l0.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture);
				//l.Text = DateTime.Parse (t.TSDropOff).AddMinutes (-t.StartPlaceTravelTime).ToLocalTime ().ToString ("t");
			}
			l0.VerticalOptions = LayoutOptions.Start;
			l0.FontAttributes = FontAttributes.Bold;
			detailGrid.Children.Add (l0, 1, 2, 0, 2);

			Label l2 = new Label ();
			l2.VerticalOptions = LayoutOptions.StartAndExpand;
			l2.FormattedText = new FormattedString ();
			if (t.IsPickup) {
				l2.FormattedText.Spans.Add (new Span { Text = "Leave current location", ForegroundColor = Color.Black });
				l2.FormattedText.Spans.Add (new Span {
					Text = "\nDrive " + Math.Round(travelDistance,1) + " miles",
					ForegroundColor = Color.Gray,
					FontAttributes = FontAttributes.Italic
				});
			} else {
				l2.FormattedText.Spans.Add (new Span { Text = "Leave current location", ForegroundColor = Color.Black });
				l2.FormattedText.Spans.Add (new Span {
					Text = "\nDrive " + Math.Round(travelDistance,1) + " miles",
					ForegroundColor = Color.Gray,
					FontAttributes = FontAttributes.Italic
				});
			}

			//if this is for another day, we'll calculate the time/distance then
			if (!isToday) {
				l0.Text = "";
				l2.Text = "";
			}


//			Label l0 = new Label ();
//			if (mv.MessageToday.IsPickup) {
//				DateTime intermediate = DateTime.Today.Add (t.TSPickup.Subtract (TimeSpan.FromMinutes (t.EndPlaceTravelTime)));
//				l0.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture);
//				//l.Text = DateTime.Parse (t.TSPickup).AddMinutes (-t.EndPlaceTravelTime).ToLocalTime ().ToString ("t");
//			} else {
//				DateTime intermediate = DateTime.Today.Add (t.TSDropOff.Subtract (TimeSpan.FromMinutes (t.StartPlaceTravelTime)));
//				l0.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture);
//				//l.Text = DateTime.Parse (t.TSDropOff).AddMinutes (-t.StartPlaceTravelTime).ToLocalTime ().ToString ("t");
//			}
//			l0.VerticalOptions = LayoutOptions.Start;
//			l0.FontAttributes = FontAttributes.Bold;
//			detailGrid.Children.Add (l0, 1, 2, 0, 2);
//
//			Label l2 = new Label ();
//			l2.VerticalOptions = LayoutOptions.StartAndExpand;
//			l2.FormattedText = new FormattedString ();
//			if (t.IsPickup) {
//				l2.FormattedText.Spans.Add (new Span { Text = "Leave " + t.EndPlaceName, ForegroundColor = Color.Black });
//				l2.FormattedText.Spans.Add (new Span {
//					Text = "\nDrive " + Math.Round(t.EndPlaceDistance,1) + " miles",
//					ForegroundColor = Color.Gray,
//					FontAttributes = FontAttributes.Italic
//				});
//			} else {
//				l2.FormattedText.Spans.Add (new Span { Text = "Leave " + t.StartPlaceName, ForegroundColor = Color.Black });
//				l2.FormattedText.Spans.Add (new Span {
//					Text = "\nDrive " + Math.Round(t.StartPlaceDistance,1) + " miles",
//					ForegroundColor = Color.Gray,
//					FontAttributes = FontAttributes.Italic
//				});
//			}




			l2.FontAttributes = FontAttributes.Bold;
			detailGrid.Children.Add (l2, 2, 3, 0, 2 );

//			Button b = new Button ();
//			b.Image = arrowpink;
//			b.HorizontalOptions = LayoutOptions.Center;
//			b.VerticalOptions = LayoutOptions.Start;
//			b.Clicked += async delegate(object sender, EventArgs e) {
//				//await ((TodayView)this.ParentView.Parent.Parent).DisplayAlert ("Fetch!", "create a fetch request", "Cancel");
//				await ((TodayView)this.ParentView.Parent.Parent).Navigation.PushAsync(new FetchRequest1());
//			};
//			detailGrid.Children.Add (b, 3, 4, 0, 1);

			if (t.TrafficWarning) {
				Image i2 = new Image ();
				i2.Source = "icn_alert.png";	
				i2.HorizontalOptions = LayoutOptions.Center;
				i2.VerticalOptions = LayoutOptions.Start;
				detailGrid.Children.Add (i2, 3, 4, 2, 3);
			}

			Label l3 = new Label ();
			if (t.IsPickup) {
				DateTime intermediate = DateTime.Today.Add (t.TSPickup);
				l3.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture);
			} else {
				DateTime intermediate = DateTime.Today.Add (t.TSDropOff);
				l3.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture); 
			}
			l3.VerticalOptions = LayoutOptions.Start;
			l3.FontAttributes = FontAttributes.Bold;
			detailGrid.Children.Add (l3, 1, 2, 2, 3);

			Label l4 = new Label ();
			l4.FormattedText = new FormattedString ();
			if (t.IsPickup) {
				l4.FormattedText.Spans.Add (new Span { Text = t.Activity + " Pickup", ForegroundColor = Color.Black });
			} else {
				l4.FormattedText.Spans.Add (new Span { Text = t.Activity + " Dropoff", ForegroundColor = Color.Black });
			}
			l4.FormattedText.Spans.Add (new Span { Text = "\n" + t.Address, ForegroundColor = Color.Gray});
			l4.LineBreakMode = LineBreakMode.WordWrap;
			l4.VerticalOptions = LayoutOptions.StartAndExpand;

			//count the newlines in address and add height for each one

			if (CountOfNewlines(t.Address) > 2) {
				mainlayout.HeightRequest += CountOfNewlines (t.Address) * (l4.FontSize);
			}

			l4.FontAttributes = FontAttributes.Bold;
			StackLayout slDrop = new StackLayout ();
			slDrop.Orientation = StackOrientation.Vertical;
			slDrop.VerticalOptions = LayoutOptions.StartAndExpand;

			slDrop.Children.Add (l4);

			StackLayout slKids = new StackLayout ();
			slKids.Orientation = StackOrientation.Horizontal;
			slKids.WidthRequest = 60;
			//slKids.BackgroundColor = Color.Blue;

			//split the kids
			if (!string.IsNullOrEmpty (t.Kids)) {
				string[] kids = t.Kids.Split ('^');
				mainlayout.HeightRequest += 50;
				foreach (string s in kids) {

					string[] parts = s.Split ('|');
					string azureURL = AzureStorageConstants.BlobEndPoint + t.AccountID.ToLower () + "/" + parts [1].Trim ().ToLower () + ".jpg";

					foreach (Kid k in App.myKids) {
						if (k.Id.ToLower () == parts [1].ToLower ()) {
							azureURL = k.PhotoURL;
							break;
						}
					}
					foreach (Kid k in App.otherKids) {
						if (k.Id.ToLower () == parts [1].ToLower ()) {
							azureURL = k.PhotoURL;
							break;
						}
					}

					//string azureURL = AzureStorageConstants.BlobEndPoint + t.AccountID.ToLower () + "/" + parts [1].Trim ().ToLower () + ".jpg";
					//					Uri auri = new Uri (azureURL);
					//					var uis = new UriImageSource ();
					//					uis.CacheValidity = new TimeSpan (0, 5, 0);
					//					uis.CachingEnabled = false;
					//					uis.Uri = auri;

					ImageCircle.Forms.Plugin.Abstractions.CircleImage ci1 = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
						BorderColor = Color.Black,
						BorderThickness = 1,
						Aspect = Aspect.AspectFill,
						WidthRequest = 50,
						HeightRequest = 50,
						HorizontalOptions = LayoutOptions.Center,
						Source = azureURL
					};	
					slKids.WidthRequest += 60;	
					slKids.Children.Add (ci1);
				}
				slDrop.Children.Add (slKids);
				detailGrid.Children.Add (slDrop, 2, 3, 2, 3);
			}

			mainlayout.Children.Add (detailGrid);

			StackLayout slDetail = new StackLayout ();
			slDetail.WidthRequest = App.ScaledWidth - 20;
			slDetail.BackgroundColor = Color.FromRgb (157, 157, 157);
			slDetail.Padding = new Thickness (0.5);
			slDetail.HorizontalOptions = LayoutOptions.Center;
			slDetail.Children.Add(mainlayout);


			slMain.Children.Add (slDetail);

			
			if (_isActionable) {

				//now add the buttons
				StackLayout slButtons = new StackLayout ();
				slButtons.Orientation = StackOrientation.Horizontal;
				slButtons.BackgroundColor = Color.White;
				slButtons.HorizontalOptions = LayoutOptions.FillAndExpand;
				slButtons.Padding = new Thickness (10, 10, 10, 10);
				//<Button x:Name="btnToday" VerticalOptions="End" HorizontalOptions="Center" HeightRequest="50" WidthRequest="340" FontAttributes="Bold" FontSize="18" Text="Pick Today" TextColor="#F6637F" BackgroundColor="#49376D" BorderColor="#54D29F" BorderRadius="8" BorderWidth="2"></Button>

				Button bAccept = new Button ();
				bAccept.VerticalOptions = LayoutOptions.Center;
				bAccept.HorizontalOptions = LayoutOptions.StartAndExpand;
				bAccept.HeightRequest = 40;
				bAccept.WidthRequest = App.ScaledQuarterWidth - 30;
				bAccept.FontAttributes = FontAttributes.Bold;
				bAccept.FontSize = 18;
				bAccept.TextColor = Color.FromRgb (84, 210, 159);
				bAccept.BorderColor = Color.FromRgb (84, 210, 159);
				bAccept.BorderRadius = 8;
				bAccept.BorderWidth = 2;
				bAccept.BackgroundColor = Color.White;
				bAccept.Text = "Accept";
				bAccept.Clicked += delegate(object sender, EventArgs e) {

					//start preemptive
					//before we allow them to accept, let's check preemptively
					App.hudder.showHUD("Preemptive check...");
					ActivityAddEditViewModel aaevm = new ActivityAddEditViewModel(App.client, null);
					aaevm.CheckPreemptive(mv.ScheduleDate.DayOfWeek.ToString().Substring(0,2)).ConfigureAwait(false);
					string tester = "";

					mv.MessageToday.StartPlaceTravelTime = travelMinutes;
					mv.MessageToday.EndPlaceTravelTime = travelMinutes;

					Period currentDropoffPeriod = new Period(mv.MessageToday.DropoffDiff, mv.MessageToday.DropoffDT);
					Period currentPickupPeriod = new Period(mv.MessageToday.PickupDiff, mv.MessageToday.PickupDT);

					foreach (Preemptive thispe in aaevm.Preemptives)
					{
						if (thispe.id == mv.MessageToday.id)
						{
							continue;
						}

						Period pDropoff = new Period(thispe.DropoffDiff, thispe.DropoffDT);
						Period pPickup = new Period(thispe.PickupDiff, thispe.PickupDT);
						if (pDropoff.Overlaps(currentPickupPeriod))
						{
							tester += mv.MessageToday.Activity + " pickup conflicts with " + thispe.Activity + " dropoff" + Environment.NewLine;
						}
						if (pDropoff.Overlaps(currentDropoffPeriod))
						{
							tester +=  mv.MessageToday.Activity + " pickup conflicts with " + thispe.Activity + " pickup" + Environment.NewLine;
						}
						if (pPickup.Overlaps(currentPickupPeriod))
						{
							tester += mv.MessageToday.Activity + " dropoff conflicts with " + thispe.Activity + " dropoff" + Environment.NewLine;
						}
						if (pPickup.Overlaps(currentDropoffPeriod))
						{
							tester += mv.MessageToday.Activity + " dropoff conflicts with " + thispe.Activity + " pickup" + Environment.NewLine;
						}							
					}
					App.hudder.hideHUD();

					RespondMessage rm = new RespondMessage ();
					if (tester.Length > 0)
					{		
						rm.Conditional = tester;
//						bool ret = DisplayAlert("Preemptive Check", tester, "Continue", "Cancel");
//						if (!ret)
//						{
//							return;
//						}
					}

					//end preemptive


					rm.MessageID = mv.Id;
					rm.Response = "1";
					rm.Status = "read";
					rm.PostUpdate = "today";
					MessagingCenter.Send<RespondMessage> (rm, "messageresponse");
				};
				slButtons.Children.Add (bAccept);

				Button bDecline = new Button ();
				bDecline.VerticalOptions = LayoutOptions.Center;
				bDecline.HorizontalOptions = LayoutOptions.EndAndExpand;
				bDecline.HeightRequest = 40;
				bDecline.WidthRequest = App.ScaledQuarterWidth - 30;
				bDecline.FontAttributes = FontAttributes.Bold;
				bDecline.FontSize = 18;
				bDecline.TextColor = Color.FromRgb (246, 99, 127);
				bDecline.BorderColor = Color.FromRgb (246, 99, 127);
				bDecline.BorderRadius = 8;
				bDecline.BorderWidth = 2;
				bDecline.BackgroundColor = Color.White;
				bDecline.Text = "Decline";
				bDecline.Clicked += delegate(object sender, EventArgs e) {
					RespondMessage rm = new RespondMessage ();
					rm.MessageID = mv.Id;

					rm.Response = "0";
					rm.Status = "read";
					MessagingCenter.Send<RespondMessage> (rm, "messageresponse");
				};
				slButtons.Children.Add (bDecline);

				StackLayout slButtonBorder = new StackLayout ();
				slButtonBorder.BackgroundColor = Color.FromRgb (157, 157, 157);
				slButtonBorder.Padding = new Thickness (0.5);
				slButtonBorder.WidthRequest = App.ScaledWidth - 20;
				slButtonBorder.HorizontalOptions = LayoutOptions.Center;
				slButtonBorder.Children.Add (slButtons);
				slMain.Children.Add (slButtonBorder);

				bv = new BoxView ();
				bv.HeightRequest = 10;
				slMain.Children.Add (bv);
			}

			View = slMain;
		}
	}

	public class CircleRequestCell : ViewCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			MessageView mv = (MessageView)c;

			if (mv == null) {
				return;
			}

			StackLayout slMain = new StackLayout ();
		
			slMain.Orientation = StackOrientation.Vertical;
			slMain.WidthRequest = App.ScaledWidth - 20;
			slMain.HorizontalOptions = LayoutOptions.FillAndExpand;
			slMain.Spacing = 0;

			//add some space
			BoxView bv = new BoxView();
			bv.HeightRequest = 10;
			slMain.Children.Add (bv);

			//top box
			StackLayout sl = new StackLayout ();
			sl.Padding = new Thickness (10, 10, 10, 10);
			sl.Orientation = StackOrientation.Horizontal;
			sl.BackgroundColor = Color.White;
			sl.HorizontalOptions = LayoutOptions.FillAndExpand;

			StackLayout slVert = new StackLayout ();
			slVert.Orientation = StackOrientation.Vertical;

			Label l = new Label ();
			l.Text = "Request to join Circle";
			l.FontFamily = "HelveticaNeue-Light";
			l.FontSize = 24;
			l.HorizontalOptions = LayoutOptions.StartAndExpand;
			slVert.Children.Add (l);

			Label lFrom = new Label();
			lFrom.Text = "from " + mv.Sender + " • " + DateTimeExtensions.GetTimeSpan (mv.Created.ToLocalTime ().DateTime);
			lFrom.TextColor = Color.FromRgb (157, 157, 157);
			slVert.Children.Add (lFrom);

			sl.Children.Add (slVert);

			ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
				BorderColor = Color.Black,
				BorderThickness = 1,
				Aspect = Aspect.AspectFill,
				WidthRequest = 50,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Source= mv.SenderPhotoURL
			};	
			sl.Children.Add (ci);
		
			StackLayout slBorder = new StackLayout ();
			slBorder.WidthRequest = App.ScaledWidth - 20;
			slBorder.BackgroundColor = Color.FromRgb (157, 157, 157);
			slBorder.Padding = new Thickness (0.5);
			slBorder.Children.Add (sl);
			slBorder.HorizontalOptions = LayoutOptions.Center;
			slMain.Children.Add (slBorder);



			//now add the buttons
			StackLayout slButtons = new StackLayout ();
			slButtons.Orientation = StackOrientation.Horizontal;
			slButtons.BackgroundColor = Color.White;
			slButtons.HorizontalOptions = LayoutOptions.FillAndExpand;
			slButtons.Padding = new Thickness (10, 10, 10, 10);
			//<Button x:Name="btnToday" VerticalOptions="End" HorizontalOptions="Center" HeightRequest="50" WidthRequest="340" FontAttributes="Bold" FontSize="18" Text="Pick Today" TextColor="#F6637F" BackgroundColor="#49376D" BorderColor="#54D29F" BorderRadius="8" BorderWidth="2"></Button>

			Button bAccept = new Button ();
			bAccept.VerticalOptions = LayoutOptions.Center;
			bAccept.HorizontalOptions = LayoutOptions.StartAndExpand;
			bAccept.HeightRequest = 40;
			bAccept.WidthRequest = App.ScaledQuarterWidth - 30;
			bAccept.FontAttributes = FontAttributes.Bold;
			bAccept.FontSize = 18;
			bAccept.TextColor = Color.FromRgb (84, 210, 159);
			bAccept.BorderColor = Color.FromRgb (84, 210, 159);
			bAccept.BorderRadius = 8;
			bAccept.BorderWidth = 2;
			bAccept.BackgroundColor = Color.White;
			bAccept.Text = "Accept";

			bAccept.Clicked +=  delegate(object sender, EventArgs e) {
				RespondMessage rm = new RespondMessage();
				rm.MessageID = mv.Id;
				rm.Response = "1";
				rm.Status = "read";
				rm.PostUpdate = "circlekids";
				MessagingCenter.Send<RespondMessage>(rm, "messageresponse");
			};

			slButtons.Children.Add (bAccept);

			Button bDecline = new Button ();
			bDecline.VerticalOptions = LayoutOptions.Center;
			bDecline.HorizontalOptions = LayoutOptions.EndAndExpand;
			bDecline.HeightRequest = 40;
			bDecline.WidthRequest = App.ScaledQuarterWidth - 30;
			bDecline.FontAttributes = FontAttributes.Bold;
			bDecline.FontSize = 18;
			bDecline.TextColor = Color.FromRgb (246, 99, 127);
			bDecline.BorderColor = Color.FromRgb (246, 99, 127);
			bDecline.BorderRadius = 8;
			bDecline.BorderWidth = 2;
			bDecline.BackgroundColor = Color.White;
			bDecline.Text = "Decline";

			bDecline.Clicked +=  delegate(object sender, EventArgs e) {
				RespondMessage rm = new RespondMessage();
				rm.MessageID = mv.Id;
				rm.Response = "0";
				rm.Status = "read";
				MessagingCenter.Send<RespondMessage>(rm, "messageresponse");
			};

			slButtons.Children.Add (bDecline);

			StackLayout slButtonBorder = new StackLayout ();
			slButtonBorder.BackgroundColor = Color.FromRgb (157, 157, 157);
			slButtonBorder.Padding = new Thickness (0.5);
			slButtonBorder.WidthRequest = App.ScaledWidth - 20;
			slButtonBorder.HorizontalOptions = LayoutOptions.Center;
			slButtonBorder.Children.Add (slButtons);
			slMain.Children.Add (slButtonBorder);

			bv = new BoxView();
			bv.HeightRequest = 10;
			slMain.Children.Add (bv);

			View = slMain;

		}
	}

	public class MessageCell : ViewCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			MessageView mv = (MessageView)c;

			StackLayout slMain = new StackLayout ();

			slMain.Orientation = StackOrientation.Vertical;
			slMain.WidthRequest = App.ScaledWidth - 20;
			slMain.HorizontalOptions = LayoutOptions.FillAndExpand;
			slMain.Spacing = 0;

			//add some space
			BoxView bv = new BoxView();
			bv.HeightRequest = 10;
			slMain.Children.Add (bv);


			StackLayout sl = new StackLayout ();
			sl.Padding = new Thickness (5);
			sl.Orientation = StackOrientation.Horizontal;
			sl.BackgroundColor = Color.White;
			Label l = new Label ();
			l.Text = mv.Message;
			//l.SetBinding (Label.TextProperty, "Message");
			sl.Children.Add (l);

			StackLayout slBorder = new StackLayout ();
			slBorder.WidthRequest = App.ScaledWidth - 20;
			slBorder.BackgroundColor = Color.FromRgb (157, 157, 157);
			slBorder.Padding = new Thickness (0.5);
			slBorder.Children.Add (sl);
			slBorder.HorizontalOptions = LayoutOptions.Center;
			slMain.Children.Add (slBorder);

			//now add the buttons
			StackLayout slButtons = new StackLayout ();
			slButtons.Orientation = StackOrientation.Vertical;
			slButtons.BackgroundColor = Color.White;
			slButtons.HorizontalOptions = LayoutOptions.FillAndExpand;
			slButtons.Padding = new Thickness (10, 10, 10, 10);

			//<Button x:Name="btnToday" VerticalOptions="End" HorizontalOptions="Center" HeightRequest="50" WidthRequest="340" FontAttributes="Bold" FontSize="18" Text="Pick Today" TextColor="#F6637F" BackgroundColor="#49376D" BorderColor="#54D29F" BorderRadius="8" BorderWidth="2"></Button>

			Button bAccept = new Button ();
			bAccept.VerticalOptions = LayoutOptions.Center;
			bAccept.HorizontalOptions = LayoutOptions.Center;
			bAccept.HeightRequest = 40;
			bAccept.WidthRequest = App.ScaledQuarterWidth - 30;
			bAccept.FontAttributes = FontAttributes.Bold;
			bAccept.FontSize = 18;
			bAccept.TextColor = Color.FromRgb (84, 210, 159);
			bAccept.BorderColor = Color.FromRgb (84, 210, 159);
			bAccept.BorderRadius = 8;
			bAccept.BorderWidth = 2;
			bAccept.BackgroundColor = Color.White;
			bAccept.Text = "OK";

			bAccept.Clicked +=  delegate(object sender, EventArgs e) {
				RespondMessage rm = new RespondMessage();
				rm.MessageID = mv.Id;
				rm.Response = "1";
				rm.Status = "read";
				rm.PostUpdate = "";
				MessagingCenter.Send<RespondMessage>(rm, "messageresponse");
			};

			slButtons.Children.Add (bAccept);

			StackLayout slButtonBorder = new StackLayout ();
			slButtonBorder.BackgroundColor = Color.FromRgb (157, 157, 157);
			slButtonBorder.Padding = new Thickness (0.5);
			slButtonBorder.WidthRequest = App.ScaledWidth - 20;
			slButtonBorder.HorizontalOptions = LayoutOptions.Center;
			slButtonBorder.Children.Add (slButtons);
			slMain.Children.Add (slButtonBorder);

			bv = new BoxView();
			bv.HeightRequest = 10;
			slMain.Children.Add (bv);

			View = slMain;

		}
	}
}

