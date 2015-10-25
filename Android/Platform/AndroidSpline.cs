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
				AntiAlias = true
			};

			canvas.DrawOval(new RectF(rect), paint);
		}
	}
}

