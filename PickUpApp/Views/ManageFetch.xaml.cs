using System;
using System.Collections.Generic;

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
			stacker.Children.Add (bvSpace);


				Button btnCancel = new Button ();
				btnCancel.VerticalOptions = LayoutOptions.CenterAndExpand;
				btnCancel.HorizontalOptions = LayoutOptions.CenterAndExpand;
				btnCancel.HeightRequest = 50;
				btnCancel.WidthRequest = (App.ScaledWidth) - 50;
				btnCancel.FontAttributes = FontAttributes.Bold;
				btnCancel.FontSize = 18;
				btnCancel.BorderRadius = 8;
				btnCancel.BackgroundColor = Color.FromRgb (73, 55, 109);
				btnCancel.TextColor = Color.FromRgb (84, 210, 159);
				btnCancel.Text = "Cancel Fetch";
			if (allowCancel) {
				stacker.Children.Add (btnCancel);
				stacker.Children.Add (bvSpace);
			}

			//need to be able to do interfetch messaging
			ChatListView clv = new ChatListView ();
			clv.BackgroundColor = AppColor.AppGray;
			clv.ItemsSource = ViewModel.ChatMessages;
			clv.ItemTemplate = new DataTemplate(CreateMessageCell);
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

			this.Appearing += delegate(object sender, EventArgs e) {
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
				ViewModel.ExecuteLoadChatCommand (CurrentToday).ConfigureAwait (false);
			};

			this.Disappearing += delegate(object sender, EventArgs e) {
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

			var inputBox = new Entry();
			inputBox.HorizontalOptions = LayoutOptions.FillAndExpand;
			inputBox.Keyboard = Keyboard.Chat;
			inputBox.Placeholder = "Type a message...";
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

			};
//			if (Device.OS == TargetPlatform.WinPhone)
//			{
//				sendButton.BackgroundColor = Color.Green;
//				sendButton.BorderColor = Color.Green;
//				sendButton.TextColor = Color.White; 
//			}




			StackLayout slHoriz = new StackLayout ();
			slHoriz.Padding = new Thickness (5);
			slHoriz.Orientation = StackOrientation.Horizontal;
			slHoriz.Children.Add (inputBox);
			slHoriz.Children.Add (sendButton);
			stacker.Children.Add (slHoriz);

			BoxViewKeyboardHeight bvkh = new BoxViewKeyboardHeight ();
			stacker.Children.Add (bvkh);
//			bvkh.BoxChanged += delegate(object sender, EventArgs e) {
//				//we want to scroll here
//				Device.BeginInvokeOnMainThread(()=>{
//					if (ViewModel.ChatMessages.Count > 0)
//					{
//						System.Threading.Tasks.Task.Delay(5000);
//						clv.ScrollTo(ViewModel.ChatMessages[ViewModel.ChatMessages.Count-1],ScrollToPosition.End, false);
//					}
//				});
//			};



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

