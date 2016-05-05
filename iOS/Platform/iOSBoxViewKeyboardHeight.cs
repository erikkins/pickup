using System;
using UIKit;
using PickUpApp;
using PickUpApp.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer( typeof(BoxViewKeyboardHeight), typeof(BoxViewKeyboardHeightRenderer))]
namespace PickUpApp.iOS
{
	public class BoxViewKeyboardHeightRenderer : BoxRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged (e);

			if (Element != null)
			{
				Element.HeightRequest = 0;
			}

			UIKeyboard.Notifications.ObserveWillShow ((sender, args) => {

				if (Element != null)
				{
					Element.HeightRequest = args.FrameEnd.Height;
				}

			});

			UIKeyboard.Notifications.ObserveWillHide ((sender, args) => {

				if (Element != null)
				{
					Element.HeightRequest = 0;
				}

			});
		}
	}
}
