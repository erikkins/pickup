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
		public MessageView CurrentMessage
		{
			get{
				return _currentMessage;
			}
			set{
				_currentMessage = value;
				NotifyPropertyChanged ();
				}
		}

		private RespondMessage _currentResponse;
		public RespondMessage CurrentMessageResponse
		{
			get{
				return _currentResponse;
			}
			set{
				_currentResponse = value;
				NotifyPropertyChanged ();
				}
		}

		private ObservableCollection<MessageView> _chatMessages;
		public ObservableCollection<MessageView> ChatMessages
		{
			get {
				return _chatMessages;
			}
			set{
				if (_chatMessages != value) {
					_chatMessages = value;
					NotifyPropertyChanged ();
				}
			}
		}
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

		//private ObservableCollection<MessageView>_messages;
//		public ObservableCollection<MessageView> Messages{
//			get{
//				return App.myMessages;//_messages;
//			}
//			set{
//				//_messages = value;
//				App.myMessages = value;
//				NotifyPropertyChanged ();
//			}
//		}

		public MessageViewModel (MobileServiceClient client, MessageView currentMessage)
		{
			this.client = client;
			_currentMessage = currentMessage;
			ChatMessages = new ObservableCollection<MessageView> ();
			//App.myMessages = new ObservableCollection<MessageView> ();
		}

		public async Task<Today> LoadToday(string todayId, string senderId)
		{
			Dictionary<string,string> dict = new Dictionary<string, string>();
			dict.Add("activityId", todayId);
			dict.Add ("senderId", senderId);
			var today = await client.InvokeApiAsync<Dictionary<string,string>,List<Today>> ("getactivity", dict);
			return today.FirstOrDefault();
		}

		private Command loadChatCommand;
		public Command LoadChatCommand
		{
			get { return loadChatCommand ?? (loadChatCommand = new Command<Today>(async (t) => await ExecuteLoadChatCommand(t))); }
		}

		public virtual async Task ExecuteLoadChatCommand(Today currentToday)
		{
			try{
				MessageView mvLoad = new MessageView ();
				mvLoad.Link = currentToday.id;
				if (currentToday.IsPickup) {
					mvLoad.LinkDetail = "pickup";
				} else {
					mvLoad.LinkDetail = "dropoff";
				}
				mvLoad.MessageToday = currentToday;

				var chatdata = await client.InvokeApiAsync<MessageView, List<MessageView>>("getchat",mvLoad);
				ChatMessages.Clear();
				foreach(MessageView mv in chatdata)
				{
					ChatMessages.Add(mv);
				}
				MessagingCenter.Send<EmptyClass>(new EmptyClass(), "chatloaded");
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}

		private Command loadChatCommandFromMessage;
		public Command LoadChatCommandFromMessage
		{
			get { return loadChatCommandFromMessage ?? (loadChatCommandFromMessage = new Command<MessageView>(async (mv) => await ExecuteLoadChatCommandFromMessage(mv))); }
		}

		public virtual async Task ExecuteLoadChatCommandFromMessage(MessageView currentMessage)
		{
			try{
				var chatdata = await client.InvokeApiAsync<MessageView, List<MessageView>>("getchat",currentMessage);
				ChatMessages.Clear();
				foreach(MessageView mv in chatdata)
				{
					ChatMessages.Add(mv);
				}
				MessagingCenter.Send<EmptyClass>(new EmptyClass(), "chatloaded");
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}


		private Command createCommand;
		public Command CreateCommand
		{
			get { return createCommand ?? (createCommand = new Command<MessageView>(async (mv) => await ExecuteCreateCommand(mv))); }
		}

		public virtual async Task ExecuteCreateCommand(MessageView messageView)
		{
			try{
				var invitedata = await client.InvokeApiAsync<MessageView, EmptyClass>("savemessage",messageView);

				if (messageView.MessageType=="chat")
				{
					MessagingCenter.Send<MessageView>(messageView, "chatsent");
				}
				else{
					MessagingCenter.Send<MessageView>(messageView, "messagesent");
				}

			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}

		private Command cancelCommand;
		public Command CancelCommand
		{
			get { return cancelCommand ?? (cancelCommand = new Command<Today>(async (ct) => await ExecuteCancelCommand(ct))); }
		}

		public virtual async Task ExecuteCancelCommand(Today currentToday)
		{
			try{
				var canceldata = await client.InvokeApiAsync<Today, EmptyClass>("cancelpickup",currentToday);
				MessagingCenter.Send<Today>(currentToday, "fetchcanceled");

			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
		}



		public override async Task ExecuteAddEditCommand ()
		{
			IsLoading = true;
			try{
					System.Diagnostics.Debug.WriteLine("RespondingMessageVM");
					var msgresponse = await client.InvokeApiAsync<RespondMessage, EmptyClass> ("respondmessage", _currentResponse);
			
					MessagingCenter.Send<RespondMessage> (_currentResponse, "messagesupdated");				
				}
				catch(Exception ex) {
				//what should we really be doing with exceptions?
				System.Diagnostics.Debug.WriteLine(ex);
			}
			finally {
				IsLoading = false;
			}
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try
			{
				//load this invite!
				var messages = await client.InvokeApiAsync<List<MessageView>>("getmymessages");
				App.myMessages.Clear();
				//Messages.Clear();
				if (messages.Count > 0)
				{
					//atleast right now, we don't need the pipe-delimited kids variable, so we need to fix that
					foreach (MessageView mv in messages)
					{
						if (mv.MessageType == "pickup")
						{
							Today tempToday = await LoadToday(mv.Link, mv.SenderID);
							if (tempToday == null)
							{
								continue;
							}
							if (mv.LinkDetail == "pickup")
							{
								//here's some secret sauce...if this was a PICKUP request, then we need to clone and make it the pickup data
								Today pickup = tempToday.Clone();
								pickup.IsPickup = true;
								mv.MessageToday = pickup;
							}
							else{
								mv.MessageToday = tempToday;
							}

						}
						mv.IsActionable = true;
						//Messages.Add(mv);
						App.myMessages.Add(mv);
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

