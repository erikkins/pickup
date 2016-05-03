using System;
using PickUpApp.droid;
using Android.Graphics;
using System.IO;
using Android.Media;
using Android.Content.Res;

[assembly: Xamarin.Forms.Dependency (typeof(AndroidCircleText))]
namespace PickUpApp.droid
{
	public class AndroidCircleText : ICircleText
	{
		public string CreateCircleText(string text, float width, float height)
		{
			
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);

			var filename = System.IO.Path.Combine (documents, text + ".png");

			//if (!File.Exists (filename)) {

				/*

				Bitmap b = Bitmap.CreateBitmap ((int)width, (int)height, Bitmap.Config.Argb8888);
				Canvas c = new Canvas (b);
				Paint p = new Paint (PaintFlags.AntiAlias | PaintFlags.LinearText);
				Rect r = new Rect ();
				c.GetClipBounds (r);
				int cHeight = r.Height();
				int cWidth = r.Width();
				p.GetTextBounds (text, 0, text.Length, r);	
				float x = (cWidth / 2f - r.Width () / 2f - r.Left) -5 ;
				float y = (cHeight / 2f + r.Height () / 2f - r.Bottom) + 2;

				p.Color = Color.Rgb(73, 55, 109);
				p.SetStyle (Paint.Style.Fill);
				c.DrawPaint (p);

				p.Dither = true;
				p.Color = Color.White;
				p.TextAlign = Paint.Align.Left;
				p.FakeBoldText = true;
				p.TextSize = 16 * Resources.System.DisplayMetrics.Density;
				p.SetTypeface (Typeface.SansSerif);
				c.DrawText (text, x, y, p);

				*/

				Bitmap b = Bitmap.CreateBitmap ((int)width, (int)height, Bitmap.Config.Argb8888);
				Canvas c = new Canvas (b);
				Paint p = new Paint (PaintFlags.AntiAlias | PaintFlags.LinearText);
				Rect r = new Rect ();
				c.GetClipBounds (r);
				int cHeight = r.Height();
				int cWidth = r.Width();
				//p.GetTextBounds (text, 0, text.Length, r);		

				p.Color = Color.Rgb(73, 55, 109);
				p.SetStyle (Paint.Style.Fill);
				c.DrawPaint (p);


			 	p.TextSize = 16 * Resources.System.DisplayMetrics.Density;
				p.SetTypeface (Typeface.SansSerif);
				p.FakeBoldText = true;
				RectF bounds = new RectF(r);
				// measure text width
				bounds.Right = p.MeasureText(text, 0, text.Length);
				// measure text height
				bounds.Bottom = p.Descent() - p.Ascent();

				bounds.Left += ((float)r.Width() - bounds.Right) / 2.0f;
				bounds.Top += ((float)r.Height() - bounds.Bottom) / 2.0f;

				p.Color = Color.White;
				c.DrawText(text, bounds.Left, bounds.Top - p.Ascent(), p);


				var stream = new FileStream (filename, FileMode.Create);
				//bring it back down to size
				//Bitmap resizedImage = Bitmap.CreateScaledBitmap(b, (int)width, (int)height, true);
				//resizedImage.Compress (Bitmap.CompressFormat.Png, 100, stream);
				b.Compress (Bitmap.CompressFormat.Png, 100, stream);
				stream.Close ();
			//}

			//return image.AsPNG ().ToArray ();
			return filename;




		}
	}
}

