using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;

namespace PickUpApp
{	
	public partial class AddEditKid : ContentPage
	{	
		public AddEditKid (Kid selectedKid)
		{
			InitializeComponent ();
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			btnSave.Clicked += HandleClicked;
			btnCancel.Clicked += HandleClicked1;
			this.ViewModel = new KidAddEditViewModel (App.client);
			if (selectedKid != null) {
				//txtFirstname.Text = selectedKid.Firstname;
				//txtLastname.Text = selectedKid.Lastname;
				ViewModel.CurrentKid = selectedKid;
			}
			/*
			MessagingCenter.Subscribe<Kid>(this, "KidAdded", (s) =>
				{
					//ViewModel.Refresh();
					Navigation.PopModalAsync();
				});
			*/
		}

		void HandleClicked1 (object sender, EventArgs e)
		{
			Navigation.PopModalAsync ();
		}

		void HandleClicked (object sender, EventArgs e)
		{
			if (ViewModel.CurrentKid == null) {
				ViewModel.CurrentKid = new Kid ();
			} 
			//ViewModel.CurrentKid.Firstname = txtFirstname.Text;
			//ViewModel.CurrentKid.Lastname = txtLastname.Text;

			ViewModel.ExecuteAddEditCommand ().ConfigureAwait(false);
		}
		protected KidAddEditViewModel ViewModel
		{
			get { return this.BindingContext as KidAddEditViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

