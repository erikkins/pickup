using System;
using System.Collections.Generic;
using Xamarin.Forms;
using PickUpApp.ViewModels;

namespace PickUpApp
{
	public partial class CircleSelect : ContentPage
	{
		private Schedule _selectedSchedule;


		public CircleSelect (Schedule schedule)
		{
			InitializeComponent ();
			_selectedSchedule = schedule;

			this.ViewModel = new CircleSelectViewModel (_selectedSchedule, App.client);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			placePicker.Items.Clear ();
			foreach (AccountPlace ap in App.myPlaces)
			{
				placePicker.Items.Add (ap.PlaceName);
			}
			if (App.myPlaces.Count == 1) {
				//if there's only one, always pick it
				placePicker.SelectedIndex = 0;
			}
			//and arguably...always set it to something...SOMETHING is always going to be statistically more correct than nothing

			btnCancel.Clicked += (object sender, EventArgs e) => 
			{
				lstCircle.SelectedItem = null;
				Navigation.PopModalAsync();
			};
			btnSend.Clicked += async (object sender, EventArgs e) => 
			{ 
				//ok, we have the list of recipients
				//now we need to notify each of them
				//I guess the server needs to know whether to message through
				//push notification or with a Twilio text message

				//make sure the placePicker has a place selected
				if (placePicker.SelectedIndex < 0)
				{
					await DisplayAlert("Oops", "You must select a dropoff location!", "OK");
					return;
				}

				//we're going to send the request for help, including the recipient IDs
				string selectedFriends = "";
				foreach (Account a in App.myCircle)
				{
					if (a.Selected)
					{
						selectedFriends += a.id + ",";
					}
				}
				if (selectedFriends == "")
				{
					await DisplayAlert("Oops", "You must select atleast one recipient!", "OK");
					return;
				}
				ViewModel.ReturnTo = placePicker.Items[placePicker.SelectedIndex];

				//take off the last comma, for savings
				selectedFriends = selectedFriends.Substring(0, selectedFriends.Length-1);

				ViewModel.MySelectedCircle = selectedFriends;
				//this should create a new invite and send out the calls for each recipient
				await ViewModel.ExecuteAddEditCommand();
				lstCircle.SelectedItem = null;
				await Navigation.PopModalAsync(); 

			};


		}
		protected CircleSelectViewModel ViewModel
		{
			get { return this.BindingContext as CircleSelectViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

