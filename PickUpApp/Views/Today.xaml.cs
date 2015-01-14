using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using PickUpApp.ViewModels;

namespace PickUpApp
{	
	public partial class Today 
	{
		public Today ()
		{
			InitializeComponent ();

			this.ViewModel = new TodayViewModel(App.client);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			lstAccount.ItemSelected += lstAccount_ItemSelected;

			//can I get the list?
			//Account acct = PickupService.DefaultService.GetAccount ();

			//PickupService.LoadAccount ();

			//Task<System.Collections.ObjectModel.ObservableCollection<Account>> loaded = PickupService.LoadAccount ();
			//ViewModels.CurrentAccount ca = new PickUpApp.ViewModels.CurrentAccount ();
			//ca.Accounts = loaded.Result;

			//System.Diagnostics.Debug.WriteLine (acct.Email);
		}

		protected TodayViewModel ViewModel
		{
			get { return this.BindingContext as TodayViewModel; }
			set { this.BindingContext = value; }
		}

		void lstAccount_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			//Navigation.PushAsync(new RebatesView(e.SelectedItem as Store));
			lstAccount.SelectedItem = null;
		}

			
//		public List<Account> getAccount()
//		{
//			List<Account> theList = new List<Account> ();
//			theList.Add (PickupService.DefaultService.GetAccount ().Result);
//			return theList;
//
//		}

	}
}


