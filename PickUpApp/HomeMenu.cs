using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using XLabs.Forms.Controls;

namespace PickUpApp
{
	public class HomeMenu : ContentPage
	{
		public HomeMenu ()
		{
			this.Title = "MY SETTINGS";
			this.BackgroundColor = Color.FromRgb (73, 55, 109);

			//this.Icon = Device.OS == TargetPlatform.iOS ? "settings.png" : null;
		//	this.Icon = "icn_settings.png";


			this.Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 5);

			//this.Icon = "appbarlayoutexpandrightvariant.png";


			// create an array of the Page names
//			string[] myPageNames = {
//				"Today",
//				"Manage Schedule",
//				"My Kids",
//				"My Circle",
//				"My Info"
//			};
//
//			HomeMenuItem[] menuNames = {
//				new HomeMenuItem ("Today", ""),
//				new HomeMenuItem ("Manage Schedule", ""),
//				new HomeMenuItem ("My Kids", "children100.png"),
//				new HomeMenuItem ("My Circle", ""),
//				new HomeMenuItem ("My Info", "")
//			};
//
//			// Create ListView for the Master page.
//			ListView listView = new ListView
//			{
//				ItemsSource = menuNames,
//				ItemTemplate = new DataTemplate (typeof(HomeTemplateLayout)),
//			};

			ListView listView = new MenuListView();
			listView.ItemsSource = App.menuItems;

