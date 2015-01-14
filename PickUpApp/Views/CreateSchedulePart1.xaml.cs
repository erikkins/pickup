using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PickUpApp
{	
	public partial class CreateSchedulePart1 : ContentPage
	{	
		public CreateSchedulePart1 (Schedule selectedSchedule)
		{
			InitializeComponent ();
			this.ViewModel = new ScheduleAddEditViewModel (App.client, selectedSchedule, null, ViewModel.Kids);
			//ViewModel.CurrentSchedule = selectedSchedule;
	
			//ViewModel.LoadKidSchedulesCommand.Execute (null);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			btnNext.Clicked += HandleClicked;
			btnCancel.Clicked += HandleCancel;

			MessagingCenter.Subscribe<Schedule> (this, "Updated", (s) => {
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
			/*
			MessagingCenter.Subscribe<Schedule> (this, "ScheduleAdded", (s) => {
				ViewModel.Refresh();
				Navigation.PopModalAsync(); //once enough?
			});
			*/
		}

		void HandleCancel (object sender, EventArgs e)
		{
			//just pop the modal
			Navigation.PopModalAsync ();
		}
		void HandleClicked (object sender, EventArgs e)
		{
			//need to add the selected kids to this schedule item...hmmm

			foreach (Kid k in ViewModel.Kids) {
				if (k.Selected) {
					KidSchedule ks = new KidSchedule ();
					ks.KidID = k.Id;
					ks.ScheduleID = ViewModel.CurrentSchedule.id;

					//there's GOT to be a better way
					bool needsAdd = true;
					foreach (KidSchedule kse in ViewModel.KidSchedules) {
						if (kse.KidID == k.Id) {
							needsAdd = false;
							continue;
						}
					}
					if (needsAdd) {
						ViewModel.KidSchedules.Add (ks);
					}
				} else {
					//remove 'em
					foreach (KidSchedule kse in ViewModel.KidSchedules) {
						if (kse.KidID == k.Id) {
							ViewModel.KidSchedules.Remove (kse);
							break;
						}
					}
				}
			}

			//ok, we're finished with part 1, send them to part 2
			Navigation.PushModalAsync (new CreateSchedulePart2 (ViewModel.CurrentSchedule, ViewModel.KidSchedules));
		}

		protected ScheduleAddEditViewModel ViewModel
		{
			get { return this.BindingContext as ScheduleAddEditViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

