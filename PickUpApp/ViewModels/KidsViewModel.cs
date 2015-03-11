using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace PickUpApp
{
	public class KidsViewModel: BaseViewModel
	{
		public ObservableCollection<Kid> Kids 
		{ get{ return App.myKids; } 
			set{
				if (value != App.myKids) {
					App.myKids = value;
				}
			} 
		}
		public KidsViewModel ()
		{
			App.myKids = new ObservableCollection<Kid> ();
			//Kids = new ObservableCollection<Kid> ();
		}



		public KidsViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
			LoadItemsCommand.Execute (null);

		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			try
			{
				var kids = await client.GetTable<Kid>().ToListAsync();

				App.myKids.Clear();
				foreach (var kid in kids)
				{
					App.myKids.Add(kid);
				}

			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Kids. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
		}
		/*
		public override async Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				var kid = client.GetTable<Kid>();
				MessagingCenter.Send(App.myAccount, "Refresh");
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}

			IsLoading = false;
		}
		*/

	}
}

