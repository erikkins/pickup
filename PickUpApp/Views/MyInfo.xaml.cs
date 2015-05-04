using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;


namespace PickUpApp
{
	public partial class MyInfo : ContentPage
	{
		public MyInfo ()
		{
			InitializeComponent ();
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			btnManagePlaces.Clicked += async delegate(object sender, EventArgs e) {
				Navigation.PushModalAsync(new MyPlaces());
			};

			ImageCircle.Forms.Plugin.Abstractions.CircleImage myImage = new ImageCircle.Forms.Plugin.Abstractions.CircleImage () {
				BorderColor = Color.Black,
				BorderThickness = 1,
				Aspect = Aspect.AspectFill,
				WidthRequest = 200,
				HeightRequest = 200,
				HorizontalOptions = LayoutOptions.Center,
			};
			myImage.SetBinding (ImageCircle.Forms.Plugin.Abstractions.CircleImage.SourceProperty, "PhotoURL");

			stacker.Children.Add (myImage);

			Button btnUpdate = new Button ();
			btnUpdate.Text = "Update";
			btnUpdate.Clicked += async delegate(object sender, EventArgs e) {
				await ViewModel.ExecuteAddEditCommand();
			};
			stacker.Children.Add (btnUpdate);

			this.ViewModel = new SplashViewModel (App.client);
		}

		protected SplashViewModel ViewModel
		{
			get { return this.BindingContext as SplashViewModel; }
			set { this.BindingContext = value; }
		}

	}
}

