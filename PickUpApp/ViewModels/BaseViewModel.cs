
using System;
using System.ComponentModel;
using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PickUpApp.ViewModels
{
	public class BaseViewModel:INotifyPropertyChanged
	{
		protected MobileServiceClient client;

		private bool isLoading;
		public bool IsLoading
		{
			get { return isLoading; }
			set
			{
				isLoading = value;
				NotifyPropertyChanged();
			}
		}


		private bool isAuthenticated;
		public bool IsAuthenticated
		{
			get { return isAuthenticated; }
			set
			{
				if (value != isAuthenticated)
				{
					isAuthenticated = value;
					NotifyPropertyChanged();
				}
			}
		}

		private string viewName;

		public virtual string ViewName
		{
			get { return viewName; }
			set
			{
				viewName = value;
				NotifyPropertyChanged();
			}
		}

		public virtual void Refresh()
		{
			NotifyPropertyChanged("ViewName");
			NotifyPropertyChanged("IsLoading");
			NotifyPropertyChanged("IsAuthenticated");
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#region LoadItems
		private Command loadItemsCommand;
		public Command LoadItemsCommand
		{
			get { return loadItemsCommand ?? (loadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand())); }
		}

		public virtual async Task ExecuteLoadItemsCommand()
		{
			//this is just the placeholder...should be completely overriden
			var page = new ContentPage();
			var result = await page.DisplayAlert("Not Configured", "You must override ExecuteLoadItemsCommand", "OK", "Cancel");
			System.Diagnostics.Debug.WriteLine (result.ToString ());
		}
		#endregion

		#region AddEdit
		private Command addEditCommand;
		public Command AddEditCommand
		{
			get { return addEditCommand ?? (addEditCommand = new Command(async () => await ExecuteAddEditCommand())); }
		}

		public virtual async Task ExecuteAddEditCommand()
		{

		}

		#endregion


		#region Login

		private Command loginCommand;
		public Command LoginCommand
		{
			get { return loginCommand ?? (loginCommand = new Command<string>(async (s) => await ExecuteLoginCommand(s))); }
		}
		public async Task ExecuteLoginCommand(string service)
		{
			if (IsLoading || string.IsNullOrEmpty(service)) return;

			MobileServiceAuthenticationProvider provider;

			switch (service)
			{
			case "Facebook":
				provider = MobileServiceAuthenticationProvider.Facebook;
				break;

			case "Twitter":
				provider = MobileServiceAuthenticationProvider.Twitter;
				break;

			case "Microsoft":
				provider = MobileServiceAuthenticationProvider.MicrosoftAccount;
				break;

			case "Google":
				provider = MobileServiceAuthenticationProvider.Google;
				break;

			default:
				throw new ArgumentOutOfRangeException(service);
			}

			IsLoading = true;

			try
			{
				//await App.Platform.Authorize(container, provider);
				var user = await DependencyService.Get<IMobileClient>().Authorize(provider);
				System.Diagnostics.Debug.WriteLine(user.UserId);
				try{

				MessagingCenter.Send(client, "LoggedIn");
				}
				catch(Exception ex2)
				{
					System.Diagnostics.Debug.WriteLine(ex2.Message);
				}
			}
 			catch (InvalidOperationException ex)
			{
				if (ex.Message.Contains("Authentication was cancelled by the user"))
				{

				}
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				await page.DisplayAlert("Error", "Error logging in. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}

			IsLoading = false;
		}

		public void Logout()
		{
			DependencyService.Get<IMobileClient>().Logout();
			Refresh();
		}


		#endregion
		
	}
}

