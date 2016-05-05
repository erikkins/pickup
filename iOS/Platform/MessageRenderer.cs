using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using PickUpApp;
using PickUpApp.iOS;

[assembly: ExportRenderer(typeof(MessageViewCell), typeof(MessageRenderer))]
namespace PickUpApp.iOS
{
	public class MessageRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell (Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			//var cell = base.GetCell (item, reusableCell, tv);

			var textVm = item.BindingContext as MessageView;
			if (textVm != null)
			{
				string nameText = "";
				if (textVm.IsMine) {
					nameText = "Me";
				} else {
					nameText = textVm.Sender;
				}

				var chatBubble = new ChatBubble(!textVm.IsMine, textVm.Message, nameText );
				return chatBubble.GetCell(tv);
			}
				
			return base.GetCell (item, reusableCell, tv);
		} 

//		public override UITableViewCell GetCell(Cell item, UITableView tv)
//		{
//			var textVm = item.BindingContext as TextMessageViewModel;
//			if (textVm != null)
//			{
//				string text = textVm.ImageId.HasValue ? "<IOS client doesn't support image messages yet ;(>" : (textVm.IsMine ? "Me" : textVm.AuthorName) + ": " + textVm.Text;
//				var chatBubble = new ChatBubble(!textVm.IsMine, text);
//				return chatBubble.GetCell(tv);
//			}
//			return base.GetCell(item, tv);
//		}
	}
}

