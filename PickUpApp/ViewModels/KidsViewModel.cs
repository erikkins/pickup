using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace PickUpApp
{
	public class KidsViewModel: BaseViewModel
	{
		public ObservableCollection<Grouping<string, Kid>> KidsSorted { get; set; }

		public ObservableCollection<Kid> Kids 
		{ get{ return App.myKids; } 
			set{
				if (value != App.myKids) {
					App.myKids = value;
					NotifyPropertyChanged ();
				}
			} 
		}



		public KidsViewModel ()
		{
			App.myKids = new ObservableCollection<Kid> ();
			App.otherKids = new ObservableCollection<Kid> ();
			//Kids = new ObservableCollection<Kid> ();
		}



		public KidsViewModel(MobileServiceClient client) //: this()
		{
			if (App.myKids == null) {
				App.myKids = new ObservableCollection<Kid> ();
			}
			if (App.otherKids == null) {
				App.otherKids = new ObservableCollection<Kid> ();
			}

			if (KidsSorted == null && App.myKids != null) {
				//this is just kludgy...need to trace why this was needed
				KidsSorted = new ObservableCollection<Grouping<string, Kid>>();
				KidsSorted.Add (new Grouping<string, Kid> ("MY KIDS", App.myKids));
				KidsSorted.Add (new Grouping<string, Kid> ("OTHER KIDS", App.otherKids));
			}
			//KidsSorted = new ObservableCollection<Grouping<string, Kid>> ();
			this.client = client;
			//LoadItemsCommand.Execute (null);

		}

		public override async Task ExecuteLoadItemsCommand ()
		{
			try
			{
				IsLoading = true;
				//var kids = await client.GetTable<Kid>().OrderBy(x => x.Firstname).ToListAsync();

				var kids = await client.InvokeApiAsync<List<Kid>>("getmykids");

				App.myKids.Clear();
				App.otherKids.Clear();
			
				foreach (var kid in kids)
				{
					if (kid.Mine)
					{
						App.myKids.Add(kid);
					}
					else{
						App.otherKids.Add(kid);
					}

				}
				//if (KidsSorted == null)
				//{
				//	KidsSorted = new ObservableCollection<Grouping<string, Kid>>();
				//}

				//KidsSorted.Clear();
				//KidsSorted.Add(new Grouping<string, Kid>("MY KIDS", App.myKids));
				//KidsSorted.Add(new Grouping<string, Kid>("OTHER KIDS", App.otherKids));

				FFMenuItem kidmenu = new FFMenuItem("Kids", App.myKids.Count);
				foreach (Kid k in App.myKids)
				{
					if (k.PhotoURL == null)
					{
						string initials = "";
						if (k.Firstname == null)
						{
							initials = k.Lastname.Substring(0,1).ToUpper();
						}
						else if (k.Lastname == null)
						{
							initials = k.Firstname.Substring(0,1).ToUpper();
						}
						else
						{
							initials = k.Firstname.Substring(0,1).ToUpper() + k.Lastname.Substring(0,1).ToUpper();
						}

						var dep = DependencyService.Get<PickUpApp.ICircleText>();
						string filename = dep.CreateCircleText(initials,50,50);
						k.PhotoURL = filename;
					}


					kidmenu.Photos.Add(k.PhotoURL);
				}

				if (App.menuItems.Any(mi => mi.MenuName == "Kids"))
				{
					//we already have this item, let's just update it
					System.Collections.Generic.IEnumerable<FFMenuItem> ffmi = from menus in App.menuItems where menus.MenuName == "Kids" select menus;
					ffmi.FirstOrDefault().Count = App.myKids.Count;

					//hope this updates?
				}
				else{
					App.menuItems.Insert(1, kidmenu);
				}

			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Error loading data Kids. Please check connectivity and try again.", "OK", "Cancel");
				System.Diagnostics.Debug.WriteLine (ex.Message + result.Status.ToString ());
				MessagingCenter.Send<Exception> (ex, "Error");
			}
			finally {
				IsLoading = false;
			}
		}
		/*
		public override async Task ExecuteAddEditCommand ()
		{
			if (IsLoading) return;
			IsLoading = true;

			try
			{
				var kid = client.GetTable<Kid>();
				MessagingCenter.Send(App.myAccount, "Refresh");
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				page.DisplayAlert("Error", "Error saving data. Please check connectivity and try again." + ex.Message, "OK", "Cancel");
			}

			IsLoading = false;
		}
		*/

	}
}

