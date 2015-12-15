using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace PickUpApp
{
	public class RouteDetailViewModel : BaseViewModel
	{
		public RouteDetailViewModel ()
		{
			
		}

		public RouteDetailViewModel(MobileServiceClient client) : this()
		{
			this.client = client;
			LoadItemsCommand.Execute (null);

		}


		private Command auditCommand;
		public Command AuditCommand
		{
			get { return auditCommand ?? (auditCommand = new Command<ScheduleAudit>(async (sa) => await ExecuteAuditCommand(sa))); }
		}

		public virtual async Task ExecuteAuditCommand(ScheduleAudit AuditEntry)
		{
			try{
				var logger =  await client.InvokeApiAsync<ScheduleAudit, EmptyClass>("savescheduleaudit", AuditEntry);
				System.Diagnostics.Debug.WriteLine(logger.Status);
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}



		public override async Task ExecuteLoadItemsCommand ()
		{
			try
			{
				IsLoading = true;
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
			finally {
				IsLoading = false;
			}
		}
	}
}

