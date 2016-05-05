using System.Drawing;
using Foundation;
using UIKit;
using CoreGraphics;

namespace PickUpApp.iOS
{
	public class BubbleCell : UITableViewCell
	{
		public static NSString KeyLeft = new NSString("BubbleElementLeft");
		public static NSString KeyRight = new NSString("BubbleElementRight");
		public static UIImage bleft, bright, left, right;
		public static UIFont font = UIFont.SystemFontOfSize(16);
		public static UIColor color = UIColor.Black;
		public static UIFont nameFont = UIFont.ItalicSystemFontOfSize (14);
		UIView view;
		UIView imageView;
		UILabel label;
		UILabel nameLabel;
		bool isLeft;

		static BubbleCell()
		{
			bright = UIImage.FromFile("greenchat.png");
			bleft = UIImage.FromFile("greenchat.png");

			// buggy, see https://bugzilla.xamarin.com/show_bug.cgi?id=6177
			//left = bleft.CreateResizableImage (new UIEdgeInsets (10, 16, 18, 26));
			//right = bright.CreateResizableImage (new UIEdgeInsets (11, 11, 17, 18));
			left = bleft.StretchableImage(26, 16);
			right = bright.StretchableImage(11, 11);
		}

		//public BubbleCell(bool isLeft)
		//	: base(UITableViewCellStyle.Default, isLeft ? KeyLeft : KeyRight)
		public BubbleCell(bool isLeft)
			: base()
		{
			var rect = new RectangleF(0, 0, 1, 1);
			this.isLeft = isLeft;
			view = new UIView(rect);
			imageView = new UIImageView(isLeft ? left : right);
			view.AddSubview(imageView);


			label = new UILabel(rect)
			{
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0,
				Font = font,
				BackgroundColor = UIColor.Clear,
				TextColor = color
			};
			view.AddSubview(label);

			nameLabel = new UILabel (rect) {
				Font = nameFont,
				TextColor = UIColor.DarkGray,
				BackgroundColor = UIColor.Clear
			};
			view.AddSubview (nameLabel);


			ContentView.Add(view);
		}

		public override void LayoutSubviews()
		{
			/*
			 * 
			base.LayoutSubviews();
			var frame = ContentView.Frame;
			var size = GetSizeForText(this, label.Text) + BubblePadding;
			imageView.Frame = new RectangleF(new PointF(isLeft ? 10 : (float)frame.Width - size.Width - 10, (float)frame.Y), size);
			view.SetNeedsDisplay();
			frame = imageView.Frame;
			label.Frame = new RectangleF(new PointF((float)frame.X + (isLeft ? 12 : 8), (float)frame.Y + 6), size - BubblePadding);

			 */

			base.LayoutSubviews();

			var frame = ContentView.Frame;
			var size = GetSizeForText(this, label.Text) + BubblePadding;
			imageView.Frame = new RectangleF(new PointF(isLeft ? 10 : (float)frame.Width - size.Width - 10, (float)frame.Y + 40), size);
			view.SetNeedsDisplay();
			frame = imageView.Frame;
			label.Frame = new RectangleF(new PointF((float)frame.X + (isLeft ? 12 : 8), (float)frame.Y + 6), size - BubblePadding);

			nameLabel.Frame = new RectangleF(new PointF((float)frame.X + (isLeft ? 4 : size.Width-BubblePadding.Width), (float)frame.Y -30), new SizeF(100,40));
		}

		static internal SizeF BubblePadding = new SizeF(22, 16);

		static internal System.Drawing.SizeF GetSizeForText(UIView tv, string text)
		{						
			SizeF sizeItThinks = new SizeF ((float)tv.Bounds.Width * .7f - 10 - 22, 99999);
			return (SizeF)UIKit.UIStringDrawing.StringSize (text, font, sizeItThinks , UILineBreakMode.WordWrap);
			//return tv.StringSize(text, font, new SizeF((float)tv.Bounds.Width * .7f - 10 - 22, 99999));

		}

		public void Update(string text, string nameText)
		{
			if (text == null) {
				text = "";
			}
			label.Text = text;
			nameLabel.Text = nameText;
			SetNeedsLayout();
		}

		public float GetHeight(UIView tv)
		{
			return GetSizeForText(tv, label.Text).Height + BubblePadding.Height;
		}
	}
}

