﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;

//using System.Threading.Tasks;
//using PickUpApp.ViewModels;

namespace PickUpApp
{	
	public partial class Splash : ContentPage
	{	
		public Splash ()
		{
			InitializeComponent ();
			this.ViewModel = new SplashViewModel (App.client);
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			btnFacebook.Command = ViewModel.LoginCommand;
			btnLogout.Clicked += btnLogout_Clicked;

			MessagingCenter.Subscribe<MobileServiceClient>(this, "LoggedIn", (s) =>{
					ViewModel.Refresh();
					ViewModel.IsAuthenticated = true;
					//ok, so we logged in through the service...let's check and see if this account already exists
					ViewModel.ExecuteLoadItemsCommand(s.CurrentUser.UserId).ConfigureAwait(false);
					//Navigation.PushAsync(new TabbedMaster());
				});

			MessagingCenter.Subscribe<Account> (this, "loaded", async(s) => {

				//if we're launching out to the TabbedMaster, unsubscribe to the invite and pickup events
				//MessagingCenter.Unsubscribe<Invite>(this, "invite");
				//MessagingCenter.Unsubscribe<Invite>(this, "pickup");

				//if we don't have sufficient user information, pop the MyInfo page!
//				if (string.IsNullOrEmpty(App.myAccount.Email) || string.IsNullOrEmpty(App.myAccount.Phone))
//				{
//					Navigation.PushModalAsync(new MyInfo());
//				}
//				else{
//					Navigation.PushModalAsync (new TabbedMaster ()); 
//				}

				//now we're fully loaded, account, logged in, but we need to preload the other pages (kids, circle, schedule, places)
				this.BindingContext = new KidsViewModel(App.client);
				lblActivity.Text = "Loading Kids";
				await ((KidsViewModel)BindingContext).ExecuteLoadItemsCommand();

				this.BindingContext = new MyCircleViewModel(App.client);
				lblActivity.Text = "Loading Circle";
				await ((MyCircleViewModel)BindingContext).ExecuteLoadItemsCommand();

				this.BindingContext = new AccountPlaceViewModel(App.client);
				lblActivity.Text = "Loading Places";
				await ((AccountPlaceViewModel)BindingContext).ExecuteLoadItemsCommand();
//
				//when done loading other stuff, reset it to be just a splashviewmodel
				this.BindingContext = new SplashViewModel(App.client);


				//really since we have a message waiting on our auth, we want to atleast load the
				//main page before popping the popover (else we won't see it!)

				MessagingCenter.Send<Splash>(this, "auth");
				MessagingCenter.Send<string>("splash", "NeedsRefresh");
				await Navigation.PopModalAsync();
			});

			MessagingCenter.Subscribe<Account> (this, "Refresh", (s) => {
				//right now this is only triggered when the Account is Updated (e.g. from MyInfo)
				//Navigation.PushModalAsync (new TabbedMaster ()); 
			});


			var v  = DependencyService.Get<Refractored.Xam.Vibrate.Abstractions.IVibrate>();
			//I've received an invite
			MessagingCenter.Subscribe<Invite> (this, "invite", (i) => {
				v.Vibration (500);	
				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							Device.BeginInvokeOnMainThread(()=>{
								Navigation.PushModalAsync(new InviteView(i));
							});
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					Device.BeginInvokeOnMainThread(()=>{
						Navigation.PushModalAsync(new InviteView(i));
					});
				}
			});

			//someone has picked up my kids
			MessagingCenter.Subscribe<Invite> (this, "pickup", (i) => {
				v.Vibration (500);	
				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					//though this doesn't seem to be working very well
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							Device.BeginInvokeOnMainThread(()=>{
							Navigation.PushModalAsync(new Confirmation(i));
							});
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					Device.BeginInvokeOnMainThread(()=>{
						Navigation.PushModalAsync(new Confirmation(i));
					});
				}
			});

			//someone has accepted my invite
			MessagingCenter.Subscribe<Invite> (this, "accepted", (i) => {
				v.Vibration (500);	
				DisplayAlert("Invite has been accepted!", i.Message, "OK");
				MessagingCenter.Send<string>("InviteAccepted", "NeedsRefresh");
				//let's just make this an alert
//				if (App.client.CurrentUser == null)
//				{
//					//we've encountered a race condition where we got a notification,
//					//we want to launch the invite, but we're not yet logged in...
//					//though this doesn't seem to be working very well
//					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
//						{
//							Device.BeginInvokeOnMainThread(()=>{
//								Navigation.PushModalAsync(new InviteAccept());
//							});
//							MessagingCenter.Unsubscribe<Splash>(this, "auth");
//						});
//				}
//				else{
//					Device.BeginInvokeOnMainThread(()=>{
//						Navigation.PushModalAsync(new InviteAccept());
//					});
//				}
			});

