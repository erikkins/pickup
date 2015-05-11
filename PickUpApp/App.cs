﻿using System;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

//using PCLStorage;
using System.Collections.ObjectModel;
using Xamarin.Forms.Labs.Services.Geolocation;
using System.Threading;

namespace PickUpApp
{
	public class App:Application
	{
		public const string applicationURL = @"https://pickup.azure-mobile.net/";
		public const string applicationKey = @"smfGLlHZSdujNrejkOSaRtVbGmPwwz12";
		public static MobileServiceClient client = new MobileServiceClient(applicationURL, applicationKey);
		public static MobileServiceUser user;

		public static Account myAccount = new Account();
		public static AccountDevice myDevice = new AccountDevice();

		public static ObservableCollection<Kid> myKids = new ObservableCollection<Kid>();
		public static ObservableCollection<Account> myCircle = new ObservableCollection<Account> ();
		public static ObservableCollection<AccountPlace> myPlaces = new ObservableCollection<AccountPlace> ();

		//public static Schedule pendingInvite = null;  //are we actually using this?

		private static readonly TaskScheduler _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		private static CancellationTokenSource _cancelSource;
		private static IGeolocator _geolocator;
		private static string _positionStatus = string.Empty;
		private static string _positionLatitude = string.Empty;
		private static string _positionLongitude = string.Empty;
		public static bool IsUpdatingPosition;


		public App ()
		{
			myDevice.PropertyChanged += async (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
				if (myDevice.accountid != null && myDevice.notificationid != null && myDevice.userId != null)
				{
					await PickupService.DefaultService.InsertAccountDeviceAsync(myDevice);
					//await MessagingCenter.Send(myDevice, "changed");				
				}
				//System.Diagnostics.Debug.WriteLine("here");
			};
			GetPosition ().ConfigureAwait (false);
				// The root page of your application
			//	MainPage = new Splash ();
			MainPage = new HomePage ();

		}

		//deprecated
		//public static Page GetMainPage ()
		//{	
			//first things first, see if we already have an account registered here


			//return new Splash ();


			//return new Today ();
			//return new CarouselMaster ();

			//ViewModels.CurrentAccount fvm = new PickUpApp.ViewModels.CurrentAccount ();
			//fvm.Accounts = PickupService.LoadAccount ().Result;

			//return new TabbedMaster ();

			/*
			return new ContentPage { 
				Content = new Label {
					Text = "Howdy, Partners!",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				},
			};
			*/
		
		//}
		public static string PositionStatus
		{
			get
			{
				return _positionStatus;
			}
			set
			{
				_positionStatus = value;

				//SetProperty(ref _positionStatus, value);
			}
		}
		public static string PositionLatitude
		{
			get
			{
				return _positionLatitude;
			}
			set
			{
				_positionLatitude = value;
				//SetProperty(ref _positionLatitude, value);
			}
		}
		public static string PositionLongitude
		{
			get
			{
				return _positionLongitude;
			}
			set
			{
				_positionLongitude = value;
				//SetProperty(ref _positionLongitude, value);
			}
		}
		private static IGeolocator Geolocator
		{
			get
			{
				if (_geolocator == null)
				{
					_geolocator = DependencyService.Get<IGeolocator>();
					_geolocator.PositionError += OnListeningError;
					_geolocator.PositionChanged += OnPositionChanged;
				}
				return _geolocator;
			}
		}
		public static async Task GetPosition()
		{
			_cancelSource = new CancellationTokenSource();

			PositionStatus = string.Empty;
			PositionLatitude = string.Empty;
			PositionLongitude = string.Empty;
			IsUpdatingPosition = true;
			await
			Geolocator.GetPositionAsync(10000, _cancelSource.Token, true)
				.ContinueWith(t =>
					{
						IsUpdatingPosition = false;
						if (t.IsFaulted)
						{
							PositionStatus = ((GeolocationException) t.Exception.InnerException).Error.ToString();
						}
						else if (t.IsCanceled)
						{
							PositionStatus = "Canceled";
						}
						else
						{
							PositionStatus = t.Result.Timestamp.ToString("G");
							PositionLatitude = t.Result.Latitude.ToString("N4");
							PositionLongitude = t.Result.Longitude.ToString("N4");
						}
					}, _scheduler);
		}
		private static void OnListeningError(object sender, PositionErrorEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("OOPS");
			////			BeginInvokeOnMainThread (() => {
			////				ListenStatus.Text = e.Error.ToString();
			////			});
		}

		private static void OnPositionChanged(object sender, PositionEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Changed");
			////			BeginInvokeOnMainThread (() => {
			////				ListenStatus.Text = e.Position.Timestamp.ToString("G");
			////				ListenLatitude.Text = "La: " + e.Position.Latitude.ToString("N4");
			////				ListenLongitude.Text = "Lo: " + e.Position.Longitude.ToString("N4");
			////			});
		}
	}
}
