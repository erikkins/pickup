using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;

using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
namespace PickUpApp
{
	public class MessageViewModel:BaseViewModel
	{
		private MessageView _currentMessage;
//
//		private Today _currentToday;
//		public Today CurrentToday
//		{
//			get { return _currentToday; }
//			set{
//				_currentToday = value;
//				NotifyPropertyChanged ();
//			}
//		}

		private ObservableCollection<MessageView>_messages;
		public ObservableCollection<MessageView> Messages{
			get{
				return _messages;
			}
			set{
				_messages = value;
				NotifyPropertyChanged ();
			}
		}

		public MessageViewModel (MobileServiceClient client, MessageView currentMessage)
		{
			this.client = client;
			_currentMessage = currentMessage;
			_messages = new ObservableCollection<MessageView> ();
		}

		public async Task<Today> LoadToday(string todayId, string senderId)
		{
			Dictionary<string,string> dict = new Dictionary<string, string>();
			dict.Add("activityId", todayId);
			dict.Add ("senderId", senderId);
			var today = await client.InvokeApiAsync<Dictionary<string,string>,List<Today>> ("getactivity", dict);
			return today.FirstOrDefault();
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try
			{
				//load this invite!
				var messages = await client.InvokeApiAsync<List<MessageView>>("getmymessages");
				Messages.Clear();
				if (messages.Count > 0)
				{
					//atleast right now, we don't need the pipe-delimited kids variable, so we need to fix that
					foreach (MessageView mv in messages)
					{
						if (mv.MessageType == "fetch")
						{
							mv.MessageToday = await LoadToday(mv.Link, mv.SenderID);

						}
						Messages.Add(mv);
					}
				}
				MessagingCenter.Send<string>("messages", "messagesloaded");
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Messages. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
			}
			finally {
				IsLoading = false;
			}
		}

	}
}

