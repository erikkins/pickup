using System.ComponentModel;
using System.Net;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using PickUpApp;
using PickUpApp.droid;
using PickUpApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(MessageViewCell), typeof(MessageRenderer))]

namespace PickUpApp.droid
{
	public class MessageRenderer : ViewCellRenderer
	{
		protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
		{
			var inflatorservice = (LayoutInflater)Forms.Context.GetSystemService(Android.Content.Context.LayoutInflaterService);

			var textMsgVm = item.BindingContext as MessageView;
			if (textMsgVm != null)
			{				
				var template = (LinearLayout)inflatorservice.Inflate(textMsgVm.IsMine ? Resource.Layout.message_item_owner : Resource.Layout.message_item_opponent, null, false);
				//template.FindViewById<TextView>(Resource.Id.timestamp).Text = textMsgVm.Timestamp.ToString("HH:mm");
				template.FindViewById<TextView>(Resource.Id.nick).Text = textMsgVm.IsMine ? "Me:" : textMsgVm.Sender + ":";
				template.FindViewById<TextView>(Resource.Id.message).Text = textMsgVm.Message;
				return template;				
			}

			return base.GetCellCore(item, convertView, parent, context);
		}

		private Bitmap GetImageBitmapFromUrl(string url)
		{
			Bitmap imageBitmap = null;
			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}
			return imageBitmap;
		}


		protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnCellPropertyChanged(sender, e);
		}
	}
}