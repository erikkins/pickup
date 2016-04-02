
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

		private AuthAccounts _AuthAccount;
		public AuthAccounts AuthAccount
		{
			get{
				return _AuthAccount;
			}
			set {
				_AuthAccount = value;
				NotifyPropertyChanged ("AuthAccount");
			}
		}

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

		public void SaveLog(string what)
		{
			fflog ff = new fflog (what);
			ExecuteLogCommand (ff).ConfigureAwait (false);
		}

		#region log
		private Command logCommand;
		public Command LogCommand
		{
			get { return logCommand ?? (logCommand = new Command<fflog>(async (f) => await ExecuteLogCommand(f))); }
		}

		public virtual async Task ExecuteLogCommand(fflog LogEntry)
		{
			try{
			var logger =  client.GetTable<fflog> ();
			await logger.InsertAsync (LogEntry);
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}

		#endregion

		#region locationlog
		private Command locationLogCommand;
		public Command LocationLogCommand
		{
			get { return locationLogCommand ?? (locationLogCommand = new Command<LocationLog>(async (f) => await ExecuteLocationLogCommand(f))); }
		}

		public virtual async Task ExecuteLocationLogCommand(LocationLog LogEntry)
		{
			if (string.IsNullOrEmpty (LogEntry.UserId)) {
				return;
			}
			try{
				var logger =  client.GetTable<LocationLog> ();
				await logger.InsertAsync (LogEntry);
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}

		#endregion

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


		#region Delete
		private Command deleteCommand;
		public Command DeleteCommand
		{
			get { return deleteCommand ?? (deleteCommand = new Command(async () => await ExecuteDeleteCommand())); }
		}

		public virtual async Task ExecuteDeleteCommand()
		{
			//this is just the placeholder...should be completely overriden
			var page = new ContentPage();
			var result = await page.DisplayAlert("Not Configured", "You must override ExecuteDeleteCommand", "OK", "Cancel");
			System.Diagnostics.Debug.WriteLine ("Unconfigured DeleteItems! " + result.ToString ());
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
			
			//make sure we can connect!
			if (App.Device.Network.InternetConnectionStatus () == XLabs.Platform.Services.NetworkStatus.NotReachable) {
				//uh oh
				Exception newEx = new Exception("Uh oh! You must be connected to the internet for FamFetch to work!");
				MessagingCenter.Send<Exception> (newEx, "Error");
				return;
			}


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
				aa.Email = _AuthAccount.Email;
				aa.Username = _AuthAccount.Username;
				aa.Password = _AuthAccount.Password;
//				aa.Email = "erik@test.com";
//				aa.Username = "erik@test.com";
//				aa.Password = "test123";
//				aa.UserID = "";
//				aa.Token = "";

				//add this for login instead of create
				System.Collections.Generic.Dictionary<string,string> parms = new System.Collections.Generic.Dictionary<string, string> ();
				parms.Add ("login", "yo");

				try{

					await accounts.InsertAsync (aa, parms);
					Microsoft.WindowsAzure.MobileServices.MobileServiceUser currentUser = new MobileServiceUser(aa.UserID);
					currentUser.MobileServiceAuthenticationToken = aa.Token;
					client.CurrentUser = currentUser;
					IsAuthenticated = true;

					if (Settings.RememberPassword)
					{
						Settings.CachedAuthToken = aa.Token;
						Settings.CachedUserName = aa.Email;
					}
					else{
						Settings.CachedAuthToken = "";
						Settings.CachedUserName = "";
					}

					MessagingCenter.Send (client, "LoggedIn");
				}
				catch(Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException msEx) {
					if (msEx.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
						//bad login
						MessagingCenter.Send<string> (msEx.Message, "LoginError");
					}
					System.Diagnostics.Debug.WriteLine (msEx.Message);
				}
				catch(System.Net.Http.HttpRequestException httpEx) {
					System.Diagnostics.Debug.WriteLine (httpEx.Message);
				}
				catch(Exception ex) {
					//gotta bubble up the error
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}



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


		private Command registerCommand;
		public Command RegisterCommand
		{
			get { return registerCommand ?? (registerCommand = new Command<string>(async (s) => await ExecuteRegisterCommand())); }
		}
		public async Task ExecuteRegisterCommand()
		{
			//make sure we can connect!
			if (App.Device.Network.InternetConnectionStatus () == XLabs.Platform.Services.NetworkStatus.NotReachable) {
				//uh oh
				Exception newEx = new Exception("Uh oh! You must be connected to the internet for FamFetch to work!");
				MessagingCenter.Send<Exception> (newEx, "Error");
				return;
			}

			//this really is only for custom reg!

			if (IsLoading) return;

			//MobileServiceAuthenticationProvider provider = MobileServiceAuthenticationProvider.Facebook;


			IsLoading = true;

				//let's do this directly to azure
				var accounts = client.GetTable<AuthAccounts>();
				AuthAccounts aa = new AuthAccounts ();
				aa.Email = _AuthAccount.Email;
				aa.Username = _AuthAccount.Username;
				aa.Password = _AuthAccount.Password;
				//				aa.Email = "erik@test.com";
				//				aa.Username = "erik@test.com";
				//				aa.Password = "test123";
				//				aa.UserID = "";
				//				aa.Token = "";

				//add this for login instead of create
				//System.Collections.Generic.Dictionary<string,string> parms = new System.Collections.Generic.Dictionary<string, string> ();
				//parms.Add ("login", "yo");


				try{
					await accounts.InsertAsync (aa);
					Microsoft.WindowsAzure.MobileServices.MobileServiceUser currentUser = new MobileServiceUser(aa.UserID);
					currentUser.MobileServiceAuthenticationToken = aa.Token;
					client.CurrentUser = currentUser;
					isAuthenticated = true;
					MessagingCenter.Send (client, "Registered");
				}
				catch(Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException msEx) {
					if (msEx.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
						//bad login
						MessagingCenter.Send<string> (msEx.Message, "RegisterError");
					}
					System.Diagnostics.Debug.WriteLine (msEx.Message);
				}
				catch(System.Net.Http.HttpRequestException httpEx) {
					System.Diagnostics.Debug.WriteLine (httpEx.Message);
				}
				catch(Exception ex) {
					//gotta bubble up the error
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}



				System.Diagnostics.Debug.WriteLine (aa.Token);




			IsLoading = false;  //redundant
		}
		public void Logout()
		{
			Settings.CachedAuthToken = "";
			Settings.CachedUserName = "";
			Settings.RememberPassword = false;
			MessagingCenter.Send<string>("menu", "login");

			//DependencyService.Get<IMobileClient>().Logout();
			Refresh();
		}


		#endregion
		
	}
}

