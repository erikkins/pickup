﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using System.Linq;
using System.Diagnostics;

namespace PickUpApp
{
	public partial class AddEditActivity : ContentPage
	{
		ExtendedTableView tv = new ExtendedTableView ();
		TableSection ts = new TableSection ();

		public AddEditActivity (Schedule CurrentActivity)
		{
			InitializeComponent ();
			//TODO: fix this
			this.ViewModel = new ActivityAddEditViewModel(App.client, CurrentActivity); 

			NavigationPage.SetBackButtonTitle (this, "");

			this.ToolbarItems.Add (new ToolbarItem ("Done", null, async() => {
				//pop the calendar window
				await this.ViewModel.ExecuteAddEditCommand();

				//await Navigation.PopAsync();
			}));
				
			stacker.Spacing = 0;
			tv.HasUnevenRows = true;
			stacker.Children.Add (tv);

			this.BackgroundColor = Color.FromRgb (73, 55, 109);

			loadSelf (CurrentActivity);

			MessagingCenter.Subscribe<Schedule> (this, "UpdatePlease", async(s) => {
				//this should be a soft save (though we do need UI updates)

				//Debug.WriteLine("AddEditActivity -- UpdatePlease Fired");
				ViewModel.ReturnVerb = "RefreshComplete"; //trick it so that we don't update the parent view just yet...
				await this.ViewModel.ExecuteAddEditCommand();
			});

			MessagingCenter.Subscribe<Schedule>(this, "ScheduleAdded", (t) => {
//				Device.BeginInvokeOnMainThread( ()=>{
//					//await System.Threading.Tasks.Task.Delay(25);
//					Navigation.PopAsync();
//				});
				//Debug.WriteLine("AddEditActivity -- ScheduleAdded Fired");
				//now tell the parent controller to reload its listview
				MessagingCenter.Send<Schedule>(t, "RefreshSched");
			});

			MessagingCenter.Subscribe<Schedule> (this, "RefreshComplete", async(s) => {
				//parent controller has completed its update...
				tv.Root.Clear();
				ViewModel.CurrentSchedule = s;
				//dang...we need to keep track if we actually popped anything
				//if (Navigation != null && Navigation.NavigationStack != null && Navigation.NavigationStack.Count >= 2) //was ==3
				//{
				//	Debug.WriteLine("Popping with " + Navigation.NavigationStack.Count.ToString() + " views");
				try{
					await Navigation.PopAsync();
				}
				catch(Exception ex)
				{
					//hm, why the crash vern?
					DisplayAlert("hm", "Why the crash, Vern?", "oops");
				}
				//}
				loadSelf(s);

			});

		}

		private void loadSelf(Schedule CurrentActivity)
		{

			ts = new TableSection ();
			ts.Add (new ActivityCell ());
			PlacePickerCell dest = new PlacePickerCell (PlaceType.ActivityPlace);
			ts.Add (dest);
			dest.Tapped += async delegate(object sender, EventArgs e) {
				
				System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in this.ViewModel.AccountPlaces where aps.id == this.ViewModel.CurrentSchedule.AccountPlaceID select aps;
				if (ap.Count() == 0)
				{
					await Navigation.PushAsync(new PlaceSelector(CurrentActivity, this.ViewModel.KidSchedules, this.ViewModel.Kids, null, PlaceType.ActivityPlace), true);
				}
				else{
				await Navigation.PushAsync(new PlaceSelector(CurrentActivity, this.ViewModel.KidSchedules, this.ViewModel.Kids, ap.FirstOrDefault(), PlaceType.ActivityPlace), true);
				}
			};
				

			KidCell kc = new KidCell ();
			ts.Add (kc);
			kc.Tapped += async delegate(object sender, EventArgs e) {
				await Navigation.PushAsync(new KidSelector(CurrentActivity, this.ViewModel.KidSchedules, this.ViewModel.Kids), true);
			};


			PlacePickerCell origin = new PlacePickerCell (PlaceType.StartingPlace);
			ts.Add (origin);
			origin.Tapped += async delegate(object sender, EventArgs e) {
//				AccountPlace ap = new AccountPlace();
//				//since we know this is the origin cell, we need to set the accountplace accordingly
//				ap.Address = this.ViewModel.CurrentSchedule.StartPlaceAddress;
//				ap.id = this.ViewModel.CurrentSchedule.StartPlaceID;
//				ap.PlaceName = this.ViewModel.CurrentSchedule.StartPlaceName;

				System.Collections.Generic.IEnumerable<AccountPlace> ap = from aps in this.ViewModel.AccountPlaces where aps.id == this.ViewModel.CurrentSchedule.StartPlaceID select aps;
				if (ap.Count() == 0)
				{
					await Navigation.PushAsync(new PlaceSelector(CurrentActivity, this.ViewModel.KidSchedules, this.ViewModel.Kids, null, PlaceType.StartingPlace), true);
				}
				else{
					await Navigation.PushAsync(new PlaceSelector(CurrentActivity, this.ViewModel.KidSchedules, this.ViewModel.Kids, ap.FirstOrDefault(), PlaceType.StartingPlace), true);
				}
			};
		
			ts.Add (new PickupDropoffSelectorCell (false));
			ts.Add (new PickupDropoffSelectorCell (true));
			ts.Add (new WeekCell ());
			ts.Add (new DatePickerCell (true));
			ts.Add (new DatePickerCell (false));
			ts.Add (new BlackoutCell ());

			tv.Root.Add (ts);


		}

		protected ActivityAddEditViewModel ViewModel
		{
			get { return this.BindingContext as ActivityAddEditViewModel; }
			set { this.BindingContext = value; }
		}


