using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PickUpApp
{
	public partial class Confirmation : ContentPage
	{
		public Confirmation (Invite invite)
		{
			InitializeComponent ();
			this.ViewModel = new ConfirmationViewModel (App.client, invite);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			MessagingCenter.Subscribe<InviteInfo> (this, "confirmationloaded", (s) => {
				stacker.Children.Clear();
				//until we track down WHY this was getting called a second time with a null invite
				if (ViewModel.ThisInvite == null || ViewModel.ThisInvite.Kids == null)
				{
					return;
				}
				try{
				TableView tv = new TableView ();
				tv.HasUnevenRows = true;
				TableSection ts = new TableSection ();
				ts.Add (new ActivityCell (ViewModel.ThisInvite.Kids, ViewModel.ThisInvite.AccountID));



				string infoString = "";

				if (ViewModel.ThisInvite.Kids.IndexOf (',') > -1) {
					//more than 1 kid
					infoString = "have ";
				} else {
					//just one kid
					infoString = "has ";
				}


				//string timeonly = string.Format("{0}:{1}", ViewModel.CurrentInviteInfo.CompleteAtWhen.ToLocalTime().Hour, ViewModel.CurrentInviteInfo.CompleteAtWhen.ToLocalTime().Minute.ToString().PadLeft(2,'0'), ViewModel.CurrentInviteInfo.CompleteAtWhen.ToLocalTime());
				string timeampm = string.Format("{0:hh:mm tt}", ViewModel.ThisInvite.CompleteAtWhen.ToLocalTime());

				infoString += "been Picked Up from " + ViewModel.ThisInvite.Activity;// + " at " + timeampm;
				string timeString = "at " + timeampm;
//				Label l = new Label ();
//				l.Text = infoString;
//				l.FontSize = 24;
//				stacker.Children.Add (l);

				TextCell tc = new TextCell();
				tc.Height = 100;
				tc.TextColor = Device.OnPlatform(Color.Black, Color.FromRgb(211,211,211), Color.Black);
				tc.Text = infoString;
				tc.Detail = timeString;
				ts.Add(tc);

				tv.Root.Add (ts);
				stacker.Children.Add (tv);

				Button btnOK = new Button ();
				btnOK.Text = "Dismiss";
				btnOK.TextColor = Color.Black;
				btnOK.FontSize = 18;
				btnOK.BackgroundColor = Color.Green;
				btnOK.VerticalOptions = LayoutOptions.End;
				btnOK.Clicked += async delegate(object sender, EventArgs e) {
					MessagingCenter.Send<string>("Confirmation", "NeedsRefresh");
					await Navigation.PopModalAsync();
				};
				stacker.Children.Add (btnOK);

				}
				catch(Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}
			});






		}

		protected ConfirmationViewModel ViewModel
		{
			get { return this.BindingContext as ConfirmationViewModel; }
			set { this.BindingContext = value; }
		}
		void OnDismissClicked(object sender, EventArgs args)
		{
			Navigation.PopModalAsync ();
		}
	}
}

