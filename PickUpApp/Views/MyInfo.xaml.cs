using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using XLabs.Forms.Controls;
using System.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace PickUpApp
{
	public partial class MyInfo : ContentPage
	{

		SimpleBoundTextCell sbtcFirstName;
		SimpleBoundTextCell sbtcLastName;
		SimpleBoundTextCell sbtcEmail;
		SimpleBoundTextCell sbtcPhone;
		SimpleImageCell sicPhoto;

		public MyInfo ()
		{
			InitializeComponent ();
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			this.BackgroundColor = Color.FromRgb(238, 236, 243);
			this.ViewModel = new SplashViewModel (App.client);

			this.ToolbarItems.Add (new ToolbarItem ("Done", null, async() => {
				await ViewModel.ExecuteAddEditCommand();
			}));

			ExtendedTableView tv = new ExtendedTableView ();
			tv.BindingContext = this.ViewModel;
			tv.Intent = TableIntent.Data;
			tv.BackgroundColor = Color.FromRgb (238, 236, 243);

			tv.HasUnevenRows = true;

			TableSection ts = new TableSection ();
			sicPhoto = new SimpleImageCell (ViewModel.PhotoURL);
			ts.Add (sicPhoto);
			sicPhoto.Tapped += async delegate(object sender, EventArgs e) {

				var action = await DisplayActionSheet ("Choose Image Source", "Cancel", null, "Camera", "Photos");
				Debug.WriteLine("Action: " + action); 

				string filepath = "";

				//ok, since we're storing the filename as the kid.id guid, we need to make sure there is one
				//otherwise, make it up.
				string photoid = App.myAccount.id;
				switch (action)
				{
				case "Camera":

					if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported) {
					
						var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
							{ 
								Directory = "MyPics",
								Name = photoid + ".jpg",
								DefaultCamera = CameraDevice.Front
							});

						if (file == null)
						{
							//they canceled!
							return;
						}
						//update the image immediately
						sicPhoto.ImagePath = file.Path;

						filepath = file.Path;	
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
							ViewModel.PhotoURL = AzureStorageConstants.BlobEndPoint + App.myAccount.id.ToLower() + "/" + photoid + ".jpg";
							await ViewModel.ExecuteAddEditCommand();
							//sicPhoto.ImagePath = ViewModel.PhotoURL;

							tv.OnDataChanged();
						}
					}
					else{
						await DisplayAlert("Oops", "Your camera is not available!", "OK");
					}
					break;
				case "Photos":
					if (!CrossMedia.Current.IsPickPhotoSupported)
					{
						await DisplayAlert("Oops", "Photo picking is not supported!", "OK");
						return;
					}
					var photofile  = await CrossMedia.Current.PickPhotoAsync();
					if (photofile == null)
					{
						return;
					}
					sicPhoto.ImagePath = photofile.Path;
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
						ViewModel.PhotoURL = AzureStorageConstants.BlobEndPoint + App.myAccount.id.ToLower() + "/" + photoid + ".jpg";
						await ViewModel.ExecuteAddEditCommand();
						//sicPhoto.ImagePath = ViewModel.PhotoURL;

						tv.OnDataChanged();
					}

					break;
				}
			};
			sbtcFirstName = new SimpleBoundTextCell ("First name", "Firstname");
			ts.Add (sbtcFirstName);
			sbtcLastName = new SimpleBoundTextCell ("Last name", "Lastname");
			ts.Add (sbtcLastName);
			sbtcPhone = new SimpleBoundTextCell ("Mobile phone", "Phone");
			ts.Add (sbtcPhone);
			sbtcEmail = new SimpleBoundTextCell ("Email", "Email");
			ts.Add (sbtcEmail);

			tv.Root.Add (ts);
			stacker.Children.Add (tv);


//			ImageCircle.Forms.Plugin.Abstractions.CircleImage myImage = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
//				BorderColor = Color.Black,
//				BorderThickness = 1,
//				Aspect = Aspect.AspectFill,
//				WidthRequest = 200,
//				HeightRequest = 200,
//				HorizontalOptions = LayoutOptions.Center,
//			};
//			myImage.SetBinding (ImageCircle.Forms.Plugin.Abstractions.CircleImage.SourceProperty, "PhotoURL");
//
//			stacker.Children.Add (myImage);
//
//			Button btnUpdate = new Button ();
//			btnUpdate.Text = "Update";
//			btnUpdate.Clicked += async delegate(object sender, EventArgs e) {
//				await ViewModel.ExecuteAddEditCommand();
//			};
//			stacker.Children.Add (btnUpdate);


		}

		protected SplashViewModel ViewModel
		{
			get { return this.BindingContext as SplashViewModel; }
			set { this.BindingContext = value; }
		}

	}
}