			listView.ItemSelected += (sender, args) =>
			{
				// Set the BindingContext of the detail page.

				//((MasterDetailPage)this.Parent).Detail.BindingContext = args.SelectedItem;
				//Console.WriteLine("The args.SelectedItem is {0}",args.SelectedItem);


					// This is where you would put your “go to one of the selected pages” 

				switch (((FFMenuItem)args.SelectedItem).MenuName)
				{
				case "Today":
					App.CurrentToday = DateTime.Today.ToLocalTime();
					//Page displayPage = (Page)Activator.CreateInstance (typeof(TodayView));
					//((MasterDetailPage)this.Parent).Detail = new NavigationPage(displayPage);
					((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new TodayView()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//this is a special page because it's the first hosted page and therefore receives
					//its command to load from external sources
					TodayView tv = (TodayView)((NavigationPage)((MasterDetailPage)this.Parent.Parent).Detail).CurrentPage;
					tv.ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
					//listView.SelectedItem =  null;
					break;
				case "Activities":
					((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new MySchedule()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//listView.SelectedItem =  null;
					break;
				case "Kids":
					((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new MyKids()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//listView.SelectedItem =  null;
					break;
				case "Circle":
					((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new MyCircle()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//listView.SelectedItem =  null;
					break;
				case "Account":
					((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new MyInfo()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//listView.SelectedItem =  null;
					break;
				case "Places":
					((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new MyPlaces()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//listView.SelectedItem =  null;
					break;
				case "Calendar":
					((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new CalendarTest()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//listView.SelectedItem =  null;
					break;
				case "Messages":
					((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new MessageCenter()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//listView.SelectedItem =  null;
					break;
				case "Help/Feedback":
					((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new Help()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//listView.SelectedItem =  null;
					break;
				case "Logout":

					//actually need to pop the masterdetailpage down to AppRoot, then clear Settings, then push Login
					//((MasterDetailPage)this.Parent.Parent).Navigation.PopModalAsync();
					Settings.CachedAuthToken = "";
					Settings.CachedUserName = "";
					Settings.RememberPassword = false;
					MessagingCenter.Send<string>("menu", "login");

					//((MasterDetailPage)this.Parent.Parent).Detail = new NavigationPage(new Splash()){ BarTextColor = Device.OnPlatform(Color.White,Color.White,Color.Black), BarBackgroundColor=Color.FromRgb(247,99,127) };
					//listView.SelectedItem =  null;
					break;
				case "Intro":
					((MasterDetailPage)this.Parent.Parent).Navigation.PushModalAsync(new CarouselMaster());
					break;
				case "Reg":
					((MasterDetailPage)this.Parent.Parent).Navigation.PushModalAsync(new Register());
					break;
				}

					// Show the detail page.
					((MasterDetailPage)this.Parent.Parent).IsPresented = false;
				//listView.SelectedItem = null;		
			};


			Content = new StackLayout { 
				Children = {
					listView
				}, Padding = 0
			};
		}
	}

	public class MenuListView : ListView
	{
		public MenuListView ()
		{
			//ObservableCollection<FFMenuItem> data = new MenuListData ();

			ItemsSource = App.menuItems; //data;
			VerticalOptions = LayoutOptions.FillAndExpand;
			BackgroundColor = Color.Transparent;
			HasUnevenRows = true;
			SeparatorColor = Color.Black;
			SeparatorVisibility = SeparatorVisibility.Default;
		
			//RowHeight = 141;

			var cell = new DataTemplate (typeof(FFMenuCell));
			//cell.SetBinding (TextCell.TextProperty, "MenuName");
			//cell.SetValue (TextCell.TextColorProperty, Device.OnPlatform (Color.White, Color.FromRgb (211, 211, 211), Color.Black));
			//cell.SetBinding (ImageCell.ImageSourceProperty, "MenuIconURL");

			ItemTemplate = cell;
		}

	}
		

	public class MenuListData : TrulyObservableCollection<FFMenuItem>, INotifyCollectionChanged
	{
		public MenuListData ()
		{
			//FFMenuItem kids = new FFMenuItem ("Kids", App.myKids.Count);		
			//this.Add (kids);
			//this.Add (new FFMenuItem ("Circle", App.myCircle.Count));
			//this.Add (new FFMenuItem ("Places", App.myPlaces.Count));
			this.Add (new FFMenuItem("Today", 0));
			this.Add (new FFMenuItem ("Activities", 0));
			this.Add (new FFMenuItem ("Account", 0));
			//this.Add(new FFMenuItem("Messages", 0));
			this.Add(new FFMenuItem("Help/Feedback", 0));
			this.Add (new FFMenuItem ("Logout", 0));
			this.OnCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
			this.OnPropertyChanged (new System.ComponentModel.PropertyChangedEventArgs (""));
			//this.Add(new FFMenuItem

//			this.Add (new HomeMenuItem ("Today", "sun.png"));
//			this.Add (new HomeMenuItem ("Manage Schedule", "calendar.png"));
//			this.Add (new HomeMenuItem ("My Kids", "children.png"));
//			this.Add (new HomeMenuItem ("My Circle", "groups.png"));
//			this.Add (new HomeMenuItem ("My Info", "contacts.png"));
//			this.Add (new HomeMenuItem ("Calendar", "calendar.png"));
//			this.Add (new HomeMenuItem ("Logout", "contacts.png"));


		}


	}


	public class FFMenuCell : ViewCell
	{
		public FFMenuCell()
		{
			//init
		}
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 75;

			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;
			sl.HorizontalOptions = LayoutOptions.Center;
			sl.VerticalOptions = LayoutOptions.Center;
			sl.BackgroundColor = Color.FromRgb (73, 55, 109);
			sl.HeightRequest = 141;
			sl.WidthRequest = App.ScaledWidth;
			sl.MinimumWidthRequest = App.ScaledWidth;

			StackLayout slPix = new StackLayout ();
			slPix.Orientation = StackOrientation.Vertical;
			slPix.HorizontalOptions = LayoutOptions.Center;
			slPix.VerticalOptions = LayoutOptions.Center;
			slPix.WidthRequest = 175;



			//really two things in this cell
			//the name (count)
			//and the supporting images (3 per row)

			if (c == null) {
				return;
				System.Diagnostics.Debug.WriteLine ("OOPS");
			}


			Label menuNameLabel = new Label ();
			if (((FFMenuItem)c).MenuName == "Logout") {
				menuNameLabel.TextColor = Color.FromRgb (246, 99, 127);
			} else {
				menuNameLabel.TextColor = Color.White;
			}
			menuNameLabel.FontAttributes = FontAttributes.Bold;
			menuNameLabel.FontSize = 18;
			menuNameLabel.HorizontalOptions = LayoutOptions.Start;
			menuNameLabel.VerticalOptions = LayoutOptions.Center;
			menuNameLabel.Text = ((FFMenuItem)c).MenuName;
			sl.Children.Add (menuNameLabel);


				Label countLabel = new Label ();
				countLabel.TextColor = Color.White;
				countLabel.FontSize = 18;
				countLabel.FontFamily = "HelveticaNeue-Light";
			if (((FFMenuItem)c).Count > 0) {
				countLabel.Text = "(" + ((FFMenuItem)c).Count.ToString () + ")";
			}
				countLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
				countLabel.VerticalOptions = LayoutOptions.Center;
				//countLabel.SetBinding (Label.TextProperty, "App.myKids.Count");
				sl.Children.Add (countLabel);



			//maybe we have a few special cases
			switch (((FFMenuItem)c).MenuName) {
			case "Places":
				Image imgPlaces = new Image ();
				imgPlaces.Source = "icn_places.png";
				imgPlaces.HorizontalOptions = LayoutOptions.End;
				sl.Children.Add (imgPlaces);
				break;
			case "Activities":
				Image imgActivities = new Image ();
				imgActivities.Source = "icn_activities.png";
				imgActivities.HorizontalOptions = LayoutOptions.End;
				sl.Children.Add (imgActivities);
				break;
			case "Account":
				Image imgAccount = new Image ();
				imgAccount.Source = "icn_account.png";
				imgAccount.HorizontalOptions = LayoutOptions.End;
				sl.Children.Add (imgAccount);
				break;
			case "Kids":
				Image imgKids = new Image ();
				imgKids.Source = "icn_kids.png";
				imgKids.HorizontalOptions = LayoutOptions.End;
				sl.Children.Add (imgKids);
				break;
			case "Circle":
				Image imgCircle = new Image ();
				imgCircle.Source = "icn_circle.png";
				imgCircle.HorizontalOptions = LayoutOptions.End;
				sl.Children.Add (imgCircle);
				break;
			
			default:
				//this is basically unused now
				//ok, now let's add any photos
				if (((FFMenuItem)c).Photos.Count > 0) {

					int cnt = 0;
					StackLayout slPixSub = new StackLayout ();
					slPixSub.Orientation = StackOrientation.Horizontal;
					slPixSub.HorizontalOptions = LayoutOptions.Center;
					slPixSub.VerticalOptions = LayoutOptions.End;

					foreach (string url in ((FFMenuItem)c).Photos) {
						if (cnt % 3 == 0 && slPixSub.Children.Count > 0) {
							//we've now added 3 photos...create a new horizontal stacker
							slPix.Children.Add(slPixSub);
							this.Height += 30;
							slPixSub = new StackLayout ();
							slPixSub.Orientation = StackOrientation.Horizontal;
							slPixSub.HorizontalOptions = LayoutOptions.Center;
							slPixSub.VerticalOptions = LayoutOptions.End;
						}
						//Uri auri = new Uri (url);
						ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
							BorderColor = Color.Black,
							BorderThickness = 1,
							Aspect = Aspect.AspectFill,
							WidthRequest = 42,
							HeightRequest = 42,
							HorizontalOptions = LayoutOptions.Center,
							VerticalOptions = LayoutOptions.Center,
							Source = url
						};	
						slPixSub.Children.Add (ci);
						cnt++;
					}
					if (slPixSub.Children.Count > 0) {
						slPix.Children.Add (slPixSub);
						this.Height += 30;
					}
				}
				break;
			}

			if (slPix.Children.Count > 0) {
				sl.Children.Add (slPix);
			}

			View = sl;

		}
	}

	public class FFMenuItem : BaseModel
	{
		public FFMenuItem (string MenuName, int Count)
		{
			_menuName = MenuName;
			_count = Count;
			Photos = new List<string> ();
		}


		private string _menuName;
		public string MenuName
		{
			get{ return _menuName; }
			set { _menuName = value; NotifyPropertyChanged ("MenuName");}
		}

		private int _count;
		public int Count
		{
			get { return _count; }
			set { _count = value; NotifyPropertyChanged ("Count"); }
		}

		private List<string> _photos;
		public List<string> Photos
		{
			get{ return _photos; }
			set {_photos = value; }
		}

	}

	public class HomeMenuItem
	{
		public HomeMenuItem(string MenuName, string MenuIconURL)
		{
			_menuName = MenuName;
			_menuIconURL = MenuIconURL;
		}
		private string _menuName;
		public string MenuName{
			get { return _menuName; }
			set{_menuName = value;}
		}
		private string _menuIconURL;
		public string MenuIconURL {
			get{ return _menuIconURL; }
			set{_menuIconURL = value; }
		}
	}

//	public class HomeTemplateLayout : ViewCell
//	{
//		protected override void OnBindingContextChanged()
//		{
//			base.OnBindingContextChanged();
//
//			dynamic c = BindingContext;
//			this.Height = 55;
//			StackLayout sl = new StackLayout ();
//
//			sl.Orientation = StackOrientation.Horizontal;
//			ImageCell i = new ImageCell ();
//		
//			i.SetBinding (ImageCell.ImageSourceProperty, "MenuIconURL");
//			sl.Children.Add (i);
//
//			Label lblMenu = new Label();
//			lblMenu.TextColor = Color.Gray;
//			lblMenu.FontSize = 20;
//			lblMenu.SetBinding (Label.TextProperty, "MenuName");
//			sl.Children.Add (lblMenu);
//			View = sl;
//
//
//
//			/*
//			StackLayout slmain = new StackLayout ();
//			slmain.Orientation = StackOrientation.Vertical;
//
//			StackLayout sl = new StackLayout ();
//			sl.Orientation = StackOrientation.Horizontal;
//			sl.VerticalOptions = LayoutOptions.Center;
//			Label namelabel = new Label ();
//			namelabel.HorizontalOptions = LayoutOptions.FillAndExpand;
//			namelabel.SetBinding (Label.TextProperty, "Activity");
//			//namelabel.Text = c.Activity;
//			sl.Children.Add (namelabel);
//			Label startlabel = new Label();
//			startlabel.SetBinding (Label.TextProperty, "ActualAtWhen");
//			startlabel.HorizontalOptions = LayoutOptions.End;
//			//startlabel.Text = c.StartTime;
//			sl.Children.Add (startlabel);
//
//			Label pickerUpperLabel = new Label ();
//			pickerUpperLabel.HorizontalOptions = LayoutOptions.Start;
//			pickerUpperLabel.VerticalOptions = LayoutOptions.Start;
//			pickerUpperLabel.SetBinding (Label.TextProperty, "TodayDescriptor");
//
//
//			slmain.Children.Add (sl);
//			slmain.Children.Add (pickerUpperLabel);
//			*/
//
//			//View = slmain;
//		}
//	}
}


