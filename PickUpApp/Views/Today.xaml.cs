﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using PickUpApp.ViewModels;

namespace PickUpApp
{	
	public partial class TodayView : ContentPage
	{
		public TodayView ()
		{
			InitializeComponent ();

			this.ViewModel = new TodayViewModel(App.client);
			this.Padding = new Thickness(0, Device.OnPlatform(0, 0, 0), 0, 5);
			//lstAccount.ItemSelected += lstAccount_ItemSelected;

			this.ToolbarItems.Add (new ToolbarItem ("Calendar", "icn_cal.png", async() => {
				//pop the calendar window
				//await DisplayAlert("CAL!", "show the calendar", "Cancel");
				await Navigation.PushAsync(new CalendarPicker());
			}));

//			//start android
//
//			var listView = new ListView ();
//			listView.SetBinding<TodayViewModel> (ListView.ItemsSourceProperty, vm => vm.Todays);
//
//			var refreshView = new PullToRefreshListView {
//				RefreshCommand = viewModel.RefreshCommand,
//				Content = listView
//			};
//
//			refreshView.SetBinding<TodayViewModel> (PullToRefreshListView.IsRefreshingProperty, vm => vm.IsBusy);
//			//end android

			stacker.Spacing = 0;

//			TableView tv = new TableView ();
//			tv.HasUnevenRows = true;
//			TableSection ts = new TableSection ();
//
//			ts.BindingContext = ViewModel.Todays;
//
//			ts.Add (new TodayCell());		
//			tv.Root.Add (ts);
//			stacker.Children.Add (tv);

			this.BackgroundColor = Color.FromRgb (73, 55, 109);

			ListView lvToday = new ListView () {
				RefreshCommand = ViewModel.LoadItemsCommand,
				ItemsSource = ViewModel.Todays,
				ItemTemplate = new DataTemplate (typeof(TodayCell)),
				SeparatorVisibility = SeparatorVisibility.None,
				IsPullToRefreshEnabled = true,
				HasUnevenRows = true,
				BackgroundColor = Color.Transparent,
				Header = null
			};
			MessagingCenter.Subscribe<TodayViewModel>(this, "TodayLoaded", (t) => {
				lvToday.IsRefreshing = false;
			});


			MessagingCenter.Subscribe<string> (this, "NeedsRefresh", async(nr) => {
				await ViewModel.ExecuteLoadItemsCommand();

			});


			lvToday.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {

				if (e.SelectedItem == null)
				{
					return;
				}

				Navigation.PushAsync(new RouteDetail());
				lvToday.SelectedItem = null;
				return;

				Today today = ((Today)e.SelectedItem);
				if (today.RowType == "schedule")
				{
					if (string.IsNullOrEmpty(today.ConfirmedBy))
					{
						Schedule s = new Schedule();
						s.id = today.id;
						Navigation.PushModalAsync(new CircleSelect(s));
					}
					else{
						//issue here is that we have the scheduleid but we really need the inviteid
						Navigation.PushModalAsync(new InviteResponseView(today));
					}
				}
				if (today.RowType == "invite") //should be "invite"
				{
					//seems a little silly to have these supertypes that cross map to each other...
					InviteInfo i = new InviteInfo();
					i.Activity = today.Activity;
					i.Address = today.Address;
					i.EndTimeTicks = today.EndTimeTicks;
					i.Id = today.id;
					i.Kids = today.Kids;
					i.Latitude = double.Parse(today.Latitude);
					i.Location = today.Location;
					i.Longitude = double.Parse(today.Longitude);
					i.Message = today.Message;
					i.PickupDate = today.PickupDate;
					i.Requestor = today.Requestor;
					i.RequestorPhone = today.RequestorPhone;
					//i.Solved missing
					//i.SolvedBy missing
					i.StartTimeTicks = today.StartTimeTicks;
					i.Complete = false;
					i.LocationMessage = today.LocationMessage;
					i.AccountID = today.AccountID;

					i.ReturnTo = today.ReturnTo;
					i.ReturnToAddress = today.ReturnToAddress;
					i.ReturnToLatitude = today.ReturnToLatitude;
					i.ReturnToLongitude = today.ReturnToLongitude;

					Navigation.PushModalAsync(new InviteHUD(i));
				}
				lvToday.SelectedItem = null;
			};
			stacker.Children.Add (lvToday);


//			var refreshList = new PullToRefreshListView {
//				RefreshCommand = ViewModel.LoadItemsCommand, 
//				Message = "Loading...",
//				ItemTemplate = new DataTemplate(typeof(TodayTemplateLayout))
//			};
//
//			refreshList.ItemSelected +=	 (object sender, SelectedItemChangedEventArgs e) => {
//
//				//need to differentiate between a schedule item and an accepted invite item...how?
//				Today today = ((Today)e.SelectedItem);
//				if (today.RowType == "schedule")
//				{
//					Schedule s = new Schedule();
//					s.id = today.id;
//					Navigation.PushModalAsync(new CircleSelect(s));
//				}
//				if (today.RowType == "invite") 
//				{
//					//seems a little silly to have these supertypes that cross map to each other...
//					InviteInfo i = new InviteInfo();
//					i.Activity = today.Activity;
//					i.Address = today.Address;
//					i.EndTimeTicks = today.EndTimeTicks;
//					i.Id = today.id;
//					i.Kids = today.Kids;
//					i.Latitude = double.Parse(today.Latitude);
//					i.Location = today.Location;
//					i.Longitude = double.Parse(today.Longitude);
//					i.Message = today.Message;
//					i.PickupDate = today.PickupDate;
//					i.Requestor = today.Requestor;
//					//i.Solved missing
//					//i.SolvedBy missing
//					i.StartTimeTicks = today.StartTimeTicks;
//
//					Navigation.PushModalAsync(new InviteHUD(i));
//				}
//			};
//			refreshList.SetBinding<TodayViewModel> (PullToRefreshListView.IsRefreshingProperty, vm => vm.IsLoading);
//			refreshList.SetBinding<TodayViewModel> (PullToRefreshListView.ItemsSourceProperty, vm => vm.Todays);
//			stacker.Children.Add (refreshList);

		}

