using System;
using System.Collections.Generic;
using RestSharp.Portable;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace PickUpApp
{
	public partial class AddEditPlace : ContentPage
	{
		public AddEditPlace (AccountPlace selectedPlace)
		{
			InitializeComponent ();

			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			this.ViewModel = new AccountPlaceViewModel (App.client);
			this.ViewModel.CurrentPlace = selectedPlace;

			btnSave.Clicked += async delegate(object sender, EventArgs e) {

				//before we save this, we have to validate the address to make sure the GPS coordinates work
				using (var client = new RestClient(new Uri("https://maps.googleapis.com/maps/api/place/")))
				{
					var request = new RestRequest("textsearch/json", System.Net.Http.HttpMethod.Get);	

					request.AddQueryParameter ("query", txtAddress.Text);
					request.AddQueryParameter("key", "AIzaSyDpVbafIazS-s6a82lp4fswviB_Kb0fbmQ");

					var result =  await client.Execute(request);
					GoogleResponse yr = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleResponse>(System.Text.Encoding.UTF8.GetString(result.RawBytes, 0, result.RawBytes.Length));

					ObservableCollection<GoogleResult> places = yr.Results;
					//there should really be one and only one
					if (places.Count == 1)
					{
						//this is really it!
						this.ViewModel.CurrentPlace.Address = places[0].Address;
						this.ViewModel.CurrentPlace.Latitude = places[0].Geometry.Location.Latitude.ToString();
						this.ViewModel.CurrentPlace.Longitude = places[0].Geometry.Location.Longitude.ToString();
						await this.ViewModel.ExecuteAddEditCommand();
					}
					//now deal with 0 or more than 1
				}

			};

			btnCancel.Clicked += async delegate(object sender, EventArgs e) {
				await Navigation.PopModalAsync();
			};
		}


		protected AccountPlaceViewModel ViewModel
		{
			get { return this.BindingContext as AccountPlaceViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

