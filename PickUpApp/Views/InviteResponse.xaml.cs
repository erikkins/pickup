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
		public void OnCancel (object sender, EventArgs e) {
			ViewModel.ExecuteCancelCommand ().ConfigureAwait (false);
			Navigation.PopModalAsync ();

		}
		public void OnMessage (object sender, EventArgs e) {
			Navigation.PopModalAsync ();
		}
		protected InviteResponseViewModel ViewModel
		{
			get { return this.BindingContext as InviteResponseViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

