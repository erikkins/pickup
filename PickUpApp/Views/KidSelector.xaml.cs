﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XLabs.Forms.Controls;
using Xamarin.Forms;

namespace PickUpApp
{
	public partial class KidSelector : ContentPage
	{

		private Schedule _currentSchedule;
		private ObservableCollection<KidSchedule> _kidschedule;
		private ObservableCollection <Kid> _kids;

		public KidSelector (Schedule CurrentSchedule, ObservableCollection<KidSchedule> KidSchedule, ObservableCollection<Kid> Kids )
		{
			InitializeComponent ();

			this.ToolbarItems.Add (new ToolbarItem ("Done", null, async() => {
				//pop the calendar window
				//basically need to save the thing

				_kidschedule.Clear();
				foreach (Kid k in _kids)
				{
					if (k.Selected)
					{
						KidSchedule ks = new PickUpApp.KidSchedule();
						ks.KidID = k.Id;
						ks.ScheduleID = _currentSchedule.id;
						_kidschedule.Add(ks);
					}
				}

				await this.ViewModel.ExecuteAddEditCommand();
				await Navigation.PopAsync();

			}));

			_currentSchedule = CurrentSchedule;
			_kidschedule = KidSchedule;
			_kids = Kids;
			this.ViewModel = new ActivityAddEditViewModel (App.client, _currentSchedule, _kidschedule, _kids);
			this.BackgroundColor = Color.FromRgb (238, 236, 243);
			ListView lvKids = new ListView () {
				ItemsSource = _kids,
				ItemTemplate = new DataTemplate (typeof(KidCell)),
				IsPullToRefreshEnabled = false,
				HasUnevenRows = false,
				BackgroundColor = Color.Transparent,
				RowHeight = 75,
				Header = null
			};


			stacker.Children.Add (lvKids);

		}

		protected ActivityAddEditViewModel ViewModel
		{
			get { return this.BindingContext as ActivityAddEditViewModel; }
			set { this.BindingContext = value; }
		}
	}
	public class KidCell : ViewCell
	{

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 75;
			Kid k = (Kid)c;

			StackLayout slHoriz = new StackLayout ();
			slHoriz.Orientation = StackOrientation.Horizontal;

			BoxView bv = new BoxView ();
			bv.WidthRequest = 10;

			slHoriz.Children.Add (bv);

			ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
				BorderColor = Color.Black,
				BorderThickness = 0,
				Aspect = Aspect.AspectFill,
				WidthRequest = 50,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
				Source= k.PhotoURL
			};

			slHoriz.Children.Add (ci);

			slHoriz.Children.Add (bv);

			Label l = new Label ();
			l.Text = k.Fullname;
			l.VerticalOptions = LayoutOptions.Center;
			l.HorizontalOptions = LayoutOptions.Start;

			slHoriz.Children.Add (l);

			ImageButton ib = new ImageButton ();
			ib.HorizontalOptions = LayoutOptions.EndAndExpand;
			ib.VerticalOptions = LayoutOptions.Center;
			ib.ImageHeightRequest = 27;
			ib.ImageWidthRequest = 27;
			if (k.Selected) {
				ib.Source = "ui_check_filled.png";
			} else {
				ib.Source = "ui_check_empty.png";
			}
			ib.Clicked += delegate(object sender, EventArgs e) {
				if (k.Selected)
				{
					k.Selected = false;
					ib.Source = "ui_check_empty.png";
				}
				else{
					k.Selected = true;
					ib.Source = "ui_check_filled.png";
				}
			};

			slHoriz.Children.Add (ib);


			View = slHoriz;

		}
	}
}

