using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class CalendarPicker : ContentPage
	{
		Grid calGrid;
		DateTime currentDay = App.CurrentToday;


		public CalendarPicker ()
		{
			InitializeComponent ();

			this.ToolbarItems.Add (new ToolbarItem ("Close", "icn_close.png", async() => {
				//pop the calendar window
				//await DisplayAlert("CAL!", "show the calendar", "Cancel");
				await Navigation.PopAsync();
			}));

			btnToday.WidthRequest = App.ScaledWidth - 20;

			btnToday.Clicked += delegate(object sender, EventArgs e) {
				currentDay = DateTime.Today;
				App.CurrentToday = DateTime.Today.ToLocalTime();
				MessagingCenter.Send<string>("calendar", "NeedsRefresh");
				Navigation.PopAsync();


				//RenderCalendar();
			};


			//figure out the correct width of each cell, given the device's width (with 40 pixels of padding)
			double cellwidth = Math.Round((App.ScaledWidth-30) / 7);

			this.BackgroundColor = Color.FromRgb (73, 55, 100);
			calGrid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.FromRgb(73,55,109),
				RowSpacing = 0,
				//ColumnSpacing = 0,
				RowDefinitions = 
				{
					new RowDefinition { Height = new GridLength(60, GridUnitType.Absolute) },
					new RowDefinition {Height = new GridLength(50, GridUnitType.Absolute)},
					new RowDefinition {Height = new GridLength(50, GridUnitType.Absolute) },
					new RowDefinition {Height = new GridLength(50, GridUnitType.Absolute) },
					new RowDefinition {Height = new GridLength(50, GridUnitType.Absolute) },
					new RowDefinition {Height = new GridLength(50, GridUnitType.Absolute) },
					new RowDefinition {Height = new GridLength(50, GridUnitType.Absolute) },
					new RowDefinition {Height = new GridLength(50, GridUnitType.Absolute) }
				},
				ColumnDefinitions = 
				{
					new ColumnDefinition { Width = new GridLength(cellwidth, GridUnitType.Absolute) },
					new ColumnDefinition { Width = new GridLength(cellwidth, GridUnitType.Absolute) },
					new ColumnDefinition { Width = new GridLength(cellwidth, GridUnitType.Absolute) },
					new ColumnDefinition { Width = new GridLength(cellwidth, GridUnitType.Absolute) },
					new ColumnDefinition { Width = new GridLength(cellwidth, GridUnitType.Absolute) },
					new ColumnDefinition { Width = new GridLength(cellwidth, GridUnitType.Absolute) },
					new ColumnDefinition { Width = new GridLength(cellwidth, GridUnitType.Absolute) },
				}
				};


			RenderCalendar ();

		}


		public void RenderCalendar()
		{
			calGrid.Children.Clear ();

			DateTime currentStartDay = new DateTime (currentDay.Year, currentDay.Month, 1);

			Button backButton = new Button ();
			backButton.Image = "icn_back.png";
			backButton.HorizontalOptions = LayoutOptions.Start;
			backButton.VerticalOptions = LayoutOptions.Center;
			backButton.Clicked += delegate(object sender, EventArgs e) {
				currentDay = currentDay.AddMonths(-1);
				RenderCalendar();
			};
			calGrid.Children.Add (backButton, 0, 0);

			Button nextButton = new Button ();
			nextButton.Image = "icn_next.png";
			nextButton.VerticalOptions = LayoutOptions.Center;
			nextButton.HorizontalOptions = LayoutOptions.End;
			nextButton.Clicked += delegate(object sender, EventArgs e) {
				currentDay = currentDay.AddMonths(1);
				RenderCalendar();
			};
			calGrid.Children.Add (nextButton, 6, 0);

			Label monthName = new Label ();
			monthName.Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName (currentDay.Month).ToUpper() + " " + currentDay.Year.ToString (); //"OCT 2015";
			monthName.FontSize = 40;
			monthName.TextColor = Color.White;
			monthName.HorizontalOptions = LayoutOptions.Center;
			monthName.VerticalOptions = LayoutOptions.Center;
			calGrid.Children.Add (monthName, 1, 6, 0, 1);
			stacker.Children.Add (calGrid);


			Label monday = new Label ();
			monday.Text = "MON";
			monday.TextColor = Color.White;
			monday.FontAttributes = FontAttributes.Bold;
			monday.VerticalOptions = LayoutOptions.Center;
			monday.HorizontalOptions = LayoutOptions.Start;
			calGrid.Children.Add (monday, 0, 1);

			Label tuesday = new Label ();
			tuesday.Text = "TUE";
			tuesday.TextColor = Color.White;
			tuesday.FontAttributes = FontAttributes.Bold;
			tuesday.VerticalOptions = LayoutOptions.Center;
			tuesday.HorizontalOptions = LayoutOptions.Start;
			calGrid.Children.Add (tuesday, 0, 2);

			Label wednesday = new Label ();
			wednesday.Text = "WED";
			wednesday.TextColor = Color.White;
			wednesday.FontAttributes = FontAttributes.Bold;
			wednesday.VerticalOptions = LayoutOptions.Center;
			wednesday.HorizontalOptions = LayoutOptions.Start;
			calGrid.Children.Add (wednesday, 0, 3);

			Label thursday = new Label ();
			thursday.Text = "THU";
			thursday.TextColor = Color.White;
			thursday.FontAttributes = FontAttributes.Bold;
			thursday.VerticalOptions = LayoutOptions.Center;
			thursday.HorizontalOptions = LayoutOptions.Start;
			calGrid.Children.Add (thursday, 0, 4);

			Label friday = new Label ();
			friday.Text = "FRI";
			friday.TextColor = Color.White;
			friday.FontAttributes = FontAttributes.Bold;
			friday.VerticalOptions = LayoutOptions.Center;
			friday.HorizontalOptions = LayoutOptions.Start;
			calGrid.Children.Add (friday, 0, 5);

			Label saturday = new Label ();
			saturday.Text = "SAT";
			saturday.TextColor = Color.White;
			saturday.FontAttributes = FontAttributes.Bold;
			saturday.VerticalOptions = LayoutOptions.Center;
			saturday.HorizontalOptions = LayoutOptions.Start;
			calGrid.Children.Add (saturday, 0, 6);

			Label sunday = new Label ();
			sunday.Text = "SUN";
			sunday.TextColor = Color.White;
			sunday.FontAttributes = FontAttributes.Bold;
			sunday.VerticalOptions = LayoutOptions.Center;
			sunday.HorizontalOptions = LayoutOptions.Start;
			calGrid.Children.Add (sunday, 0, 7);


			//ok, let's figure out a looper to stick the days into the calendar


			int week = 1;
			bool selected = false;

			Image pinkCircle = new Image ();
			pinkCircle.Source = "ui_cal_circle.png";
			pinkCircle.HorizontalOptions = LayoutOptions.Center;
			pinkCircle.VerticalOptions = LayoutOptions.Center;



			for (int d = 0; d < DateTime.DaysInMonth (currentStartDay.Year, currentStartDay.Month); d++) {
				//ok, d is the current DAY within the month

//				Label theDay = new Label();
//				int realDay = d + 1;
//				theDay.TextColor = Color.White;
//				theDay.FontAttributes = FontAttributes.Bold;
//				theDay.Text = realDay.ToString();
//				theDay.HorizontalOptions = LayoutOptions.Center;
//				theDay.VerticalOptions = LayoutOptions.Center;
//				if (currentDay.Day == d + 1) {
//					theDay.Opacity = 1.0;
//					//we also need to throw in the pink circle
//					selected = true;
//				} else {
//					theDay.Opacity = 0.3;
//				}

				//the trick is, it's a button...not a label
				Button theDay = new Button();
				int realDay = d + 1;
				theDay.TextColor = Color.White;
				theDay.FontAttributes = FontAttributes.Bold;
				theDay.Text = realDay.ToString();
				theDay.HorizontalOptions = LayoutOptions.Center;
				theDay.VerticalOptions = LayoutOptions.Center;
				if (currentDay.Day == d + 1) {
					theDay.Opacity = 1.0;
					//we also need to throw in the pink circle
					selected = true;
				} else {
					theDay.Opacity = 0.3;
				}
				theDay.Clicked += delegate(object sender, EventArgs e) {
					//create a new day
					DateTime nextDate = new DateTime(currentDay.Year, currentDay.Month, int.Parse(theDay.Text));
					currentDay = nextDate;

					App.CurrentToday = nextDate;
					MessagingCenter.Send<string>("calendar", "NeedsRefresh");
					Navigation.PopAsync();
					//RenderCalendar();
				};


				if (Convert.ToDateTime(currentStartDay.AddDays(d)).ToString("dddd") == "Monday")
				{
					//add this to the Monday row
					calGrid.Children.Add(theDay,week, 1);
					if (selected) {
						calGrid.Children.Add (pinkCircle, week, 1);
					}
				}
				if (Convert.ToDateTime(currentStartDay.AddDays(d)).ToString("dddd") == "Tuesday")
				{
					//add this to the Monday row
					calGrid.Children.Add(theDay,week, 2);
					if (selected) {
						calGrid.Children.Add (pinkCircle, week, 2);
					}
				}
				if (Convert.ToDateTime(currentStartDay.AddDays(d)).ToString("dddd") == "Wednesday")
				{
					//add this to the Monday row
					calGrid.Children.Add(theDay,week, 3);
					if (selected) {
						calGrid.Children.Add (pinkCircle, week, 3);
					}
				}
				if (Convert.ToDateTime(currentStartDay.AddDays(d)).ToString("dddd") == "Thursday")
				{
					//add this to the Monday row
					calGrid.Children.Add(theDay,week, 4);
					if (selected) {
						calGrid.Children.Add (pinkCircle, week, 4);
					}
				}
				if (Convert.ToDateTime(currentStartDay.AddDays(d)).ToString("dddd") == "Friday")
				{
					//add this to the Monday row
					calGrid.Children.Add(theDay,week, 5);
					if (selected) {
						calGrid.Children.Add (pinkCircle, week, 5);
					}
				}
				if (Convert.ToDateTime(currentStartDay.AddDays(d)).ToString("dddd") == "Saturday")
				{
					//add this to the Monday row
					calGrid.Children.Add(theDay,week, 6);
					if (selected) {
						calGrid.Children.Add (pinkCircle, week, 6);
					}
				}
				if (Convert.ToDateTime(currentStartDay.AddDays(d)).ToString("dddd") == "Sunday")
				{
					//add this to the Monday row
					calGrid.Children.Add(theDay,week, 7);
					if (selected) {
						calGrid.Children.Add (pinkCircle, week, 7);
					}
					week++;
				}
				selected = false;
			}


		}
	}
}

