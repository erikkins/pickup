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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Droid.Services.Geolocation;
using PickUpApp.Android;
using Xamarin.Forms.Labs.Services.Geolocation;

[assembly: Dependency(typeof (Geolocator))]

namespace Xamarin.Forms.Labs.Droid.Services.Geolocation
{
	public class Geolocator : IGeolocator
	{
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private readonly LocationManager manager;
		private readonly object positionSync = new object();
		private readonly string[] providers;
		private string headingProvider;
		private Position lastPosition;
		private GeolocationContinuousListener listener;

		public Geolocator()
		{
			manager = (LocationManager) Application.Context.GetSystemService(Context.LocationService);
			providers = manager.GetProviders(false).Where(s => s != LocationManager.PassiveProvider).ToArray();
		}

		public event EventHandler<PositionErrorEventArgs> PositionError;
		public event EventHandler<PositionEventArgs> PositionChanged;

		public bool IsListening
		{
			get { return listener != null; }
		}

		public double DesiredAccuracy { get; set; }

		public bool SupportsHeading
		{
			get
			{
				return false;
				//				if (this.headingProvider == null || !this.manager.IsProviderEnabled (this.headingProvider))
				//				{
				//					Criteria c = new Criteria { BearingRequired = true };
				//					string providerName = this.manager.GetBestProvider (c, enabledOnly: false);
				//
				//					LocationProvider provider = this.manager.GetProvider (providerName);
				//
				//					if (provider.SupportsBearing())
				//					{
				//						this.headingProvider = providerName;
				//						return true;
				//					}
				//					else
				//					{
				//						this.headingProvider = null;
				//						return false;
				//					}
				//				}
				//				else
				//					return true;
			}
		}

		public bool IsGeolocationAvailable
		{
			get { return providers.Length > 0; }
		}

		public bool IsGeolocationEnabled
		{
			get { return providers.Any(manager.IsProviderEnabled); }
		}

		public Task<Position> GetPositionAsync(CancellationToken cancelToken)
		{
			return GetPositionAsync(cancelToken, false);
		}

		public Task<Position> GetPositionAsync(CancellationToken cancelToken, bool includeHeading)
		{
			return GetPositionAsync(Timeout.Infinite, cancelToken);
		}

		public Task<Position> GetPositionAsync(int timeout)
		{
			return GetPositionAsync(timeout, false);
		}

		public Task<Position> GetPositionAsync(int timeout, bool includeHeading)
		{
			return GetPositionAsync(timeout, CancellationToken.None);
		}

		public Task<Position> GetPositionAsync(int timeout, CancellationToken cancelToken)
		{
			return GetPositionAsync(timeout, cancelToken, false);
		}

		public Task<Position> GetPositionAsync(int timeout, CancellationToken cancelToken, bool includeHeading)
		{
			if (timeout <= 0 && timeout != Timeout.Infinite)
				throw new ArgumentOutOfRangeException("timeout", "timeout must be greater than or equal to 0");

			var tcs = new TaskCompletionSource<Position>();

			if (!IsListening)
			{
				GeolocationSingleListener singleListener = null;
				singleListener = new GeolocationSingleListener((float) DesiredAccuracy, timeout,
					providers.Where(manager.IsProviderEnabled), () =>
					{
						for (int i = 0; i < providers.Length; ++i)
							manager.RemoveUpdates(singleListener);
					});

				if (cancelToken != CancellationToken.None)
				{
					cancelToken.Register(() =>
						{
							singleListener.Cancel();

							for (int i = 0; i < providers.Length; ++i)
								manager.RemoveUpdates(singleListener);
						}, true);
				}

				try
				{
					Looper looper = Looper.MyLooper() ?? Looper.MainLooper;

					int enabled = 0;
					for (int i = 0; i < providers.Length; ++i)
					{
						if (manager.IsProviderEnabled(providers[i]))
							enabled++;

						manager.RequestLocationUpdates(providers[i], 0, 0, singleListener, looper);
					}

					if (enabled == 0)
					{
						for (int i = 0; i < providers.Length; ++i)
							manager.RemoveUpdates(singleListener);

						tcs.SetException(new GeolocationException(GeolocationError.PositionUnavailable));
						return tcs.Task;
					}
				}
				catch (SecurityException ex)
				{
					tcs.SetException(new GeolocationException(GeolocationError.Unauthorized, ex));
					return tcs.Task;
				}

				return singleListener.Task;
			}

			// If we're already listening, just use the current listener
			lock (positionSync)
			{
				if (lastPosition == null)
				{
					if (cancelToken != CancellationToken.None)
					{
						cancelToken.Register(() => tcs.TrySetCanceled());
					}

					EventHandler<PositionEventArgs> gotPosition = null;
					gotPosition = (s, e) =>
					{
						tcs.TrySetResult(e.Position);
						PositionChanged -= gotPosition;
					};

					PositionChanged += gotPosition;
				}
				else
				{
					tcs.SetResult(lastPosition);
				}
			}

			return tcs.Task;
		}

		public void StartListening(uint minTime, double minDistance)
		{
			StartListening(minTime, minDistance, false);
		}

		public void StartListening(uint minTime, double minDistance, bool includeHeading)
		{
			if (minTime < 0)
				throw new ArgumentOutOfRangeException("minTime");
			if (minDistance < 0)
				throw new ArgumentOutOfRangeException("minDistance");
			if (IsListening)
				throw new InvalidOperationException("This Geolocator is already listening");

			listener = new GeolocationContinuousListener(manager, TimeSpan.FromMilliseconds(minTime), providers);
			listener.PositionChanged += OnListenerPositionChanged;
			listener.PositionError += OnListenerPositionError;

			Looper looper = Looper.MyLooper() ?? Looper.MainLooper;
			for (int i = 0; i < providers.Length; ++i)
				manager.RequestLocationUpdates(providers[i], minTime, (float) minDistance, listener, looper);
		}

		public void StopListening()
		{
			if (listener == null)
				return;

			listener.PositionChanged -= OnListenerPositionChanged;
			listener.PositionError -= OnListenerPositionError;

			for (int i = 0; i < providers.Length; ++i)
				manager.RemoveUpdates(listener);

			listener = null;
		}

		private void OnListenerPositionChanged(object sender, PositionEventArgs e)
		{
			if (!IsListening) // ignore anything that might come in afterwards
				return;

			lock (positionSync)
			{
				lastPosition = e.Position;

				EventHandler<PositionEventArgs> changed = PositionChanged;
				if (changed != null)
					changed(this, e);
			}
		}

		private void OnListenerPositionError(object sender, PositionErrorEventArgs e)
		{
			StopListening();

			EventHandler<PositionErrorEventArgs> error = PositionError;
			if (error != null)
				error(this, e);
		}

		internal static DateTimeOffset GetTimestamp(Location location)
		{


			return new DateTimeOffset(Epoch.AddMilliseconds(location.Time));
		}
	}
}
