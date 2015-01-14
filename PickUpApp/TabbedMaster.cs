using System;
using Xamarin.Forms;

namespace PickUpApp
{
	public class TabbedMaster:TabbedPage
	{
		public TabbedMaster ()
		{
			this.Children.Add (new Today ());
			this.Children.Add (new MyCircle ());
			this.Children.Add (new MyKids ());
			this.Children.Add (new MySchedule ());

		}
	}
}

