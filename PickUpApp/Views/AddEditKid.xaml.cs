using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms.Labs.Controls;
using Refractored.Xam.Vibrate.Abstractions;
using System.IO;
using System.Net;
using System.Text;
namespace PickUpApp
{	
	public partial class AddEditKid : ContentPage
	{	
		ImageCircle.Forms.Plugin.Abstractions.CircleImage kidImage;

		public AddEditKid (Kid selectedKid)
		{
			InitializeComponent ();
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			btnSave.Clicked += HandleClicked;
			btnCancel.Clicked += HandleClicked1;
			this.ViewModel = new KidAddEditViewModel (App.client);

			List<string> genders = new List<string> ();
			genders.Add ("Unknown");
			genders.Add ("Female");
			genders.Add ("Male");

			foreach (string g in genders) {
				genderPicker.Items.Add (g);
			}

			genderPicker.SelectedIndex = genders.IndexOf (selectedKid.Gender);

			genderPicker.SelectedIndexChanged += (object sender, EventArgs e) => {
				ViewModel.CurrentKid.Gender = genderPicker.Items[genderPicker.SelectedIndex];
			};
				

			if (selectedKid != null) {
				//txtFirstname.Text = selectedKid.Firstname;
				//txtLastname.Text = selectedKid.Lastname;
				ViewModel.CurrentKid = selectedKid;
			}
			if (ViewModel.CurrentKid.PhotoURL == null) {
				kidImage = new ImageCircle.Forms.Plugin.Abstractions.CircleImage ();
			}
			else{
				UriImageSource uis = new UriImageSource ();
				uis.Uri = new Uri (ViewModel.CurrentKid.PhotoURL);
				uis.CachingEnabled = false;

				kidImage = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
					BorderColor = Color.Black,
					BorderThickness = 1,
					Aspect = Aspect.AspectFill,
					WidthRequest = 200,
					HeightRequest = 200,
					HorizontalOptions = LayoutOptions.Center,
					Source = uis //UriImageSource.FromUri (new Uri (ViewModel.CurrentKid.PhotoURL))
				};
			}
			stacker.Children.Add (kidImage);
				

			btnPic.Clicked += async delegate {
				string filepath = "";

				//ok, since we're storing the filename as the kid.id guid, we need to make sure there is one
				//otherwise, make it up.
				string photoid = ViewModel.CurrentKid.Id.ToLower();
				if (string.IsNullOrEmpty(photoid))
				{
					photoid = Guid.NewGuid().ToString().ToLower();
					//I guess we should just set it
					ViewModel.CurrentKid.Id = photoid;
				}

				if (Media.Plugin.CrossMedia.Current.IsCameraAvailable) {

					var file = await Media.Plugin.CrossMedia.Current.TakePhotoAsync(new Media.Plugin.Abstractions.StoreCameraMediaOptions
						{ 
							Directory = "MyKidPics",
							Name = photoid + ".jpg",

						});
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
					if (bytes.Length > 0)
					{
						//resize it!
					
						var dep = DependencyService.Get<PickUpApp.IImageResizer>();
						bytes = dep.ResizeImage(bytes, 300,300, file.Path);

						//upload it!

						await AzureBlobAccess.addContainerIfNotExists_async(App.myAccount.id);
						await AzureBlobAccess.uploadToBlobStorage_async(bytes, photoid + ".jpg", App.myAccount.id.ToLower());
						//ok, let's create the photo URL
						ViewModel.CurrentKid.PhotoURL = AzureStorageConstants.BlobEndPoint + App.myAccount.id.ToLower() + "/" + photoid + ".jpg";
						await ViewModel.ExecuteAddEditCommand();
					}
				}
					

				kidImage = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
					BorderColor = Color.Black,
					BorderThickness = 1,
					Aspect = Aspect.AspectFill,
					WidthRequest = 200,
					HeightRequest = 200,
					HorizontalOptions = LayoutOptions.Center,
					Source = UriImageSource.FromUri (new Uri (ViewModel.CurrentKid.PhotoURL))
				};


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

