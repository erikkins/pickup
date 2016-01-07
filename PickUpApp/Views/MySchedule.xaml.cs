using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PickUpApp
{	
	public partial class MySchedule : ContentPage
	{	
		AddEditActivity editor;

		public MySchedule ()
		{
			InitializeComponent ();
			this.ViewModel = new ScheduleViewModel (App.client);
			NavigationPage.SetBackButtonTitle (this, "");
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			this.ToolbarItems.Add (new ToolbarItem ("Add Activity", "icn_new.png", () => {

				//let's make sure that no kids are selected at this point (nasty cache)
				foreach (Kid k in App.myKids)
				{
					k.Selected = false;
				}

				if (editor != null)
				{
				MessagingCenter.Unsubscribe<Schedule> (editor, "UpdatePlease");
				MessagingCenter.Unsubscribe<Schedule> (editor, "ScheduleAdded");
				MessagingCenter.Unsubscribe<Schedule> (editor, "DetailUpdate");
				MessagingCenter.Unsubscribe<Schedule> (editor, "RefreshComplete");
				}
				editor = new AddEditActivity(new Schedule());

				Navigation.PushAsync(editor);

//				Schedule s = new Schedule ();
//				s.AtWhen = Util.RoundUp (DateTime.Now, TimeSpan.FromMinutes (30));
//				s.StartTimeTicks = s.AtWhen.TimeOfDay.Ticks;
//				s.AtWhenEnd = Util.RoundUp (DateTime.Now, TimeSpan.FromMinutes (30)).AddHours (1);
//				s.EndTimeTicks = s.AtWhenEnd.TimeOfDay.Ticks;
//				s.Frequency = "";
//				Navigation.PushModalAsync (new Schedule1 (s));
			}));

			//lstSched.ItemSelected += HandleItemSelected;
			lstSched.ItemSelected += async delegate(object sender, SelectedItemChangedEventArgs e) {
				if (e.SelectedItem == null) return;
				if (editor != null)
				{
				MessagingCenter.Unsubscribe<Schedule> (editor, "UpdatePlease");
				MessagingCenter.Unsubscribe<Schedule> (editor, "ScheduleAdded");
				MessagingCenter.Unsubscribe<Schedule> (editor, "DetailUpdate");
				MessagingCenter.Unsubscribe<Schedule> (editor, "RefreshComplete");
				}
				editor = new AddEditActivity(e.SelectedItem as Schedule);

				await Navigation.PushAsync(editor);
				lstSched.SelectedItem = null;
			};
				
			MessagingCenter.Subscribe<EmptyClass> (this, "ActivityDeleted", (p) => {
				if (string.IsNullOrEmpty(p.Status))
				{
					//Navigation.PopAsync();
					ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
				}
				else{
					DisplayAlert("Could not delete", p.Status, "OK");
				}
			});
		}

		void HandleClicked (object sender, EventArgs e)
		{
			//create a new default schedule
			Schedule s = new Schedule ();
			s.AtWhen = Util.RoundUp (DateTime.Now, TimeSpan.FromMinutes (30));
			s.StartTimeTicks = s.AtWhen.TimeOfDay.Ticks;
			s.AtWhenEnd = Util.RoundUp (DateTime.Now, TimeSpan.FromMinutes (30)).AddHours (1);
			s.EndTimeTicks = s.AtWhenEnd.TimeOfDay.Ticks;
			s.Frequency = "";
			Navigation.PushModalAsync (new Schedule1 (s));
		}
		void HandleItemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			//I guess we'd edit from here
			if (e.SelectedItem == null) return;
			//Navigation.PushModalAsync (new Schedule1 (e.SelectedItem as Schedule));
			Navigation.PushAsync(new AddEditActivity(e.SelectedItem as Schedule));
			lstSched.SelectedItem = null;

		}

		public void OnDelete (object sender, EventArgs e) {
			var mi = ((MenuItem)sender);
			Schedule s = (Schedule)mi.CommandParameter;
			ViewModel.CurrentSchedule = s;
			ViewModel.ExecuteDeleteCommand ().ConfigureAwait (false);
			//DisplayAlert("Delete Context Action", s.id + " delete context action", "OK");
		}

		protected ScheduleViewModel ViewModel
		{
			get { return this.BindingContext as ScheduleViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

