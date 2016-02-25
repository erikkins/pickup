using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class TestView : ContentPage
	{
		public TestView ()
		{
			InitializeComponent ();
			this.ViewModel = new TestViewModel (App.client);


			Button b = new Button ();
			b.Text = "SetValidation";
			b.Clicked += async delegate(object sender, EventArgs e) {
				await ViewModel.TestValidation().ConfigureAwait(false);
			};

			stacker.Children.Add (b);

			Button b2 = new Button ();
			b2.Text = "SaveCircle";
			b2.Clicked += async delegate(object sender, EventArgs e) {
				await ViewModel.TestCircle().ConfigureAwait(false);
			};

			stacker.Children.Add (b2);

		}

		protected TestViewModel ViewModel
		{
			get { return this.BindingContext as TestViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

