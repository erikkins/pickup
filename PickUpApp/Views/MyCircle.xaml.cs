using System;
using System.Collections.Generic;
using Xamarin.Forms;
//using Xamarin.Contacts;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using XLabs.Forms.Controls;

namespace PickUpApp
{	
	public partial class MyCircle : ContentPage
	{	
		bool keepListening = false;

		public MyCircle ()
		{
			InitializeComponent ();
			this.ViewModel = new MyCircleViewModel (App.client);

			ExtendedListView lstCircle = new ExtendedListView ();
			lstCircle.SeparatorColor = Color.Black;
			lstCircle.ItemsSource = ViewModel.Circle;
			lstCircle.ItemTemplate = new DataTemplate (typeof(CircleCell));
			lstCircle.RefreshCommand = ViewModel.LoadItemsCommand;
			lstCircle.IsPullToRefreshEnabled = true;
			lstCircle.HasUnevenRows = true;
			lstCircle.BackgroundColor = AppColor.AppGray;
			//lstCircle.SetBinding (ListView.IsRefreshingProperty, "ViewModel.IsLoading");
			stacker.Children.Add (lstCircle);

			lstCircle.ItemSelected += async delegate(object sender, SelectedItemChangedEventArgs e) {
				if (e.SelectedItem == null) return;

				//the reality here is that this should be NON-EDITABLE (with the exception of CoParent!)

				AccountCircle ac = e.SelectedItem as AccountCircle;
				if (!ac.Accepted)
				{
					//can't do anything until it's been accepted!
					lstCircle.SelectedItem = null;
					return;
				}
				keepListening = true;
				LocalContact lc = new LocalContact();
				lc.Coparent = ac.Coparent;
				lc.DisplayName = ac.Fullname;
				lc.Email = ac.Email;
				lc.FirstName = ac.Firstname;
				lc.Id = ac.id;
				lc.LastName = ac.Lastname;
				lc.Phone = ac.Phone;
				lc.PhotoId = ac.PhotoURL;
				lc.Accepted = ac.Accepted;

				await Navigation.PushAsync(new AddEditContact(lc, false));
				lstCircle.SelectedItem = null;
			};

			this.Disappearing += delegate(object sender, EventArgs e) {
				MessagingCenter.Unsubscribe<string>(this, "circleloaded");
				MessagingCenter.Unsubscribe<AccountCircle>(this, "deleteac");
				MessagingCenter.Unsubscribe<EmptyClass>(this, "CircleChanged");

				if (!keepListening)
				{
					MessagingCenter.Unsubscribe<LocalContact>(this, "contactpicked");
					MessagingCenter.Unsubscribe<LocalContact>(this, "ContactAdded");
				}
			};

			this.Appearing += delegate(object sender, EventArgs e) {
				//NOTE: this gets called when the circle gets changed via APN, so make sure there's no popping going on here
				MessagingCenter.Unsubscribe<EmptyClass>(this, "CircleChanged");
				MessagingCenter.Subscribe<EmptyClass> (this, "CircleChanged", (p) => {
					System.Diagnostics.Debug.WriteLine("CIRCLECHANGED");
					if (string.IsNullOrEmpty(p.Status))
					{
						//Navigation.PopAsync();
						ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(true);
						//really need to reload the kids too
						//MyCircleViewModel bc = this.BindingContext as MyCircleViewModel;
						//this.BindingContext = new KidsViewModel(App.client);

						KidsViewModel kvm = new KidsViewModel(App.client);
						App.hudder.showHUD("Loading Kids");
						kvm.ExecuteLoadItemsCommand().ConfigureAwait(true);


						//((KidsViewModel)BindingContext).ExecuteLoadItemsCommand().ConfigureAwait(false);
						App.hudder.hideHUD();
						//this.BindingContext = bc;
						lstCircle.IsRefreshing = false;
					}
					else{
						DisplayAlert("Could not delete", "This user is in use in the following activities: " + p.Status, "OK");
					}
				});

				MessagingCenter.Unsubscribe<string>(this, "circleloaded");
				MessagingCenter.Subscribe<string> (this, "circleloaded", (s) => {
					System.Diagnostics.Debug.WriteLine("CIRCLELOADED");
					lstCircle.IsRefreshing = false;
				});

				MessagingCenter.Unsubscribe<AccountCircle>(this, "deleteac");
				MessagingCenter.Subscribe<AccountCircle>(this, "deleteac", async(ac) => {
					System.Diagnostics.Debug.WriteLine("DELETEAC");
					ViewModel.CurrentAccountCircle = ac;
					await ViewModel.ExecuteDeleteCommand();
				});

				MessagingCenter.Unsubscribe<LocalContact>(this, "contactpicked");
				MessagingCenter.Subscribe<LocalContact> (this, "contactpicked", async(lc) => {
					System.Diagnostics.Debug.WriteLine("CONTACTPICKED");
					MessagingCenter.Unsubscribe<LocalContact>(this, "ContactAdded");
					keepListening = true; //because they're popping to addeditcontact which still needs the listener
					await Navigation.PopAsync(false);
					await Navigation.PushAsync(new AddEditContact(lc, true),false);
				});

				MessagingCenter.Unsubscribe<LocalContact>(this, "ContactAdded");
				MessagingCenter.Subscribe<LocalContact> (this, "ContactAdded", (s) => {
					System.Diagnostics.Debug.WriteLine("CONTACTADDED");
					MessagingCenter.Unsubscribe<LocalContact>(this, "ContactAdded");
					MessagingCenter.Unsubscribe<LocalContact>(this, "contactpicked");
					keepListening = false;
					//gotta do some housekeeping here			
					Navigation.PopAsync ();
					ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
					//ViewModel.Refresh();
				});
			};


			this.ToolbarItems.Add (new ToolbarItem ("Add Contact", "icn_new.png", async() => {
				keepListening = true;
				//we really want to show a actionsheet allowing them to manually add the contact or select them from their existing contact list
				var action = await DisplayActionSheet ("Contact Source", "Cancel", null, "Manually Enter", "From Contacts List");
				if (action == "From Contacts List")
				{
					//really this should push AddEditContact, then push the selectcontact screen modally
					await Navigation.PushAsync(new SelectContact());
				}
				else
					if (action == "Manually Enter")
					{
						LocalContact lc = new LocalContact();
						await Navigation.PushAsync(new AddEditContact(lc, true));
					}	
				else{
						return;
				}

			}));

			this.BackgroundColor = Color.FromRgb (238, 236, 243);
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);	

		}


