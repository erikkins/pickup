using PickUpApp;
using PickUpApp.iOS;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRendererAttribute(typeof(FFTimePicker), typeof(FFTimePickerRenderer))]
namespace PickUpApp.iOS
{
	public class FFTimePickerRenderer : TimePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged (e);

			FFTimePicker timePicker = (FFTimePicker)Element;

			if (timePicker != null) {
				SetBorderStyle (timePicker);
				SetTextColor (timePicker);
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

			FFTimePicker timePicker = (FFTimePicker)Element;

			if (e.PropertyName == FFTimePicker.TextColorProperty.PropertyName) {
				this.Control.TextColor = timePicker.TextColor.ToUIColor ();
			}
		}

		void SetBorderStyle (FFTimePicker timePicker)
		{
			this.Control.BorderStyle = (timePicker.HasBorder == true) ? UIKit.UITextBorderStyle.RoundedRect : UIKit.UITextBorderStyle.None;
		}

		void SetTextColor (FFTimePicker timePicker)
		{
			this.Control.TextColor = timePicker.TextColor.ToUIColor ();
		}
	}
}
