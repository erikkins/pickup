using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace PickUpApp
{	
	public partial class Schedule2 : ContentPage
	{	
		public Schedule2 (Schedule selectedSchedule, ObservableCollection<Kid> kids, ObservableCollection<KidSchedule> kidSchedule)
		{
			InitializeComponent ();
			chkRecurring.Toggled+= HandleToggled;
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			this.ViewModel = new Schedule2ViewModel (App.client, selectedSchedule, kids, kidSchedule);

			btnSave.Clicked += async (object sender, EventArgs e) => 
			{ 
				await ViewModel.ExecuteAddEditCommand(); 
			};
			btnBack.Clicked += HandleBack;
			btnCancel.Clicked += HandleCancel;

		}
		void OnAtWhenChanged(object sender, DateChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (e.NewDate.ToString ());
		}

		void HandleCancel (object sender, EventArgs e)
		{
			//need to pop back 2
			Navigation.PopModalAsync ();
			Navigation.PopModalAsync ();
		}

		void HandleBack(object sender, EventArgs e)
		{
			//pop back 1
			Navigation.PopModalAsync ();
		}

		void HandleToggled (object sender, ToggledEventArgs e)
		{
			if (e.Value) {
				//tblDays.IsVisible = true;
				dateLayout.IsVisible = false;
				recurringLayout.IsVisible = true;;

			} else {
				//tblDays.IsVisible = false;
				recurringLayout.IsVisible = false;
				dateLayout.IsVisible = true;
			}
		}
		protected Schedule2ViewModel ViewModel
		{
			get { return this.BindingContext as Schedule2ViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

