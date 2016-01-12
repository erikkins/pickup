using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using System.IO;
using PickUpApp.iOS;
using PickUpApp;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Facebook.CoreKit;

[assembly: ExportRenderer(typeof(FBLike), typeof(FBLikeRenderer))]
namespace PickUpApp.iOS
{
	public class FBLikeRenderer : ViewRenderer<FBLike, UIButton>
	{
		Facebook.ShareKit.LikeButton likeButton;

		protected override void OnElementChanged (ElementChangedEventArgs<FBLike> e)
		{
			base.OnElementChanged (e);

			if (Control == null) {
				likeButton = new Facebook.ShareKit.LikeButton (base.Frame);
				likeButton.UserInteractionEnabled = true;
				likeButton.SetObjectId ("http://www.facebook.com/famfetch");//("https://apps.facebook.com/445633295574438");

				SetNativeControl (likeButton);
			}

			Facebook.CoreKit.Profile.Notifications.ObserveDidChange ((sender, e2) => {				
				likeButton.SetTitle(e2.NewProfile.Name, UIControlState.Normal);
				//nameLabel.Text = e2.NewProfile.Name;
			});


//			if (e.NewElement != null) {
//				likeButton.TouchUpInside += delegate(object sender, EventArgs e2) {
//					BTProgressHUD.ShowToast("clicked", true, 1000);
//
//				};;
//			}
		}
			
			

		/*
		public override void Draw (CGRect rect)
		{
			using (var context = UIGraphics.GetCurrentContext ()) {
				Facebook.ShareKit.LikeButton lb = new Facebook.ShareKit.LikeButton (rect);
				lb.SetObjectId ("https://apps.facebook.com/445633295574438");
				lb.SetTitle ("LIKE", UIControlState.Normal);
				lb.Draw (rect);
			}
		}
		*/

	}
}



