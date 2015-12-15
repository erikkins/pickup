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
		public MyCircle ()
		{
			InitializeComponent ();
			this.ViewModel = new MyCircleViewModel (App.client);

			ExtendedListView lstCircle = new ExtendedListView ();

			lstCircle.ItemsSource = ViewModel.Circle;
			lstCircle.ItemTemplate = new DataTemplate (typeof(CircleCell));
			lstCircle.RefreshCommand = ViewModel.LoadItemsCommand;
			lstCircle.IsPullToRefreshEnabled = true;
			lstCircle.HasUnevenRows = true;
			lstCircle.BackgroundColor = AppColor.AppGray;
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


			MessagingCenter.Subscribe<string> (this, "circleloaded", (s) => {
				lstCircle.IsRefreshing = false;
			});
			MessagingCenter.Subscribe<AccountCircle>(this, "deleteac", (ac) => {
				ViewModel.CurrentAccountCircle = ac;
				ViewModel.ExecuteDeleteCommand();
			});

			MessagingCenter.Subscribe<LocalContact> (this, "contactpicked", async(lc) => {
				await Navigation.PopAsync(false);
				await Navigation.PushAsync(new AddEditContact(lc, true),false);
			});

			this.ToolbarItems.Add (new ToolbarItem ("Add Contact", "icn_new.png", async() => {

				//we really want to show a actionsheet allowing them to manually add the contact or select them from their existing contact list
				var action = await DisplayActionSheet ("Contact Source", "Cancel", null, "Manually Enter", "From Contacts List");
				if (action == "From Contacts List")
				{
					//really this should push AddEditContact, then push the selectcontact screen modally
					await Navigation.PushAsync(new SelectContact());
				}
				else{
					LocalContact lc = new LocalContact();
					await Navigation.PushAsync(new AddEditContact(lc, true));
				}

			}));

			this.BackgroundColor = Color.FromRgb (238, 236, 243);
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);	
			MessagingCenter.Subscribe<LocalContact> (this, "ContactAdded", (s) => {
				Navigation.PopAsync ();
				ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
				ViewModel.Refresh();
			});
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


	public class CircleCell : ViewCell
	{
		public CircleCell()
		{
			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));

			deleteAction.Clicked += async (sender, e) => {
				var mi = ((MenuItem)sender);
				//Debug.WriteLine("Delete Context Action clicked: " + mi.CommandParameter);
				string circleid = ((AccountCircle)((MenuItem)sender).BindingContext).CircleID;
				AccountCircle ac = new AccountCircle();
				ac.id = circleid;

				MessagingCenter.Send<AccountCircle>(ac, "deleteac");

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
				//BorderThickness = 1,
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
				//really shouldn't be able to select this
				//this.IsEnabled = false;
			}


			sl.Children.Add (l);



			View = sl;
		}
	}
}

