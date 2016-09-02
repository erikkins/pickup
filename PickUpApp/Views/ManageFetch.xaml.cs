using System;
using System.Collections.Generic;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace PickUpApp
{
	public partial class ManageFetch : ContentPage
	{
		public ManageFetch (Today CurrentToday, bool allowCancel)
		{
			InitializeComponent ();
			this.ViewModel = new MessageViewModel (App.client,null);
			this.Padding = new Thickness(0, Device.OnPlatform(0, 0, 0), 0, 0);
			this.BackgroundColor = AppColor.AppGray;

			BoxView bvSpace = new BoxView ();
			bvSpace.HeightRequest = 5;
			bvSpace.BackgroundColor = AppColor.AppGray;
			//stacker.Children.Add (bvSpace);

			#region TodayStyleHeaderSection
			//start FetchInfo (WHOLLY STOLEN FROM THE TODAY SCREEN...NO REUSE HERE!)
			var t = CurrentToday;


			TodayView.ActivityState currentState = TodayView.ActivityState.Future;
			if (t.IsNext) {
				currentState = TodayView.ActivityState.Next;
			}

			if (t.IsPickup) {
				if (t.PickupComplete) {
					currentState = TodayView.ActivityState.Complete;
				}
			} 
			else 
			{
				if (t.DropOffComplete) {
					currentState = TodayView.ActivityState.Complete;
				}
			}
			/*
			Color bgColor = AppColor.AppGray;
			if (currentState == TodayView.ActivityState.Next) {
				bgColor = Color.White;
			}
			*/


			Grid detailGrid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				RowSpacing = 0,
				//ColumnSpacing = 0,
				RowDefinitions = 
				{
					//new RowDefinition { Height = new GridLength(25, GridUnitType.Absolute) },
					//new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)},
					new RowDefinition {Height = GridLength.Auto}//new GridLength(48, GridUnitType.Absolute) }
				},
				ColumnDefinitions = 
				{
					new ColumnDefinition { Width = new GridLength(58, GridUnitType.Absolute) },
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(46, GridUnitType.Absolute) }
				}
				};	

			const string pingreen = "icn_pin_check.png";
			const string pinuppink = "icn_pin_up_pink.png";
			const string pindownpink = "icn_pin_dwn_pink";
			const string pindowngray = "icn_pin_dwn_grey.png";
			const string pinupgray = "icn_pin_up_grey";
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
			detailGrid.Children.Add (pin, 0, 1, 0, 3);

		
			Label l3 = new Label ();
			l3.TextColor = Color.Black;
			if (t.IsPickup) {
				DateTime intermediate = DateTime.Today.Add (t.TSPickup);
				l3.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture);
			} else {
				DateTime intermediate = DateTime.Today.Add (t.TSDropOff);
				l3.Text = intermediate.ToString (@"h\:mm", System.Globalization.CultureInfo.InvariantCulture); 
			}
			l3.VerticalOptions = LayoutOptions.Start;
			l3.FontAttributes = FontAttributes.Bold;
			detailGrid.Children.Add (l3, 1, 2, 0, 3);
		


			StackLayout slDrop = new StackLayout ();

			slDrop.Orientation = StackOrientation.Vertical;
			slDrop.VerticalOptions = LayoutOptions.StartAndExpand;
			//slDrop.BackgroundColor = Color.Blue;
			//slDrop.HeightRequest += 30;
			Label lDate = new Label();
			lDate.FontAttributes = FontAttributes.Bold;
			lDate.TextColor = Color.Black;
			if (t.IsPickup)
			{
				if (DateTime.Now.Date == t.PickupDT.Date)
				{
					lDate.Text = "TODAY";
				}
				else{
					lDate.Text = t.PickupDT.ToString ("dddd") + " " + t.PickupDT.ToString ("d");
				}
			}
			else
			{
				if (DateTime.Now.Date == t.DropoffDT.Date)
				{
					lDate.Text = "TODAY";
				}
			else{
					lDate.Text = t.DropoffDT.ToString ("dddd") + " " + t.DropoffDT.ToString ("d");
			}
				
			}

			slDrop.Children.Add(lDate);

			Label l4 = new Label ();
			l4.FormattedText = new FormattedString ();
			if (t.IsPickup) {
				l4.FormattedText.Spans.Add (new Span { Text = t.Activity + " Pickup", ForegroundColor = Color.Black });
			} else {
				l4.FormattedText.Spans.Add (new Span { Text = t.Activity + " Dropoff", ForegroundColor = Color.Black });
			}
			l4.FontAttributes = FontAttributes.Bold;
			//no need for address here
			//l4.FormattedText.Spans.Add (new Span { Text = "\n" + t.Address, ForegroundColor = Color.Gray});
			l4.LineBreakMode = LineBreakMode.WordWrap;
			l4.VerticalOptions = LayoutOptions.StartAndExpand;
			slDrop.Children.Add (l4);

			StackLayout slKids = new StackLayout ();
			slKids.Orientation = StackOrientation.Horizontal;
			slKids.VerticalOptions = LayoutOptions.Start;
			//slKids.WidthRequest = 60;
			//slKids.BackgroundColor = Color.Blue;

			//split the kids
			if (!string.IsNullOrEmpty (t.Kids)) {
				string[] kids = t.Kids.Split ('^');
				//this.Height += 95;
				//slDrop.HeightRequest = addressLabelHeight + 75;

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


					CachedImage cachedimg = new CachedImage ();
					cachedimg.Source = azureURL;
					cachedimg.CacheDuration = TimeSpan.FromDays (30);
					cachedimg.DownsampleToViewSize = true;
					cachedimg.TransparencyEnabled = false;
					cachedimg.Aspect = Aspect.AspectFill;
					cachedimg.HeightRequest = 50;
					cachedimg.WidthRequest = 50;
					cachedimg.HorizontalOptions = LayoutOptions.Center;
					//cachedimg.VerticalOptions = LayoutOptions.Center;
					cachedimg.Transformations.Add (new FFImageLoading.Transformations.CircleTransformation (1, "000000"));
					slKids.WidthRequest += 60;
					slKids.Children.Add (cachedimg);
				}
				slDrop.Children.Add (slKids);
				detailGrid.Children.Add (slDrop, 2, 3, 0, 3);
			}

			//make a white box to hold it all
			StackLayout slPickup = new StackLayout ();		
			slPickup.WidthRequest = App.ScaledWidth - 20;
			slPickup.HorizontalOptions = LayoutOptions.Center;
			slPickup.BackgroundColor = Color.White;
			slPickup.Orientation = StackOrientation.Vertical;

			slPickup.Children.Add (detailGrid);
			BoxView bvnew = new BoxView ();
			bvnew.HeightRequest = 2;
			slPickup.Children.Add (bvnew);

			Frame f = new Frame ();
			f.WidthRequest = App.ScaledWidth - 20;
			f.Content = slPickup;
			f.Padding = 2;
			f.HasShadow = false;
