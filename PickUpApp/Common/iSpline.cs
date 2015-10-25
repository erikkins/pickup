using System;

using Xamarin.Forms;
using System.Linq;

namespace PickUpApp
{
	public class FFSpline : View
	{
		public static readonly BindableProperty ColorProperty =
			BindableProperty.Create<FFSpline, Color>(p => p.Color, Color.Accent);

		public static readonly BindableProperty StartPointProperty = BindableProperty.Create<FFSpline, Point>(p => p.StartPoint, new Point(0,0));
		public static readonly BindableProperty EndPointProperty = BindableProperty.Create<FFSpline, Point>(p => p.EndPoint, new Point(100,100));

		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		public Point StartPoint
		{
			get{ return (Point)GetValue (StartPointProperty); }
			set { SetValue (StartPointProperty, value); }
		}

		public Point EndPoint
		{
			get{ return (Point)GetValue (EndPointProperty); }
			set { SetValue (EndPointProperty, value); }
		}
			
	}
}
