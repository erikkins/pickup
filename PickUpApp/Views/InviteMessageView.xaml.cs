using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class InviteMessageView : ContentPage
	{
		public InviteMessageView (InviteInfo invite)
		{
			InitializeComponent ();
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			Today t = new Today ();
			t.id = invite.Id;
			this.ViewModel = new InviteResponseViewModel (App.client, t);

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

			//if we get a notification from the other sender...
			MessagingCenter.Subscribe<InviteMessage>(this, "arrived", (s) =>
			{
				//presumably there's a new message arriveth
				ViewModel.LoadItemsCommand.Execute(null);
			});
		}
			
		public void OnClose (object sender, EventArgs e) {
				Navigation.PopModalAsync ();
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

			//Navigation.PopModalAsync ();
		}

		protected InviteResponseViewModel ViewModel
		{
			get { return this.BindingContext as InviteResponseViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