		public class WeekCell : ViewCell
		{
			public WeekCell()
			{
				//init
			}
			protected override void OnBindingContextChanged()
			{
				base.OnBindingContextChanged ();

				dynamic c = BindingContext;
				this.Height = 125;
				Schedule s = ((ActivityAddEditViewModel)c).CurrentSchedule;

//				Grid g = new Grid ();
//				g.ColumnDefinitions = new ColumnDefinitionCollection ();
//				ColumnDefinition cd = new ColumnDefinition ();
//				cd.Width = 150;
//				g.ColumnDefinitions.Add (cd);
//				cd = new ColumnDefinition ();
//				cd.Width = GridLength.Auto;
//				g.ColumnDefinitions.Add (cd);

				StackLayout sl = new StackLayout ();
				sl.Orientation = StackOrientation.Vertical;
				sl.HorizontalOptions = LayoutOptions.Start;
				sl.VerticalOptions = LayoutOptions.Center;
				sl.BackgroundColor = Color.FromRgb (238, 236, 243);
				sl.HeightRequest = 75;
				//sl.WidthRequest = (App.ScaledWidth)- 60;
				//sl.MinimumWidthRequest = (App.ScaledWidth) - 60;


				StackLayout slSide = new StackLayout ();
				slSide.Orientation = StackOrientation.Horizontal;

				BoxView bv = new BoxView ();
				bv.WidthRequest = 10;
				slSide.Children.Add (bv);

				Label l = new Label ();
				l.Text = "Days";

				l.TextColor = Color.FromRgb (246, 99, 127);
				l.HorizontalOptions = LayoutOptions.StartAndExpand;
				l.VerticalOptions = LayoutOptions.Center;
				l.FontAttributes = FontAttributes.Bold;
				l.FontSize = 16;
				l.WidthRequest = 50;

				slSide.Children.Add (l);
				sl.Children.Add (slSide);

				StackLayout slDays = new StackLayout ();
				slDays.Orientation = StackOrientation.Horizontal;
				slDays.VerticalOptions = LayoutOptions.Start;
				slDays.HorizontalOptions = LayoutOptions.Center;
				//slDays.WidthRequest = App.ScaledWidth;
				//slDays.Spacing = 20;

				StackLayout slMonday = new StackLayout ();
				slMonday.Orientation = StackOrientation.Vertical;

				ImageButton ib = new ImageButton ();
				ib.TranslationX = 5;
				ib.ImageHeightRequest = 27;
				ib.ImageWidthRequest = 27;
				if (s.Monday) {
					ib.Source = "ui_check_filled.png";
				} else {
					ib.Source = "ui_check_empty.png";
				}
				ib.VerticalOptions = LayoutOptions.Center;
				ib.HorizontalOptions = LayoutOptions.Center;
				ib.Clicked += delegate(object sender, EventArgs e) {
					if (s.Monday)
					{
						s.Monday = false;
						ib.Source = "ui_check_empty.png";
					}
					else{
						s.Monday = true;
						ib.Source = "ui_check_filled.png";
					}
				};
				slMonday.Children.Add (ib);



				Label d = new Label ();
				d.Text = "M";
				d.FontAttributes = FontAttributes.Bold;
				d.FontSize = 18;
				d.VerticalOptions = LayoutOptions.Center;
				d.HorizontalOptions = LayoutOptions.Center;
				slMonday.Children.Add(d);
				slDays.Children.Add (slMonday);

				StackLayout slTuesday = new StackLayout ();
				slTuesday.Orientation = StackOrientation.Vertical;
				ImageButton ib2 = new ImageButton ();
				ib2.TranslationX = 5;
				ib2.ImageHeightRequest = 27;
				ib2.ImageWidthRequest = 27;
				if (s.Tuesday) {
					ib2.Source = "ui_check_filled.png";
				} else {
					ib2.Source = "ui_check_empty.png";
				}
				ib2.VerticalOptions = LayoutOptions.Center;
				ib2.HorizontalOptions = LayoutOptions.Center;
				ib2.Clicked += delegate(object sender, EventArgs e) {
					if (s.Tuesday)
					{
						s.Tuesday = false;
						ib2.Source = "ui_check_empty.png";
					}
					else{
						s.Tuesday = true;
						ib2.Source = "ui_check_filled.png";
					}
				};
				slTuesday.Children.Add (ib2);

				Label d2 = new Label ();
				d2.Text = "T";
				d2.FontAttributes = FontAttributes.Bold;
				d2.FontSize = 18;
				d2.VerticalOptions = LayoutOptions.Center;
				d2.HorizontalOptions = LayoutOptions.Center;
				slTuesday.Children.Add(d2);
				slDays.Children.Add (slTuesday);

				StackLayout slWednesday = new StackLayout ();
				slWednesday.Orientation = StackOrientation.Vertical;
				ImageButton ib3 = new ImageButton ();
				ib3.TranslationX = 5;
				ib3.ImageHeightRequest = 27;
				ib3.ImageWidthRequest = 27;
				if (s.Wednesday) {
					ib3.Source = "ui_check_filled.png";
				} else {
					ib3.Source = "ui_check_empty.png";
				}
				ib3.VerticalOptions = LayoutOptions.Center;
				ib3.HorizontalOptions = LayoutOptions.Center;
				ib3.Clicked += delegate(object sender, EventArgs e) {
					if (s.Wednesday)
					{
						s.Wednesday = false;
						ib3.Source = "ui_check_empty.png";
					}
					else{
						s.Wednesday = true;
						ib3.Source = "ui_check_filled.png";
					}
				};
				slWednesday.Children.Add (ib3);
				Label d3 = new Label ();
				d3.Text = "W";
				d3.FontAttributes = FontAttributes.Bold;
				d3.FontSize = 18;
				d3.VerticalOptions = LayoutOptions.Center;
				d3.HorizontalOptions = LayoutOptions.Center;
				slWednesday.Children.Add(d3);
				slDays.Children.Add (slWednesday);

				StackLayout slThursday = new StackLayout ();
				slThursday.Orientation = StackOrientation.Vertical;
				ImageButton ib4 = new ImageButton ();
				ib4.TranslationX = 5;
				ib4.ImageHeightRequest = 27;
				ib4.ImageWidthRequest = 27;
				if (s.Thursday) {
					ib4.Source = "ui_check_filled.png";
				} else {
					ib4.Source = "ui_check_empty.png";
				}
				ib4.VerticalOptions = LayoutOptions.Center;
				ib4.HorizontalOptions = LayoutOptions.Center;
				ib4.Clicked += delegate(object sender, EventArgs e) {
					if (s.Thursday)
					{
						s.Thursday = false;
						ib4.Source = "ui_check_empty.png";
					}
					else{
						s.Thursday = true;
						ib4.Source = "ui_check_filled.png";
					}
				};
				slThursday.Children.Add (ib4);
				Label d4 = new Label ();
				d4.Text = "T";
				d4.FontAttributes = FontAttributes.Bold;
				d4.FontSize = 18;
				d4.VerticalOptions = LayoutOptions.Center;
				d4.HorizontalOptions = LayoutOptions.Center;
				slThursday.Children.Add(d4);
				slDays.Children.Add (slThursday);

				StackLayout slFriday = new StackLayout ();
				slFriday.Orientation = StackOrientation.Vertical;
				ImageButton ib5 = new ImageButton ();
				ib5.TranslationX = 5;
				ib5.ImageHeightRequest = 27;
				ib5.ImageWidthRequest = 27;
				if (s.Friday) {
					ib5.Source = "ui_check_filled.png";
				} else {
					ib5.Source = "ui_check_empty.png";
				}
				ib5.VerticalOptions = LayoutOptions.Center;
				ib5.HorizontalOptions = LayoutOptions.Center;
				ib5.Clicked += delegate(object sender, EventArgs e) {
					if (s.Friday)
					{
						s.Friday = false;
						ib5.Source = "ui_check_empty.png";
					}
					else{
						s.Friday = true;
						ib5.Source = "ui_check_filled.png";
					}
				};
				slFriday.Children.Add (ib5);
				Label d5 = new Label ();
				d5.Text = "F";
				d5.FontAttributes = FontAttributes.Bold;
				d5.FontSize = 18;
				d5.VerticalOptions = LayoutOptions.Center;
				d5.HorizontalOptions = LayoutOptions.Center;
				slFriday.Children.Add(d5);
				slDays.Children.Add (slFriday);

				StackLayout slSaturday = new StackLayout ();
				slSaturday.Orientation = StackOrientation.Vertical;
				ImageButton ib6 = new ImageButton ();
				ib6.TranslationX = 5;
				ib6.ImageHeightRequest = 27;
				ib6.ImageWidthRequest = 27;
				if (s.Saturday) {
					ib6.Source = "ui_check_filled.png";
				} else {
					ib6.Source = "ui_check_empty.png";
				}
				ib6.VerticalOptions = LayoutOptions.Center;
				ib6.HorizontalOptions = LayoutOptions.Center;
				ib6.Clicked += delegate(object sender, EventArgs e) {
					if (s.Saturday)
					{
						s.Saturday = false;
						ib6.Source = "ui_check_empty.png";
					}
					else{
						s.Saturday = true;
						ib6.Source = "ui_check_filled.png";
					}
				};
				slSaturday.Children.Add (ib6);
				Label d6 = new Label ();
				d6.Text = "S";
				d6.FontAttributes = FontAttributes.Bold;
				d6.FontSize = 18;
				d6.VerticalOptions = LayoutOptions.Center;
				d6.HorizontalOptions = LayoutOptions.Center;
				slSaturday.Children.Add(d6);
				slDays.Children.Add (slSaturday);

				StackLayout slSunday = new StackLayout ();
				slSunday.Orientation = StackOrientation.Vertical;
				ImageButton ib7 = new ImageButton ();
				ib7.TranslationX = 5;
				ib7.ImageHeightRequest = 27;
				ib7.ImageWidthRequest = 27;
				if (s.Sunday) {
					ib7.Source = "ui_check_filled.png";
				} else {
					ib7.Source = "ui_check_empty.png";
				}
				ib7.VerticalOptions = LayoutOptions.Center;
				ib7.HorizontalOptions = LayoutOptions.Center;
				ib7.Clicked += delegate(object sender, EventArgs e) {
					if (s.Sunday)
					{
						s.Sunday = false;
						ib7.Source = "ui_check_empty.png";
					}
					else{
						s.Sunday = true;
						ib7.Source = "ui_check_filled.png";
					}
				};
				slSunday.Children.Add (ib7);
				Label d7 = new Label ();
				d7.Text = "S";
				d7.FontAttributes = FontAttributes.Bold;
				d7.FontSize = 18;
				d7.VerticalOptions = LayoutOptions.Center;
				d7.HorizontalOptions = LayoutOptions.Center;
				slSunday.Children.Add(d7);
				slDays.Children.Add (slSunday);


				//need this to span 2!
				//g.Children.Add (slDays, 0, 1);
				//Grid.SetColumnSpan (slDays, 2);

				sl.Children.Add (slDays);

				View = sl;
			}
		}

