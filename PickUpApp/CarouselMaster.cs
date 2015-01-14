using System;
using Xamarin.Forms;

namespace PickUpApp
{
	public class CarouselMaster : CarouselPage
	{
		public CarouselMaster ()
		{
			this.Children.Add (new Today ());
			this.Children.Add (new MyCircle ());
			this.Children.Add (new MyKids ());
			//InitializeComponent ();
		}
	}
}

