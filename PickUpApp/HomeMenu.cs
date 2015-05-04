using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PickUpApp
{
	public class HomeMenu : ContentPage
	{
		public HomeMenu ()
		{
			this.Title = "Menu";
			this.Icon = Device.OS == TargetPlatform.iOS ? "logout.png" : null;
			this.Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 5);

			//this.Icon = "appbarlayoutexpandrightvariant.png";
			Label header = new Label
			{
				Text = "MENU",
				FontSize = 20,
				HorizontalOptions = LayoutOptions.Center
			};
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

			listView.ItemSelected += (sender, args) =>
			{
				// Set the BindingContext of the detail page.

				//((MasterDetailPage)this.Parent).Detail.BindingContext = args.SelectedItem;
				//Console.WriteLine("The args.SelectedItem is {0}",args.SelectedItem);


					// This is where you would put your “go to one of the selected pages” 

				switch (((HomeMenuItem)args.SelectedItem).MenuName)
				{
				case "Today":
					//Page displayPage = (Page)Activator.CreateInstance (typeof(TodayView));
					//((MasterDetailPage)this.Parent).Detail = new NavigationPage(displayPage);
					((MasterDetailPage)this.Parent).Detail = new NavigationPage(new TodayView()){ BarTextColor = Device.OnPlatform(Color.Black,Color.White,Color.Black) };
					//this is a special page because it's the first hosted page and therefore receives
					//its command to load from external sources
					TodayView tv = (TodayView)((NavigationPage)((MasterDetailPage)this.Parent).Detail).CurrentPage;
					tv.ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
					break;
				case "Manage Schedule":
					((MasterDetailPage)this.Parent).Detail = new NavigationPage(new MySchedule()){ BarTextColor = Device.OnPlatform(Color.Black,Color.White,Color.Black) };
					break;
				case "My Kids":
					((MasterDetailPage)this.Parent).Detail = new NavigationPage(new MyKids()){ BarTextColor = Device.OnPlatform(Color.Black,Color.White,Color.Black) };
					break;
				case "My Circle":
					((MasterDetailPage)this.Parent).Detail = new NavigationPage(new MyCircle()){ BarTextColor = Device.OnPlatform(Color.Black,Color.White,Color.Black) };
					break;
				case "My Info":
					((MasterDetailPage)this.Parent).Detail = new NavigationPage(new MyInfo()){ BarTextColor = Device.OnPlatform(Color.Black,Color.White,Color.Black) };
					break;
				}

					// Show the detail page.
					((MasterDetailPage)this.Parent).IsPresented = false;
					};


			Content = new StackLayout { 
				Children = {
					header, listView
				}
			};
		}
	}

	public class MenuListView : ListView
	{
		public MenuListView ()
		{
			List<HomeMenuItem> data = new MenuListData ();

			ItemsSource = data;
			VerticalOptions = LayoutOptions.FillAndExpand;
			BackgroundColor = Color.Transparent;

			RowHeight = 70;

			var cell = new DataTemplate (typeof(ImageCell));
			cell.SetBinding (TextCell.TextProperty, "MenuName");
			cell.SetValue (TextCell.TextColorProperty, Device.OnPlatform (Color.Black, Color.FromRgb (211, 211, 211), Color.Black));
			cell.SetBinding (ImageCell.ImageSourceProperty, "MenuIconURL");

			ItemTemplate = cell;
		}
	}

	public class MenuListData : List<HomeMenuItem>
	{
		public MenuListData ()
		{
			this.Add (new HomeMenuItem ("Today", "sun.png"));
			this.Add (new HomeMenuItem ("Manage Schedule", "calendar.png"));
			this.Add (new HomeMenuItem ("My Kids", "children.png"));
			this.Add (new HomeMenuItem ("My Circle", "groups.png"));
			this.Add (new HomeMenuItem ("My Info", "contacts.png"));
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


