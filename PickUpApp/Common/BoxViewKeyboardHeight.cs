using System;
using Xamarin.Forms;
namespace PickUpApp
{
	public class BoxViewKeyboardHeight : BoxView
	{
		public event EventHandler BoxChanged;

		public virtual void OnBoxChanged()
		{
			if (BoxChanged != null) {
				this.BoxChanged (this, EventArgs.Empty);
			}
		}

		protected override void OnPropertyChanging (string propertyName)
		{
			base.OnPropertyChanging (propertyName);
			if (propertyName == "Height") {
				OnBoxChanged ();
			}
		}
	}
}

