//
//  Copyright 2011-2013, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using CoreLocation;
using System.Threading.Tasks;
using System.Threading;
//using Xamarin.Forms.Labs.Services.Geolocation;
using XLabs.Platform.Services.Geolocation;
using ObjCRuntime;
using PickUpApp.iOS;
using PickUpApp;

namespace Xamarin.Forms.Labs.iOS
{
	internal class GeolocationSingleUpdateDelegate
		: CLLocationManagerDelegate
	{
		public GeolocationSingleUpdateDelegate (CLLocationManager manager, double desiredAccuracy, bool includeHeading, int timeout, CancellationToken cancelToken)
		{
			this.manager = manager;
//			this.manager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => {
//				PickUpApp.Location l = new PickUpApp.Location();
//				l.Latitude = e.Locations[0].Coordinate.Latitude.ToString();
//				l.Longitude = e.Locations[0].Coordinate.Longitude.ToString();
//				MessagingCenter.Send<PickUpApp.Location>(l, "BackgroundLocationUpdated");
//				LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length-1]));
//			};
			this.tcs = new TaskCompletionSource<Position> (manager);
			this.desiredAccuracy = desiredAccuracy;
			this.includeHeading = includeHeading;
			if (this.manager.RespondsToSelector (new Selector ("allowsBackgroundLocationUpdates"))) {
				this.manager.AllowsBackgroundLocationUpdates = true;
			}
			if (timeout != Timeout.Infinite)
			{
				Timer t = null;
				t = new Timer (s =>
					{
						if (this.haveLocation)
							this.tcs.TrySetResult (new Position (this.position));
						else
							this.tcs.TrySetCanceled();
						//maybe never stop listening
						//System.Diagnostics.Debug.WriteLine("Background Lock");
						//StopListening();
						t.Dispose();
					}, null, timeout, 0);
			}

			cancelToken.Register (() =>
				{
					System.Console.WriteLine("CancelTokenFired");
					StopListening();
					this.tcs.TrySetCanceled();
				});
		}
		//public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

		public Task<Position> Task
		{
			get { return this.tcs.Task; }
		}

		public override void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status)
		{
			// If user has services disabled, we're just going to throw an exception for consistency.
			if (status == CLAuthorizationStatus.Denied || status == CLAuthorizationStatus.Restricted)
			{
				System.Console.WriteLine("AuthChangedSingle");
				StopListening();
				this.tcs.TrySetException (new GeolocationException (GeolocationError.Unauthorized));
			}
		}

		public override void Failed (CLLocationManager manager, Foundation.NSError error)
		{
			System.Console.WriteLine("FailedPos");
			/*this shit wasn't working anyway
			switch ((CLError)error)
			{
			case CLError.Network:
				StopListening();	
				this.tcs.SetException (new GeolocationException (GeolocationError.PositionUnavailable));
				break;
			}
			*/
		}

		public override bool ShouldDisplayHeadingCalibration (CLLocationManager manager)
		{
			return false;
			/*
			//this pops up annoyingly frequently while testing...
			#if DEBUG
			return false;
			#endif
			//not sure how it will be in production!
			return true;
			*/
		}

		public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
		{
			if (newLocation.HorizontalAccuracy < 0)
				return;

			if (this.haveLocation && newLocation.HorizontalAccuracy > this.position.Accuracy)
				return;

			this.position.Accuracy = newLocation.HorizontalAccuracy;
			this.position.Altitude = newLocation.Altitude;
			this.position.AltitudeAccuracy = newLocation.VerticalAccuracy;
			this.position.Latitude = newLocation.Coordinate.Latitude;
			this.position.Longitude = newLocation.Coordinate.Longitude;
			this.position.Speed = newLocation.Speed;
			this.position.Timestamp = new DateTimeOffset (NSDateToDateTime(newLocation.Timestamp));

			this.haveLocation = true;

			//send the location every X seconds
			PickUpApp.LocationLog l = new PickUpApp.LocationLog();
			l.Latitude = this.position.Latitude.ToString ();
			l.Longitude = this.position.Longitude.ToString ();
			l.LogType = "background";

			MessagingCenter.Send<LocationLog>(l, "BackgroundLocationUpdated");


			if ((!this.includeHeading || this.haveHeading) && this.position.Accuracy <= this.desiredAccuracy)
			{
				this.tcs.TrySetResult (new Position (this.position));
				System.Console.WriteLine("Butstopping");
				StopListening();
			}
		}
		public static DateTime NSDateToDateTime(Foundation.NSDate date)
		{
			return (new DateTime(2001,1,1,0,0,0)).AddSeconds(date.SecondsSinceReferenceDate);
		}

		public override void UpdatedHeading (CLLocationManager manager, CLHeading newHeading)
		{
			if (newHeading.HeadingAccuracy < 0)
				return;
			if (this.bestHeading != null && newHeading.HeadingAccuracy >= this.bestHeading.HeadingAccuracy)
				return;

			this.bestHeading = newHeading;
			this.position.Heading = newHeading.TrueHeading;
			this.haveHeading = true;

			if (this.haveLocation && this.position.Accuracy <= this.desiredAccuracy)
			{
				this.tcs.TrySetResult (new Position (this.position));
				System.Console.WriteLine("UpdatedHeadingButStopping");
				StopListening();
			}
		}

		private bool haveHeading;
		private bool haveLocation;
		private readonly Position position = new Position();
		private CLHeading bestHeading;

		private readonly double desiredAccuracy;
		private readonly bool includeHeading;
		private readonly TaskCompletionSource<Position> tcs;
		private readonly CLLocationManager manager;

		private void StopListening()
		{
			System.Console.WriteLine("StopPosSingle");
			if (CLLocationManager.HeadingAvailable)
				this.manager.StopUpdatingHeading();

			this.manager.StopUpdatingLocation();
		}
	}
}