//			f.Margin =10;  	//ANDROID DOESN'T SUPPORT MARGINS APPARENTLY!

			stacker.Children.Add (f);

			//end FetchInfo
			#endregion



				Button btnCancel = new Button ();
				btnCancel.VerticalOptions = LayoutOptions.Start;
				btnCancel.HorizontalOptions = LayoutOptions.CenterAndExpand;
				btnCancel.HeightRequest = 50;
				btnCancel.WidthRequest = (App.ScaledWidth) - 20;
				btnCancel.FontAttributes = FontAttributes.Bold;
				btnCancel.FontSize = 18;
				btnCancel.BorderRadius = 8;
				btnCancel.BackgroundColor = Color.FromRgb (73, 55, 109);
				btnCancel.TextColor = Color.FromRgb (84, 210, 159);
				btnCancel.Text = "Cancel Fetch";
			if (allowCancel) {
				stacker.Children.Add (btnCancel);
				bvSpace = new BoxView ();
				bvSpace.HeightRequest = 2;
				//stacker.Children.Add (bvSpace);
			}




			ChatListView clv = new ChatListView ();
			clv.VerticalOptions = LayoutOptions.EndAndExpand;
			clv.BackgroundColor = AppColor.AppGray;
			clv.ItemsSource = ViewModel.ChatMessages;
			clv.ItemTemplate = new DataTemplate(CreateMessageCell);
			clv.HasUnevenRows = true;

			stacker.Children.Add (clv);