		public class PickupDropoffSelectorCell : ViewCell
		{
			bool _isPickup;
			public PickupDropoffSelectorCell(bool isPickup)
			{
				//init
				_isPickup = isPickup;
			}
			protected override void OnBindingContextChanged()
			{
				base.OnBindingContextChanged ();

				dynamic c = BindingContext;
				this.Height = 150;
				Schedule s = ((ActivityAddEditViewModel)c).CurrentSchedule;

				//ok this one needs to be 2x2
				Grid g = new Grid ();
				g.ColumnDefinitions = new ColumnDefinitionCollection ();
				ColumnDefinition cd = new ColumnDefinition ();
				cd.Width = 150;
				g.ColumnDefinitions.Add (cd);
				cd = new ColumnDefinition ();
				cd.Width = GridLength.Auto;
				g.ColumnDefinitions.Add (cd);
				g.RowDefinitions.Add (new RowDefinition { Height = new GridLength (1, GridUnitType.Star) });
				g.RowDefinitions.Add (new RowDefinition { Height = new GridLength(1,  GridUnitType.Star) });

				StackLayout sl = new StackLayout ();
				sl.Orientation = StackOrientation.Horizontal;
				sl.HorizontalOptions = LayoutOptions.Start;
				sl.VerticalOptions = LayoutOptions.Center;
				sl.BackgroundColor = Color.FromRgb (238, 236, 243);
				sl.HeightRequest = 75;
				sl.WidthRequest = App.ScaledWidth;
				sl.MinimumWidthRequest = App.ScaledWidth;

				BoxView bv = new BoxView ();
				bv.WidthRequest = 10;
				sl.Children.Add (bv);

				Label l = new Label ();
				if (_isPickup) {
					l.Text = "Pickup Time";
				} else {
					l.Text = "Dropoff Time";
				}
				l.TextColor = Color.FromRgb (246, 99, 127);
				l.HorizontalOptions = LayoutOptions.StartAndExpand;
				l.VerticalOptions = LayoutOptions.Center;
				l.FontAttributes = FontAttributes.Bold;
				l.FontSize = 16;
				//l.WidthRequest = 50;

				g.Children.Add (l, 0, 0);

				StackLayout slIconTime = new StackLayout ();
				slIconTime.Orientation = StackOrientation.Horizontal;

				Image img = new Image ();
				img.VerticalOptions = LayoutOptions.Center;
				if (_isPickup) {
					img.Source = "icn_arrow_up.png";
				} else {
					img.Source = "icn_arrow_dwn.png";
				}
				slIconTime.Children.Add (img);

				//this will need to be a button
				//Label lTime = new Label ();
				//lTime.FormattedText = new FormattedString ();
				//lTime.VerticalOptions = LayoutOptions.Center;

//				lTime.FormattedText.Spans.Add (new Span {
//					Text = parts[0],
//					FontSize = 24,
//					ForegroundColor = Color.Black,
//					FontAttributes = FontAttributes.Bold
//				});
//				lTime.FormattedText.Spans.Add (new Span {
//					Text = parts[1],
//					FontFamily = Device.OnPlatform ("HelveticaNeue-Light", "", ""),
//					FontSize = 24,
//					ForegroundColor = Color.Black,
//					FontAttributes = FontAttributes.None
//				});



				ExtendedTimePicker tp = new ExtendedTimePicker();
				tp.HasBorder = false;

				//string shortTime;
				if (_isPickup) {

					//shortTime = s.AtWhenPickup.ToLocalTime ().ToString ("t");
					//tp.Time = s.AtWhenPickup.ToLocalTime ().TimeOfDay;

					tp.SetBinding (ExtendedTimePicker.TimeProperty, "CurrentSchedule.TSPickup");

				} else {
					//shortTime = s.AtWhenDropOff.ToLocalTime ().ToString ("t");
					//tp.Time = s.AtWhenDropOff.ToLocalTime ().TimeOfDay;
					tp.SetBinding (ExtendedTimePicker.TimeProperty, "CurrentSchedule.TSDropOff");
				}
				//string[] parts = shortTime.Split (' ');


				slIconTime.Children.Add (tp);

				g.Children.Add (slIconTime, 1, 0);

				//now the second row!
				Label l2 = new Label ();
				if (_isPickup) {
					l2.Text = "Pickup Notes";
				} else {
					l2.Text = "Dropoff Notes";
				}
				l2.TextColor = Color.FromRgb (246, 99, 127);
				l2.HorizontalOptions = LayoutOptions.StartAndExpand;
				l2.VerticalOptions = LayoutOptions.Center;
				l2.FontAttributes = FontAttributes.Bold;
				l2.FontSize = 16;


				g.Children.Add (l2, 0, 1);


				bool noNote = false;

				if (_isPickup) {
					if (string.IsNullOrEmpty (s.PickupNotes)) {
						//show the add button
						noNote = true;
					} else {
						Label pnotes = new Label ();
						pnotes.SetBinding (Label.TextProperty, "CurrentSchedule.PickupNotes");
						pnotes.VerticalOptions = LayoutOptions.Center;
						pnotes.FontSize = 14;
						pnotes.LineBreakMode = LineBreakMode.WordWrap;
						pnotes.WidthRequest = (App.ScaledWidth) - 205;
						g.Children.Add (pnotes, 1, 1);

						TapGestureRecognizer labelTap = new TapGestureRecognizer ();
						labelTap.Tapped += async delegate(object sender, EventArgs e) {
							var b = (Label)sender;
							await ((ContentPage)b.ParentView.ParentView.ParentView.ParentView.ParentView).Navigation.PushAsync(new AddEditNote(_isPickup, ((ActivityAddEditViewModel)c).CurrentSchedule, ((ActivityAddEditViewModel)c).KidSchedules, ((ActivityAddEditViewModel)c).Kids));
						};
						pnotes.GestureRecognizers.Add (labelTap);

					}
				} else {
					if (string.IsNullOrEmpty (s.DropOffNotes)) {
						noNote = true;
					} else {
						Label dnotes = new Label ();
						dnotes.SetBinding (Label.TextProperty, "CurrentSchedule.DropOffNotes");
						dnotes.VerticalOptions = LayoutOptions.Center;
						dnotes.LineBreakMode = LineBreakMode.WordWrap;
						dnotes.WidthRequest = (App.ScaledWidth) - 205;
						dnotes.FontSize = 14;
						g.Children.Add (dnotes, 1, 1);
						TapGestureRecognizer tgr = new TapGestureRecognizer ();
						tgr.Tapped += async delegate(object sender, EventArgs e) {
							var b = (Label)sender;
							await ((ContentPage)b.ParentView.ParentView.ParentView.ParentView.ParentView).Navigation.PushAsync(new AddEditNote(_isPickup, ((ActivityAddEditViewModel)c).CurrentSchedule, ((ActivityAddEditViewModel)c).KidSchedules, ((ActivityAddEditViewModel)c).Kids));
						};

						dnotes.GestureRecognizers.Add (tgr);
					}
				}
				if (noNote) {
					//and now a notes thingy
					StackLayout slHoriz = new StackLayout ();
					slHoriz.Orientation = StackOrientation.Horizontal;
					slHoriz.HorizontalOptions = LayoutOptions.Start;

//					Image imgAddNote = new Image ();
//					imgAddNote.Source = "icn_new_grey.png";
//					imgAddNote.VerticalOptions = LayoutOptions.Center;
//					imgAddNote.HorizontalOptions = LayoutOptions.Start;
//					slHoriz.Children.Add (imgAddNote);

					Button bAddNote = new Button ();
					bAddNote.Image = "icn_new_grey.png";
					bAddNote.VerticalOptions = LayoutOptions.Center;
					bAddNote.HorizontalOptions = LayoutOptions.Start;
					slHoriz.Children.Add (bAddNote);


					bAddNote.Clicked += async delegate(object sender, EventArgs e) {
						var b = (Button)sender;
						await ((ContentPage)b.ParentView.ParentView.ParentView.ParentView.ParentView.ParentView).Navigation.PushAsync(new AddEditNote(_isPickup, ((ActivityAddEditViewModel)c).CurrentSchedule, ((ActivityAddEditViewModel)c).KidSchedules, ((ActivityAddEditViewModel)c).Kids));
						//await ((ContentPage)((ListView)((StackLayout)b.ParentView).ParentView).ParentView).Navigation.PushAsync(new AddEditNote(_isPickup, ((ActivityAddEditViewModel)c).CurrentSchedule, ((ActivityAddEditViewModel)c).KidSchedules, ((ActivityAddEditViewModel)c).Kids));
					};

					Label lAddNote = new Label ();
					lAddNote.Text = "Add a note";
					lAddNote.TextColor = Color.FromRgb (186, 186, 186);
					lAddNote.VerticalOptions = LayoutOptions.Center;
					lAddNote.HorizontalOptions = LayoutOptions.Start;
					slHoriz.Children.Add (lAddNote);

					g.Children.Add (slHoriz, 1, 1);
				}

				sl.Children.Add (g);

				View = sl;
			}
		}

