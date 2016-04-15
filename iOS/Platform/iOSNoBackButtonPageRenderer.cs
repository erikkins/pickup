
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using PickUpApp.iOS;

using Foundation;
using NotificationCenter;
using WebKit;


[assembly:ExportRenderer (typeof(ContentPage), typeof(iOSNoBackButtonPageRenderer))]

namespace PickUpApp.iOS
{
	public class iOSNoBackButtonPageRenderer : PageRenderer
	{
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			// Xamarin.Forms wraps page's view controller
			if (ViewController.Title == "CALENDAR") {
				ViewController.ParentViewController.NavigationItem.Title = "";
				ViewController.ParentViewController.NavigationItem.SetHidesBackButton (true, false);
			}

			if (ViewController.Title == "FETCH REQUEST") {
				ViewController.ParentViewController.NavigationItem.Title = "FETCH REQUEST";
				ViewController.ParentViewController.NavigationItem.BackBarButtonItem = new UIBarButtonItem ("", UIBarButtonItemStyle.Plain, null);

				//ViewController.ParentViewController.NavigationItem.BackBarButtonItem.Title = "";
				//ViewController.ParentViewController.NavigationItem.SetHidesBackButton (true, false);
			}

		}
	}
}
