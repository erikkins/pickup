using PickUpApp.droid;
using PickUpApp;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(FFDatePicker), typeof(FFDatePickerRenderer))]
namespace PickUpApp.droid
{
	class FFDatePickerRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			FFDatePicker datePicker = (FFDatePicker)Element;

			if (datePicker != null)
			{
				SetTextColor(datePicker);
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

			FFDatePicker datePicker = (FFDatePicker)Element;

			if (e.PropertyName == FFDatePicker.TextColorProperty.PropertyName)
			{
				this.Control.SetTextColor(datePicker.TextColor.ToAndroid());
			}
		}

		void SetTextColor(FFDatePicker datePicker)
		{
			this.Control.SetTextColor(datePicker.TextColor.ToAndroid());
		}
	}
}
