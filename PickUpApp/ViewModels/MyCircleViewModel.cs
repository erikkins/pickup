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
		public ObservableCollection<AccountCircle> Circle 
		{ get{ return App.myCircle; } 
			set{ if (value == App.myCircle) {
					App.myCircle = value;
					NotifyPropertyChanged ();
					}
				} 
		}

		public AccountCircle CurrentAccountCircle;
	
		public MyCircleViewModel ()
		{
			App.myCircle = new ObservableCollection<AccountCircle> ();
		}

		public MyCircleViewModel(MobileServiceClient client) //: this()
		{
			if (App.myCircle == null) {
				App.myCircle = new ObservableCollection<AccountCircle> ();
			}
			this.client = client;
			//LoadItemsCommand.Execute (null);
		}

		public override async Task ExecuteDeleteCommand ()
		{
			IsLoading = true;

			try
			{
				IMobileServiceTable<AccountCircle> circle = client.GetTable<AccountCircle>();
				await circle.DeleteAsync(CurrentAccountCircle);
				await ExecuteLoadItemsCommand();
			}
			catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
			}
			finally{
				IsLoading = false;
			}
		}


		public override async Task ExecuteLoadItemsCommand ()
		{
			IsLoading = true;
			try
			{
				var circle = await client.InvokeApiAsync<List<AccountCircle>>("getmycircle");
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

					if (App.menuItems.Any(mi => mi.MenuName == "Circle"))
					{
						//we already have this item, let's just update it
						System.Collections.Generic.IEnumerable<FFMenuItem> ffmi = from menus in App.menuItems where menus.MenuName == "Circle" select menus;
						ffmi.FirstOrDefault().Count = App.myCircle.Count;

						//hope this updates?
					}
					else{
						App.menuItems.Insert(2, circlemenu);
					}
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