		public class DatePickerCell : ViewCell
		{
			bool _startsOn;
			public DatePickerCell(bool startsOn)
			{
				//init
				_startsOn = startsOn;
			}
			protected override void OnBindingContextChanged()
			{
				base.OnBindingContextChanged ();

				dynamic c = BindingContext;
				this.Height = 75;

				Schedule s = ((ActivityAddEditViewModel)c).CurrentSchedule;

				Grid g = new Grid ();
				g.ColumnDefinitions = new ColumnDefinitionCollection ();
				ColumnDefinition cd = new ColumnDefinition ();
				cd.Width = 150;
				g.ColumnDefinitions.Add (cd);
				cd = new ColumnDefinition ();
				cd.Width = GridLength.Auto;
				g.ColumnDefinitions.Add (cd);

				StackLayout sl = new StackLayout ();
				sl.Orientation = StackOrientation.Horizontal;
				sl.HorizontalOptions = LayoutOptions.Start;
				sl.VerticalOptions = LayoutOptions.Center;
				sl.BackgroundColor = Color.FromRgb (238, 236, 243);
				sl.HeightRequest = 75;
				sl.WidthRequest = App.ScaledWidth;
				sl.MinimumWidthRequest = App.ScaledWidth;

				BoxView bv = new BoxView ();
				bv.WidthRequest = 10;
				sl.Children.Add (bv);

				Label l = new Label ();
				if (_startsOn) {
					l.Text = "Starts on";
				} else {
					l.Text = "Ends";
				}


				l.TextColor = Color.FromRgb (246, 99, 127);
				l.HorizontalOptions = LayoutOptions.StartAndExpand;
				l.VerticalOptions = LayoutOptions.Center;
				l.FontAttributes = FontAttributes.Bold;
				l.FontSize = 16;
				//l.WidthRequest = 50;

				g.Children.Add (l, 0, 0);

				ExtendedDatePicker dp = new ExtendedDatePicker();
				if (_startsOn) {
					//dp.Date = s.AtWhen;
					dp.SetBinding (ExtendedDatePicker.DateProperty, "CurrentSchedule.AtWhen");
				} else {
					//dp.Date = s.AtWhenEnd;
					dp.SetBinding(ExtendedDatePicker.DateProperty, "CurrentSchedule.AtWhenEnd");
				}
				dp.HasBorder = false;

				g.Children.Add (dp, 1, 0);

				sl.Children.Add (g);

				View = sl;
			}
		}