		public  TodayViewModel ViewModel
		{
			get { return this.BindingContext as TodayViewModel; }
			set { this.BindingContext = value; }
		}

		void lstAccount_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			//Navigation.PushAsync(new RebatesView(e.SelectedItem as Store));
			//lstAccount.SelectedItem = null;
		}




			
//		public List<Account> getAccount()
//		{
//			List<Account> theList = new List<Account> ();
//			theList.Add (PickupService.DefaultService.GetAccount ().Result);
//			return theList;
//
//		}

	}

	public class TodayTemplateLayout : ViewCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			dynamic c = BindingContext;
			this.Height = 55;
			StackLayout slmain = new StackLayout ();
			slmain.Orientation = StackOrientation.Vertical;

			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;
			sl.VerticalOptions = LayoutOptions.Center;
			Label namelabel = new Label ();
			namelabel.TextColor = Color.White;
			namelabel.HorizontalOptions = LayoutOptions.FillAndExpand;
			namelabel.SetBinding (Label.TextProperty, "Activity");
			//namelabel.Text = c.Activity;
			sl.Children.Add (namelabel);
			Label startlabel = new Label();
			startlabel.TextColor = Color.White;
			startlabel.SetBinding (Label.TextProperty, "ActualAtWhen");
			startlabel.HorizontalOptions = LayoutOptions.End;
			//startlabel.Text = c.StartTime;
			sl.Children.Add (startlabel);

			Label pickerUpperLabel = new Label ();
			pickerUpperLabel.TextColor = Color.White;	
			pickerUpperLabel.HorizontalOptions = LayoutOptions.Start;
			pickerUpperLabel.VerticalOptions = LayoutOptions.Start;
			pickerUpperLabel.SetBinding (Label.TextProperty, "TodayDescriptor");


			slmain.Children.Add (sl);
			slmain.Children.Add (pickerUpperLabel);

			View = slmain;
		}
	}



	public class TodayCell : ViewCell
	{
		//NOTE: THIS IS COUNTING NEWLINES AND COMMAS
		private static int CountOfNewlines(string s)
		{
			int n = 0;
			foreach( var c in s )
			{
				if ( c == '\n' ) n++;
				if (c == ',')
					n++;
			}
			return n+1;
		}

		private enum ActivityState
		{
			Complete,
			Next,
			Future
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			dynamic c = BindingContext;
			this.Height = 195;
			Today t = (Today)c;

			ActivityState currentState = ActivityState.Future;
			if (t.IsNext) {
				currentState = ActivityState.Next;
			} else {
				if (t.PickupComplete) {
					currentState = ActivityState.Complete;
				}
			}

			StackLayout mainlayout = new StackLayout ();
			mainlayout.Spacing = 0;
			mainlayout.Orientation = StackOrientation.Vertical;
			mainlayout.VerticalOptions = LayoutOptions.StartAndExpand;
			mainlayout.BackgroundColor = Color.FromRgb (73, 55, 55);//109);

			//make a purple header
			StackLayout sl = new StackLayout ();
			sl.BackgroundColor = Color.FromRgb (73,55,109);
			sl.Orientation = StackOrientation.Horizontal;
			sl.VerticalOptions = LayoutOptions.Start;
			sl.HeightRequest = 46;

			Label headerlabel = new Label();
			//headerlabel.SetBinding (Label.TextProperty, "Activity");
			headerlabel.VerticalOptions = LayoutOptions.CenterAndExpand;
			headerlabel.FontSize = 18;
			headerlabel.FontAttributes = FontAttributes.Bold;
			headerlabel.TranslationX = 26;
			headerlabel.LineBreakMode = LineBreakMode.NoWrap;
			if (t.IsPickup) {
				headerlabel.Text = "AFTER " + t.Activity.ToUpper ();
			} else {
				headerlabel.Text = "BEFORE " + t.Activity.ToUpper ();
			}
			headerlabel.TextColor = Color.White;
			sl.Children.Add (headerlabel);

			mainlayout.Children.Add (sl);


			Color bgColor = Color.FromRgb (238, 236, 243);
			if (currentState == ActivityState.Next) {
				bgColor = Color.White;
			}

			//add a spacer for the grid...20px or so
			BoxView bv = new BoxView();
			bv.BackgroundColor = bgColor;

			bv.HeightRequest = 20;
			mainlayout.Children.Add (bv);

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
			switch (currentState) {
			case ActivityState.Complete:
				triangle.Source = trigreen;
				break;
			case ActivityState.Future:
				triangle.Source = trigray;
				break;
			case ActivityState.Next:
				triangle.Source = tripink;
				break;
			}
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
				switch (currentState) {
				case ActivityState.Complete:
					line.Source = linegreen;
					break;
				case ActivityState.Future:
					line.Source = linegray;
					break;
				case ActivityState.Next:
					line.Source = linepink;
					break;
				}
				slLine.Children.Add (line);
			}
			detailGrid.Children.Add (slLine, 0, 1, 1, 2);


			Image pin = new Image ();
			switch (currentState) {
			case ActivityState.Complete:
				pin.Source = pingreen;
				break;
			case ActivityState.Future:
				if (t.IsPickup) {
					pin.Source = pinupgray;
				} else {
					pin.Source = pindowngray;
				}
				break;
			case ActivityState.Next:
				if (t.IsPickup) {
					pin.Source = pinuppink;
				} else {
					pin.Source = pindownpink;
				}
				break;
			}
			pin.HorizontalOptions = LayoutOptions.Center;
			pin.VerticalOptions = LayoutOptions.Start;
			detailGrid.Children.Add (pin, 0, 1, 2, 3);

			//end nav images

			Label l = new Label ();
			if (t.IsPickup) {
				l.Text = DateTime.Parse (t.ActualAtWhen).AddMinutes (-t.EndPlaceTravelTime).ToLocalTime ().ToString ("t");
			} else {
				l.Text = DateTime.Parse (t.ActualAtWhen).AddMinutes (-t.StartPlaceTravelTime).ToLocalTime ().ToString ("t");
			}
			l.VerticalOptions = LayoutOptions.Start;
			l.FontAttributes = FontAttributes.Bold;
			detailGrid.Children.Add (l, 1, 2, 0, 2);

			Label l2 = new Label ();
			l2.VerticalOptions = LayoutOptions.StartAndExpand;
			l2.FormattedText = new FormattedString ();
			if (t.IsPickup) {
				l2.FormattedText.Spans.Add (new Span { Text = "Leave " + t.EndPlaceName, ForegroundColor = Color.Black });
				l2.FormattedText.Spans.Add (new Span {
					Text = "\nDrive " + t.EndPlaceDistance + " miles",
					ForegroundColor = Color.Gray,
					FontAttributes = FontAttributes.Italic
				});
			} else {
				l2.FormattedText.Spans.Add (new Span { Text = "Leave " + t.StartPlaceName, ForegroundColor = Color.Black });
				l2.FormattedText.Spans.Add (new Span {
					Text = "\nDrive " + t.StartPlaceDistance + " miles",
					ForegroundColor = Color.Gray,
					FontAttributes = FontAttributes.Italic
				});
			}

			l2.FontAttributes = FontAttributes.Bold;
			detailGrid.Children.Add (l2, 2, 3, 0, 2 );


			if (currentState != ActivityState.Complete) {
				Button b = new Button ();
				switch (currentState) {
				case ActivityState.Future:
					b.Image = arrowgray;
					break;
				case ActivityState.Next:
					b.Image = arrowpink;
					break;
				}
				b.HorizontalOptions = LayoutOptions.Center;
				b.VerticalOptions = LayoutOptions.Start;
				b.Clicked += async delegate(object sender, EventArgs e) {
					//await ((TodayView)this.ParentView.Parent.Parent).DisplayAlert ("Fetch!", "create a fetch request", "Cancel");
					await ((TodayView)this.ParentView.Parent.Parent).Navigation.PushAsync(new FetchRequest1());
				};
				detailGrid.Children.Add (b, 3, 4, 0, 1);
			}


			if (t.TrafficWarning) {
				Image i2 = new Image ();
				i2.Source = "icn_alert.png";	
				i2.HorizontalOptions = LayoutOptions.Center;
				i2.VerticalOptions = LayoutOptions.Start;
				detailGrid.Children.Add (i2, 3, 4, 2, 3);
			}



			Label l3 = new Label ();
			l3.Text = DateTime.Parse (t.ActualAtWhen).ToLocalTime ().ToString ("t");
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
				this.Height += CountOfNewlines (t.Address) * (l4.FontSize);
			}

			l4.FontAttributes = FontAttributes.Bold;
			StackLayout slDrop = new StackLayout ();
			slDrop.Orientation = StackOrientation.Vertical;
			slDrop.VerticalOptions = LayoutOptions.StartAndExpand;

			slDrop.Children.Add (l4);

			StackLayout slKids = new StackLayout ();
			slKids.Orientation = StackOrientation.Horizontal;

			//split the kids
			if (!string.IsNullOrEmpty (t.Kids)) {
				string[] kids = t.Kids.Split ('~');
				this.Height += 50;
				foreach (string s in kids) {

					string[] parts = s.Split ('|');
					string azureURL = AzureStorageConstants.BlobEndPoint + t.AccountID.ToLower () + "/" + parts [1].Trim ().ToLower () + ".jpg";
					Uri auri = new Uri (azureURL);
					ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
						BorderColor = Color.Black,
						BorderThickness = 0,
						Aspect = Aspect.AspectFill,
						WidthRequest = 50,
						HeightRequest = 50,
						HorizontalOptions = LayoutOptions.Center,
						Source = auri
					};	
						
					slKids.Children.Add (ci);
				}
				slDrop.Children.Add (slKids);
				detailGrid.Children.Add (slDrop, 2, 3, 2, 3);
			}

			mainlayout.Children.Add (detailGrid);

			View = mainlayout;
		}
	}
}


