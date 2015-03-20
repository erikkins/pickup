using System;
using Xamarin.Forms;


namespace PickUpApp
{
	public class Util
	{
		public static DateTime RoundUp(DateTime dt, TimeSpan d)
		{
			return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
		}

		public Util ()
		{
		}



//		public Image ConvertToGrayscale(Image source)
//		{
//			Image im = new Image ();
//
//		}

//		public Bitmap ConvertToGrayscale(Bitmap source)
//		{
//			Bitmap bm = new Bitmap(source.Width,source.Height);
//			for(int y=0;y<bm.Height;y++)
//			{
//				for(int x=0;x<bm.Width;x++)
//				{
//					Color c=source.GetPixel(x,y);
//					int luma = (int)(c.R*0.3 + c.G*0.59+ c.B*0.11);
//					bm.SetPixel(x,y,Color.FromArgb(luma,luma,luma));
//				}
//			}
//			return bm;
//		}

//		UIImage ConvertToGrayScale (UIImage image)
//		{
//			RectangleF imageRect = new RectangleF (PointF.Empty, image.Size);
//			using (var colorSpace = CGColorSpace.CreateDeviceGray ())
//			using (var context = new CGBitmapContext (IntPtr.Zero, (int) imageRect.Width, (int) imageRect.Height, 8, 0, colorSpace, CGImageAlphaInfo.None)) {
//				context.DrawImage (imageRect, image.CGImage);
//				using (var imageRef = context.ToImage ())
//					return new UIImage (imageRef);
//			}
//		}
	}


}

