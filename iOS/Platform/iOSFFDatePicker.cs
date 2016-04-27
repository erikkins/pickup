using PickUpApp;
using PickUpApp.iOS;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRendererAttribute(typeof(FFDatePicker), typeof(FFDatePickerRenderer))]
namespace PickUpApp.iOS
{
	public class FFDatePickerRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged (e);

			FFDatePicker datePicker = (FFDatePicker)Element;

			if (datePicker != null) {
				SetBorderStyle (datePicker);
				SetTextColor (datePicker);
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

			FFDatePicker datePicker = (FFDatePicker)Element;

			if (e.PropertyName == FFDatePicker.TextColorProperty.PropertyName) {
				this.Control.TextColor = datePicker.TextColor.ToUIColor ();
			}
		}

		void SetBorderStyle (FFDatePicker datePicker)
		{
			this.Control.BorderStyle = (datePicker.HasBorder == true) ? UIKit.UITextBorderStyle.RoundedRect : UIKit.UITextBorderStyle.None;
		}

		void SetTextColor (FFDatePicker datePicker)
		{
			this.Control.TextColor = datePicker.TextColor.ToUIColor ();
		}
	}
}
