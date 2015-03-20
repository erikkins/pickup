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
			btnContacts.Clicked += HandleClicked; 
			//Debug.WriteLine (lstCircle.Id.ToString ());
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			MessagingCenter.Subscribe<LocalContact> (this, "ContactAdded", (s) => {
				Navigation.PopModalAsync ();
				ViewModel.ExecuteLoadItemsCommand().ConfigureAwait(false);
				ViewModel.Refresh();
			});
		}


		void HandleClicked (object sender, EventArgs e)
		{

			//launch the contacts page modally
			Navigation.PushModalAsync (new SelectContact ());

			//change the observablecollection
			//ViewModel.search ("k");
			//lstContacts.ItemsSource = ViewModel.ContactsSorted;

			//var contacts = DependencyService.Get<iAddressBook> ().testIt ().Result;
			//Debug.WriteLine (contacts);
			/*
			var book = new AddressBook (null);
			book.RequestPermission().ContinueWith (t => {
				if (!t.Result) {
					Debug.WriteLine("Permission denied by user or manifest");
					return;
				}

				foreach (Contact contact in book.OrderBy (c => c.LastName)) {
					Debug.WriteLine ("{0} {1}", contact.FirstName, contact.LastName);
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
			*/
		}

		protected MyCircleViewModel ViewModel
		{
			get { return this.BindingContext as MyCircleViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

