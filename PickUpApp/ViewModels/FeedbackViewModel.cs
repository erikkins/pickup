using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PickUpApp
{
	public class FeedbackViewModel: BaseViewModel
	{
		public FeedbackViewModel (MobileServiceClient client)
		{
			this.client = client;
			CurrentFeedback = new Feedback ();
		}

		private Feedback _currentFeedback;
		public Feedback CurrentFeedback
		{
			get{ return _currentFeedback;
			}
			set{
				_currentFeedback = value;
				NotifyPropertyChanged ();
			}
		}

		public override async Task ExecuteAddEditCommand ()
		{
			try{
				
				var fb = client.GetTable<Feedback>();
				await fb.InsertAsync(CurrentFeedback);
				MessagingCenter.Send<Feedback>(CurrentFeedback, "FeedbackReceived");

			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}

	}
}

