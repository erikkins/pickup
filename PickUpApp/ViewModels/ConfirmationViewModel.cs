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

		private InviteInfo _thisInvite;
		public InviteInfo ThisInvite
		{
			get{
				return _thisInvite;
			}
			set{
				if (value != _thisInvite) {
					_thisInvite = value;
					NotifyPropertyChanged ();
				}
			}
		}
		public ConfirmationViewModel ()
		{
		}
		public ConfirmationViewModel(MobileServiceClient client, Invite invite) : this()
		{
			this.client = client;
			_currentInvite = invite;
			ThisInvite = new InviteInfo ();
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
					ThisInvite = inviteInfo[0];
				}

				Device.BeginInvokeOnMainThread(()=>{
				MessagingCenter.Send<InviteInfo>(ThisInvite, "confirmationloaded");
				});
				

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

