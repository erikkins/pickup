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

[assembly: ExportRenderer(typeof(FFSpline), typeof(FFSplineRenderer))]
namespace PickUpApp.iOS
{
	public class FFSplineRenderer : VisualElementRenderer<FFSpline>
	{
		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == FFSpline.ColorProperty.PropertyName)
				this.SetNeedsDisplay(); // Force a call to Draw
		}


		public override void Draw (CGRect rect)
		{
			using (var context = UIGraphics.GetCurrentContext())
			{
//				var path = CGPath.EllipseFromRect(rect);
//				context.AddPath(path);
//				context.SetFillColor(this.Element.Color.ToCGColor());
//				context.DrawPath(CGPathDrawingMode.Stroke);


				context.SetLineWidth (2);
				context.SetLineDash (0, new nfloat[]{ 5, 4 });
				this.Element.Color.ToUIColor ().SetStroke ();



				//var path = new CGPath ();
				var path = new UIBezierPath();
				CGPoint sp = new CGPoint (this.Element.StartPoint.X, this.Element.StartPoint.Y);
				CGPoint ep = new CGPoint (this.Element.EndPoint.X, this.Element.EndPoint.Y);

				CGPoint m1, m2;
				//if startpoint is above endpoint, initial curve is up!
				nfloat quarter = ep.X / 4;
				nfloat diff = ep.Y - sp.Y;

				//if the startpoint doesn't start at zero, we really just want to make this an upwards or downwards curve
				if (sp.X > 0) {
					quarter = (ep.X - sp.X) / 4;
					//we're actually just doing a simple arc, so make the control points the same
					if (sp.Y < ep.Y) {
						diff = (ep.Y - sp.Y)/2;
						m1 = new CGPoint (sp.X + quarter, ep.Y - diff);
						m2 = new CGPoint (sp.X + quarter * 3, ep.Y - diff);
					} else {
						diff = (sp.Y - ep.Y)/2;
						m1 = new CGPoint (sp.X + quarter, ep.Y + diff);
						m2 = new CGPoint (sp.X + quarter * 3, ep.Y + diff/4);
					}
				} else {
					if (sp.Y < ep.Y) {
						m1 = new CGPoint (quarter, sp.Y - diff);
						m2 = new CGPoint (quarter * 3, ep.Y + diff);
					} else {
						m1 = new CGPoint (quarter, sp.Y + diff);
						m2 = new CGPoint (quarter * 3, ep.Y - diff);
					}
				}

				path.MoveTo (sp);
				path.AddCurveToPoint (ep, m1, m2);

				//path.AddLines ((CGPoint[])this.Element.Points);
//				path.AddLines (new CGPoint[]{
//					new CGPoint (100, 200),
//					new CGPoint (160, 100), 
//					new CGPoint (220, 200)});

				//path.CloseSubpath ();
			
				//add geometry to graphics context and draw it
				context.AddPath (path.CGPath);       
				context.DrawPath (CGPathDrawingMode.Stroke);

			}

		}

	}
}

