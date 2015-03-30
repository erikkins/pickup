using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace PickUpApp
{
	public class ConfirmationViewModel : BaseViewModel
	{
		private Invite _currentInvite { get; set; }

		private InviteInfo _currentInviteInfo;
		public InviteInfo CurrentInviteInfo { 
			get{return _currentInviteInfo; }
			set{
				_currentInviteInfo = value; NotifyPropertyChanged ();
			}
		}
		public ConfirmationViewModel ()
		{
		}
		public ConfirmationViewModel(MobileServiceClient client, Invite invite) : this()
		{
			this.client = client;
			_currentInvite = invite;
			CurrentInviteInfo = new InviteInfo ();
			LoadItemsCommand.Execute (null);
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			try
			{
				IsLoading = true;
				//load this invite!
				var inviteInfo = await client.InvokeApiAsync<Invite, List<InviteInfo>>("getinviteinfo", _currentInvite);
				if (inviteInfo.Count > 0)
				{
					CurrentInviteInfo = inviteInfo[0];
				}


				MessagingCenter.Send<InviteInfo>(CurrentInviteInfo, "confirmationloaded");
				

			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Kids. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
			finally {
				IsLoading = false;
			}
		}
	}
}

