using System;
using PickUpApp.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;

namespace PickUpApp
{
	public class TestViewModel : BaseViewModel
	{
		public TestViewModel (MobileServiceClient client)
		{
			this.client = client;
		}


		private Command testValidationCommand;
		public Command TestValidationCommand
		{
			get { return testValidationCommand ?? (testValidationCommand = new Command<string>(async(s) => await TestValidation())); }
		}
		public async System.Threading.Tasks.Task TestValidation()
		{
			Account a = new Account ();
			a.id = "-1";
			a.Email = "-1";
			a.Validated = true;

			List<EmptyClass> bPre = await client.InvokeApiAsync<Account, List<EmptyClass>>("setvalidation", a);
		}

		private Command testCircleCommand;
		public Command TestCircleCommand
		{
			get { return testCircleCommand ?? (testCircleCommand = new Command<string>(async(s) => await TestCircle())); }
		}
		public async System.Threading.Tasks.Task TestCircle()
		{
			AccountCircle ac = new AccountCircle ();
			ac.id = "-1";
			ac.Firstname = "-1";
			ac.Email = "erikkins@gmail.com";


			List<EmptyClass> bPre = await client.InvokeApiAsync<AccountCircle, List<EmptyClass>>("savecircle", ac);
		}


	}
}

