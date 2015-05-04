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


		public InviteResponseViewModel ()
		{

		}
		public InviteResponseViewModel(MobileServiceClient client, Today invite) : this()
		{
			this.client = client;
			this._currentInvite = invite;
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

				MessagingCenter.Send<Today>(_currentInvite, "Completed");

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
			//we've already got this data
		}

		public override async Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			//I suppose this woule be the cancel command


			IsLoading = false; //redundant
		}
	}
}

