using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace PickUpApp
{
	public partial class TurnByTurnView : ContentPage
	{
		public TurnByTurnView (double destinationLatitude, double destinationLongitude, ObservableCollection<BingItineraryItem> itineraryItems)
		{
			InitializeComponent ();
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			ViewModel = new TurnByTurnViewModel (destinationLatitude, destinationLongitude, itineraryItems);
		}

		protected TurnByTurnViewModel ViewModel
		{
			get { return this.BindingContext as TurnByTurnViewModel; }
			set { this.BindingContext = value; }
		}
		void OnCloseClicked(object sender, EventArgs args)
		{
			Navigation.PopModalAsync ();
		}

	}


}

