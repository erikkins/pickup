using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using PickUpApp;
using Android.Graphics;
using PickUpApp.droid;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(FFSpline), typeof(FFSplineRenderer))]
namespace PickUpApp.droid
{
	public class FFSplineRenderer : VisualElementRenderer<FFSpline>
	{
		public FFSplineRenderer()
		{
			this.SetWillNotDraw (false);
		}
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == FFSpline.ColorProperty.PropertyName)
				this.Invalidate(); // Force a call to OnDraw
		}

		protected override void OnDraw(Canvas canvas)
		{
			//TODO: THIS IS THE DEFAULT...NEED TO MATCH THE iOS VERSION
			var element = this.Element;
			var rect = new Rect();
			this.GetDrawingRect(rect);

			var paint = new Paint()
			{
				Color = element.Color.ToAndroid(),
				AntiAlias = true,
				StrokeWidth = 2
			};
			paint.SetStyle (Paint.Style.Stroke);

			//canvas.DrawOval(new RectF(rect), paint);


			Path path = new Path();
	
			Android.Graphics.Point sp = new Android.Graphics.Point ((int)this.Element.StartPoint.X, (int)this.Element.StartPoint.Y);
			Android.Graphics.Point ep = new Android.Graphics.Point ((int)this.Element.EndPoint.X, (int)this.Element.EndPoint.Y);

			Android.Graphics.Point m1 = new Android.Graphics.Point ();
			Android.Graphics.Point m2 = new Android.Graphics.Point ();
			//if startpoint is above endpoint, initial curve is up!
			float quarter = ep.X / 4;
			float diff = ep.Y - sp.Y;

			//if the startpoint doesn't start at zero, we really just want to make this an upwards or downwards curve
			if (sp.X > 0) {
				quarter = (ep.X - sp.X) / 4;
				//we're actually just doing a simple arc, so make the control points the same
				if (sp.Y < ep.Y) {
					diff = (ep.Y - sp.Y)/2;
	
					m1 = new Android.Graphics.Point ((int)(sp.X + quarter), (int)(ep.Y - diff));
					m2 = new Android.Graphics.Point ((int)(sp.X + quarter * 3), (int)(ep.Y - diff));
				} else {
					diff = (sp.Y - ep.Y)/2;
					m1 = new Android.Graphics.Point ((int)(sp.X + quarter), (int)(ep.Y + diff));
					m2 = new Android.Graphics.Point ((int)(sp.X + quarter * 3), (int)(ep.Y + diff/4));
				}
			} else {
				if (sp.Y < ep.Y) {
					m1 = new Android.Graphics.Point ((int)quarter, (int)(sp.Y - diff));
					m2 = new Android.Graphics.Point ((int)(quarter * 3), (int)(ep.Y + diff));
				} else {
					m1 = new Android.Graphics.Point ((int)quarter, (int)(sp.Y + diff));
					m2 = new Android.Graphics.Point ((int)(quarter * 3), (int)(ep.Y - diff));
				}
			}

			float mult = Resources.DisplayMetrics.ScaledDensity;

			path.MoveTo (sp.X * mult, sp.Y * mult);
			path.CubicTo (m1.X * mult, m1.Y * mult, m2.X * mult, m2.Y * mult, ep.X * mult, ep.Y * mult);

			canvas.DrawPath(path, paint);

		}
	}
}

