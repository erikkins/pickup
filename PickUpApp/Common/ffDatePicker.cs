using System;
using Xamarin.Forms;

namespace PickUpApp
{
	public class FFDatePicker : DatePicker
	{
		public static readonly BindableProperty TextColorProperty = 
			BindableProperty.Create ("TextColor", typeof(Color), typeof(FFDatePicker), Color.Default);

		public Color TextColor {
			get { return (Color)GetValue (TextColorProperty); }
			set { SetValue (TextColorProperty, value); }
		}

		public static readonly BindableProperty HasBorderProperty = 
			BindableProperty.Create ("HasBorder", typeof(bool), typeof(FFDatePicker), true);

		public bool HasBorder {
			get { return (bool)GetValue (HasBorderProperty); }
			set { SetValue (HasBorderProperty, value); }
		}
	}
}

