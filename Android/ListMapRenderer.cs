using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using PickUpApp.droid;
using PickUpApp;

[assembly: ExportRenderer (typeof (ListMap), typeof (ListMapRenderer))]
namespace PickUpApp.droid
{
	public class ListMapRenderer : MapRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged (e);
			//Control.Focusable = false;
			//Control.Clickable = true;
			Control.Focusable = false;
			//e.NewElement.Unfocus ();
			//((ViewCell)Control.Parent).View.ShouldBeMadeClickable ();
			//Control.Click += Control_Click;
			//((ListView)Control.Parent).
		}

		void Control_Click (object sender, EventArgs e)
		{
			((ViewCell)((Android.Views.View)sender).Parent).View.ShouldBeMadeClickable ();

		}
	}
}

