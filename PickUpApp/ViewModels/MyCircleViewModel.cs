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

		public ObservableCollection<AccountCircle> AcceptedCircle 
		{ get{ return new ObservableCollection<AccountCircle>(App.myCircle.Where (ac => ac.Accepted == true)); } 
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
//				IMobileServiceTable<AccountCircle> circle = client.GetTable<AccountCircle>();
//				await circle.DeleteAsync(CurrentAccountCircle);
//				await ExecuteLoadItemsCommand();

				List<EmptyClass> resps = await client.InvokeApiAsync<AccountCircle, List<EmptyClass>>("deletecircle", CurrentAccountCircle);
				if (resps.Count == 0)
				{
					MessagingCenter.Send<EmptyClass>(new EmptyClass(), "CircleChanged");
				}
				else{
					MessagingCenter.Send<EmptyClass>(resps.FirstOrDefault(), "CircleChanged");
				}

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

				//this a separate list...there's gotta be a way to linq this out of myCircle, but step 1...
				App.myCoparents.Clear();
				//add myself to the coparents
				AccountCircle acMe = new AccountCircle();
				acMe.Email = App.myAccount.Email;
				acMe.Firstname = "Me";
				acMe.id = App.myAccount.id;
				acMe.Accepted = true;
				acMe.PhotoURL = App.myAccount.PhotoURL;
				acMe.UserId = App.myAccount.UserId;
				App.myCoparents.Add(acMe);


				foreach (var acct in circle)
				{					
					App.myCircle.Add(acct);

					//if it's a coparent...let's add it to that one too
					if (acct.Coparent)
					{
						App.myCoparents.Add(acct);
					}
				}
				try{
						FFMenuItem circlemenu = new FFMenuItem("Circle", App.myCircle.Count);
						/*
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
						*/

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
				MessagingCenter.Send<string>("mycircleviewmodel", "circleloaded");
			}
			IsLoading = false;  //redundant
		}
	}
}

