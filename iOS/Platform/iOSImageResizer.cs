using System.Drawing;
using UIKit;
using CoreGraphics;
using PickUpApp;
using PickUpApp.iOS;
using System;

[assembly: Xamarin.Forms.Dependency (typeof(iOSImageResizer))]
namespace PickUpApp.iOS 
{
	public class iOSImageResizer :IImageResizer
	{
		public byte[] ResizeImage (byte[] imageData, float maxWidth, float maxHeight, string filePath)
		{
			UIImage originalImage = ImageFromByteArray (imageData);

			UIKit.UIImage resized = ScaleAndRotateImage (originalImage, originalImage.Orientation);

			//UIKit.UIImage resized = ScaleImage (originalImage, (int)maxHeight);
			return resized.AsJPEG ().ToArray ();


			//maintain aspect ratio
			float width = 300, height = 300;

			double maxAspect = (double)maxWidth / (double)maxHeight;
			float aspect = (float)originalImage.Size.Width/(float)originalImage.Size.Height;

			if (maxAspect > aspect && originalImage.Size.Width > maxWidth) {
				//Width is the bigger dimension relative to max bounds
				width = maxWidth;
				height = maxWidth / aspect;
			}else if (maxAspect <= aspect && originalImage.Size.Height > maxHeight){
				//Height is the bigger dimension
				height = maxHeight;
				width = maxHeight * aspect;
			}


			//create a 24bit RGB image
			using (CGBitmapContext context = new CGBitmapContext (IntPtr.Zero,
				(int)width, (int)height, 8,
				(int)(4 * width), CGColorSpace.CreateDeviceRGB (),
				CGImageAlphaInfo.PremultipliedFirst)) {

				CGRect imageRect = new CGRect (0, 0, width, height);

				// draw the image
				context.DrawImage (imageRect, originalImage.CGImage);
				//for some reason the image was rotated left, so we're gonna rotate right one click
				UIKit.UIImage resizedImage = UIKit.UIImage.FromImage (context.ToImage (), 1.0f, UIImageOrientation.Right);


				// save the image as a jpeg
				return resizedImage.AsJPEG ().ToArray ();
			}
		}

		public static UIKit.UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null) {
				return null;
			}

			UIKit.UIImage image;
			try {
				image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
			} catch (Exception e) {
				Console.WriteLine ("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}


		public static UIImage ScaleImage(UIImage image, int maxSize)
		{

			UIImage res;

			using (CGImage imageRef = image.CGImage)
			{
				CGImageAlphaInfo alphaInfo = imageRef.AlphaInfo;
				CGColorSpace colorSpaceInfo = CGColorSpace.CreateDeviceRGB();
				if (alphaInfo == CGImageAlphaInfo.None)
				{
					alphaInfo = CGImageAlphaInfo.NoneSkipLast;
				}

				int width, height;

				width = (int)imageRef.Width;
				height = (int)imageRef.Height;


				if (height >= width)
				{
					width = (int)Math.Floor((double)width * ((double)maxSize / (double)height));
					height = maxSize;
				}
				else
				{
					height = (int)Math.Floor((double)height * ((double)maxSize / (double)width));
					width = maxSize;
				}


				CGBitmapContext bitmap;

				if (image.Orientation == UIImageOrientation.Up || image.Orientation == UIImageOrientation.Down)
				{
					bitmap = new CGBitmapContext(IntPtr.Zero, width, height, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, alphaInfo);
				}
				else
				{
					bitmap = new CGBitmapContext(IntPtr.Zero, height, width, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, alphaInfo);
				}

				switch (image.Orientation)
				{
				case UIImageOrientation.Left:
					bitmap.RotateCTM((float)Math.PI / 2);
					bitmap.TranslateCTM(0, -height);
					break;
				case UIImageOrientation.Right:
					bitmap.RotateCTM(-((float)Math.PI / 2));
					bitmap.TranslateCTM(-width, 0);
					break;
				case UIImageOrientation.Up:
					break;
				case UIImageOrientation.Down:
					bitmap.TranslateCTM(width, height);
					bitmap.RotateCTM(-(float)Math.PI);
					break;
				}

				bitmap.DrawImage(new CGRect(0, 0, width, height), imageRef);


				res = UIImage.FromImage(bitmap.ToImage());
				bitmap = null;

			}


			return res;
		}

		UIImage ScaleAndRotateImage(UIImage imageIn, UIImageOrientation orIn) {
			int kMaxResolution = 100; //1024; //2048;

			CGImage imgRef = imageIn.CGImage;
			nfloat width = imgRef.Width;
			nfloat height = imgRef.Height;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity ();
			CGRect bounds = new CGRect( 0, 0, width, height );

			if ( width > kMaxResolution || height > kMaxResolution )
			{
				nfloat ratio = width/height;

				if (ratio > 1)
				{
					bounds.Width  = kMaxResolution;
					bounds.Height = bounds.Width / ratio;
				}
				else
				{
					bounds.Height = kMaxResolution;
					bounds.Width  = bounds.Height * ratio;
				}
			}

			nfloat scaleRatio = bounds.Width / width;
			CGSize imageSize = new CGSize( width, height);
			UIImageOrientation orient = orIn;
			nfloat boundHeight;

			switch(orient)
			{
			case UIImageOrientation.Up:                                        //EXIF = 1
				transform = CGAffineTransform.MakeIdentity();
				break;

			case UIImageOrientation.UpMirrored:                                //EXIF = 2
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, 0f);
				transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
				break;

			case UIImageOrientation.Down:                                      //EXIF = 3
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, imageSize.Height);
				transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
				break;

			case UIImageOrientation.DownMirrored:                              //EXIF = 4
				transform = CGAffineTransform.MakeTranslation (0f, imageSize.Height);
				transform = CGAffineTransform.MakeScale(1.0f, -1.0f);
				break;

			case UIImageOrientation.LeftMirrored:                              //EXIF = 5
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation (imageSize.Height, imageSize.Width);
				transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
				transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI/ 2.0f);
				break;

			case UIImageOrientation.Left:                                      //EXIF = 6
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation (0.0f, imageSize.Width);
				transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.RightMirrored:                             //EXIF = 7
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
				transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.Right:                                     //EXIF = 8
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
				transform = CGAffineTransform.Rotate(transform, (float)Math.PI  / 2.0f);
				break;

			default:
				throw new Exception("Invalid image orientation");
				break;
			}

			UIGraphics.BeginImageContext(bounds.Size);

			CGContext context = UIGraphics.GetCurrentContext ();

			if ( orient == UIImageOrientation.Right || orient == UIImageOrientation.Left )
			{
				context.ScaleCTM(-scaleRatio, scaleRatio);
				context.TranslateCTM(-height, 0);
			}
			else
			{
				context.ScaleCTM(scaleRatio, -scaleRatio);
				context.TranslateCTM(0, -height);
			}

			context.ConcatCTM(transform);
			context.DrawImage (new CGRect (0, 0, width, height), imgRef);

			UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return imageCopy;
		}
	}
}

