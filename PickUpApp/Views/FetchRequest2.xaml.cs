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
			mv.MessageToday = currentToday;
			mv.RecipientString = "";

			foreach (AccountCircle ac in selectedRecipients) {
				mv.RecipientString += ac.Fullname + ", ";
			}
			if (mv.RecipientString.EndsWith (", ")) {
				mv.RecipientString = mv.RecipientString.Remove (mv.RecipientString.Length - 2);
			}

			ExtendedTableView etv = new ExtendedTableView ();
			etv.HasUnevenRows = true;

			etv.BindingContext = mv;
			etv.Intent = TableIntent.Form;
			TableSection ts = new TableSection ();
			ts.Add (new SimpleBoundLabelCell ("To", "RecipientString"));
			ts.Add (new SimpleBoundTextCell ("Subject", "Title"));
			ts.Add (new SimpleBoundTextAreaCell ("Enter message", "Message"));

			PickupRequestCell prc = new PickupRequestCell ();
			prc.IsActionable = false;
			ts.Add (prc);
			etv.Root.Add (ts);
			stacker.Children.Add (etv);


			this.ViewModel = new MessageViewModel (App.client, mv);

			this.ToolbarItems.Add (new ToolbarItem ("Send", null, async() => {
				//pop the calendar window
				//await DisplayAlert("CAL!", "show the calendar", "Cancel");
				foreach(AccountCircle ac in selectedRecipients)
				{
					mv.RecipientID = ac.id;
					await this.ViewModel.ExecuteCreateCommand(mv);
				}
				//we need to basically save a message using the contact info from the previous screen
				//await this.ViewModel.ExecuteAddEditCommand();
				 await Navigation.PopToRootAsync();
			}));
		}

		protected MessageViewModel ViewModel
		{
			get { return this.BindingContext as MessageViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

