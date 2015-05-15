using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Calendars.Plugin;
using Calendars.Plugin.Abstractions;
using System.Collections;
using XLabs.Forms.Controls;
using System.Collections.ObjectModel;

namespace PickUpApp
{
	public partial class CalendarTest : ContentPage
	{
		Calendar _currentCalendar;
		Calendar currentCalendar
		{
			get{
				return _currentCalendar;
			}
			set {
				_currentCalendar = value;
			}
		}
		ObservableCollection<Calendar> _mycals;
		ObservableCollection<Calendar> myCalendars
		{
			get{	
				return _mycals;
			}
			set{
				_mycals = value;
			}
		}

		ObservableCollection<PUCalendarEvent>_todaysEvents;
		ObservableCollection<PUCalendarEvent> todaysEvents 
		{
			get{
				return _todaysEvents;
			}
			set{
				_todaysEvents = value;
			}
		}
		CalendarView calview;

		public CalendarTest ()
		{
			InitializeComponent ();

			calview = new CalendarView ();
		
			calview.ShowNavigationArrows = true;
			calview.HorizontalOptions = LayoutOptions.CenterAndExpand;
			calview.VerticalOptions = LayoutOptions.Start;
			calview.SelectedDate = DateTime.Today;
			calview.SelectedDateBackgroundColor = Color.Black;
			calview.SelectedDateForegroundColor = Color.White;
			calview.TodayDateBackgroundColor = Color.Red;
			calview.TodayDateForegroundColor = Color.White;

			myCalendars = new ObservableCollection<Calendar> ();
			todaysEvents = new ObservableCollection<PUCalendarEvent> ();

			IList<Calendar> cals = CrossCalendars.Current.GetCalendarsAsync ().Result;
			bool isFirst = true;
			foreach (Calendar c in cals) {
				myCalendars.Add (c);
				if (isFirst) {
					currentCalendar = c;
					setCurrentEvents ();
					isFirst = false;
				}
			}
			lstEvents.ItemsSource = todaysEvents;
			lstEvents.ItemSelected += async delegate(object sender, SelectedItemChangedEventArgs e) {
				//ok they've selected an event in their existing calendar that they wish to turn into a
				//Pickup schedule item
				//I guess we queue up the Schedule1 workflow and preset 
				Schedule s = new Schedule();
				PUCalendarEvent puce = (PUCalendarEvent)e.SelectedItem;
				s.Activity = puce.Name;
				s.StartTime = TimeSpan.FromTicks(puce.Start.Ticks);
				s.EndTime = TimeSpan.FromTicks(puce.End.Ticks);
				s.AtWhen = puce.Start;
				s.UserId = App.myAccount.UserId;

				await Navigation.PushModalAsync(new Schedule1(s));

			};

			string[] buttons = new string[cals.Count];
			int i = 0;
			foreach(Calendar cal in cals)
			{
				buttons[i] = cal.Name;
				i++;
			}

			btnCalendar.Clicked += async delegate(object sender, EventArgs e) {
				var action = await DisplayActionSheet("Choose Calendar", "Cancel", null, buttons);

				//issue is if more than one calendar has the same name
				foreach (Calendar cal in cals) {
					if (cal.Name == action)
					{
						currentCalendar = cal;
						setCurrentEvents();
					}
				}
			};
		
			stacker.Children.Insert (0, calview);
			calview.DateSelected +=  (object sender, DateTime e) => {
				setCurrentEvents();
			};

		}

		void setCurrentEvents()
		{
			todaysEvents.Clear();
			IList<CalendarEvent> events = CrossCalendars.Current.GetEventsAsync (currentCalendar, calview.SelectedDate.Value, calview.SelectedDate.Value.AddDays(1)).Result;
			foreach (CalendarEvent ce in events)
			{
				PUCalendarEvent puce = new PUCalendarEvent ();
				puce.AllDay = ce.AllDay;
				puce.Description = ce.Description;
				puce.End = ce.End;
				puce.ExternalID = ce.ExternalID;
				puce.Name = ce.Name;
				puce.Start = ce.Start;

				todaysEvents.Add(puce);
			}
		}
	}
}