			//nobody has accepted my invite
			MessagingCenter.Subscribe<Invite> (this, "nobody", (i) => {
				v.Vibration (500);
				DisplayAlert("Invite not accepted!", i.Message, "OK");
//				if (App.client.CurrentUser == null)
//				{
//					//we've encountered a race condition where we got a notification,
//					//we want to launch the invite, but we're not yet logged in...
//					//though this doesn't seem to be working very well
//					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
//						{
//							Device.BeginInvokeOnMainThread(()=>{
//								Navigation.PushModalAsync(new InviteNotAccepted());
//							});
//							MessagingCenter.Unsubscribe<Splash>(this, "auth");
//						});
//				}
//				else{
//					Device.BeginInvokeOnMainThread(()=>{
//						Navigation.PushModalAsync(new InviteNotAccepted());
//					});
//				}
			});

			//I accepted and was the first to do so
			MessagingCenter.Subscribe<Invite> (this, "confirm", (i) => {
				v.Vibration (500);
				DisplayAlert("You are picking up!", i.Message, "OK");
				MessagingCenter.Send<string>("InviteIsMine", "NeedsRefresh");
//				if (App.client.CurrentUser == null)
//				{
//					//we've encountered a race condition where we got a notification,
//					//we want to launch the invite, but we're not yet logged in...
//					//though this doesn't seem to be working very well
//					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
//						{
//							Device.BeginInvokeOnMainThread(()=>{
//								Navigation.PushModalAsync(new InviteIsMine());
//							});
//							MessagingCenter.Unsubscribe<Splash>(this, "auth");
//						});
//				}
//				else{
//					Device.BeginInvokeOnMainThread(()=>{
//						Navigation.PushModalAsync(new InviteIsMine());
//					});
//				}
			});


			MessagingCenter.Subscribe<Invite> (this, "notfirst", (i) => {
				v.Vibration (500);	
				DisplayAlert("Thank you but you weren't first!", i.Message, "OK");
//				if (App.client.CurrentUser == null)
//				{
//					//we've encountered a race condition where we got a notification,
//					//we want to launch the invite, but we're not yet logged in...
//					//though this doesn't seem to be working very well
//					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
//						{
//							Device.BeginInvokeOnMainThread(()=>{
//								Navigation.PushModalAsync(new InviteIsMine());
//							});
//							MessagingCenter.Unsubscribe<Splash>(this, "auth");
//						});
//				}
//				else{
//					Device.BeginInvokeOnMainThread(()=>{
//						Navigation.PushModalAsync(new InviteIsMine());
//					});
//				}
			});


			MessagingCenter.Subscribe<Invite> (this, "cancel", (i) => {
				v.Vibration (500);	

				//need to show a little more than just a dialog? showing them WHICH pickup is canceled?

				if (App.client.CurrentUser == null)
				{
					//we've encountered a race condition where we got a notification,
					//we want to launch the invite, but we're not yet logged in...
					//though this doesn't seem to be working very well
					MessagingCenter.Subscribe<Splash>(this, "auth", (s) =>
						{
							Device.BeginInvokeOnMainThread(()=>{
								Navigation.PushModalAsync(new InviteIsMine());
							});
							MessagingCenter.Unsubscribe<Splash>(this, "auth");
						});
				}
				else{
					Device.BeginInvokeOnMainThread(()=>{
						Navigation.PushModalAsync(new InviteIsMine());
					});
				}
			});


			MessagingCenter.Subscribe<AccountDevice>(this, "changed", (s) =>
			{

				PickupService.DefaultService.InsertAccountDeviceAsync(s).Wait(10000);
			});
					
		}



		protected override void OnAppearing ()
		{
			
			base.OnAppearing ();

			if (ViewModel.IsAuthenticated) {
				//Navigation.PushModalAsync (new TabbedMaster ());
				Navigation.PopModalAsync();
			} else {
				//this.ViewModel.ExecuteLoginCommand ("Facebook").ConfigureAwait (false);
				this.ViewModel.LoginCommand.Execute("Facebook");
			}

		}
		void btnLogout_Clicked(object sender, EventArgs e)
		{
			ViewModel.Logout();
		}

		protected SplashViewModel ViewModel
		{
			get { return this.BindingContext as SplashViewModel; }
			set { this.BindingContext = value; }
		}
	}
}