		public class ActivityCell : ViewCell
		{
			public ActivityCell()
			{
				//init
			}
			protected override void OnBindingContextChanged()
			{
				base.OnBindingContextChanged ();

				dynamic c = BindingContext;
				this.Height = 75;
				Schedule s = ((ActivityAddEditViewModel)c).CurrentSchedule;

				Grid g = new Grid ();
				g.ColumnDefinitions = new ColumnDefinitionCollection ();
				ColumnDefinition cd = new ColumnDefinition ();
				cd.Width = 150;
				g.ColumnDefinitions.Add (cd);
				cd = new ColumnDefinition ();
				cd.Width = GridLength.Auto;
				g.ColumnDefinitions.Add (cd);

				StackLayout sl = new StackLayout ();
				sl.Orientation = StackOrientation.Horizontal;
				sl.HorizontalOptions = LayoutOptions.Start;
				sl.VerticalOptions = LayoutOptions.Center;
				sl.BackgroundColor = Color.FromRgb (238, 236, 243);
				sl.HeightRequest = 75;
				sl.WidthRequest = App.ScaledWidth;
				sl.MinimumWidthRequest = App.ScaledWidth;

				BoxView bv = new BoxView ();
				bv.WidthRequest = 10;
				sl.Children.Add (bv);

				Label l = new Label ();
				l.Text = "Name";

				l.TextColor = Color.FromRgb (246, 99, 127);
				l.HorizontalOptions = LayoutOptions.StartAndExpand;
				l.VerticalOptions = LayoutOptions.Center;
				l.FontAttributes = FontAttributes.Bold;
				l.FontSize = 16;
				//l.WidthRequest = 50;

				g.Children.Add (l, 0, 0);

				XLabs.Forms.Controls.ExtendedEntry e = new XLabs.Forms.Controls.ExtendedEntry ();
				//e.Text = s.Activity;
				e.SetBinding (ExtendedEntry.TextProperty, "CurrentSchedule.Activity");
				e.HorizontalOptions = LayoutOptions.StartAndExpand;
				e.VerticalOptions = LayoutOptions.Center;
				e.BackgroundColor = Color.Transparent;
				e.Placeholder = "Add Activity Name";
				e.Font = Font.OfSize ("Helvetica-Bold", 20);
				e.HasBorder = false;

				g.Children.Add (e, 1, 0);

				sl.Children.Add (g);

				View = sl;
			}
		}

