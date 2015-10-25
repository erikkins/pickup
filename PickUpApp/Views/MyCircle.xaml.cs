using System;
using System.Collections.Generic;
using Xamarin.Forms;
//using Xamarin.Contacts;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace PickUpApp
{	
	public partial class MyCircle : ContentPage
	{	
		public MyCircle ()
		{
			InitializeComponent ();
			this.ViewModel = new MyCircleViewModel (App.client);
			this.ToolbarItems.Add (new ToolbarItem ("Add Contact", "icn_new.png", async() => {
				await Navigation.PushAsync(new SelectContact());
			}));

			this.BackgroundColor = Color.FromRgb (238, 236, 243);
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);	
			MessagingCenter.Subscribe<LocalContact> (this, "ContactAdded", (s) => {
				Navigation.PopModalAsync ();
				ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
				ViewModel.Refresh();
			});
		}


		public void OnDelete (object sender, EventArgs e) {
			var mi = ((MenuItem)sender);
			Account a = (Account)mi.CommandParameter;
			DisplayAlert("Delete Context Action", a.id + " delete context action", "OK");
		}

		protected MyCircleViewModel ViewModel
		{
			get { return this.BindingContext as MyCircleViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

