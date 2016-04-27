using PickUpApp;
using PickUpApp.iOS;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRendererAttribute(typeof(FFPicker), typeof(FFPickerRenderer))]
namespace PickUpApp.iOS
{
	public class FFPickerRenderer : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged (e);

			FFPicker picker = (FFPicker)Element;

			if (picker != null) {
				SetBorderStyle (picker);
				SetTextColor (picker);
			}

			if (e.OldElement == null) {
				//Wire events
			}

			if (e.NewElement == null) {
				//Unwire events
			}
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (Control == null) {
				return;
			}

			FFPicker picker = (FFPicker)Element;

			if (e.PropertyName == FFPicker.TextColorProperty.PropertyName) {
				this.Control.TextColor = picker.TextColor.ToUIColor ();
			}
		}

		void SetBorderStyle (FFPicker picker)
		{
			this.Control.BorderStyle = (picker.HasBorder == true) ? UIKit.UITextBorderStyle.RoundedRect : UIKit.UITextBorderStyle.None;
		}

		void SetTextColor (FFPicker picker)
		{
			this.Control.TextColor = picker.TextColor.ToUIColor ();
		}
	}
}
