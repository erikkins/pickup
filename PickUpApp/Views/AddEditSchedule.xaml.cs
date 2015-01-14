using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace PickUpApp
{	
	public partial class AddEditSchedule : ContentPage
	{	
		public AddEditSchedule (Schedule selectedSchedule)
		{
			InitializeComponent ();
			chkRecurring.Toggled+= HandleToggled;
			this.ViewModel = new ScheduleAddEditViewModel (App.client, selectedSchedule, null, ViewModel.Kids);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			btnSave.Clicked += async (object sender, EventArgs e) => { await ViewModel.ExecuteAddEditCommand(); };
			btnCancel.Clicked += HandleCancel;

			MessagingCenter.Subscribe<Schedule> (this, "Updated", (s) => {
				//ViewModel.ExecuteLoadItemsCommand();
				ViewModel.Refresh();
				Navigation.PopModalAsync();
			});

			lblAddress.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(() => { 
						Navigation.PushModalAsync(new LocationSearch(ViewModel.CurrentSchedule));
					}),
					NumberOfTapsRequired = 1
				});
		}

		void HandleClicked (object sender, EventArgs e)
		{
			 ViewModel.ExecuteAddEditCommand ().ConfigureAwait (false);
		}
		void HandleCancel (object sender, EventArgs e)
		{
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

		protected ScheduleAddEditViewModel ViewModel
		{
			get { return this.BindingContext as ScheduleAddEditViewModel; }
			set { this.BindingContext = value; }
		}

	}
}

