﻿using System;
using Xamarin.Forms;

namespace PickUpApp
{
	public class FFTimePicker : TimePicker
	{
		public static readonly BindableProperty TextColorProperty = 
			BindableProperty.Create ("TextColor", typeof(Color), typeof(FFTimePicker), Color.Default);

		public Color TextColor {
			get { return (Color)GetValue (TextColorProperty); }
			set { SetValue (TextColorProperty, value); }
		}

		public static readonly BindableProperty HasBorderProperty = 
			BindableProperty.Create ("HasBorder", typeof(bool), typeof(FFTimePicker), true);

		public bool HasBorder {
			get { return (bool)GetValue (HasBorderProperty); }
			set { SetValue (HasBorderProperty, value); }
		}
	}
}

