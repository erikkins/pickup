using System;
//using Refractored.Xam.Settings;
//using Refractored.Xam.Settings.Abstractions;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PickUpApp
{
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		private const string CachedUserNameKey = "cacheduser_key";
		private static readonly string CachedUserNameDefault = "";

		public static string CachedUserName
		{
			get { return AppSettings.GetValueOrDefault<string>(CachedUserNameKey, CachedUserNameDefault); }
			set { AppSettings.AddOrUpdateValue<string>(CachedUserNameKey, value); }
		}


		private const string CachedAuthKey = "cachedauth_key";
		private static readonly string CachedAuthDefault = "";

		public static string CachedAuthToken
		{
			get { return AppSettings.GetValueOrDefault<string>(CachedAuthKey, CachedAuthDefault); }
			set { AppSettings.AddOrUpdateValue<string>(CachedAuthKey, value); }
		}

		private const string FirstTimeKey = "firsttime_key";
		private static readonly bool FirstTimeDefault = true;

		public static bool FirstTime
		{
			get { return AppSettings.GetValueOrDefault<bool>(FirstTimeKey, FirstTimeDefault); }
			set { AppSettings.AddOrUpdateValue<bool>(FirstTimeKey, value); }
		}

		private const string HasLoggedInKey = "hasloggedin_key";
		private static readonly bool HasLoggedInDefault = false;

		public static bool HasLoggedIn
		{
			get { return AppSettings.GetValueOrDefault<bool>(HasLoggedInKey, HasLoggedInDefault); }
			set { AppSettings.AddOrUpdateValue<bool>(HasLoggedInKey, value); }
		}

		private const string RememberPasswordKey = "rememberpassword_key";
		private static readonly bool RememberPasswordDefault = false;

		public static bool RememberPassword
		{
			get { return AppSettings.GetValueOrDefault<bool>(RememberPasswordKey, RememberPasswordDefault); }
			set { AppSettings.AddOrUpdateValue<bool>(RememberPasswordKey, value); }
		}

	}
}

