using System;
using PickUpApp.droid;
using Android.Graphics;
using System.IO;
using Android.Media;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidImageResizer))]
namespace PickUpApp.droid
{	
	public class AndroidImageResizer:IImageResizer
	{
		public byte[] ResizeImage (byte[] imageData, float width, float height, string filePath)
		{

			// Load the bitmap
			Bitmap originalImage = BitmapFactory.DecodeByteArray (imageData, 0, imageData.Length);

			int actualHeight = originalImage.Height;
			int actualWidth = originalImage.Width;

			float maxHeight = height;
			float maxWidth = width;
			float imgRatio = actualWidth / actualHeight;
			float maxRatio = maxWidth / maxHeight;

			//      width and height values are set maintaining the aspect ratio of the image

			if (actualHeight > maxHeight || actualWidth > maxWidth) {
				if (imgRatio < maxRatio) {
					imgRatio = maxHeight / actualHeight;
					actualWidth = (int) (imgRatio * actualWidth);
					actualHeight = (int) maxHeight;
				} else if (imgRatio > maxRatio) {
					imgRatio = maxWidth / actualWidth;
					actualHeight = (int) (imgRatio * actualHeight);
					actualWidth = (int) maxWidth;
				} else {
					actualHeight = (int) maxHeight;
					actualWidth = (int) maxWidth;

				}
			}



			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, actualWidth, actualHeight, true);

			//      check the rotation of the image and display it properly
			ExifInterface exif;
			try {
				exif = new ExifInterface(filePath);

				int orientation = exif.GetAttributeInt(
					ExifInterface.TagOrientation, 0);
				Matrix matrix = new Matrix();
				if (orientation == 6) {
					matrix.PostRotate(90);
				} else if (orientation == 3) {
					matrix.PostRotate(180);
				} else if (orientation == 8) {
					matrix.PostRotate(270);
				}
				resizedImage = Bitmap.CreateBitmap(resizedImage, 0, 0,
					resizedImage.Width, resizedImage.Height, matrix,
					true);
			} catch (IOException e) {
				System.Diagnostics.Debug.Write (e.Message);
			}

			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress (Bitmap.CompressFormat.Jpeg, 100, ms);
				return ms.ToArray ();
			}
		}
	}
}

