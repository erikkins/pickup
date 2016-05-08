using System;
using System.Reflection;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using PickUpApp;
using PickUpApp.iOS;
using CoreGraphics;

[assembly: ExportRenderer(typeof(ChatListView), typeof(ChatListViewRenderer))]
namespace PickUpApp.iOS
{
	public class ChatListViewRenderer : ListViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
		{
			base.OnElementChanged(e);
			var table = (UITableView)this.Control;
			//hoping to raise the bottom
			//table.ContentInset = new UIEdgeInsets(0, 0, 20, 0); //values
			table.AllowsSelection = false;
			table.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			table.Source = new ListViewDataSourceWrapper(this.GetFieldValue<UITableViewSource>(typeof(ListViewRenderer), "dataSource"));
		}

		protected override void UpdateNativeWidget ()
		{
			base.UpdateNativeWidget ();
			var table = (UITableView)this.Control;
			nfloat widgetHeight = table.Frame.Height;
			//table.ContentSize = new CGSize (UIScreen.MainScreen.Bounds.Width, table.Frame.Height - 20);

			//table.ContentInset = new UIEdgeInsets(0, 0, table.ContentSize.Height - table.Frame.Size.Height + 20, 0); //values
			//table.ContentInset = new UIEdgeInsets(0, 0, 20, 0); //values
			if (table.ContentSize.Height > table.Frame.Size.Height) {
				CGPoint offset = new CGPoint (0, 999999);
				table.SetContentOffset (offset, false);
				System.Diagnostics.Debug.WriteLine ("Bottom: " + table.ContentInset.Bottom.ToString ());
				table.ContentInset = new UIEdgeInsets (0, 0, table.ContentInset.Bottom + 5, 0); //still doesn't seem to add the 20px when keyboard is up
			} 
			//table.ScrollToNearestSelected(UITableViewScrollPosition.Bottom,false);
//			NSIndexPath nip = table.IndexPathForRowAtPoint(new CGPoint(0,table.Frame.Height));
//			if (nip != null) {
//				table.ScrollToRow (nip, UITableViewScrollPosition.Bottom, false);
//			}
		}			
	}

	public class ListViewDataSourceWrapper : UITableViewSource
	{
		private readonly UITableViewSource _underlyingTableSource;

		public ListViewDataSourceWrapper(UITableViewSource underlyingTableSource)
		{
			this._underlyingTableSource = underlyingTableSource;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			return this.GetCellInternal(tableView, indexPath);
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this._underlyingTableSource.RowsInSection(tableview, section);
		}

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			return this._underlyingTableSource.GetHeightForHeader(tableView, section);
		}

		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			return this._underlyingTableSource.GetViewForHeader(tableView, section);
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return this._underlyingTableSource.NumberOfSections(tableView);
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			this._underlyingTableSource.RowSelected(tableView, indexPath);
		}

		public override string[] SectionIndexTitles(UITableView tableView)
		{
			return this._underlyingTableSource.SectionIndexTitles(tableView);
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return this._underlyingTableSource.TitleForHeader(tableView, section);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var justCell = GetCellInternal (tableView, indexPath);
			var uiCell = (BubbleCell)justCell as BubbleCell;

			uiCell.SetNeedsLayout();
			uiCell.LayoutIfNeeded();
			//return uiCell.ContentView.Frame.Height;

			//added 25 because we're putting the sender's name above the bubble
			return uiCell.GetHeight (tableView) + 25;

		}

		private UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
		{
			return this._underlyingTableSource.GetCell(tableView, indexPath);
		}
	}

	public static class PrivateExtensions
	{
		public static T GetFieldValue<T>(this object @this, Type type, string name)
		{
			var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
			return (T)field.GetValue(@this);
		}

		public static T GetPropertyValue<T>(this object @this, Type type, string name)
		{
			var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
			return (T)property.GetValue(@this);
		}
	}

}

