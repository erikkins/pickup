using System;
using System.Collections.Generic;
using Xamarin.Forms;
using FFImageLoading.Forms;

namespace PickUpApp
{	
	public partial class MyKids : ContentPage
	{	
		public MyKids ()
		{
			InitializeComponent ();
			this.ViewModel = new KidsViewModel (App.client);

			this.ToolbarItems.Add (new ToolbarItem ("Add Kid", "icn_new.png", async() => {
				Kid k = new Kid ();
				k.Mine = true;
				//should this be modal?
				await Navigation.PushAsync (new AddEditKid (k));
			}));
				
			//let's do it programmatically
			ListView lstKids = new ListView();
			lstKids.SeparatorColor = Color.Black;
			lstKids.ItemsSource = ViewModel.KidsSorted;
			lstKids.ItemTemplate = new DataTemplate (typeof(MyKidCell));
			lstKids.IsGroupingEnabled = true;
			lstKids.GroupDisplayBinding = new Binding ("ViewModel.KidsSorted.Key");
			lstKids.GroupHeaderTemplate = new DataTemplate (typeof(MyKidCellHeader));
			lstKids.BackgroundColor = AppColor.AppGray;
			lstKids.HasUnevenRows = true;

			stacker.Children.Add (lstKids);

			lstKids.ItemSelected += delegate(object sender, SelectedItemChangedEventArgs e) {
				if (e.SelectedItem == null) return;
				Navigation.PushAsync(new AddEditKid(e.SelectedItem as Kid));
				lstKids.SelectedItem = null;
			};

			//lstKids.ItemSelected += HandleItemSelected;
			//btnAdd.Clicked += HandleClicked;
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);


			this.Disappearing += delegate(object sender, EventArgs e) {
				
			};

			MessagingCenter.Unsubscribe<Kid> (this, "KidAdded");
			MessagingCenter.Subscribe<Kid>(this, "KidAdded",  async(s) =>
				{
					//System.Diagnostics.Debug.WriteLine("KIDADDED");

					//ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
					//Navigation.PopAsync();

					await ViewModel.ExecuteLoadItemsCommand().ContinueWith( x => {
						Device.BeginInvokeOnMainThread(()=>{
							Navigation.PopAsync();
						});
					});

				});

			MessagingCenter.Unsubscribe<EmptyClass> (this, "KidDeleted");
			MessagingCenter.Subscribe<EmptyClass> (this, "KidDeleted", (p) => {
				if (string.IsNullOrEmpty(p.Status))
				{
					//Navigation.PopAsync();
					ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
				}
				else{
					DisplayAlert("Could not delete", "This kid is in use in the following activities: " + p.Status, "OK");
				}
			});

			MessagingCenter.Unsubscribe<Kid> (this, "deletekid");
			MessagingCenter.Subscribe<Kid> (this, "deletekid", (k) => {
				ViewModel.SelectedKid = k;
				ViewModel.ExecuteDeleteCommand().ConfigureAwait(false);
			});

		}



		public void OnDelete (object sender, EventArgs e) {
			var mi = ((MenuItem)sender);
			Kid k = (Kid)mi.CommandParameter;
			if (k.Mine) {
				ViewModel.SelectedKid = k;
				ViewModel.ExecuteDeleteCommand ().ConfigureAwait (false);
			} else {
				DisplayAlert ("Uh oh", "You cannot delete someone else's kid!", "OK");
			}
			//DisplayAlert("Delete Context Action", k.Fullname + " delete context action", "OK");
		}

		void HandleClicked (object sender, EventArgs e)
		{
			Kid k = new Kid ();
			Navigation.PushModalAsync (new AddEditKid (k));
		}

//		void HandleItemSelected (object sender, SelectedItemChangedEventArgs e)
//		{
//			//I guess we'd edit from here
//			if (e.SelectedItem == null) return;
//			Navigation.PushAsync(new AddEditKid(e.SelectedItem as Kid));
//			lstKids.SelectedItem = null;
//
//		}

