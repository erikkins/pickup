using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using PickUpApp.ViewModels;

namespace PickUpApp
{	
	public partial class TodayView : ContentPage
	{
		Label lblNone;
		FFArrow NEWarrow;
		Editor edNew;
		Label lblMessageCount;
		RelativeLayout rlMessage;
	

		public TodayView ()
		{
			InitializeComponent ();

			this.ViewModel = new TodayViewModel(App.client);
			this.Padding = new Thickness(0, Device.OnPlatform(0, 0, 0), 0, 5);
			//lstAccount.ItemSelected += lstAccount_ItemSelected;
				
//			AbsoluteLayout abs = new AbsoluteLayout();
//			abs.VerticalOptions = LayoutOptions.FillAndExpand;
//			abs.HorizontalOptions = LayoutOptions.FillAndExpand;
//

			if (!App.LaunchLocationRecorded) {
				MessagingCenter.Subscribe<Location> (this, "LocationUpdated", (l) => {
					LocationLog ll = new LocationLog ();
					ll.Latitude = App.PositionLatitude;
					ll.Longitude = App.PositionLongitude;
					ll.LogType = "launch";
					if (!string.IsNullOrEmpty (ll.Latitude) && !(string.IsNullOrEmpty (ll.Longitude))) {
						this.ViewModel.ExecuteLocationLogCommand (ll).ConfigureAwait (false);
						MessagingCenter.Unsubscribe<Location> (this, "LocationUpdated");
						App.LaunchLocationRecorded = true;
					}
				});
			}				

			StackLayout stacker = new StackLayout ();
			stacker.Orientation = StackOrientation.Vertical;


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

			lblNone = new Label ();




			//moved into Appearing
//			MessagingCenter.Subscribe<string> (this, "NeedsRefresh", async(nr) => {
//				await ViewModel.ExecuteLoadItemsCommand();
//			});
//
//			MessagingCenter.Subscribe<Today>(this, "fetchrequest", async(t) => {
//				await Navigation.PushAsync(new FetchRequest1(t));
//			});


			lvToday.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {

				if (e.SelectedItem == null)
				{
					return;
				}
				Today today = ((Today)e.SelectedItem);

				//if this is an active FetchRequest (and I'm the sender!), take them to the ManageFetch screen, otherwise to the RouteDetail
				bool allowManage = false;

				if (today.AccountID == App.myAccount.id && (!string.IsNullOrEmpty(today.PickupMessageID) || !string.IsNullOrEmpty(today.DropOffMessageID)))
				{
					if (today.IsPickup && !string.IsNullOrEmpty(today.PickupMessageID) && today.PickupMessageStatus != "Canceled")
					{
						allowManage = true;
					}
					else{
						if (!today.IsPickup && !string.IsNullOrEmpty(today.DropOffMessageID) && today.DropOffMessageStatus != "Canceled")
						{
							allowManage = true;
						}
					}
				}

				if (allowManage)
				{					
					Navigation.PushAsync(new ManageFetch(today));
				}
				else{
					//need to calculate pin positions here
					/*
					 1 = Green
					 2 = Pink Up
					 3 = Pink Down
					 4 = Grey Up
					 5 = Grey Down
					*/
				
					string pinstring = "";
					int currentOrdinal = 0;
					int tCount = 0;
					foreach (Today t in ViewModel.Todays)
					{						
						if (t.id == today.id && t.IsPickup == today.IsPickup)
						{
							currentOrdinal = tCount;
						}
						tCount++;

						ActivityState currentState = ActivityState.Future;
						if (t.IsNext) {
							currentState = ActivityState.Next;
						}

						if (t.PickupComplete || t.DropOffComplete) {
							currentState = ActivityState.Complete;
						}
						switch (currentState) {
						case ActivityState.Complete:
							pinstring += "1";
							break;
						case ActivityState.Future:
							if (t.IsPickup) {
								pinstring += "4";
							} else {
								pinstring += "5";
							}
							break;
						case ActivityState.Next:
							if (t.IsPickup) {
								pinstring += "2";
							} else {
								pinstring += "3";
							}
							break;
						}
					}


					Navigation.PushAsync(new RouteDetail(today, pinstring, currentOrdinal));
				}
				lvToday.SelectedItem = null;
				return;
				/*
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
					i.LocationMessage = today.LocationNotes;
					i.AccountID = today.AccountID;

					i.ReturnTo = today.ReturnTo;
					i.ReturnToAddress = today.ReturnToAddress;
					i.ReturnToLatitude = today.ReturnToLatitude;
					i.ReturnToLongitude = today.ReturnToLongitude;

					Navigation.PushModalAsync(new InviteHUD(i));
				}

				lvToday.SelectedItem = null;
				*/
			};
				

			stacker.Children.Add (lvToday);

			RelativeLayout rl = new RelativeLayout ();
			rl.Children.Add (stacker, 
				xConstraint: Constraint.Constant (0), 
				yConstraint: Constraint.Constant (0),
				widthConstraint: Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				heightConstraint: Constraint.RelativeToParent ((parent) => {
					return parent.Height;
				}));



			//need to put this into an absolutelayout container...and only show when no kids, no places, no activity
			NEWarrow = new FFArrow ();
			NEWarrow.Color = AppColor.AppPink;
			NEWarrow.WidthRequest = App.Device.Display.Width;
			NEWarrow.HeightRequest = App.Device.Display.Height;
			NEWarrow.StartPoint = new Point (40, 10);
			NEWarrow.EndPoint = new Point (App.ScaledQuarterWidth, App.ScaledHeight / 6);
			NEWarrow.IsVisible = false;
			rl.Children.Add (NEWarrow, Constraint.Constant(0), Constraint.Constant(0), null, null);

			edNew = new Editor ();
			edNew.IsEnabled = false;
			edNew.TextColor = AppColor.AppPink;
			edNew.BackgroundColor = Color.Transparent;
			edNew.Text = "Welcome! To get started, click on the Menu icon and begin adding Kids, Places and Activities.";
			edNew.WidthRequest = 250;
			edNew.IsVisible = false;
			rl.Children.Add (edNew, Constraint.Constant (App.ScaledQuarterWidth -115), Constraint.Constant(App.ScaledHeight/6), null, null);


			//arrow.IsVisible = false;

			//try to float the messages icon with absolute layout

				rlMessage = new RelativeLayout ();

				Image msgimg = new Image ();
				msgimg.Source = "ui_messages.png";

				rlMessage.Children.Add (msgimg,
					xConstraint: Constraint.Constant (0), 
					yConstraint: Constraint.Constant (0),
					widthConstraint: Constraint.Constant (69),
					heightConstraint: Constraint.Constant (69));

				lblMessageCount = new Label ();
				lblMessageCount.TextColor = Color.White;
				lblMessageCount.FontSize = 24;
				//lblMessageCount.Text = App.myMessages.Count.ToString ();
				//lblMessageCount.SetBinding (Label.TextProperty, "App.myMessages", 0, new CollectionConvertor());

				var binding = new Binding ();
				//binding.Path = "myMessages";
				binding.Source = App.myMessages;
				binding.Mode = BindingMode.Default;
				binding.Converter = new CollectionConvertor ();
				lblMessageCount.SetBinding (Label.TextProperty, binding);


				lblMessageCount.VerticalTextAlignment = TextAlignment.Center;
				lblMessageCount.HorizontalTextAlignment = TextAlignment.Center;


				rlMessage.Children.Add (lblMessageCount,
					xConstraint: Constraint.Constant (0), 
					yConstraint: Constraint.Constant (0),
					widthConstraint: Constraint.Constant (69),
					heightConstraint: Constraint.Constant (69));


				var tap = new TapGestureRecognizer ();
				tap.Tapped += (sender, e) => {
					Navigation.PushAsync (new MessageCenter ());
				};

				rlMessage.GestureRecognizers.Add (tap);


				rl.Children.Add (rlMessage, 
					xConstraint: Constraint.RelativeToParent ((parent) => {
						return parent.Width - 84;
					}),
					yConstraint: Constraint.RelativeToParent ((parent) => {
						return parent.Height - 84;
					}));
			
			if (App.myMessages.Count > 0) {
				rlMessage.IsVisible = true;
			} else {
				rlMessage.IsVisible = false;
			}

			App.myMessages.CollectionChanged += delegate(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
				if (App.myMessages.Count > 0) {
					rlMessage.IsVisible = true;
				} else {
					rlMessage.IsVisible = false;
				}
			};

			//moved into Appearing
//			MessagingCenter.Subscribe<string> ("today", "messagesloaded", (ec) => {
//				System.Diagnostics.Debug.WriteLine ("Received messagesloaded in Today");
//				lblMessageCount.Text = App.myMessages.Count.ToString();
//				if (App.myMessages.Count > 0)
//				{
//					rlMessage.IsVisible = true;
//				}
//				else
//				{
//					rlMessage.IsVisible = false;
//				}
//			});
//
//			MessagingCenter.Subscribe<string> ("today", "messagesupdated", (ec) => {
//				System.Diagnostics.Debug.WriteLine ("Received messagesupdated in Today");
//				if (App.myMessages.Count > 0)
//				{
//					rlMessage.IsVisible = true;
//				}
//				else
//				{
//					rlMessage.IsVisible = false;
//				}
//			});


			lblNone = new Label ();
			lblNone.Text = "You have no activities Today!";
			lblNone.TextColor = Color.White;
			lblNone.IsVisible = false;
			lblNone.VerticalOptions = LayoutOptions.Center;
			lblNone.HorizontalOptions = LayoutOptions.Center;

			rl.Children.Add(lblNone,
				Constraint.RelativeToParent (parent => {
					return parent.Width /2 - lblNone.Width / 2;
				}),Constraint.RelativeToParent (parent => {
					return parent.Height /2 - lblNone.Height / 2;
				}));
					
//			lvToday.BindingContextChanged +=  delegate(object sender, EventArgs e) {
//				if (ViewModel == null)
//				{
//					return;
//				}
//				if (ViewModel.Todays.Count == 0)
//				{
//					lblNone.IsVisible = true;
//				}
//				else{
//					lblNone.IsVisible = false;
//				}
//			};

			//moved into Appearing
//			MessagingCenter.Subscribe<TodayViewModel>(this, "TodayLoaded", (t) => {
//
//				System.Diagnostics.Debug.WriteLine("TODAYLOADED");
//
//				lvToday.IsRefreshing = false;
//
//				this.Title = App.CurrentToday.Date.ToString ("MMM dd").ToUpper();
//				if (App.CurrentToday.Date == DateTime.Today) {
//					this.Title += " (Today)";
//				}
//
//				if (t.Todays.Count == 0)
//				{
//					lblNone.IsVisible = true;
//				}
//				else
//				{
//					lblNone.IsVisible = false;
//				}
//
//				if (App.myKids.Count == 0 && App.myPlaces.Count == 0 && App.myCircle.Count == 0)
//				{
//					//must be the first time!
//					lblNone.IsVisible = false;
//					NEWarrow.IsVisible = true;
//					edNew.IsVisible = true;
//				}
//				else{					
//					NEWarrow.IsVisible = false;
//					edNew.IsVisible = false;
//				}
//
//				//let's load the messages to see if there's anything in my inbox
//				MessageView mv = new MessageView();
//				MessagingCenter.Send<MessageView>(mv, "LoadMessages");
////				TodayViewModel tvm = (TodayViewModel)this.BindingContext;
////				this.BindingContext = new MessageViewModel(App.client, null);
////				App.hudder.showHUD("Loading Messages");
////				((MessageViewModel)BindingContext).ExecuteLoadItemsCommand().ConfigureAwait(true);
////				App.hudder.hideHUD();
////				this.BindingContext = tvm;
//
//			});
//
//			MessagingCenter.Subscribe<MessageView> (this, "LoadMessages", (mv) => {	
//				System.Diagnostics.Debug.WriteLine ("LoadMessages from Today");
//				MessageViewModel mvm = new MessageViewModel(App.client, null);
//				App.hudder.showHUD("Loading Messages");
//				mvm.ExecuteLoadItemsCommand().ConfigureAwait(true);
//				App.hudder.hideHUD();
//			});


			this.Appearing += delegate(object sender, EventArgs e) {
				//do all the subscriptions
				MessagingCenter.Subscribe<string> (this, "NeedsRefresh", async(nr) => {
					await ViewModel.ExecuteLoadItemsCommand();
				});

				MessagingCenter.Subscribe<Today>(this, "fetchrequest", async(t) => {
					await Navigation.PushAsync(new FetchRequest1(t));
				});

				MessagingCenter.Subscribe<TodayViewModel>(this, "TodayLoaded", (t) => {

					//System.Diagnostics.Debug.WriteLine("TODAYLOADED");

					lvToday.IsRefreshing = false;

					this.Title = App.CurrentToday.Date.ToString ("MMM dd").ToUpper();
					if (App.CurrentToday.Date == DateTime.Today) {
						this.Title += " (Today)";
					}

					if (t.Todays.Count == 0)
					{
						lblNone.IsVisible = true;
					}
					else
					{
						lblNone.IsVisible = false;
					}

					if (App.myKids.Count == 0 && App.myPlaces.Count == 0 && App.myCircle.Count == 0)
					{
						//must be the first time!
						lblNone.IsVisible = false;
						NEWarrow.IsVisible = true;
						edNew.IsVisible = true;
					}
					else{					
						NEWarrow.IsVisible = false;
						edNew.IsVisible = false;
					}

					//and we really want to scroll to the NEXT item in the list
					foreach (Today tempT in ViewModel.Todays)
					{
						if (tempT.IsNext)
						{
							lvToday.ScrollTo(tempT, ScrollToPosition.Start, false);
							break;
						}
					}

					//let's load the messages to see if there's anything in my inbox
					MessageView mv = new MessageView();
					MessagingCenter.Send<MessageView>(mv, "LoadMessages");
				});

				MessagingCenter.Subscribe<MessageView> (this, "LoadMessages", (mv) => {	
					//System.Diagnostics.Debug.WriteLine ("LoadMessages from Today");
					MessageViewModel mvm = new MessageViewModel(App.client, null);
					App.hudder.showHUD("Loading Messages");
					mvm.ExecuteLoadItemsCommand().ConfigureAwait(true);
					App.hudder.hideHUD();
				});

				MessagingCenter.Subscribe<string> ("today", "messagesloaded", (ec) => {
					//System.Diagnostics.Debug.WriteLine ("Received messagesloaded in Today");
					lblMessageCount.Text = App.myMessages.Count.ToString();
					if (App.myMessages.Count > 0)
					{
						rlMessage.IsVisible = true;
					}
					else
					{
						rlMessage.IsVisible = false;
					}
				});

				MessagingCenter.Subscribe<string> ("today", "messagesupdated", (ec) => {
					//System.Diagnostics.Debug.WriteLine ("Received messagesupdated in Today");
					if (App.myMessages.Count > 0)
					{
						rlMessage.IsVisible = true;
					}
					else
					{
						rlMessage.IsVisible = false;
					}
				});

				ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
			};

			this.Disappearing += delegate(object sender, EventArgs e) {
				//do all the unsubscriptions
				//MessagingCenter.Unsubscribe<string> (this, "NeedsRefresh"); //had to comment this out because RouteDetail calls this when marking complete and it would load otherwise
				MessagingCenter.Unsubscribe<Today>(this, "fetchrequest");
				MessagingCenter.Unsubscribe<TodayViewModel>(this, "TodayLoaded");
				MessagingCenter.Unsubscribe<MessageView> (this, "LoadMessages");
				MessagingCenter.Unsubscribe<string> ("today", "messagesloaded");
				MessagingCenter.Unsubscribe<string> ("today", "messagesupdated");
			};




			this.Content = rl;
	
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
		public enum ActivityState
		{
			Complete,
			Next,
			Future
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



		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			dynamic c = BindingContext;
			this.Height = 195;
			Today t = (Today)c;

			if (t == null) {
				return;
			}

			TodayView.ActivityState currentState = TodayView.ActivityState.Future;
			if (t.IsNext) {
				currentState = TodayView.ActivityState.Next;
			}

			if (t.IsPickup) {
				if (t.PickupComplete) {
					currentState = TodayView.ActivityState.Complete;
					this.IsEnabled = false;
				}
			} 
			else 
			{
				if (t.DropOffComplete) {
					currentState = TodayView.ActivityState.Complete;
					this.IsEnabled = false;
				}
			}


			StackLayout mainlayout = new StackLayout ();
			mainlayout.Spacing = 0;
			mainlayout.Orientation = StackOrientation.Vertical;
			mainlayout.VerticalOptions = LayoutOptions.StartAndExpand;
			mainlayout.BackgroundColor = AppColor.AppPurple; //Color.FromRgb (73, 55, 55);//109);

			//make a purple header
			StackLayout sl = new StackLayout ();
			sl.BackgroundColor = AppColor.AppPurple;
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

			//if someone else is picking up, let's indicate that here
			if (t.IsPickup) {
				if (!string.IsNullOrEmpty (t.DefaultPickupAccount) && t.DefaultPickupAccount != App.myAccount.id) {
					//make a orange? header
					StackLayout slDefaultPickup = new StackLayout();
					slDefaultPickup.BackgroundColor = AppColor.AppOrange;
					slDefaultPickup.Orientation = StackOrientation.Horizontal;
					slDefaultPickup.VerticalOptions = LayoutOptions.Start;
					slDefaultPickup.HeightRequest = 32;
					Label sdpLabel = new Label ();
					sdpLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
					sdpLabel.FontSize = 16;
					sdpLabel.FontAttributes = FontAttributes.Bold;
					sdpLabel.TranslationX = 26;
					sdpLabel.LineBreakMode = LineBreakMode.NoWrap;
					sdpLabel.TextColor = Color.White;
					sdpLabel.Text = t.DefaultPickupAccountFirstName + " " + t.DefaultPickupAccountLastName + " is Picking Up";
					slDefaultPickup.Children.Add (sdpLabel);
					mainlayout.Children.Add (slDefaultPickup);
				}
			} else {
				if (!string.IsNullOrEmpty (t.DefaultDropOffAccount) && t.DefaultDropOffAccount != App.myAccount.id) {
					//make a orange? header
					StackLayout slDefaultDropoff = new StackLayout();
					slDefaultDropoff.BackgroundColor = AppColor.AppOrange;
					slDefaultDropoff.Orientation = StackOrientation.Horizontal;
					slDefaultDropoff.VerticalOptions = LayoutOptions.Start;
					slDefaultDropoff.HeightRequest = 32;
					Label sddLabel = new Label ();
					sddLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
					sddLabel.FontSize = 16;
					sddLabel.FontAttributes = FontAttributes.Bold;
					sddLabel.TranslationX = 26;
					sddLabel.LineBreakMode = LineBreakMode.NoWrap;
					sddLabel.TextColor = Color.White;
					sddLabel.Text = t.DefaultDropOffAccountFirstName + " " + t.DefaultDropOffAccountLastName + " is Dropping Off";
					slDefaultDropoff.Children.Add (sddLabel);
					mainlayout.Children.Add (slDefaultDropoff);
				}
			}



			Color bgColor = AppColor.AppGray;
			if (currentState == TodayView.ActivityState.Next) {
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
			case TodayView.ActivityState.Complete:
				triangle.Source = trigreen;
				break;
			case TodayView.ActivityState.Future:
				triangle.Source = trigray;
				break;
			case TodayView.ActivityState.Next:
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
				case TodayView.ActivityState.Complete:
					line.Source = linegreen;
					break;
				case TodayView.ActivityState.Future:
					line.Source = linegray;
					break;
				case TodayView.ActivityState.Next:
					line.Source = linepink;
					break;
				}
				slLine.Children.Add (line);
			}
			detailGrid.Children.Add (slLine, 0, 1, 1, 2);


			Image pin = new Image ();
			switch (currentState) {
			case TodayView.ActivityState.Complete:
				pin.Source = pingreen;
				break;
			case TodayView.ActivityState.Future:
				if (t.IsPickup) {
					pin.Source = pinupgray;
				} else {
					pin.Source = pindowngray;
				}
				break;
			case TodayView.ActivityState.Next:
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
				DateTime intermediate = DateTime.Today.Add (t.TSPickup.Subtract (TimeSpan.FromMinutes (t.EndPlaceTravelTime)));
				l.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture);
				//l.Text = DateTime.Parse (t.TSPickup).AddMinutes (-t.EndPlaceTravelTime).ToLocalTime ().ToString ("t");
			} else {
				DateTime intermediate = DateTime.Today.Add (t.TSDropOff.Subtract (TimeSpan.FromMinutes (t.StartPlaceTravelTime)));
				l.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture);
				//l.Text = DateTime.Parse (t.TSDropOff).AddMinutes (-t.StartPlaceTravelTime).ToLocalTime ().ToString ("t");
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
					Text = "\nDrive " + Math.Round(t.EndPlaceDistance,1) + " miles",
					ForegroundColor = Color.Gray,
					FontAttributes = FontAttributes.Italic
				});
			} else {
				l2.FormattedText.Spans.Add (new Span { Text = "Leave " + t.StartPlaceName, ForegroundColor = Color.Black });
				l2.FormattedText.Spans.Add (new Span {
					Text = "\nDrive " + Math.Round(t.StartPlaceDistance,1) + " miles",
					ForegroundColor = Color.Gray,
					FontAttributes = FontAttributes.Italic
				});
			}

			l2.FontAttributes = FontAttributes.Bold;
			detailGrid.Children.Add (l2, 2, 3, 0, 2 );

			//this means someone is picking up or dropping off
			if ((string.IsNullOrEmpty (t.DropOffMessageID) && string.IsNullOrEmpty (t.PickupMessageID)) || t.DropOffMessageStatus=="Canceled" || t.PickupMessageStatus=="Canceled"  || t.DropOffMessageStatus=="Pending Response" || t.PickupMessageStatus=="Pending Response") {
				//nobody's picking up or dropping off, so make the option to invite available
				//but only if it's mine to invite!
				if (currentState != TodayView.ActivityState.Complete && string.IsNullOrEmpty(t.Via)) {
					Button b = new Button ();
					switch (currentState) {
					case TodayView.ActivityState.Future:
						b.Image = arrowgray;
						break;
					case TodayView.ActivityState.Next:
						b.Image = arrowpink;
						break;
					}
					b.HorizontalOptions = LayoutOptions.Center;
					b.VerticalOptions = LayoutOptions.Start;
					b.Clicked += delegate(object sender, EventArgs e) {
						//await ((TodayView)this.ParentView.Parent.Parent).DisplayAlert ("Fetch!", "create a fetch request", "Cancel");
						//await ((TodayView)this.ParentView.Parent.Parent).Navigation.PushAsync(new FetchRequest1());
						//we probably should just fire a messagingcenter event!
						MessagingCenter.Send<Today> (t, "fetchrequest");
					};
					detailGrid.Children.Add (b, 3, 4, 0, 1);
				}
			}


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
				this.Height += CountOfNewlines (t.Address) * (l4.FontSize);
			}

			l4.FontAttributes = FontAttributes.Bold;
			StackLayout slDrop = new StackLayout ();
			slDrop.Orientation = StackOrientation.Vertical;
			slDrop.VerticalOptions = LayoutOptions.StartAndExpand;
			//slDrop.HeightRequest += 30;
			slDrop.Children.Add (l4);

			StackLayout slKids = new StackLayout ();
			slKids.Orientation = StackOrientation.Horizontal;
			slKids.WidthRequest = 60;
			//slKids.BackgroundColor = Color.Blue;

			//split the kids
			if (!string.IsNullOrEmpty (t.Kids)) {
				string[] kids = t.Kids.Split ('^');
				this.Height += 65;

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

					ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
						BorderColor = Color.Black,
						BorderThickness = 1,
						Aspect = Aspect.AspectFill,
						WidthRequest = 50,
						HeightRequest = 50,
						HorizontalOptions = LayoutOptions.Center,
						Source = azureURL
					};	
					slKids.WidthRequest += 60;	
					slKids.Children.Add (ci);
				}
				slDrop.Children.Add (slKids);
				detailGrid.Children.Add (slDrop, 2, 3, 2, 3);
			}

			//if there's a message attached to this Today, we need to wrap the grid in the pretty box with current Message Status
			string responseInfo = "";
			if (t.IsPickup && !string.IsNullOrEmpty (t.PickupMessageStatus) && t.PickupMessageStatus != "Canceled") {

				//because the starting point belongs to the sender, it makes no sense to show it here
				l.Text = "";
				l2.FontAttributes = FontAttributes.Italic;
				l2.Text = "Tap to see travel time from current location";

				detailGrid.BackgroundColor = Color.White;

				StackLayout slPickup = new StackLayout ();
				slPickup.HeightRequest = this.Height + 100;
				slPickup.WidthRequest = App.ScaledWidth - 20;
				slPickup.HorizontalOptions = LayoutOptions.Center;
				slPickup.BackgroundColor = Color.White;
				slPickup.Orientation = StackOrientation.Vertical;
				mainlayout.BackgroundColor = AppColor.AppGray;
				bv.BackgroundColor = AppColor.AppGray;
				bv.HeightRequest = 10;

				//first stick in the header
				StackLayout slp = new StackLayout ();
				switch (t.PickupMessageStatus) {
				case "Pending Response":
					slp.BackgroundColor = AppColor.AppPink;
					responseInfo = "Pending Response";
					break;
				case "Accepted":
					
					slp.BackgroundColor = AppColor.AppGreen;
					if (t.DefaultPickupAccountLastName.Length > 0) {
						responseInfo = t.PickupMessageStatus + " (" + t.DefaultPickupAccountFirstName + " " + t.DefaultPickupAccountLastName.Substring (0, 1) + ".)";
					} else {
						responseInfo = t.PickupMessageStatus + " (" + t.DefaultPickupAccountFirstName + ")";
					}
					break;
				case "Declined":
					slp.BackgroundColor = NamedColor.Red;
					responseInfo = "All Declined";
					break;
				}

				slp.Orientation = StackOrientation.Horizontal;
				slp.VerticalOptions = LayoutOptions.Start;
				slp.HeightRequest = 46;

				BoxView spacer = new BoxView ();
				spacer.WidthRequest = 5;
				slp.Children.Add (spacer);

				Label frs = new Label ();
				frs.TextColor = Color.White;
				if (t.RowType == "pickup") {
					slp.BackgroundColor = AppColor.AppGreen;
					frs.FontAttributes = FontAttributes.Bold;
					frs.Text = "You are picking up!";
					responseInfo = "";
				} else {
					frs.Text = "Fetch Request Sent";
				}
				frs.FontSize = 14;
				frs.VerticalOptions = LayoutOptions.Center;
				frs.HorizontalOptions = LayoutOptions.StartAndExpand;
				slp.Children.Add (frs);

				Label lri = new Label ();
				lri.TextColor = Color.White;
				lri.Text = responseInfo;
				lri.FontSize = 14;
				lri.FontAttributes = FontAttributes.Italic;
				lri.HorizontalOptions = LayoutOptions.End;
				lri.VerticalOptions = LayoutOptions.Center;
				slp.Children.Add (lri);

				spacer = new BoxView ();
				spacer.WidthRequest = 5;
				slp.Children.Add (spacer);

				slPickup.Children.Add (slp);
				slPickup.Children.Add (detailGrid);

				spacer = new BoxView ();
				spacer.BackgroundColor = AppColor.AppGray;
				spacer.HeightRequest = 10;
				slPickup.Children.Add (spacer);

				mainlayout.Children.Add (slPickup);

			} else if (!t.IsPickup && !string.IsNullOrEmpty (t.DropOffMessageStatus) && t.DropOffMessageStatus != "Canceled") {
				detailGrid.BackgroundColor = Color.White;
				//because the starting point belongs to the sender, it makes no sense to show it here
				l.Text = "";
				l2.FontAttributes = FontAttributes.Italic;
				l2.Text = "Tap to see travel time from current location";

				StackLayout slDropoff = new StackLayout ();
				slDropoff.Orientation = StackOrientation.Vertical;
				slDropoff.BackgroundColor = Color.White;
				slDropoff.HeightRequest = this.Height + 85;
				slDropoff.WidthRequest = App.ScaledWidth - 20;
				slDropoff.HorizontalOptions = LayoutOptions.Center;
				mainlayout.BackgroundColor = AppColor.AppGray;
				bv.BackgroundColor = AppColor.AppGray;
				bv.HeightRequest = 10;
				//first stick in the header
				StackLayout sld = new StackLayout ();

				switch (t.DropOffMessageStatus) {
				case "Pending Response":
					sld.BackgroundColor = AppColor.AppPink;
					responseInfo = "Pending Response";
					break;
				case "Accepted":
					sld.BackgroundColor = AppColor.AppGreen;
					if (t.DefaultDropOffAccountLastName.Length > 0) {
						responseInfo = t.DropOffMessageStatus + " (" + t.DefaultDropOffAccountFirstName + " " + t.DefaultDropOffAccountLastName.Substring (0, 1) + ".)";
					} else {
						responseInfo = t.DropOffMessageStatus + " (" + t.DefaultDropOffAccountFirstName + ")";
					}
					break;
				case "Declined":
					sld.BackgroundColor = NamedColor.Red;
					responseInfo = "All Declined";
					break;
				}

				sld.Orientation = StackOrientation.Horizontal;
				sld.VerticalOptions = LayoutOptions.Start;
				sld.HeightRequest = 46;

				BoxView spacer = new BoxView ();
				spacer.WidthRequest = 5;
				sld.Children.Add (spacer);

				Label frs = new Label ();
				frs.TextColor = Color.White;
				if (t.RowType == "pickup") {
					sld.BackgroundColor = AppColor.AppGreen;
					frs.FontAttributes = FontAttributes.Bold;
					frs.Text = "You are dropping off!";					
					responseInfo = "";
				} else {
					frs.Text = "Fetch Request Sent";
				}
				frs.HorizontalOptions = LayoutOptions.StartAndExpand;
				frs.VerticalOptions = LayoutOptions.Center;
				sld.Children.Add (frs);

				Label lri = new Label ();
				lri.TextColor = Color.White;
				lri.Text = responseInfo;
				lri.FontAttributes = FontAttributes.Italic;
				lri.VerticalOptions = LayoutOptions.Center;
				lri.HorizontalOptions = LayoutOptions.End;
				sld.Children.Add (lri);

				spacer = new BoxView ();
				spacer.WidthRequest = 5;
				sld.Children.Add (spacer);

				slDropoff.Children.Add (sld);
				slDropoff.Children.Add (detailGrid);

				spacer = new BoxView ();
				spacer.BackgroundColor = AppColor.AppGray;
				spacer.HeightRequest = 10;
				slDropoff.Children.Add (spacer);

				mainlayout.Children.Add (slDropoff);
			} else 
			{
				mainlayout.Children.Add (detailGrid);
			}				
			View = mainlayout;
		}
	}
}


