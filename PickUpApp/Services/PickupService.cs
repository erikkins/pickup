using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.MobileServices;


namespace PickUpApp
{
	public class PickupService : DelegatingHandler
	{
		static PickupService instance = new PickupService ();
		const string applicationURL = @"https://pickup.azure-mobile.net/";
		const string applicationKey = @"smfGLlHZSdujNrejkOSaRtVbGmPwwz12";
		public MobileServiceClient client;
		IMobileServiceTable<Kid> kidTable;
		IMobileServiceTable<Account> accountTable;
		IMobileServiceTable<Schedule> scheduleTable;
		IMobileServiceTable<AccountDevice> accountDeviceTable;
		IMobileServiceTable<LocationLog>locationLogTable;
		int busyCount = 0;

		public event Action<bool> BusyUpdate;

		PickupService ()
		{

			//CurrentPlatform.Init ();
			try{
			// Initialize the Mobile Service client with your URL and key
			client = new MobileServiceClient (applicationURL, applicationKey, this);

			// Create an MSTable instance to allow us to work with the Account table
			//need to pass in our email/identity to pull the right one! thanks Facebook?
			accountTable = client.GetTable <Account> ();
			kidTable = client.GetTable<Kid> ();
			scheduleTable = client.GetTable<Schedule> ();
			accountDeviceTable = client.GetTable<AccountDevice>();
			locationLogTable = client.GetTable<LocationLog>();
			}
			catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine ("MobServEx " + ex.Message);
			}
		}

		static PickupService()
		{
			//LoadAccount ();
		}

		public static PickupService DefaultService {
			get {

				return instance;
			}
		}

		public static IEnumerable<Account>CurrentAccount 
		{ 
			get{ return LoadAccount ().Result; } 
			private set{ } 
		}



		public async static Task<ObservableCollection<Account>> LoadAccount()
		{

			ObservableCollection<Account> ocAccount = new ObservableCollection<Account> ();
			try{
			
				//ocAccount = await PickupService.DefaultService.accountTable.ToCollectionAsync();
				List<Account> acct = await PickupService.DefaultService.accountTable.Where(acctItem => acctItem.Email=="aijamara@me.com").ToListAsync();
				ocAccount.Add(acct[0]);
			}
			catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine ("AccountEx " + ex.Message);
			}
			//CurrentAccount = ocAccount;
			return ocAccount;

			//List<Account> ret = new List<Account> ();
			//ret.Add (theAccount.Result);
			//CurrentAccount = ret;		
		}
			

		public Account GetAccount()
		{
			try{
				Task<List<Account>> acct =  accountTable.Where(acctItem => acctItem.Email=="aijamara@me.com").ToListAsync();
			CurrentAccount = acct.Result;
			return acct.Result [0];
			}
			catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine ("GetAccountEx " + ex.Message);
			}
			return null;
		}

		private async Task Authenticate(string username, string password)
		{
			try
			{
				MobileServiceUser user = await client.LoginAsync(MobileServiceAuthenticationProvider.Facebook, new Newtonsoft.Json.Linq.JObject());
				System.Diagnostics.Debug.WriteLine(user.MobileServiceAuthenticationToken);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("AuthEx " + ex.Message);
				//Console.Error.WriteLine (@"ERROR - AUTHENTICATION FAILED {0}", ex.Message);
			}
		}

		//public List<ToDoItem> Items { get; private set;}

		public async Task InsertLocationLogAsync(LocationLog logItem)
		{
			try{
				//var logger =  client.GetTable<LocationLog> ();
				//await logger.InsertAsync (logItem);
				await locationLogTable.InsertAsync(logItem);
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}

		public async Task InsertKidAsync(Kid kidItem)
		{
			try{
				await kidTable.InsertAsync(kidItem);
			} catch (MobileServiceInvalidOperationException e) {
				System.Diagnostics.Debug.WriteLine (@"Error {0}", e.Message);
			}
		}

		public async Task InsertAccountDeviceAsync(AccountDevice accountDeviceItem)
		{
			try{
				if (accountDeviceItem.id == null)
				{
					await accountDeviceTable.InsertAsync(accountDeviceItem);
				}
				else
				{
					await accountDeviceTable.UpdateAsync(accountDeviceItem);
				}
			}
			catch(MobileServiceInvalidOperationException e) {
				System.Diagnostics.Debug.WriteLine ("InsertDeviceEx " + e.Message);
			}
		}

		public async Task InsertAccountAsync (Account accountItem)
		{
			try {
				await accountTable.InsertAsync(accountItem);

			} catch (MobileServiceInvalidOperationException e) {
				//Console.Error.WriteLine (@"ERROR {0}", e.Message);
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
		}

		public async Task InsertScheduleAsync(Schedule scheduleItem)
		{
			try{
				await scheduleTable.InsertAsync(scheduleItem);
			}
			catch(MobileServiceInvalidOperationException e) {
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
		}


		void Busy (bool busy)
		{
			// assumes always executes on UI thread
			if (busy) {
				if (busyCount++ == 0 && BusyUpdate != null)
					BusyUpdate (true);

			} else {
				if (--busyCount == 0 && BusyUpdate != null)
					BusyUpdate (false);

			}
		}

		#region implemented abstract members of HttpMessageHandler

		protected override async Task<System.Net.Http.HttpResponseMessage> SendAsync (System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
		{
			Busy (true);
			var response = await base.SendAsync (request, cancellationToken);

			Busy (false);
			return response;
		}

		#endregion
	}
}

