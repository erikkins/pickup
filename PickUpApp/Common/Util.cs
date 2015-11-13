using System;
using Xamarin.Forms;
using System.Drawing;
using System.IO;
using System.Collections.ObjectModel;
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
//using System.Linq;

namespace PickUpApp
{
	public class Util
	{
		public static DateTime RoundUp (DateTime dt, TimeSpan d)
		{
			return new DateTime (((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
		}

		public Util ()
		{


		}

		public static T FindParentPage<T>(Element element)
		{
			Element Parent = element.Parent;

			while (Parent != null && Parent.GetType() != typeof(T))
			{
				Parent = Parent.Parent;
			}

			if (Parent == null)
			{
				throw new Exception(
					string.Format("FindParentPage: Parent {0} not found for element {1}", typeof(T), element));
			}

			return (T)(object)Parent;
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

	public static class Cloner
	{
		public static  T Clone<T> (this T source)
		{
			if (Object.ReferenceEquals (source, null)) {
				return default(T);
			}

			// In the PCL we do not have the BinaryFormatter
			return JsonConvert.DeserializeObject<T> (JsonConvert.SerializeObject (source));
		}
	}

	public static class DateTimeExtensions
	{
		/// <summary>
		/// Gets the relative time for a datetime.
		/// </summary>
		/// <param name="dateTime">The datetime to get the relative time.</param>
		/// <returns>A relative time in english.</returns>
		public static string GetTimeSpan(this DateTime dateTime)
		{
			TimeSpan diff = DateTime.Now.Subtract(dateTime);

			if (diff.TotalMinutes < 1)
			{
				return string.Format("{0:D2} second{1} ago", diff.Seconds, PluralizeIfNeeded(diff.Seconds));
			}

			if (diff.TotalHours < 1)
			{
				return string.Format("{0:D2} minute{1} ago", diff.Minutes, PluralizeIfNeeded(diff.Minutes));
			}

			if (diff.TotalDays < 1)
			{
				return string.Format("{0:D2} hour{2} and {1:D2} minute{3} ago", diff.Hours, diff.Minutes, PluralizeIfNeeded(diff.Hours), PluralizeIfNeeded(diff.Minutes));
			}

			if (diff.TotalDays <= 2)
			{
				return string.Format(
					"{0:D2} day{3}, {1:D2} hour{4} and {2:D2} minute{5} ago",
					diff.Days,
					diff.Hours,
					diff.Minutes,
					PluralizeIfNeeded(diff.Days),
					PluralizeIfNeeded(diff.Hours),
					PluralizeIfNeeded(diff.Minutes));
			}

			if (diff.TotalDays <= 30)
			{
				try{
				return string.Format("{0:0} days ago", diff.TotalDays);
				}
				catch(Exception ex) {
					System.Diagnostics.Debug.WriteLine (ex);
				}

			}

			return string.Format("{0:g}", dateTime);
		}

		/// <summary>
		/// Gets a 's' if value is > 1.
		/// </summary>
		/// <param name="testValue">The value to test.</param>
		/// <returns>An 's' if value is > 1, otherwise an empty string.</returns>
		private static string PluralizeIfNeeded(int testValue)
		{
			return testValue > 1 ? "s" : string.Empty;
		}
	}



}


	
		
	


