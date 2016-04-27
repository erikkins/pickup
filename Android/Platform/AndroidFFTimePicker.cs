using PickUpApp.droid;
using PickUpApp;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(FFTimePicker), typeof(FFTimePickerRenderer))]
namespace PickUpApp.droid
{
	class FFTimePickerRenderer : TimePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			FFTimePicker timePicker = (FFTimePicker)Element;

			if (timePicker != null)
			{
				SetTextColor(timePicker);
			}

			if (e.OldElement == null)
			{
				//Wire events
			}

			if (e.NewElement == null)
			{
				//Unwire events
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control == null)
			{
				return;
			}

			FFTimePicker timePicker = (FFTimePicker)Element;

			if (e.PropertyName == FFDatePicker.TextColorProperty.PropertyName)
			{
				this.Control.SetTextColor(timePicker.TextColor.ToAndroid());
			}
		}

		void SetTextColor(FFTimePicker timePicker)
		{
			this.Control.SetTextColor(timePicker.TextColor.ToAndroid());
		}
	}
}