		public class PlacePickerCell : ViewCell
		{
			PlaceType _placeType;
			public PlacePickerCell(PlaceType placeType)
			{
				_placeType = placeType;
			}
			protected override void OnBindingContextChanged()
			{
				base.OnBindingContextChanged ();

				dynamic c = BindingContext;
				this.Height = 75;

				Schedule s = ((ActivityAddEditViewModel)c).CurrentSchedule;

				Grid g = new Grid ();
				g.ColumnDefinitions = new ColumnDefinitionCollection ();
				ColumnDefinition cd = new ColumnDefinition ();
				cd.Width = 150;
				g.ColumnDefinitions.Add (cd);
				cd = new ColumnDefinition ();
				cd.Width = GridLength.Auto;
				g.ColumnDefinitions.Add (cd);

				StackLayout sl = new StackLayout ();
				sl.Orientation = StackOrientation.Horizontal;
				sl.HorizontalOptions = LayoutOptions.Start;
				sl.VerticalOptions = LayoutOptions.Center;
				sl.BackgroundColor = Color.FromRgb (238, 236, 243);
				sl.HeightRequest = 75;
				sl.WidthRequest = App.ScaledWidth;
				sl.MinimumWidthRequest = App.ScaledWidth;

				BoxView bv = new BoxView ();
				bv.WidthRequest = 10;
				sl.Children.Add (bv);

				Label l = new Label ();
				if (_placeType == PlaceType.ActivityPlace) {
					l.Text = "Place";
				} else {
					l.Text = "Leaving from";
				}


				l.TextColor = Color.FromRgb (246, 99, 127);
				l.HorizontalOptions = LayoutOptions.StartAndExpand;
				l.VerticalOptions = LayoutOptions.Center;
				l.FontAttributes = FontAttributes.Bold;
				l.FontSize = 16;
				l.WidthRequest = 100;

				g.Children.Add (l, 0, 0);

				StackLayout slPlace = new StackLayout ();
				slPlace.Orientation = StackOrientation.Horizontal;
				slPlace.HorizontalOptions = LayoutOptions.StartAndExpand;

				//first is the pin
				Image pin = new Image();
				pin.Source = "icn_pin.png";
				pin.VerticalOptions = LayoutOptions.Center;
				pin.HorizontalOptions = LayoutOptions.Start;
				slPlace.Children.Add (pin);


				//now the name/address stack
				StackLayout slAddress = new StackLayout();
				slAddress.Spacing = 1;
				slAddress.VerticalOptions = LayoutOptions.Center;
				slAddress.Orientation = StackOrientation.Vertical;
				slAddress.WidthRequest = (App.ScaledWidth) - 205;

				Label nameLabel = new Label ();
				if (_placeType == PlaceType.ActivityPlace) {
					nameLabel.Text = s.Location;
				} else {
					nameLabel.Text = s.StartPlaceName;	
				}

				nameLabel.FontSize = 14;
				nameLabel.HorizontalOptions = LayoutOptions.Start;
				nameLabel.LineBreakMode = LineBreakMode.TailTruncation;
				slAddress.Children.Add (nameLabel);

				Label addressLabel = new Label ();
				if (_placeType == PlaceType.ActivityPlace) {
					addressLabel.Text = s.Address;
				} else {
					addressLabel.Text = s.StartPlaceAddress;	
				}
				addressLabel.FontSize = 14;
				addressLabel.HorizontalOptions = LayoutOptions.Start;
				addressLabel.TextColor = Color.FromRgb (157, 157, 157);
				addressLabel.LineBreakMode = LineBreakMode.TailTruncation;
				slAddress.Children.Add (addressLabel);

				slPlace.Children.Add (slAddress);


				g.Children.Add (slPlace, 1, 0);

				sl.Children.Add (g);

				View = sl;
			}
		}

		public class KidCell : ExtendedViewCell
		{
			public KidCell()
			{
				this.SeparatorPadding = new Thickness(0);
				this.ShowDisclousure = false;
			}

			double finalHeight = 75;


			protected override void OnPropertyChanged (string propertyName)
			{
				base.OnPropertyChanged (propertyName);
				if (propertyName == "FinalHeight") {
					base.Height = finalHeight;
					//this.View.HeightRequest = finalHeight;
					//this.View.MinimumHeightRequest = finalHeight;
					((ExtendedTableView)this.Parent).OnDataChanged ();
				}
			}


