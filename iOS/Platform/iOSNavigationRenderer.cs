
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using PickUpApp.iOS;


[assembly:ExportRenderer (typeof(NavigationPage), typeof(iOSNavigationRenderer))]

namespace PickUpApp.iOS
{
	public class iOSNavigationRenderer : NavigationRenderer
	{

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();

			NavigationBar.TopItem.BackBarButtonItem =new UIBarButtonItem ("", UIBarButtonItemStyle.Plain, null);
			UIImage imgBack = UIImage.FromBundle ("icn_back.png");
			NavigationBar.BackIndicatorImage = imgBack;
			NavigationBar.BackIndicatorTransitionMaskImage = imgBack;
		}

	}
}