//			clv.SizeChanged+= delegate(object sender, EventArgs e) {
//				//be sure to scroll to the bottom!
//				Device.BeginInvokeOnMainThread(()=>{
//					if (ViewModel.ChatMessages.Count > 0)
//					{						
//						//clv.SelectedItem = ViewModel.ChatMessages[ViewModel.ChatMessages.Count-1];
//						clv.ScrollTo(ViewModel.ChatMessages[ViewModel.ChatMessages.Count-1],ScrollToPosition.End, true);
//					}
//				});
//			};


			stacker.Children.Add (bvSpace);

			var inputBox = new Entry();
			inputBox.HorizontalOptions = LayoutOptions.FillAndExpand;
			inputBox.Keyboard = Keyboard.Chat;
			inputBox.Placeholder = "Type a message...";
			inputBox.PlaceholderColor = AppColor.AppGray;
			inputBox.TextColor = Color.Black;
			inputBox.HeightRequest = 30;

			var sendButton = new Button();
			sendButton.Text = " Send ";
			sendButton.VerticalOptions = LayoutOptions.EndAndExpand;
			//sendButton.SetBinding(Button.CommandProperty, new Binding("SendMessageCommand"));
			sendButton.Clicked += async delegate(object sender, EventArgs e) {
				if (string.IsNullOrEmpty(inputBox.Text))
				{
					return;
				}
				MessageView mv = new MessageView ();
				mv.SenderID = App.myAccount.id;
				mv.Message = inputBox.Text;
				mv.Link = CurrentToday.id;
				if (CurrentToday.IsPickup) {
					mv.LinkDetail = "pickup";
				} else {
					mv.LinkDetail = "dropoff";
				}
				mv.MessageType = "chat";
				mv.Route = "app";
				mv.Status = "new";

				mv.MessageToday = CurrentToday;
				await ViewModel.ExecuteCreateCommand(mv);
				inputBox.Text = "";
				inputBox.Focus();
			};
			//			if (Device.OS == TargetPlatform.WinPhone)
			//			{
			//				sendButton.BackgroundColor = Color.Green;
			//				sendButton.BorderColor = Color.Green;
			//				sendButton.TextColor = Color.White; 
			//			}




			StackLayout slHoriz = new StackLayout ();
			slHoriz.Padding = new Thickness (5);
