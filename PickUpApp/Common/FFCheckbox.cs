using System;
using Xamarin.Forms;

namespace PickUpApp
{
	public class FFCheckbox : ContentView
	{
		private Image ToggleImage;

		public FFCheckbox()
		{
			UnCheckedImage = "ui_check_empty.png";
			CheckedImage = "ui_check_filled.png";
			this.HeightRequest = 27;
			this.WidthRequest= 27;
			this.HorizontalOptions = LayoutOptions.Center;
			this.VerticalOptions = LayoutOptions.Center;

			ToggleImage = new Image
			{
				Source = UnCheckedImage
			};

			this.Content = ToggleImage;

			this.GestureRecognizers.Add(new TapGestureRecognizer(TappedCallback));
		}
		protected override void OnBindingContextChanged ()
		{
			base.OnBindingContextChanged ();
		}
		protected override void OnPropertyChanged (string propertyName)
		{
			if (propertyName == "Checked") {
				//set the damn thing
				if (Checked) {
					ToggleImage.Source = CheckedImage;
				} else 
				{
					ToggleImage.Source = UnCheckedImage;
				}
			}

			base.OnPropertyChanged (propertyName);
		}
		private void TappedCallback(View view)
		{
			if (ToggleImage.Source == UnCheckedImage)
			{
				ToggleImage.Source = CheckedImage;
				Checked = true;
			}
			else
			{
				ToggleImage.Source = UnCheckedImage;
				Checked = false;
			}
		}

		public static readonly BindableProperty CheckedProperty =
			BindableProperty.Create<FFCheckbox, bool>(p => p.Checked, false);

		public bool Checked
		{
			get { return (bool)base.GetValue(CheckedProperty); }
			set 
			{ 
				base.SetValue(CheckedProperty, value); 
			}
		}

		public static readonly BindableProperty CheckedImageProperty =
			BindableProperty.Create<FFCheckbox, ImageSource>(p => p.CheckedImage, null);

		public ImageSource CheckedImage
		{
			get { return (ImageSource)base.GetValue(CheckedImageProperty); }
			set { base.SetValue(CheckedImageProperty, value); }
		}

		public static readonly BindableProperty UnCheckedImageProperty =
			BindableProperty.Create<FFCheckbox, ImageSource>(p => p.UnCheckedImage, null);

		public ImageSource UnCheckedImage
		{
			get { return (ImageSource)base.GetValue(UnCheckedImageProperty); }
			set { base.SetValue(UnCheckedImageProperty, value); }
		}
	}
}

