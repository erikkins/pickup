using System;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
//using PCLStorage;
using System.Collections.ObjectModel;
using Xamarin.Forms.Labs.Services.Geolocation;

namespace PickUpApp
{
	public class App
	{
		public const string applicationURL = @"https://pickup.azure-mobile.net/";
		public const string applicationKey = @"smfGLlHZSdujNrejkOSaRtVbGmPwwz12";
		public static MobileServiceClient client = new MobileServiceClient(applicationURL, applicationKey);
		public static MobileServiceUser user;

		public static Account myAccount = new Account();
		public static ObservableCollection<Kid> myKids = new ObservableCollection<Kid>();

		public static Page GetMainPage ()
		{	
			//first things first, see if we already have an account registered here


			return new Splash ();


			//return new Today ();
			//return new CarouselMaster ();

			//ViewModels.CurrentAccount fvm = new PickUpApp.ViewModels.CurrentAccount ();
			//fvm.Accounts = PickupService.LoadAccount ().Result;

			return new TabbedMaster ();

			/*
			return new ContentPage { 
				Content = new Label {
					Text = "Howdy, Partners!",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				},
			};
			*/
		
		}
	}
}