			protected override void OnBindingContextChanged()
			{
				base.OnBindingContextChanged ();

				//Debug.WriteLine ("KidCell -- BindingContextChanged");

				dynamic c = BindingContext;
				this.Height = 75;
				//finalHeight = this.Height;
				StackLayout sl = new StackLayout ();
				sl.Orientation = StackOrientation.Horizontal;
				sl.HorizontalOptions = LayoutOptions.Start;
				sl.VerticalOptions = LayoutOptions.Center;
				sl.BackgroundColor = Color.FromRgb (238, 236, 243);
				sl.WidthRequest = App.ScaledWidth;
				sl.MinimumWidthRequest = App.ScaledWidth;

				Grid g = new Grid ();
				g.ColumnDefinitions = new ColumnDefinitionCollection ();
				ColumnDefinition cd = new ColumnDefinition ();
				cd.Width = 150;
				g.ColumnDefinitions.Add (cd);
				cd = new ColumnDefinition ();
				cd.Width = GridLength.Auto;
				g.ColumnDefinitions.Add (cd);

				BoxView bv = new BoxView ();
				bv.WidthRequest = 10;
				sl.Children.Add (bv);

				Label l = new Label ();
				l.TranslationY = 10;
				l.Text = "Kids";
				l.VerticalOptions = LayoutOptions.StartAndExpand;
				l.TextColor = Color.FromRgb (246, 99, 127);
				l.HorizontalOptions = LayoutOptions.Start;
				l.FontAttributes = FontAttributes.Bold;
				l.FontSize = 16;
				l.WidthRequest = 50;

				//sl.Children.Add (l);

				g.Children.Add (l, 0, 0);

				//ObservableCollection<KidSchedule> kidschedule = ((ActivityAddEditViewModel)c).KidSchedules;
				ObservableCollection<Kid> kids = ((ActivityAddEditViewModel)c).Kids;
				StackLayout slKidHolder = new StackLayout ();
				slKidHolder.Orientation = StackOrientation.Horizontal;


				//START PRELOAD
				finalHeight = 75;
				if (((ActivityAddEditViewModel)c).KidSchedules.Count > 0)
				{
					slKidHolder.Children.Clear();
					StackLayout slKids = new StackLayout ();
					slKids.Orientation = StackOrientation.Vertical;
					slKids.HorizontalOptions = LayoutOptions.StartAndExpand;
					slKids.VerticalOptions = LayoutOptions.Center;

					foreach (KidSchedule ks in ((ActivityAddEditViewModel)c).KidSchedules) {
						StackLayout slKiddo = new StackLayout ();
						slKiddo.Orientation = StackOrientation.Horizontal;

						//actually have to go pull the kids out
						Kid thisKid = kids.Single(k=>k.Id == ks.KidID);
						ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
							BorderColor = Color.Black,
							BorderThickness = 1,
							Aspect = Aspect.AspectFill,
							WidthRequest = 50,
							HeightRequest = 50,
							HorizontalOptions = LayoutOptions.Start,
							VerticalOptions = LayoutOptions.Center,
							Source= thisKid.PhotoURL
						};	


						slKiddo.Children.Add (ci);
						Label kidName = new Label();
						kidName.VerticalOptions = LayoutOptions.Center;
						kidName.Text = thisKid.Fullname;
						kidName.FontSize = 14;
						//kidName.SetBinding (Label.TextProperty, "Fullname");
						slKiddo.Children.Add (kidName);
						slKids.Children.Add (slKiddo);

						//this.Height += 60;
						finalHeight += 40;
					}	
					//Debug.WriteLine("Adding " + ((ActivityAddEditViewModel)c).KidSchedules.Count.ToString() + " kids directly");
					slKidHolder.Children.Add(slKids);
				}
				else{

					//simply add the add kid button
					StackLayout slHoriz = new StackLayout();
					slHoriz.Orientation = StackOrientation.Horizontal;
					slHoriz.HorizontalOptions = LayoutOptions.Start;

					Image imgAddKid =  new Image();
					imgAddKid.Source = "icn_new_grey.png";
					imgAddKid.VerticalOptions = LayoutOptions.Center;
					imgAddKid.HorizontalOptions = LayoutOptions.Start;
					slHoriz.Children.Add (imgAddKid);

					Label lAddKid = new Label ();
					lAddKid.Text = "Add Kids";
					lAddKid.TextColor = Color.FromRgb (186, 186, 186);
					lAddKid.VerticalOptions = LayoutOptions.Center;
					lAddKid.HorizontalOptions = LayoutOptions.Start;
					slHoriz.Children.Add (lAddKid);

					if (((ActivityAddEditViewModel)c).KidSchedules.Count > 0) {
						slHoriz.TranslationX = 12;
					}
					//Debug.WriteLine("Adding Add Kids button directly");
					slKidHolder.Children.Add(slHoriz);
				}
				this.Height = finalHeight;
				g.Children.Add(slKidHolder, 1, 0);

				//END PRELOAD


				//this is so that if the kids collection changes, we can update it
				((ActivityAddEditViewModel)c).KidSchedules.CollectionChanged+=  delegate(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
					//Debug.WriteLine("CollectionChanged: " + e.Action.ToString());
					slKidHolder.Children.Clear();
					StackLayout slKids = new StackLayout ();
					slKids.Orientation = StackOrientation.Vertical;
					slKids.HorizontalOptions = LayoutOptions.StartAndExpand;
					slKids.VerticalOptions = LayoutOptions.Center;


					finalHeight = 75;
					//Debug.WriteLine("KidSchedulesCollectionChanged with " + ((ActivityAddEditViewModel)c).KidSchedules.Count.ToString() + " kidschedules");
					if (((ActivityAddEditViewModel)c).KidSchedules.Count > 0)
					{
						//Debug.WriteLine("Clearing slKidHolder");
						//slKidHolder.Children.Clear();


						foreach (KidSchedule ks in ((ActivityAddEditViewModel)c).KidSchedules) {
							StackLayout slKiddo = new StackLayout ();
							slKiddo.Orientation = StackOrientation.Horizontal;

							//actually have to go pull the kids out
							Kid thisKid = kids.Single(k=>k.Id == ks.KidID);
							ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
								BorderColor = Color.Black,
								BorderThickness = 1,
								Aspect = Aspect.AspectFill,
								WidthRequest = 50,
								HeightRequest = 50,
								HorizontalOptions = LayoutOptions.Start,
								VerticalOptions = LayoutOptions.Center,
								Source= thisKid.PhotoURL
							};	


							slKiddo.Children.Add (ci);
							Label kidName = new Label();
							kidName.VerticalOptions = LayoutOptions.Center;
							kidName.Text = thisKid.Fullname;
							kidName.FontSize = 14;
							//kidName.SetBinding (Label.TextProperty, "Fullname");
							slKiddo.Children.Add (kidName);
							slKids.Children.Add (slKiddo);

							//this.Height += 60;
							finalHeight += 40;
						}	
							
						//Debug.WriteLine("Adding " + ((ActivityAddEditViewModel)c).KidSchedules.Count.ToString() + " kids from Event");



					}
					slKidHolder.Children.Add(slKids);


