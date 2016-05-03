using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace PickUpApp
{
	public partial class FetchRequest1 : ContentPage
	{
		private Today _currentToday;

		public FetchRequest1 (Today today)
		{
			InitializeComponent ();
			_currentToday = today;
			ViewModel = new MyCircleViewModel (App.client);

			ListView lstCircle = new ListView ();
			lstCircle.ItemsSource = ViewModel.Circle;
			lstCircle.ItemTemplate = new DataTemplate (typeof(FetchCircleCell));
			lstCircle.HasUnevenRows = true;
			lstCircle.BackgroundColor = AppColor.AppGray;
			this.BackgroundColor = AppColor.AppGray;
			lstCircle.SeparatorColor = Color.Black;
			lstCircle.SeparatorVisibility = SeparatorVisibility.Default;

			lstCircle.VerticalOptions = LayoutOptions.StartAndExpand;
			stacker.Children.Add (lstCircle);


			this.ToolbarItems.Add (new ToolbarItem ("Next", null, () => {
				//pop the calendar window
				//await DisplayAlert("CAL!", "show the calendar", "Cancel");

				//need to put the selected list of AccountCircles!
				List<AccountCircle> selectedRecipients = new List<AccountCircle>();
				foreach (AccountCircle ac in ViewModel.Circle)
				{
					if (ac.Selected)
					{
						selectedRecipients.Add(ac);
					}
				}
				if (selectedRecipients.Count == 0)
				{
					DisplayAlert("Oops", "You must select atleast one person!", "OK");
					return;
				}

				 Navigation.PushAsync(new FetchRequest2(_currentToday, selectedRecipients));
			}));
		}

		protected MyCircleViewModel ViewModel
		{
			get { return this.BindingContext as MyCircleViewModel; }
			set { this.BindingContext = value; }
		}
	}

	public class FetchCircleCell : ViewCell
	{
		ImageButton ib = new ImageButton ();
		AccountCircle ac = new AccountCircle();

		protected override void OnTapped ()
		{
			base.OnTapped ();
			Ib_Clicked (ib, new EventArgs ());
//			if (ac.Selected) {
//				ib.Source = "ui_check_filled.png";
//			} else {
//				ib.Source = "ui_check_empty.png";
//			}
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 75;
			ac = (AccountCircle)c;

			StackLayout slHoriz = new StackLayout ();
			slHoriz.Orientation = StackOrientation.Horizontal;

			BoxView bv = new BoxView ();
			bv.WidthRequest = 10;

			slHoriz.Children.Add (bv);

			if (ac.PhotoURL == null) {
				ac.PhotoURL = "";
			}

			ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
				BorderColor = Color.Black,
				BorderThickness = 1,
				Aspect = Aspect.AspectFill,
				WidthRequest = 50,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
				Source= ac.PhotoURL
			};

			slHoriz.Children.Add (ci);

			slHoriz.Children.Add (bv);

			Label l = new Label ();

			l.Text = ac.Fullname;
			if (ac.Accepted) {
				//this person is definitely IN my circle!
				l.TextColor = Color.Black;
			} else {
				l.TextColor = Color.Gray;
				l.Text += "  (Pending)";
				l.FontAttributes = FontAttributes.Italic;
				//really shouldn't be able to select this
				//this.IsEnabled = false;
			}
			l.VerticalOptions = LayoutOptions.Center;
			l.HorizontalOptions = LayoutOptions.Start;

			slHoriz.Children.Add (l);

			if (ac.Accepted) {

				//ImageButton ib = new ImageButton ();
				ib.HorizontalOptions = LayoutOptions.EndAndExpand;
				ib.VerticalOptions = LayoutOptions.Center;
				ib.ImageHeightRequest = 27;
				ib.BackgroundColor = Color.Transparent;
				ib.ImageWidthRequest = 27;
				if (ac.Selected) {
					ib.Source = "ui_check_filled.png";
				} else {
					ib.Source = "ui_check_empty.png";
				}
				ib.Clicked += Ib_Clicked;

//				ib.Clicked += delegate(object sender, EventArgs e) {
//					if (ac.Selected) {
//						ac.Selected = false;
//						ib.Source = "ui_check_empty.png";
//					} else {
//						ac.Selected = true;
//						ib.Source = "ui_check_filled.png";
//					}
//				};

				slHoriz.Children.Add (ib);
			} else {
				//not enabled...do not allow to check
				this.IsEnabled = false;
			}


			View = slHoriz;

		}

		void Ib_Clicked (object sender, EventArgs e)
		{
			if (ac.Selected) {
				ac.Selected = false;

					
				ib.Source = "ui_check_empty.png";
			} else {
				ac.Selected = true;
				ib.Source = "ui_check_filled.png";
			}
		}
	}
}

	


