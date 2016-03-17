using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class ManageFetch : ContentPage
	{
		public ManageFetch (Today CurrentToday)
		{
			InitializeComponent ();
			this.ViewModel = new MessageViewModel (App.client,null);
			this.Padding = new Thickness(0, Device.OnPlatform(0, 0, 0), 0, 0);
			this.BackgroundColor = AppColor.AppGray;

			Button btnCancel = new Button ();
			btnCancel.VerticalOptions = LayoutOptions.CenterAndExpand;
			btnCancel.HorizontalOptions = LayoutOptions.CenterAndExpand;
			btnCancel.HeightRequest = 50;
			btnCancel.WidthRequest = (App.ScaledWidth) - 50;
			btnCancel.FontAttributes = FontAttributes.Bold;
			btnCancel.FontSize = 18;
			btnCancel.BorderRadius = 8;
			btnCancel.BackgroundColor = Color.FromRgb (73, 55, 109);
			btnCancel.TextColor = Color.FromRgb (84, 210, 159);
			btnCancel.Text = "Cancel Fetch";
			stacker.Children.Add (btnCancel);


			MessagingCenter.Subscribe<Today> (this, "fetchcanceled", async(ct) => {
				//I guess pop this and refresh today!
				App.hudder.hideHUD();
				try{
				await Navigation.PopToRootAsync();
				}
				catch(Exception ex)
				{
					//why are these excepting??
				}
				MessagingCenter.Send<string>("cancelfetch", "NeedsRefresh");
			});

			btnCancel.Clicked += delegate(object sender, EventArgs e) {
				App.hudder.showHUD("Canceling Fetch");
				//await ((RouteDetail)this.ParentView.Parent.Parent).DisplayAlert ("DONE!", "Complete", "Cancel");
				//MessagingCenter.Send<Today>(CurrentToday, "cancelfetch");
				ViewModel.ExecuteCancelCommand(CurrentToday).ConfigureAwait(false);
			};
		}

		protected MessageViewModel ViewModel
		{
			get { return this.BindingContext as MessageViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

