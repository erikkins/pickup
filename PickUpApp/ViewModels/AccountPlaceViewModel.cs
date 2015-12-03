using System;
using PickUpApp.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace PickUpApp
{
	public class AccountPlaceViewModel:BaseViewModel
	{
		private AccountPlace _currentPlace;
		public AccountPlace CurrentPlace
		{
			get { return this._currentPlace; }
			set
			{
				this._currentPlace = value;
				NotifyPropertyChanged();
			}
		}

		public ObservableCollection<AccountPlace> MyPlaces
		{
			get{
				return App.myPlaces;
			}
			set{
				if (value != App.myPlaces) {
					App.myPlaces = value;
				}
			} 
		}

		public AccountPlaceViewModel()
		{
			
		}

		public AccountPlaceViewModel (MobileServiceClient client)
		{
			this.client = client;

		}
			

		public override async Task ExecuteLoadItemsCommand ()
		{
			try
			{
				IsLoading = true;
				var places = await client.GetTable<AccountPlace>().OrderBy(x => x.PlaceName).ToListAsync();

				App.myPlaces.Clear();
				foreach (var place in places)
				{
					App.myPlaces.Add(place);
				}

				FFMenuItem placesItem = new FFMenuItem("Places", App.myPlaces.Count);
				//if( ! employee.TypeOfWorks.Any(tow => tow.Id == theNewGUID) )

				if (App.menuItems.Any(mi => mi.MenuName == "Places"))
				{
					//we already have this item, let's just update it
					System.Collections.Generic.IEnumerable<FFMenuItem> ffmi = from menus in App.menuItems where menus.MenuName == "Places" select menus;
					ffmi.FirstOrDefault().Count = placesItem.Count;

					//hope this updates?
				}
				else{
					App.menuItems.Insert(3, placesItem);
				}
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Places. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
			finally {
				IsLoading = false;
			}
		}

		public override async Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				var places = client.GetTable<AccountPlace>();

				if (string.IsNullOrEmpty(CurrentPlace.id))
					await places.InsertAsync(CurrentPlace);
				else
					await places.UpdateAsync(CurrentPlace);

				MessagingCenter.Send<AccountPlace>(CurrentPlace, "PlaceAdded");
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}
			finally{
				IsLoading = false;
			}
			IsLoading = false; //redundant
		}
	}
}