		protected KidsViewModel ViewModel
		{
			get { return this.BindingContext as KidsViewModel; }
			set { this.BindingContext = value; }
		}

	}


	public class MyKidCellHeader : ViewCell
	{

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			//this.Height = 65;
			if (c == null) {
				return;
			}

			StackLayout sl = new StackLayout ();
			sl.Padding = 0;
			sl.BackgroundColor = AppColor.AppPurple;

			Label l = new Label ();
			l.VerticalOptions = LayoutOptions.CenterAndExpand;
			l.TranslationX = 30;
			l.FontAttributes = FontAttributes.Bold;
			l.HorizontalOptions = LayoutOptions.Start;
			l.TextColor = Color.White;
			l.SetBinding (Label.TextProperty, "Key");

			sl.Children.Add (l);

			View = sl;

		}
	}

	public class MyKidCell : ViewCell
	{

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			dynamic c = BindingContext;
			this.Height = 65;
			if (c == null) {
				return;
			}
			Kid k = (Kid)c;
			this.StyleId = "Disclosure";


			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			deleteAction.Clicked += (sender, e) => {
				var mi = ((MenuItem)sender);
				Kid currKid = (Kid)mi.CommandParameter;
				if (currKid.Mine) {
					MessagingCenter.Send<Kid>(currKid, "deletekid");
				} else {
					App.hudder.showToast("Uh oh, You cannot delete someone else's kid!");
					//DisplayAlert ("Uh oh", "You cannot delete someone else's kid!", "OK");
				}
			};

			ContextActions.Add (deleteAction);

			StackLayout slHoriz = new StackLayout ();
			slHoriz.Orientation = StackOrientation.Horizontal;
			slHoriz.VerticalOptions = LayoutOptions.Center;

			BoxView bv = new BoxView ();
			bv.WidthRequest = 10;

			slHoriz.Children.Add (bv);

			/*
			ImageSource imageSource = null;

			UriImageSource uis = new UriImageSource ();
			if (string.IsNullOrEmpty (k.PhotoURL)) {
				//why is it null??? it should be atleast the filename letters
				k.PhotoURL = null;

			} else {
				uis.Uri = new Uri (k.PhotoURL);
				uis.CachingEnabled = true;
				uis.CacheValidity = new TimeSpan (0, 0, 5, 0);

				if (k.PhotoURL.ToLower ().StartsWith ("http")) {
					imageSource = uis;
				} else {
					imageSource = FileImageSource.FromFile (k.PhotoURL);
				}
			}
			ImageCircle.Forms.Plugin.Abstractions.CircleImage ci = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
				BorderColor = Color.Black,
				BorderThickness = 1,
				Aspect = Aspect.AspectFill,
				WidthRequest = 50,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
				Source= imageSource //k.PhotoURL
			};
			slHoriz.Children.Add (ci);
			*/

			CachedImage cachedimg = new CachedImage ();
			cachedimg.Source = k.PhotoURL;
			cachedimg.CacheDuration = TimeSpan.FromDays (30);
			cachedimg.DownsampleToViewSize = true;
			cachedimg.TransparencyEnabled = false;
			cachedimg.Aspect = Aspect.AspectFill;
			cachedimg.HeightRequest = 50;
			cachedimg.WidthRequest = 50;
			cachedimg.HorizontalOptions = LayoutOptions.Start;
			cachedimg.VerticalOptions = LayoutOptions.Center;
			cachedimg.Transformations.Add (new FFImageLoading.Transformations.CircleTransformation (1, "0x000000"));

			slHoriz.Children.Add (cachedimg);


			slHoriz.Children.Add (bv);

			Label l = new Label ();
			l.Text = k.Fullname;
			l.VerticalOptions = LayoutOptions.Center;
			l.HorizontalOptions = LayoutOptions.Start;
			l.TranslationX = 5;
			l.TextColor = Color.Black;
			slHoriz.Children.Add (l);

			View = slHoriz;
		}

		void OnDelete (object sender, EventArgs e)
		{
			
		}
	}
}

