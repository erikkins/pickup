﻿using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace PickUpApp
{
	public class InviteViewModel: BaseViewModel
	{
		private Invite _currentInvite { get; set; }

		private InviteInfo _currentInviteInfo;
		public InviteInfo CurrentInviteInfo { 
			get{return _currentInviteInfo; }
			set{
				_currentInviteInfo = value; NotifyPropertyChanged ();
			}
		}
		public InviteViewModel ()
		{

		}
		public InviteViewModel(MobileServiceClient client, Invite invite) : this()
		{
			this.client = client;
			this._currentInvite = invite;
			this.CurrentInviteInfo = new InviteInfo ();
			LoadItemsCommand.Execute (null);
		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			try
			{
				//load this invite!
				var inviteInfo = await client.InvokeApiAsync<Invite, List<InviteInfo>>("getinviteinfo", _currentInvite);
				if (inviteInfo.Count > 0)
				{
					//atleast right now, we don't need the pipe-delimited kids variable, so we need to fix that
					InviteInfo interimInfo = inviteInfo[0];
					string[] kiddos = interimInfo.Kids.Split(',');
					string newkids = "";
					foreach (string k in kiddos)
					{
						string[] parts = k.Split('|');
						newkids += parts[0] + ",";
					}
					newkids.Remove(newkids.Length -1);
					interimInfo.Kids = newkids;

					CurrentInviteInfo = interimInfo;
				}
				MessagingCenter.Send<InviteInfo>(CurrentInviteInfo, "inviteinfoloaded");


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
		}

		public override async Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				var inviteresponse = await client.InvokeApiAsync<InviteInfo, List<InviteResponse>>("respondinvite",CurrentInviteInfo);
				System.Diagnostics.Debug.WriteLine("responses " + inviteresponse.Count.ToString());
				MessagingCenter.Send<InviteInfo>(CurrentInviteInfo, "InfoSubmitted");
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
