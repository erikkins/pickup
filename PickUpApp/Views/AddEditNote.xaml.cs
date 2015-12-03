using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace PickUpApp
{
	public partial class AddEditNote : ContentPage
	{
		public AddEditNote (bool IsPickup, Schedule CurrentSchedule, TrulyObservableCollection<KidSchedule> kidsched, ObservableCollection<Kid> kids )
		{
			InitializeComponent ();
			this.ViewModel = new ActivityAddEditViewModel (App.client, CurrentSchedule, kidsched, kids);

			this.BackgroundColor = Color.FromRgb (238, 236, 243);

			Label l = new Label ();
			if (IsPickup) {
				l.Text = "Pickup notes:";
			} else {
				l.Text = "Dropoff notes:";
			}
			stacker.Children.Add (l);

			ExtendedEditor note = new ExtendedEditor ();
			note.HeightRequest = 200;
			note.WidthRequest = App.ScaledWidth - 20;
			note.HorizontalOptions = LayoutOptions.Center;
		
			note.Completed += delegate(object sender, EventArgs e) {
				MessagingCenter.Send<Schedule>(CurrentSchedule, "UpdatePlease");
			};
			if (IsPickup) {
				note.SetBinding (ExtendedEditor.TextProperty, "CurrentSchedule.PickupNotes");
			} else {
				note.SetBinding (ExtendedEditor.TextProperty, "CurrentSchedule.DropOffNotes");
			}
			stacker.Children.Add (note);

			this.ToolbarItems.Add (new ToolbarItem ("Save", "", () => {
				MessagingCenter.Send<Schedule>(CurrentSchedule, "UpdatePlease");
				//Navigation.PopAsync();
			}));

		}


		protected ActivityAddEditViewModel ViewModel
		{
			get { return this.BindingContext as ActivityAddEditViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

