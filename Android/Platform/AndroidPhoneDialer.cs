using System;
using PickUpApp.droid;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Android.Net;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidPhoneDialer))]
namespace PickUpApp.droid
{
	public class AndroidPhoneDialer : IPhoneDialer
	{
		public AndroidPhoneDialer(){
		}

		public void test()
		{
			DialPhone ("+17736191320");
		}

		public void DialPhone (string phoneNumber)
		{
			//var url = new Uri ("tel:" + phoneNumber);
			var uri = Android.Net.Uri.Parse ("tel: " + phoneNumber);


			var intent = new Intent (Intent.ActionView, uri); 
			global::Xamarin.Forms.Forms.Context.StartActivity (intent);  

		}
	}
}

