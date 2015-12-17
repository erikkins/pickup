using System;
using UIKit;
using System.Drawing;
using CoreGraphics;
using Foundation;
using PickUpApp.iOS;
using System.IO;

[assembly: Xamarin.Forms.Dependency (typeof(iOSCircleText))]
namespace PickUpApp.iOS
{
	
	public class iOSCircleText:ICircleText
	{
		public string CreateCircleText(string text, float width, float height)
		{
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filename = Path.Combine (documents, text + ".png");

			if (!File.Exists (filename)) {
				UIGraphics.BeginImageContextWithOptions (new CoreGraphics.CGSize (width, height), false, 0);
				var context = UIGraphics.GetCurrentContext ();

				var content = text;
				var font = UIFont.SystemFontOfSize (16);

				//const float width = 150;
				//const float height = 150;

				context.SetFillColor (UIColor.FromRGB(73, 55, 109).CGColor);
				context.FillEllipseInRect (new CGRect (0, 0, width, height));

				var contentString = new NSString (content);
				var contentSize = contentString.StringSize (font);

				var rect = new CGRect (0, ((height - contentSize.Height) / 2) + 0, width, contentSize.Height);

				context.SetFillColor (UIColor.White.CGColor);
				new NSString (content).DrawString (rect, font, UILineBreakMode.WordWrap, UITextAlignment.Center);

				var image = UIGraphics.GetImageFromCurrentImageContext ();

				File.WriteAllBytes (filename, image.AsPNG ().ToArray ());
			}

			//return image.AsPNG ().ToArray ();
			return filename;

		}
	}
}

