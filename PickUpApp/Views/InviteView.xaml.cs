using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms.Maps;

namespace PickUpApp
{
	public partial class InviteView : ContentPage
	{
		public InviteView (Invite invite)
		{
			InitializeComponent ();
			this.ViewModel = new InviteViewModel (App.client, invite);
			MessagingCenter.Subscribe<InviteInfo> (this, "inviteinfoloaded", (ii) => {
				Xamarin.Forms.Maps.Position thispos = new Xamarin.Forms.Maps.Position (ii.Latitude, ii.Longitude);

				map.MoveToRegion (MapSpan.FromCenterAndRadius (thispos,
					Distance.FromMiles (0.1)));
				map.Pins.Add (new Pin {
					Label = ii.Address,
					Position = thispos,
					Address = ii.Address
				});

				//map.MoveToRegion (new Xamarin.Forms.Maps.MapSpan (map.VisibleRegion.Center, ii.Latitude, ii.Longitude));	
			});
			this.ViewModel.ExecuteLoadItemsCommand ().ConfigureAwait (false);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			MessagingCenter.Subscribe<InviteInfo> (this, "InfoSubmitted", (s) => {
				Navigation.PopModalAsync ();
			});

		}

		protected InviteViewModel ViewModel
		{
			get { return this.BindingContext as InviteViewModel; }
			set { this.BindingContext = value; }
		}
		void OnDismissClicked(object sender, EventArgs args)
		{
			this.ViewModel.CurrentInviteInfo.Solved = false;
			this.ViewModel.CurrentInviteInfo.SolvedBy = App.myAccount.id;
			this.ViewModel.ExecuteAddEditCommand ().ConfigureAwait (false);
		}
		void OnAcceptClicked(object sender, EventArgs args)
		{
			this.ViewModel.CurrentInviteInfo.Solved = true;
			this.ViewModel.CurrentInviteInfo.SolvedBy = App.myAccount.id;
			this.ViewModel.ExecuteAddEditCommand ().ConfigureAwait (false);
		}
	}
}

