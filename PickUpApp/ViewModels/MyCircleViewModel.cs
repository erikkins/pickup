using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.IO;

namespace PickUpApp
{
	public class MyCircleViewModel:BaseViewModel
	{
		public ObservableCollection<Account> Circle 
		{ get{ return App.myCircle; } 
			set{ if (value == App.myCircle) {
					App.myCircle = value;
					NotifyPropertyChanged ();
					}
				} 
		}
	
		public MyCircleViewModel ()
		{
			App.myCircle = new ObservableCollection<Account> ();
		}

		public MyCircleViewModel(MobileServiceClient client) //: this()
		{
			if (App.myCircle == null) {
				App.myCircle = new ObservableCollection<Account> ();
			}
			this.client = client;
			//LoadItemsCommand.Execute (null);
		}



		public override async Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try
			{
				var circle = await client.InvokeApiAsync<List<Account>>("getmycircle");
				App.myCircle.Clear();
				foreach (var acct in circle)
				{
					App.myCircle.Add(acct);
				}
				try{
						FFMenuItem circlemenu = new FFMenuItem("Circle", App.myCircle.Count);
						foreach (Account circ in App.myCircle)
						{
							if (circ.PhotoURL == null)
							{
							string initials = "";
							if (circ.Firstname == null)
							{
								initials = circ.Lastname.Substring(0,1).ToUpper();
							}
							else if (circ.Lastname == null)
							{
								initials = circ.Firstname.Substring(0,1).ToUpper();
							}
							else
							{
								initials = circ.Firstname.Substring(0,1).ToUpper() + circ.Lastname.Substring(0,1).ToUpper();
							}
								
							var dep = DependencyService.Get<PickUpApp.ICircleText>();
							string filename = dep.CreateCircleText(initials,50,50);
							circ.PhotoURL = filename;

							circlemenu.Photos.Add(filename);
							}
							else
							{
								circlemenu.Photos.Add(circ.PhotoURL);
							}
						}
						App.menuItems.Insert(2, circlemenu);
				}
				catch(Exception exer)
				{
					System.Diagnostics.Debug.WriteLine(exer);
				}

			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Circle. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine ("CircleEx " + ex.Message + result.Status.ToString ());
			}
			finally{
				IsLoading = false;
			}
			IsLoading = false;  //redundant
		}
	}
}

