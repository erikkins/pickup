using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
//using Xamarin.Forms.Labs.Controls;
using XLabs.Forms.Controls;
using Refractored.Xam.Vibrate.Abstractions;
using System.IO;
using System.Net;
using System.Text;
namespace PickUpApp
{	
	public partial class AddEditKid : ContentPage
	{	
		//ImageCircle.Forms.Plugin.Abstractions.CircleImage kidImage;
		SimpleBoundTextCell stcFirstName;
		SimpleBoundTextCell stcLastName;
		SimpleDateCell stcDOB;
		SimplePickerCell stcGender;
		SimpleImageCell sicPic;

		public AddEditKid (Kid selectedKid)
		{
			InitializeComponent ();
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			this.BackgroundColor = Color.FromRgb (238, 236, 243);

			//btnSave.Clicked += HandleClicked;
			//btnCancel.Clicked += HandleClicked1;
			this.ViewModel = new KidAddEditViewModel (App.client);
			this.ViewModel.CurrentKid = selectedKid;

			this.ToolbarItems.Add (new ToolbarItem ("Done", null, async() => {
				//pop the calendar window

//				Device.BeginInvokeOnMainThread(async() => {
//					await System.Threading.Tasks.Task.Delay(50);
//					await Navigation.PopAsync();
//				});

				//ViewModel.CurrentKid = selectedKid;
				await this.ViewModel.ExecuteAddEditCommand();

			}));
//			MessagingCenter.Subscribe<Kid>(this, "KidAdded", (s) =>
//			{					
//					Device.BeginInvokeOnMainThread(async() => {
//						await System.Threading.Tasks.Task.Delay(50);
//						await Navigation.PopAsync();
//					});
//			});

			List<string> genders = new List<string> ();
			genders.Add ("Unknown");
			genders.Add ("Female");
			genders.Add ("Male");

			ExtendedTableView tv = new ExtendedTableView ();
			tv.Intent = TableIntent.Data;
			tv.BackgroundColor = Color.FromRgb (238, 236, 243);
			tv.BindingContext = selectedKid;
			tv.HasUnevenRows = true;
			//tv.RowHeight = 75;

			TableSection ts = new TableSection ();
			sicPic = new SimpleImageCell (selectedKid.PhotoURL);
			ts.Add (sicPic);
			stcFirstName = new SimpleBoundTextCell ("First name", "Firstname");
			ts.Add (stcFirstName);
			stcLastName = new SimpleBoundTextCell ("Last name", "Lastname");
			ts.Add (stcLastName);
			stcDOB = new SimpleDateCell ("Date of Birth", selectedKid.DateOfBirth, "DateOfBirth");
			ts.Add (stcDOB);
			stcGender = new SimplePickerCell ("Gender", selectedKid.Gender, genders);
			ts.Add (stcGender);

			tv.Root.Add (ts);
			stacker.Children.Add (tv);

//			foreach (string g in genders) {
//				genderPicker.Items.Add (g);
//			}
//
//			genderPicker.SelectedIndex = genders.IndexOf (selectedKid.Gender);
//
//			genderPicker.SelectedIndexChanged += (object sender, EventArgs e) => {
//				ViewModel.CurrentKid.Gender = genderPicker.Items[genderPicker.SelectedIndex];
//			};
				

//			if (selectedKid != null) {
//				//txtFirstname.Text = selectedKid.Firstname;
//				//txtLastname.Text = selectedKid.Lastname;
//				ViewModel.CurrentKid = selectedKid;
//			}
//			if (ViewModel.CurrentKid.PhotoURL == null) {
//				kidImage = new ImageCircle.Forms.Plugin.Abstractions.CircleImage ();
//			}
//			else{
//				UriImageSource uis = new UriImageSource ();
//				uis.Uri = new Uri (ViewModel.CurrentKid.PhotoURL);
//				uis.CachingEnabled = false;
//
//				kidImage = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
//					BorderColor = Color.Black,
//					BorderThickness = 1,
//					Aspect = Aspect.AspectFill,
//					WidthRequest = 200,
//					HeightRequest = 200,
//					HorizontalOptions = LayoutOptions.Center,
//					Source = uis //UriImageSource.FromUri (new Uri (ViewModel.CurrentKid.PhotoURL))
//				};
//			}
//			stacker.Children.Add (kidImage);
				
			sicPic.Tapped += async delegate {
				try{

				var action = await DisplayActionSheet ("Choose Image Source", "Cancel", null, "Camera", "Photos");

				string filepath = "";

				//ok, since we're storing the filename as the kid.id guid, we need to make sure there is one
				//otherwise, make it up.

				string photoid = "";
				if (ViewModel.CurrentKid == null)
				{
					//ViewModel.CurrentKid = new Kid();
					photoid = Guid.NewGuid().ToString().ToLower();
				}
				else{
						if (string.IsNullOrEmpty(ViewModel.CurrentKid.Id))
						{
							photoid = Guid.NewGuid().ToString().ToLower();
						}
						else{
							photoid = ViewModel.CurrentKid.Id.ToLower();
						}
				}
				

				ViewModel.CurrentKid.PhotoURL = AzureStorageConstants.BlobEndPoint + App.myAccount.id.ToLower() + "/" + photoid + ".jpg";
					
				switch (action)
				{
				case "Camera":
					if (Media.Plugin.CrossMedia.Current.IsCameraAvailable && Media.Plugin.CrossMedia.Current.IsTakePhotoSupported) {

						var file = await Media.Plugin.CrossMedia.Current.TakePhotoAsync(new Media.Plugin.Abstractions.StoreCameraMediaOptions
							{ 
								Directory = "MyKidPics",
								Name = photoid + ".jpg",

							});
						filepath = file.Path;	
						sicPic.ImagePath = filepath;

						var bytes = default(byte[]);
						using (var streamReader = new StreamReader(file.GetStream()))
						{
							using (var memstream = new MemoryStream())
							{
								streamReader.BaseStream.CopyTo(memstream);
								bytes = memstream.ToArray();
							}
						}
						file.Dispose();
						if (bytes.Length > 0)
						{
							//resize it!
						
							var dep = DependencyService.Get<PickUpApp.IImageResizer>();
							bytes = dep.ResizeImage(bytes, 150,150, filepath);

							//upload it!

							await AzureBlobAccess.addContainerIfNotExists_async(App.myAccount.id);
							await AzureBlobAccess.uploadToBlobStorage_async(bytes, photoid + ".jpg", App.myAccount.id.ToLower());
							//ok, let's create the photo URL
							//ViewModel.CurrentKid.PhotoURL = AzureStorageConstants.BlobEndPoint + App.myAccount.id.ToLower() + "/" + photoid + ".jpg";
							//await ViewModel.ExecuteAddEditCommand();
							tv.OnDataChanged();
						}
					}
					else{
						await DisplayAlert("Oops", "Your camera is not available!", "OK");
					}
					break;
				case "Photos":
					if (!Media.Plugin.CrossMedia.Current.IsPickPhotoSupported)
					{
						await DisplayAlert("Oops", "Photo picking is not supported!", "OK");
						return;
					}
					var photofile  = await Media.Plugin.CrossMedia.Current.PickPhotoAsync();
					if (photofile == null)
					{
						return;
					}
					sicPic.ImagePath = photofile.Path;
					var photobytes = default(byte[]);
					using (var streamReader = new StreamReader(photofile.GetStream()))
					{
						using (var memstream = new MemoryStream())
						{
							streamReader.BaseStream.CopyTo(memstream);
							photobytes = memstream.ToArray();
						}
					}
					if (photobytes.Length > 0)
					{
						//resize it!

						var dep = DependencyService.Get<PickUpApp.IImageResizer>();
						photobytes = dep.ResizeImage(photobytes, 150,150, photofile.Path);

						//upload it!

						await AzureBlobAccess.addContainerIfNotExists_async(App.myAccount.id);
						await AzureBlobAccess.uploadToBlobStorage_async(photobytes, photoid + ".jpg", App.myAccount.id.ToLower());
						//ok, let's create the photo URL
						
						//await ViewModel.ExecuteAddEditCommand();

						tv.OnDataChanged();
					}

					break;
				}
					

//				kidImage = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
//					BorderColor = Color.Black,
//					BorderThickness = 1,
//					Aspect = Aspect.AspectFill,
//					WidthRequest = 200,
//					HeightRequest = 200,
//					HorizontalOptions = LayoutOptions.Center,
//					Source = UriImageSource.FromUri (new Uri (ViewModel.CurrentKid.PhotoURL))
//				};

				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex);
				}
			};

			/*
			MessagingCenter.Subscribe<Kid>(this, "KidAdded", (s) =>
				{
					//ViewModel.Refresh();
					Navigation.PopModalAsync();
				});
			*/
		}

		void HandleClicked1 (object sender, EventArgs e)
		{
			Navigation.PopModalAsync ();

		}

		void HandleClicked (object sender, EventArgs e)
		{
			if (ViewModel.CurrentKid == null) {
				ViewModel.CurrentKid = new Kid ();
			} 
			//ViewModel.CurrentKid.Firstname = txtFirstname.Text;
			//ViewModel.CurrentKid.Lastname = txtLastname.Text;

			ViewModel.ExecuteAddEditCommand ().ConfigureAwait(false);
		}
		protected KidAddEditViewModel ViewModel
		{
			get { return this.BindingContext as KidAddEditViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

