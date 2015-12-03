using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace PickUpApp
{
	public class InviteResponseViewModel:BaseViewModel
	{
		private Today _currentInvite { get; set; }
		public Today CurrentInvite
		{
			get { return _currentInvite; }
		}

		private InviteMessage _thisMessage;
		public InviteMessage ThisMessage
		{
			get { return _thisMessage; }
			set{
				_thisMessage = value;
				NotifyPropertyChanged ();
			}
		}

		private ObservableCollection<InviteMessage> _Messages;
		public ObservableCollection<InviteMessage> Messages{
			get {
				return _Messages;
			}
			set{
				_Messages = value;
				NotifyPropertyChanged ();
			}
		}

		public InviteResponseViewModel ()
		{

		}
		public InviteResponseViewModel(MobileServiceClient client, Today invite) : this()
		{
			this.client = client;
			this._currentInvite = invite;
			Messages = new ObservableCollection<InviteMessage> ();
			LoadItemsCommand.Execute (null);
		}

		private Command cancelCommand;
		public Command CancelCommand
		{
			get { return cancelCommand ?? (cancelCommand = new Command(async () => await ExecuteCancelCommand())); }
		}
		public async Task ExecuteCancelCommand()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				Invite i = new Invite()
				{
					Id = _currentInvite.id
				};

				await client.InvokeApiAsync<Invite, EmptyClass>("cancelpickup",i);

				MessagingCenter.Send<Today>(_currentInvite, "Canceled");

			}
			catch (Exception ex) {
				IsLoading = false; //finally doesn't seem to catch these

				var page = new ContentPage();			
				await page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}
			finally{
				IsLoading = false;
			}

			IsLoading = false;  //redundant
		}


		public override async Task ExecuteLoadItemsCommand ()
		{
			try{
				IsLoading = true;
			//we have the invite data, but I really want the InviteMessage data
			var msgs = await client.InvokeApiAsync<Today,List<InviteMessage>>("getinvitemessages", _currentInvite);

			Messages.Clear ();
			foreach (var msg in msgs)
			{
				Messages.Add (msg);
			}
			}
			catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex.ToString ());
			}
			finally {
				IsLoading = false;
			}
		}

		public override async Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				//OK THIS IS NOT BEING USED ANY MORE
				var invitedata = await client.InvokeApiAsync<InviteMessage, EmptyClass>("savemessageDONOTUSE",ThisMessage);

//				var msgs = client.GetTable<InviteMessage>();
//
//				if (string.IsNullOrEmpty(ThisMessage.Id))
//					await msgs.InsertAsync(ThisMessage);
//				else
//					await msgs.UpdateAsync(ThisMessage);

				//really we need to just update the LoadItems
				await ExecuteLoadItemsCommand();
				//MessagingCenter.Send<InviteMessage>(ThisMessage, "MessageAdded");
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}
			finally{
				IsLoading = false;
			}
			IsLoading = false; //redundant
		}
	}
}

