
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
			System.Diagnostics.Debug.WriteLine ("Unconfigured LoadItems! " + result.ToString ());
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
			//this is just the placeholder...should be completely overriden
			var page = new ContentPage();
			var result = await page.DisplayAlert("Not Configured", "You must override ExecuteAddEditCommand", "OK", "Cancel");
			System.Diagnostics.Debug.WriteLine ("Unconfigured AddEditItems! " + result.ToString ());
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
			bool isCustom = false;
			if (IsLoading || string.IsNullOrEmpty(service)) return;

			MobileServiceAuthenticationProvider provider = MobileServiceAuthenticationProvider.Facebook;

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
			case "AD":
				provider = MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory;
				break;
			case "Custom":
				//ok, we're logging in using our own method
				isCustom = true;
				break;
			default:
				throw new ArgumentOutOfRangeException(service);
			}

			IsLoading = true;
			if (isCustom) {

				//let's do this directly to azure
				var accounts = client.GetTable<AuthAccounts>();
				AuthAccounts aa = new AuthAccounts ();
				aa.Email = "erik@test.com";
				aa.Username = "erik@test.com";
				aa.Password = "test123";
				aa.UserID = "";
				aa.Token = "";
				System.Collections.Generic.Dictionary<string,string> parms = new System.Collections.Generic.Dictionary<string, string> ();
				parms.Add ("login", "yo");
				await accounts.InsertAsync (aa, parms);



				System.Diagnostics.Debug.WriteLine (aa.Token);


			} else {
				try {
					//await App.Platform.Authorize(container, provider);
					var user = await DependencyService.Get<IMobileClient> ().Authorize (provider);
					System.Diagnostics.Debug.WriteLine (user.UserId);
					try {

						MessagingCenter.Send (client, "LoggedIn");
					} catch (Exception ex2) {
						System.Diagnostics.Debug.WriteLine (ex2.Message);
					}
				} catch (InvalidOperationException ex) {
					if (ex.Message.Contains ("Authentication was cancelled by the user")) {

					}
				} catch (Exception ex) {
					var page = new ContentPage ();
					await page.DisplayAlert ("Error", "Error logging in. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
				} finally {
					IsLoading = false;
				}
			}

			IsLoading = false;  //redundant
		}

		public void Logout()
		{
			DependencyService.Get<IMobileClient>().Logout();
			Refresh();
		}


		#endregion
		
	}
}

