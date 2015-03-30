using System;

namespace PickUpApp.droid
{
	public class Constants
	{
		public const string SenderID = "842065825073"; // Google API Project Number

		// Azure app specific connection string and hub path


			public const string ConnectionString = "Endpoint=sb://pickuphub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=RwsApEzeS+I08PaJJOtAvnbpVTzEJaTyi/R73XbXDyg=";
		public const string NotificationHubPath = "pickuphub";
	}
}

