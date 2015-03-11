using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class Confirmation : ContentPage
	{
		public Confirmation (Invite invite)
		{
			InitializeComponent ();
			this.ViewModel = new ConfirmationViewModel (App.client, invite);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
		}

		protected ConfirmationViewModel ViewModel
		{
			get { return this.BindingContext as ConfirmationViewModel; }
			set { this.BindingContext = value; }
		}
		void OnDismissClicked(object sender, EventArgs args)
		{
			Navigation.PopModalAsync ();
		}
	}
}