		public void OnDelete (object sender, EventArgs e) {
			var mi = ((MenuItem)sender);
			Account a = (Account)mi.CommandParameter;



			DisplayAlert("Delete Context Action", a.id + " delete context action", "OK");
		}

		protected MyCircleViewModel ViewModel
		{
			get { return this.BindingContext as MyCircleViewModel; }
			set { this.BindingContext = value; }
		}
	}


	public class CircleCellNoDelete: ViewCell
	{
		public CircleCellNoDelete()
		{
			
		}			

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			AccountCircle ac = (AccountCircle)c;

			if (ac == null) {
				return;
			}

			this.Height = 65;

			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;
			sl.HorizontalOptions = LayoutOptions.Center;
			sl.VerticalOptions = LayoutOptions.Center;
			sl.BackgroundColor = Color.FromRgb (238, 236, 243);
			sl.HeightRequest = 65;

			BoxView bv = new BoxView ();
			bv.WidthRequest = 10;
			sl.Children.Add (bv);

			ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
				BorderColor = Color.Black,
				BorderThickness = 1,
				Aspect = Aspect.AspectFill,
				WidthRequest = 50,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Source= ac.PhotoURL
			};	

			sl.Children.Add (ci);

		
			Label l = new Label ();
			l.Text = ac.Fullname;
			l.HorizontalOptions = LayoutOptions.Start;
			l.VerticalOptions = LayoutOptions.Center;
			l.TranslationX = 5;
			l.FontAttributes = FontAttributes.Bold;

			if (ac.Accepted) {
				//this person is definitely IN my circle!
				l.TextColor = Color.Black;
			} else {
				l.TextColor = Color.Gray;
				l.Text += "  (Pending)";
				l.FontAttributes = FontAttributes.Italic;
			}

			sl.Children.Add (l);

			ImageButton ib = new ImageButton ();
			ib.HorizontalOptions = LayoutOptions.EndAndExpand;
			ib.VerticalOptions = LayoutOptions.Center;
			ib.ImageHeightRequest = 27;
			ib.ImageWidthRequest = 27;
			if (ac.Selected) {
				ib.Source = "ui_check_filled.png";
			} else {
				ib.Source = "ui_check_empty.png";
			}

			ib.Clicked += delegate(object sender, EventArgs e) {

				var b = (ImageButton)sender;
				var t = b.CommandParameter;

				((ListView)((StackLayout)b.ParentView).ParentView).SelectedItem = t;
				if (ac.Selected)
				{
					ib.Source = "ui_check_filled.png";
				}
				else{
					ib.Source = "ui_check_empty.png";
				}
			};

//			ib.Clicked += delegate(object sender, EventArgs e) {
//				if (ac.Selected) {
//					ac.Selected = false;
//					ib.Source = "ui_check_empty.png";
//				} else {
//					ac.Selected = true;
//					ib.Source = "ui_check_filled.png";
//				}
//			};

			sl.Children.Add (ib);

			View = sl;
		}
	}

	public class CircleCell : ViewCell
	{
		public CircleCell()
		{
			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));

			deleteAction.Clicked += (sender, e) => {
				//var mi = ((MenuItem)sender);
				string circleid = ((AccountCircle)((MenuItem)sender).BindingContext).CircleID;
				AccountCircle ac = new AccountCircle ();
				ac.id = circleid;

				MessagingCenter.Send<AccountCircle> (ac, "deleteac");

			};
			this.ContextActions.Add (deleteAction);
		}			

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			AccountCircle ac = (AccountCircle)c;

			if (ac == null) {
				return;
			}

			this.Height = 65;

			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;
			sl.HorizontalOptions = LayoutOptions.Center;
			sl.VerticalOptions = LayoutOptions.Center;
			sl.BackgroundColor = Color.FromRgb (238, 236, 243);
			sl.HeightRequest = 65;

			BoxView bv = new BoxView ();
			bv.WidthRequest = 10;
			sl.Children.Add (bv);

			ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
				BorderColor = Color.Black,
				BorderThickness = 1,
				Aspect = Aspect.AspectFill,
				WidthRequest = 50,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Source= ac.PhotoURL
			};	

			sl.Children.Add (ci);

			Label l = new Label ();
			l.Text = ac.Fullname;
			l.HorizontalOptions = LayoutOptions.Start;
			l.VerticalOptions = LayoutOptions.Center;
			l.TranslationX = 5;
			l.FontAttributes = FontAttributes.Bold;

			if (ac.Accepted) {
				//this person is definitely IN my circle!
				l.TextColor = Color.Black;
			} else {
				l.TextColor = Color.Gray;
				l.Text += "  (Pending)";
				l.FontAttributes = FontAttributes.Italic;
			}
				
			sl.Children.Add (l);

			View = sl;
		}
	}
}

