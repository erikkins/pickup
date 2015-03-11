using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace PickUpApp
{
	public class MyCircleViewModel:BaseViewModel
	{
		public ObservableCollection<Account> Circle 
		{ get{ return App.myCircle; } 
			set{ if (value == App.myCircle) {
					App.myCircle = value;
					}
				} 
		}
	
		public MyCircleViewModel ()
		{
			App.myCircle = new ObservableCollection<Account> ();
		}

		public MyCircleViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
			LoadItemsCommand.Execute (null);
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try
			{
				var circle = await client.InvokeApiAsync<List<Account>>("getmycircle");
				App.myCircle.Clear();
				foreach (var acct in circle)
				{
					App.myCircle.Add(acct);
				}

			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Circle. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
			IsLoading = false;
		}
	}
}