					//simply add the add kid button
					StackLayout slHoriz = new StackLayout();
					slHoriz.Orientation = StackOrientation.Horizontal;
					slHoriz.HorizontalOptions = LayoutOptions.Start;

					Image imgAddKid =  new Image();
					imgAddKid.Source = "icn_new_grey.png";
					imgAddKid.VerticalOptions = LayoutOptions.Center;
					imgAddKid.HorizontalOptions = LayoutOptions.Start;
					slHoriz.Children.Add (imgAddKid);

					Label lAddKid = new Label ();
					lAddKid.Text = "Add Kids";
					lAddKid.TextColor = Color.FromRgb (186, 186, 186);
					lAddKid.VerticalOptions = LayoutOptions.Center;
					lAddKid.HorizontalOptions = LayoutOptions.Start;
					slHoriz.Children.Add (lAddKid);
					if (((ActivityAddEditViewModel)c).KidSchedules.Count > 0)
					{
						//need to slide the plus over if we have kids else it looks funny
						slHoriz.TranslationX = 12;
					}
					//Debug.WriteLine("Adding Add Kids button from Event");
					slKids.Children.Add(slHoriz);


					//g.Children.Add (slKids, 1, 0);
					//Debug.WriteLine("Adding slKidHolder to the grid");
					g.Children.Add(slKidHolder, 1, 0);
					if (finalHeight > 0)
					{
						this.OnPropertyChanged("FinalHeight");
					}

				};
					
				sl.Children.Add (g);
				

				View = sl;

			}
		}

		public class BlackoutCell : ViewCell
		{

			public BlackoutCell()
			{

			}
			protected override void OnBindingContextChanged()
			{
				base.OnBindingContextChanged ();

				dynamic c = BindingContext;
				this.Height = 155;

				Grid g = new Grid ();
				g.ColumnDefinitions = new ColumnDefinitionCollection ();
				ColumnDefinition cd = new ColumnDefinition ();
				cd.Width = 150;
				g.ColumnDefinitions.Add (cd);
				cd = new ColumnDefinition ();
				cd.Width = GridLength.Auto;
				g.ColumnDefinitions.Add (cd);

				StackLayout sl = new StackLayout ();
				sl.Orientation = StackOrientation.Horizontal;
				sl.HorizontalOptions = LayoutOptions.Start;
				sl.VerticalOptions = LayoutOptions.Center;
				sl.BackgroundColor = Color.FromRgb (238, 236, 243);
				sl.HeightRequest = 75;
				sl.WidthRequest = App.ScaledWidth;
				sl.MinimumWidthRequest = App.ScaledWidth;

				BoxView bv = new BoxView ();
				bv.WidthRequest = 10;
				sl.Children.Add (bv);

				Label l = new Label ();
				l.Text = "Blackout Dates";

				l.TextColor = Color.FromRgb (246, 99, 127);
				l.HorizontalOptions = LayoutOptions.StartAndExpand;
				l.VerticalOptions = LayoutOptions.Start;
				l.FontAttributes = FontAttributes.Bold;
				l.FontSize = 16;
				l.WidthRequest = 100;

				g.Children.Add (l, 0, 0);

				StackLayout slBOD = new StackLayout ();
				slBOD.Orientation = StackOrientation.Vertical;
				slBOD.HorizontalOptions = LayoutOptions.StartAndExpand;

				StackLayout slHoriz = new StackLayout ();
				slHoriz.Orientation = StackOrientation.Horizontal;

				bool _checked = true;

				//first
				ImageButton ib = new ImageButton ();
				ib.ImageHeightRequest = 27;
				ib.ImageWidthRequest = 27;
				if (_checked) {
					ib.Source = "ui_check_filled.png";
				} else {
					ib.Source = "ui_check_empty.png";
				}
				ib.VerticalOptions = LayoutOptions.Center;
				ib.HorizontalOptions = LayoutOptions.Center;
				ib.Clicked += delegate(object sender, EventArgs e) {
					if (_checked)
					{
						_checked = false;
						ib.Source = "ui_check_empty.png";
					}
					else{
						_checked = true;
						ib.Source = "ui_check_filled.png";
					}
				};
				slHoriz.Children.Add (ib);
				Label l2 = new Label();
				l2.Text = "Labor Day";
				l2.VerticalOptions = LayoutOptions.Center;
				slHoriz.Children.Add(l2);
				slBOD.Children.Add (slHoriz);

				//second
				slHoriz = new StackLayout();
				slHoriz.Orientation = StackOrientation.Horizontal;
				bool _checked2 = true;
				ImageButton ib2 = new ImageButton ();
				ib2.ImageHeightRequest = 27;
				ib2.ImageWidthRequest = 27;
				if (_checked2) {
					ib2.Source = "ui_check_filled.png";
				} else {
					ib2.Source = "ui_check_empty.png";
				}
				ib2.VerticalOptions = LayoutOptions.Center;
				ib2.HorizontalOptions = LayoutOptions.Center;
				ib2.Clicked += delegate(object sender, EventArgs e) {
					if (_checked2)
					{
						_checked2 = false;
						ib2.Source = "ui_check_empty.png";
					}
					else{
						_checked2 = true;
						ib2.Source = "ui_check_filled.png";
					}
				};
				slHoriz.Children.Add (ib2);
				Label l3 = new Label();
				l3.Text = "Memorial Day";
				l3.VerticalOptions = LayoutOptions.Center;
				slHoriz.Children.Add(l3);
				slBOD.Children.Add (slHoriz);


				g.Children.Add (slBOD, 1, 0);

				sl.Children.Add (g);

				View = sl;
			}
		}
	}
}