//			slHoriz.Margin = 0;  //ANDROID DOESN'T HAVE MARGIN ON STACKLAYOUT!!!
			slHoriz.Orientation = StackOrientation.Horizontal;
			slHoriz.Children.Add (inputBox);
			slHoriz.Children.Add (sendButton);
			stacker.Children.Add (slHoriz);
			inputBox.Focus ();

			BoxViewKeyboardHeight bvkh = new BoxViewKeyboardHeight ();
			stacker.Children.Add (bvkh);

			//			bvkh.BoxChanged += delegate(object sender, EventArgs e) {
			//				//we want to scroll here
			//				Device.BeginInvokeOnMainThread(()=>{
			//					if (ViewModel.ChatMessages.Count > 0)
			//					{						
			//						clv.ScrollTo(ViewModel.ChatMessages[ViewModel.ChatMessages.Count-1],ScrollToPosition.End, false);
			//					}
			//				});
			//			};



			this.Appearing += delegate(object sender, EventArgs e) {
				App.InChatSession = true;
				inputBox.Focus();

				MessagingCenter.Subscribe<MessageView>(this, "chatsent", async(msg)=>{
					
					await ViewModel.ExecuteLoadChatCommand (CurrentToday).ConfigureAwait (false);
				});

				MessagingCenter.Subscribe<EmptyClass>(this, "chatloaded", (ec)=>{
					App.hudder.hideHUD();
					Device.BeginInvokeOnMainThread(()=>{
						
						if (ViewModel.ChatMessages.Count > 0)
						{
							clv.ScrollTo(ViewModel.ChatMessages[ViewModel.ChatMessages.Count-1],ScrollToPosition.End, false);
						}
					});
				});


				MessagingCenter.Subscribe<MessageView>(this, "chatreceived", (mv)=>{
					//this is a MessageView with ONLY a messageid in it...we need to get the other stuff to know which conversation we're in
					ViewModel.ExecuteLoadChatCommandFromMessage(mv).ConfigureAwait(false);
				});


				//initial load
				App.hudder.showHUD("Loading Chat...");
				ViewModel.ExecuteLoadChatCommand (CurrentToday).ConfigureAwait (true);

			};

			this.Disappearing += delegate(object sender, EventArgs e) {
				App.InChatSession = false;

				MessagingCenter.Unsubscribe<MessageView>(this, "chatsent");
				MessagingCenter.Unsubscribe<EmptyClass>(this, "chatloaded");
				MessagingCenter.Unsubscribe<MessageView>(this, "chatreceived");
			};

			/*
			MessageView mv = new MessageView ();
			mv.Message = "can you be a few minutes early? I have something for you";
			mv.SenderID = App.myAccount.id;
			ViewModel.ChatMessages.Add (mv);

			mv = new MessageView ();
			mv.Message = "sure...10?";
			mv.SenderID = "123";
			mv.Sender = "Bob";
			ViewModel.ChatMessages.Add (mv);

			mv = new MessageView ();
			mv.Message = "that works! thx";
			mv.SenderID = App.myAccount.id;
			ViewModel.ChatMessages.Add (mv);

			mv = new MessageView ();
			mv.Message = "np";
			mv.SenderID = "123";
			mv.Sender = "Bob";
			ViewModel.ChatMessages.Add (mv);
			*/





			MessagingCenter.Subscribe<Today> (this, "fetchcanceled", async(ct) => {
				//I guess pop this and refresh today!
				App.hudder.hideHUD();
				try{
				await Navigation.PopToRootAsync();
				}
				catch(Exception ex)
				{
					//why are these excepting??
				}
				MessagingCenter.Send<string>("cancelfetch", "NeedsRefresh");
			});

			btnCancel.Clicked += async delegate(object sender, EventArgs e) {


				bool sure = await DisplayAlert("Double check", "Are you sure you want to cancel?", "Yes", "Cancel");
				if (sure)
				{
					App.hudder.showHUD("Canceling Fetch");
					//await ((RouteDetail)this.ParentView.Parent.Parent).DisplayAlert ("DONE!", "Complete", "Cancel");
					//MessagingCenter.Send<Today>(CurrentToday, "cancelfetch");
					await ViewModel.ExecuteCancelCommand(CurrentToday).ConfigureAwait(false);
				}
			};
		}

		protected MessageViewModel ViewModel
		{
			get { return this.BindingContext as MessageViewModel; }
			set { this.BindingContext = value; }
		}

		private Cell CreateMessageCell()
		{
			var timestampLabel = new Label();
			timestampLabel.SetBinding(Label.TextProperty, new Binding("Created", stringFormat: "[{0:HH:mm}]"));
			timestampLabel.TextColor = Color.Silver;
			timestampLabel.FontSize = 14;
			//timestampLabel.Font = Font.SystemFontOfSize(14);

			var authorLabel = new Label();
			authorLabel.SetBinding(Label.TextProperty, new Binding("Sender", stringFormat: "{0}: "));
			authorLabel.TextColor = Device.OnPlatform(Color.Blue, Color.Yellow, Color.Yellow);
			authorLabel.FontSize = 14;
			//authorLabel.Font = Font.SystemFontOfSize(14);

			var messageLabel = new Label();
			messageLabel.SetBinding(Label.TextProperty, new Binding("Message"));
			messageLabel.FontSize = 14;
			//messageLabel.Font = Font.SystemFontOfSize(14);

			var stack = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = {authorLabel, messageLabel}
			};

			if (Device.Idiom == TargetIdiom.Tablet)
			{
				stack.Children.Insert(0, timestampLabel);
			}

			var view = new MessageViewCell
			{
				View = stack
			};
			return view;
		}
	}
}

