using System;
using UIKit;
using System.Drawing;
using CoreGraphics;
using Foundation;
using PickUpApp.iOS;
using System.IO;
using PickUpApp;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FFArrow), typeof(FFArrowRenderer))]
namespace PickUpApp.iOS
{
	public class FFArrowRenderer : VisualElementRenderer<FFArrow>
	{
		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == FFArrow.ColorProperty.PropertyName)
				this.SetNeedsDisplay(); // Force a call to Draw
		}


		public override void Draw (CGRect rect)
		{
			using (var context = UIGraphics.GetCurrentContext())
			{

				context.SetLineWidth (1);
				//context.SetLineDash (0, new nfloat[]{ 5, 4 });
				this.Element.Color.ToUIColor ().SetStroke ();

				//System.Drawing.Color theColor = System.Drawing.Color.FromArgb ((int)Element.Color.A, (int)Element.Color.R, (int)Element.Color.G, (int)Element.Color.B);

				CGPoint starter = new CGPoint (Element.StartPoint.X, Element.StartPoint.Y);
				CGPoint ender = new CGPoint (Element.EndPoint.X, Element.EndPoint.Y);
			
				context.SetStrokeColor (Element.Color.ToCGColor());
				context.SetFillColor (Element.Color.ToCGColor ());

				//add geometry to graphics context and draw it
				context.AddPath (pathWithArrowFromPoint(ender, starter, 1, 10, 16));       
				context.DrawPath (CGPathDrawingMode.FillStroke);
			}

		}
		public static CGPath pathWithArrowFromPoint(
			CGPoint startPoint,
			CGPoint endPoint,
			float tailWidth,
			float headWidth,
			float headLength)
		{
			var dx = endPoint.X - startPoint.X;
			var dy = endPoint.Y - startPoint.Y;
			var length = (float)Math.Sqrt(dx*dx + dy*dy);
			var points = getAxisAlignedArrowPoints(length, tailWidth, headWidth, headLength);
			var transform = transformForStartPoint(startPoint, endPoint, length);

			var path = new CGPath();
			path.AddLines(transform, points);
			path.CloseSubpath();

			return path;
		}

		static CGPoint[] getAxisAlignedArrowPoints(
			float length,
			float tailWidth, 
			float headWidth, 
			float headLength)
		{
			var tailLength = length - headLength;
			var points = new CGPoint[7];

			points[0] = new CGPoint(0, tailWidth / 2);
			points[1] = new CGPoint(tailLength, tailWidth / 2);
			points[2] = new CGPoint(tailLength, headWidth / 2);
			points[3] = new CGPoint(length, 0);
			points[4] = new CGPoint(tailLength, -headWidth / 2);
			points[5] = new CGPoint(tailLength, -tailWidth / 2);
			points[6] = new CGPoint(0, -tailWidth / 2);

			return points;
		}

		static CGAffineTransform transformForStartPoint(CGPoint startPoint, CGPoint endPoint, float length)
		{
			var cosine = (endPoint.X - startPoint.X) / length;
			var sine = (endPoint.Y - startPoint.Y) / length;
			return new CGAffineTransform(cosine, sine, -sine, cosine, startPoint.X, startPoint.Y);
		}
	}
}