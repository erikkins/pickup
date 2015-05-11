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
						if (places[0].Address == "United States" && places[0].Geometry.Location != null)
						{
							//it has latlong but not a readable address...let's just leave it how it was entered
							this.ViewModel.CurrentPlace.Address = txtAddress.Text;
						}
						else
						{
							this.ViewModel.CurrentPlace.Address = places[0].Address;
						}
						this.ViewModel.CurrentPlace.Latitude = places[0].Geometry.Location.Latitude.ToString();
						this.ViewModel.CurrentPlace.Longitude = places[0].Geometry.Location.Longitude.ToString();
						await this.ViewModel.ExecuteAddEditCommand();
					}
					else{
						if (places.Count == 0)
						{
							//no matches
							await DisplayAlert("Oops", "No address could be found for " + txtAddress.Text + ". Please revise.", "OK");
						}
						else
						{
							//for now, just hope they can modify it...later we'll add a picker
							await DisplayAlert("Oops", places.Count.ToString() + " addresses were found for " + txtAddress.Text + ". Please revise.", "OK");
						}
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

