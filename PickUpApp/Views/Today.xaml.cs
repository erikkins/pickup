using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using PickUpApp.ViewModels;

namespace PickUpApp
{	
	public partial class TodayView : ContentPage
	{
		public TodayView ()
		{
			InitializeComponent ();

			this.ViewModel = new TodayViewModel(App.client);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			//lstAccount.ItemSelected += lstAccount_ItemSelected;


//			//start android
//
//			var listView = new ListView ();
//			listView.SetBinding<TodayViewModel> (ListView.ItemsSourceProperty, vm => vm.Todays);
//
//			var refreshView = new PullToRefreshListView {
//				RefreshCommand = viewModel.RefreshCommand,
//				Content = listView
//			};
//
//			refreshView.SetBinding<TodayViewModel> (PullToRefreshListView.IsRefreshingProperty, vm => vm.IsBusy);
//			//end android



			ListView lvToday = new ListView () {
				RefreshCommand = ViewModel.LoadItemsCommand,
				ItemTemplate = new DataTemplate (typeof(TodayTemplateLayout)),
				ItemsSource = ViewModel.Todays,
				IsPullToRefreshEnabled = true,
				HasUnevenRows = true
			};
			MessagingCenter.Subscribe<TodayViewModel>(this, "TodayLoaded", (t) => {
				lvToday.IsRefreshing = false;
			});


			MessagingCenter.Subscribe<string> (this, "NeedsRefresh", async(nr) => {
				await ViewModel.ExecuteLoadItemsCommand();

			});


			lvToday.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {

				if (e.SelectedItem == null)
				{
					return;
				}

				Today today = ((Today)e.SelectedItem);
				if (today.RowType == "schedule")
				{
					if (string.IsNullOrEmpty(today.ConfirmedBy))
					{
						Schedule s = new Schedule();
						s.id = today.id;
						Navigation.PushModalAsync(new CircleSelect(s));
					}
					else{
						
						Navigation.PushModalAsync(new InviteResponseView(today));
					}
				}
				if (today.RowType == "invite") //should be "invite"
				{
					//seems a little silly to have these supertypes that cross map to each other...
					InviteInfo i = new InviteInfo();
					i.Activity = today.Activity;
					i.Address = today.Address;
					i.EndTimeTicks = today.EndTimeTicks;
					i.Id = today.id;
					i.Kids = today.Kids;
					i.Latitude = double.Parse(today.Latitude);
					i.Location = today.Location;
					i.Longitude = double.Parse(today.Longitude);
					i.Message = today.Message;
					i.PickupDate = today.PickupDate;
					i.Requestor = today.Requestor;
					i.RequestorPhone = today.RequestorPhone;
					//i.Solved missing
					//i.SolvedBy missing
					i.StartTimeTicks = today.StartTimeTicks;
					i.Complete = false;
					i.LocationMessage = today.LocationMessage;
					i.AccountID = today.AccountID;

					Navigation.PushModalAsync(new InviteHUD(i));
				}
				lvToday.SelectedItem = null;
			};
			stacker.Children.Add (lvToday);


//			var refreshList = new PullToRefreshListView {
//				RefreshCommand = ViewModel.LoadItemsCommand, 
//				Message = "Loading...",
//				ItemTemplate = new DataTemplate(typeof(TodayTemplateLayout))
//			};
//
//			refreshList.ItemSelected +=	 (object sender, SelectedItemChangedEventArgs e) => {
//
//				//need to differentiate between a schedule item and an accepted invite item...how?
//				Today today = ((Today)e.SelectedItem);
//				if (today.RowType == "schedule")
//				{
//					Schedule s = new Schedule();
//					s.id = today.id;
//					Navigation.PushModalAsync(new CircleSelect(s));
//				}
//				if (today.RowType == "invite") 
//				{
//					//seems a little silly to have these supertypes that cross map to each other...
//					InviteInfo i = new InviteInfo();
//					i.Activity = today.Activity;
//					i.Address = today.Address;
//					i.EndTimeTicks = today.EndTimeTicks;
//					i.Id = today.id;
//					i.Kids = today.Kids;
//					i.Latitude = double.Parse(today.Latitude);
//					i.Location = today.Location;
//					i.Longitude = double.Parse(today.Longitude);
//					i.Message = today.Message;
//					i.PickupDate = today.PickupDate;
//					i.Requestor = today.Requestor;
//					//i.Solved missing
//					//i.SolvedBy missing
//					i.StartTimeTicks = today.StartTimeTicks;
//
//					Navigation.PushModalAsync(new InviteHUD(i));
//				}
//			};
//			refreshList.SetBinding<TodayViewModel> (PullToRefreshListView.IsRefreshingProperty, vm => vm.IsLoading);
//			refreshList.SetBinding<TodayViewModel> (PullToRefreshListView.ItemsSourceProperty, vm => vm.Todays);
//			stacker.Children.Add (refreshList);

		}

		public  TodayViewModel ViewModel
		{
			get { return this.BindingContext as TodayViewModel; }
			set { this.BindingContext = value; }
		}

		void lstAccount_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			//Navigation.PushAsync(new RebatesView(e.SelectedItem as Store));
			//lstAccount.SelectedItem = null;
		}




			
//		public List<Account> getAccount()
//		{
//			List<Account> theList = new List<Account> ();
//			theList.Add (PickupService.DefaultService.GetAccount ().Result);
//			return theList;
//
//		}

	}

	public class TodayTemplateLayout : ViewCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			dynamic c = BindingContext;
			this.Height = 55;
			StackLayout slmain = new StackLayout ();
			slmain.Orientation = StackOrientation.Vertical;

			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;
			sl.VerticalOptions = LayoutOptions.Center;
			Label namelabel = new Label ();
			namelabel.HorizontalOptions = LayoutOptions.FillAndExpand;
			namelabel.SetBinding (Label.TextProperty, "Activity");
			//namelabel.Text = c.Activity;
			sl.Children.Add (namelabel);
			Label startlabel = new Label();
			startlabel.SetBinding (Label.TextProperty, "ActualAtWhen");
			startlabel.HorizontalOptions = LayoutOptions.End;
			//startlabel.Text = c.StartTime;
			sl.Children.Add (startlabel);

			Label pickerUpperLabel = new Label ();
			pickerUpperLabel.HorizontalOptions = LayoutOptions.Start;
			pickerUpperLabel.VerticalOptions = LayoutOptions.Start;
			pickerUpperLabel.SetBinding (Label.TextProperty, "TodayDescriptor");


			slmain.Children.Add (sl);
			slmain.Children.Add (pickerUpperLabel);

			View = slmain;
		}
	}
}


