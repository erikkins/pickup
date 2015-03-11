using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using PickUpApp.iOS.Renderers;
using PickUpApp;
using UIKit;

[assembly:ExportRenderer(typeof(PullToRefreshListView), typeof(PullToRefreshListViewRenderer))]
namespace PickUpApp.iOS.Renderers
{
	public class PullToRefreshListViewRenderer : ListViewRenderer
	{
		public PullToRefreshListViewRenderer ()
		{
		}

		private FormsUIRefreshControl _refreshControl;

	
		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
		{
			base.OnElementChanged(e);

			var pullToRefreshListView = (PullToRefreshListView)e.NewElement;
			_refreshControl = new FormsUIRefreshControl
			{
				RefreshCommand = pullToRefreshListView.RefreshCommand,
				Message = pullToRefreshListView.Message
			};

			Control.AddSubview(_refreshControl);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			var pullToRefreshListView = (PullToRefreshListView)Element;

			if (e.PropertyName == PullToRefreshListView.IsRefreshingProperty.PropertyName)
			{
				_refreshControl.IsRefreshing = pullToRefreshListView.IsRefreshing;
			}
			else if (e.PropertyName == PullToRefreshListView.MessageProperty.PropertyName)
			{
				_refreshControl.Message = pullToRefreshListView.Message;
			}
			else if (e.PropertyName == PullToRefreshListView.RefreshCommandProperty.PropertyName)
			{
				_refreshControl.RefreshCommand = pullToRefreshListView.RefreshCommand;
			}
		}


		/*
		protected override void OnModelSet(VisualElement model)
		{
			base.OnModelSet(model);

			var pullToRefreshListView = (PullToRefreshListView)this.Model; 
			var tableView = (UITableView) this.Control;

			refreshControl = new FormsUIRefreshControl ();
			refreshControl.RefreshCommand = pullToRefreshListView.RefreshCommand;
			refreshControl.Message = pullToRefreshListView.Message;
			tableView.AddSubview (refreshControl);
		}


		/// <summary>
		/// Called when the underlying model's properties are changed
		/// </summary>
		/// <param name="sender">Model</param>
		/// <param name="e">Event arguments</param>
		protected override void OnHandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnHandlePropertyChanged(sender, e);

			var pullToRefreshListView = (PullToRefreshListView)this.Model; 


			if (e.PropertyName == PullToRefreshListView.IsRefreshingProperty.PropertyName) {
				refreshControl.IsRefreshing = pullToRefreshListView.IsRefreshing;
			} else if (e.PropertyName == PullToRefreshListView.MessageProperty.PropertyName) {
				refreshControl.Message = pullToRefreshListView.Message;
			} else if (e.PropertyName == PullToRefreshListView.RefreshCommandProperty.PropertyName) {
				refreshControl.RefreshCommand = pullToRefreshListView.RefreshCommand;
			}
		}
		*/
	}
}