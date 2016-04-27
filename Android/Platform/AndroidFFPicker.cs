using PickUpApp.droid;
using PickUpApp;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(FFPicker), typeof(FFPickerRenderer))]
namespace PickUpApp.droid
{
	class FFPickerRenderer : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			FFPicker picker = (FFPicker)Element;

			if (picker != null)
			{
				SetTextColor(picker);
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

			FFPicker picker = (FFPicker)Element;

			if (e.PropertyName == FFPicker.TextColorProperty.PropertyName)
			{
				this.Control.SetTextColor(picker.TextColor.ToAndroid());
			}
		}

		void SetTextColor(FFPicker picker)
		{
			this.Control.SetTextColor(picker.TextColor.ToAndroid());
		}
	}
}
