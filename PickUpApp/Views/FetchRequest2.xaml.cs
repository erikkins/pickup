using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace PickUpApp
{
	public partial class FetchRequest2 : ContentPage
	{
		public FetchRequest2 (Today currentToday, List<AccountCircle>selectedRecipients)
		{
			InitializeComponent ();

			MessageView mv = new MessageView ();
			mv.Link = currentToday.id;
			if (currentToday.IsPickup) {
				mv.Title = "Request Pickup Help";
				mv.LinkDetail = "pickup";
			} else {
				mv.Title = "Request Dropoff Help";
				mv.LinkDetail = "dropoff";
			}
			mv.MessageType = "pickup";
			mv.Route = "app";
			mv.Status = "new";

			mv.SenderID = App.myAccount.id;
			mv.SenderPhotoURL = App.myAccount.PhotoURL;
			mv.Sender = "Me";
			mv.MessageToday = currentToday;
			mv.RecipientString = "";

			mv.MessageToday = currentToday;
			mv.IsActionable = false;

			foreach (AccountCircle ac in selectedRecipients) {
				mv.RecipientString += ac.Fullname + ", ";
			}
			if (mv.RecipientString.EndsWith (", ")) {
				mv.RecipientString = mv.RecipientString.Remove (mv.RecipientString.Length - 2);
			}



			ExtendedTableView etv = new ExtendedTableView ();
			etv.HeightRequest = 303;

			//etv.HasUnevenRows = true;
			etv.RowHeight = 65;
			etv.BindingContext = mv;
			etv.Intent = TableIntent.Data;
			TableSection ts = new TableSection ();
			ts.Add (new SimpleBoundLabelCell ("To", "RecipientString"));
			ts.Add (new SimpleBoundTextCell ("Subject", "Title", Keyboard.Text));
			ts.Add (new SimpleBoundTextAreaCell ("Enter message", "Message"));

			List<MessageView> kludgyFixlist = new List<MessageView> ();
			kludgyFixlist.Add (mv);

			ExtendedListView elv = new ExtendedListView ();
			elv.BackgroundColor = AppColor.AppGray;
			elv.HasUnevenRows = true;
			elv.ItemsSource = kludgyFixlist;
			elv.ItemTemplate = new DataTemplate (typeof(PickupRequestCell));

			/*
			PickupRequestCell prc = new PickupRequestCell ();
			prc.IsActionable = false;
			ts.Add (prc);
			*/
			etv.Root.Add (ts);
			stacker.Children.Add (etv);

			stacker.Children.Add (elv);


			this.ViewModel = new MessageViewModel (App.client, mv);

			this.ToolbarItems.Add (new ToolbarItem ("Send", null, async() => {
				//pop the calendar window
				//await DisplayAlert("CAL!", "show the calendar", "Cancel");
				App.hudder.showHUD("Sending Fetch Request");
				foreach(AccountCircle ac in selectedRecipients)
				{
					mv.RecipientID = ac.id;
					await this.ViewModel.ExecuteCreateCommand(mv);
				}
				//clear all the previously selected circles
				foreach (AccountCircle ac in App.myCircle)
				{
					ac.Selected = false;
				}



				//we need to basically save a message using the contact info from the previous screen
				//await this.ViewModel.ExecuteAddEditCommand();
				 //await Navigation.PopToRootAsync();
			}));

			MessagingCenter.Subscribe<MessageView>(this, "messagesent", async(msg)=>{
				App.hudder.hideHUD();
				MessagingCenter.Unsubscribe<MessageView>(this, "messagesent");
				try{
				await Navigation.PopToRootAsync();
				}
				catch{}
				MessagingCenter.Send<string>("fetch", "NeedsRefresh");
			});
		}

		protected MessageViewModel ViewModel
		{
			get { return this.BindingContext as MessageViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

