using System;
using Foundation;	
using UIKit;
using MonoTouch.Dialog;

namespace PickUpApp.iOS
{
	public class ChatBubble:Element, IElementSizing
	{
		bool isLeft;
		string _name;

		public ChatBubble(bool isLeft, string text, string nameText)
			: base(text)
		{
			this.isLeft = isLeft;
			_name = nameText;
		}


		public override UITableViewCell GetCell(UITableView tv)
		{
			var cell = tv.DequeueReusableCell(isLeft ? BubbleCell.KeyLeft : BubbleCell.KeyRight) as BubbleCell;
			if (cell == null) {
				cell = new BubbleCell (isLeft);
			}
			cell.Update(Caption, _name);
			return cell;
		}

		public nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			
			return BubbleCell.GetSizeForText(tableView, Caption).Height + BubbleCell.BubblePadding.Height;
		}
	}
}

