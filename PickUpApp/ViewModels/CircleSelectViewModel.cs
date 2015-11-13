using System;
using Xamarin.Forms;
using PickUpApp.ViewModels;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;

namespace PickUpApp
{
	public class CircleSelectViewModel: BaseViewModel
	{
		private Schedule _sched;
		private string _myselectedcircle;
		public string MySelectedCircle
		{
			get { return _myselectedcircle; }
			set {
				if (value != _myselectedcircle) {
					_myselectedcircle = value;
					NotifyPropertyChanged ();
				}
			}
		}

		private string _note;
		public string Note
		{
			get{ return _note; }
			set{ if (value != _note) {
					_note = value; NotifyPropertyChanged ();
				}
				}
		}

		private string _returnTo;
		public string ReturnTo
		{
			get{ return _returnTo; }
			set{ if (value != _returnTo) {
					_returnTo = value; NotifyPropertyChanged ();
				}
			}
		}

		private ObservableCollection<Kid> _myKids
		{
			get{
				return App.myKids;
			}
		}

		public ObservableCollection<AccountCircle> _myCircle
		{
			get{
				return App.myCircle;
			}
		}

		public CircleSelectViewModel (Schedule schedule, MobileServiceClient client)
		{
			_sched = schedule;
			this.client = client;
		}

		public override async System.Threading.Tasks.Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{

				InviteRequest req = new InviteRequest()
				{
					ScheduleID = _sched.id,
					note = _note,
					circle = _myselectedcircle,
					returnto = _returnTo
				};
				var invitedata = await client.InvokeApiAsync<InviteRequest, List<Account>>("createinvite",req);
				System.Diagnostics.Debug.WriteLine("createinvite respondents: " + invitedata.Count.ToString());
			
				/*
				var kids = client.GetTable<Kid>();

				if (string.IsNullOrEmpty(CurrentKid.Id))
					await kids.InsertAsync(CurrentKid);
				else
					await kids.UpdateAsync(CurrentKid);

				MessagingCenter.Send<Kid>(CurrentKid, "KidAdded");
				*/
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

