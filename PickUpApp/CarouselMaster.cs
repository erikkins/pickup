using System;
using Xamarin.Forms;

namespace PickUpApp
{
	public class CarouselMaster : CarouselPage
	{
		public CarouselMaster ()
		{
			this.Children.Add (new Intro1());
			this.Children.Add (new Intro2 ());
			this.Children.Add (new Intro3 ());
			this.Children.Add (new Intro4 ());
			this.Children.Add (new Intro5 ());
			this.Children.Add (new Intro6 ());
			//InitializeComponent ();
		}
	}
}

