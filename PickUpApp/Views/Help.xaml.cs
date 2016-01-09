using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XLabs.Forms;

namespace PickUpApp
{
	public partial class Help : ContentPage
	{
		public Help ()
		{
			InitializeComponent ();
			this.ViewModel = new FeedbackViewModel (App.client);
			this.BackgroundColor = AppColor.AppGray;
			submitter.Clicked += async delegate(object sender, EventArgs e) {
				ViewModel.CurrentFeedback.Content = contenteditor.Text;
				await ViewModel.ExecuteAddEditCommand();
			};

			XLabs.Forms.Controls.WrapLayout wrapper = new XLabs.Forms.Controls.WrapLayout ();
			wrapper.Orientation = StackOrientation.Horizontal;
			wrapper.Spacing = 0;
			wrapper.HorizontalOptions = LayoutOptions.Center;

			XLabs.Forms.Controls.HyperLinkLabel hll = new XLabs.Forms.Controls.HyperLinkLabel ();
			hll.TextColor = AppColor.AppPurple;
			hll.NavigateUri = "http://www.famfetch.com/how-to";
			hll.Text = "here";



			Label l = new Label ();
			l.Text = "For online help, please click ";
			l.TextColor = AppColor.AppPink;
			wrapper.Children.Add (l);
			wrapper.Children.Add (hll);
			stacker.Children.Add (wrapper);


			MessagingCenter.Subscribe<Feedback> (this, "FeedbackReceived", (f) => {
				if (contenteditor.Text.Length > 0)
				{
					contenteditor.Text = "";
					ViewModel.CurrentFeedback = new Feedback();
					App.hudder.showToast("Thank you!  Your feedback has been received!");
				}
			});

		}


		protected FeedbackViewModel ViewModel
		{
			get { return this.BindingContext as FeedbackViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

