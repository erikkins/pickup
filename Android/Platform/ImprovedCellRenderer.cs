﻿using System;
using PickUpApp.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Android.Content;

[assembly: ExportRenderer(typeof(TextCell), typeof(ImprovedTextCellRenderer))]

namespace PickUpApp.Android
{
	public class ImprovedTextCellRenderer : TextCellRenderer
	{
		protected override global::Android.Views.View GetCellCore(Cell item, global::Android.Views.View convertView, ViewGroup parent, Context context)
		{
			var view = base.GetCellCore(item, convertView, parent, context) as ViewGroup;
			if (String.IsNullOrEmpty((item as TextCell).Text)) {
				view.Visibility = ViewStates.Gone;
				while (view.ChildCount > 0)
					view.RemoveViewAt(0);
				view.SetMinimumHeight(0);
				view.SetPadding(0, 0, 0, 0);
			}
			return view;
		}
	}
}
