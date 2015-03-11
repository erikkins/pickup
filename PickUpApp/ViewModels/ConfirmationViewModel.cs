using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace PickUpApp
{
	public class ConfirmationViewModel : BaseViewModel
	{
		public ConfirmationViewModel ()
		{
		}
		public ConfirmationViewModel(MobileServiceClient client, Invite invite) : this()
		{
			this.client = client;
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			try
			{
				//var confirmData = await client.GetTable<Schedule>().ToListAsync();
				//				var kids = await client.GetTable<Kid>().ToListAsync();
				//
				//				App.myKids.Clear();
				//				foreach (var kid in kids)
				//				{
				//					App.myKids.Add(kid);
				//				}

			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Kids. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
		}
	}
}

