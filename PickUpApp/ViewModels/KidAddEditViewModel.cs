using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace PickUpApp
{
	public class KidAddEditViewModel: BaseViewModel
	{
		private Kid _currentKid;
		public KidAddEditViewModel ()
		{

		}
		public KidAddEditViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
		}
		public KidAddEditViewModel(Kid kid) : this()
		{
			this.CurrentKid = kid;
		}
		public Kid CurrentKid
		{
			get { return this._currentKid; }
			set
			{
				this._currentKid = value;
				NotifyPropertyChanged();
				//NotifyPropertyChanged("ViewName");
			}
		}

		public override async System.Threading.Tasks.Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				var kids = client.GetTable<Kid>();

				if (string.IsNullOrEmpty(CurrentKid.Id))
					await kids.InsertAsync(CurrentKid);
				else
					await kids.UpdateAsync(CurrentKid);

				MessagingCenter.Send<Kid>(CurrentKid, "KidAdded");
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

