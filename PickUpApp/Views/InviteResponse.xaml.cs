using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class InviteResponseView : ContentPage
	{
		public InviteResponseView (Today invite)
		{
			InitializeComponent ();
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			this.ViewModel = new InviteResponseViewModel (App.client, invite);
			//lstMessages.ScrollTo(ViewModel.Messages[ViewModel.Messages.Count], ScrollToPosition.End, false);
			ViewModel.Messages.CollectionChanged += delegate(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {				
				if (ViewModel.Messages.Count > 0)
				{
					Device.BeginInvokeOnMainThread(()=>{
					lstMessages.SelectedItem = ViewModel.Messages[ViewModel.Messages.Count-1];
					});
				}
				//await System.Threading.Tasks.Task.Delay(500);
				//Device.BeginInvokeOnMainThread(()=>{
				//lstMessages.ScrollTo(ViewModel.Messages[ViewModel.Messages.Count-1], ScrollToPosition.End, false);
				//});
			};

			MessagingCenter.Subscribe<InviteMessage>(this, "arrived", (s) =>
				{
					//presumably there's a new message arriveth
					ViewModel.LoadItemsCommand.Execute(null);	
				});

			MessagingCenter.Subscribe<Today> (this, "Completed", async(s) => {

				//also need to reload Today
				try{
					await Navigation.PopModalAsync();
				}
				catch(Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("POPIR SUX:" + ex.Message + ex.StackTrace);
				}
				MessagingCenter.Send<string>("InviteResponse", "NeedsRefresh");

			});
		}

		public void OnClose (object sender, EventArgs e) {
			Navigation.PopModalAsync ();
		}

		public void OnCancel (object sender, EventArgs e) {
			//maybe make sure they want to cancel?
			bool accepted = DisplayAlert("Confirm", "Are you sure you want to cancel the PickUp?", "Yes", "No").Result;
			if (accepted) {
				ViewModel.ExecuteCancelCommand ().ConfigureAwait (false);
				Navigation.PopModalAsync ();
			}
		}

		public void OnMessage (object sender, EventArgs e) {
			//basically we're saving a new row into the InviteMessage table
			InviteMessage im = new InviteMessage();
			im.AccountID = App.myAccount.id;
			im.InviteID = this.ViewModel.CurrentInvite.id;
			im.Message = messageEditor.Text;

			ViewModel.ThisMessage = im;
			ViewModel.ExecuteAddEditCommand ().ConfigureAwait (false);
			messageEditor.Text = "";
		}
		protected InviteResponseViewModel ViewModel
		{
			get { return this.BindingContext as InviteResponseViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

