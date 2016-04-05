using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace PickUpApp
{
	public partial class AddEditContact : ContentPage
	{
		SimpleImageCell sicPhoto;
		SimpleBoundTextCell stcFirstName;
		SimpleBoundTextCell stcLastName;
		SimpleBoundTextCell stcCellPhone;
		SimpleBoundTextCell stcEmail;
		SimpleBoundRadioCell srcCoParent;
		SimpleBoundLabelCell slcFirstName;
		SimpleBoundLabelCell slcLastName;
		SimpleBoundLabelCell slcCellPhone;
		SimpleBoundLabelCell slcEmail;

		public AddEditContact (LocalContact selectedContact, bool AllowEdit)
		{
			InitializeComponent ();
			this.BackgroundColor = Color.FromRgb (238, 236, 243);
			this.ViewModel = new SelectContactViewModel (App.client);

			this.ViewModel.CurrentContact = selectedContact;

			this.ToolbarItems.Add (new ToolbarItem ("Done", null, async() => {

				//set the required fields
				if (AllowEdit)
				{
					if (string.IsNullOrEmpty(this.ViewModel.CurrentContact.FirstName))
					{
						await DisplayAlert("Uh oh", "You must supply First Name for this contact!", "OK");
						return;
					}
					if (string.IsNullOrEmpty(this.ViewModel.CurrentContact.LastName))
					{
						await DisplayAlert("Uh oh", "You must supply Last Name for this contact!", "OK");
						return;
					}
					if (string.IsNullOrEmpty(this.ViewModel.CurrentContact.Email))
					{
						await DisplayAlert("Uh oh", "You must supply Email for this contact!", "OK");
						return;
					}

					if (!Regex.Match(this.ViewModel.CurrentContact.Email.ToLower(), Util.emailRegex).Success)
					{
						await DisplayAlert("Uh oh", "You must supply a valid email", "OK");
						return;     
					}

					if (!string.IsNullOrEmpty(this.ViewModel.CurrentContact.Phone))
					{
						//if (!Regex.Match(this.ViewModel.CurrentContact.Phone, Util.phoneRegex).Success && !Regex.Match(this.ViewModel.CurrentContact.Phone, Util.simplePhoneRegex).Success)
						if ( !Regex.Match(this.ViewModel.CurrentContact.Phone, Util.simplePhoneRegex).Success)
						{
							await DisplayAlert("Uh oh", "You must supply a valid phone number for this contact!", "OK");
							return;     
						}
					}

				}

				//make sure we set the coparent value
				this.ViewModel.CurrentContact.Coparent = srcCoParent.IsChecked;

				await this.ViewModel.ExecuteAddEditCommand();

			}));

			ExtendedTableView tv = new ExtendedTableView ();
			tv.Intent = TableIntent.Data;
			tv.BackgroundColor = Color.FromRgb (238, 236, 243);
			tv.BindingContext = ViewModel.CurrentContact;
			tv.HasUnevenRows = true;
			//tv.RowHeight = 75;

			TableSection ts = new TableSection ();
			if (AllowEdit) {
				stcFirstName = new SimpleBoundTextCell ("First name", "FirstName", Keyboard.Text);
				ts.Add (stcFirstName);
				stcLastName = new SimpleBoundTextCell ("Last name", "LastName", Keyboard.Text);
				ts.Add (stcLastName);
				stcCellPhone = new SimpleBoundTextCell ("Mobile phone", "Phone", Keyboard.Telephone);
				ts.Add (stcCellPhone);
				stcEmail = new SimpleBoundTextCell ("Email", "Email", Keyboard.Email);
				ts.Add (stcEmail);
			} else {
				sicPhoto = new SimpleImageCell (selectedContact.PhotoId);
				ts.Add (sicPhoto);
				slcFirstName = new SimpleBoundLabelCell ("First name", "FirstName");
				ts.Add (slcFirstName);
				slcLastName = new SimpleBoundLabelCell ("Last name", "LastName");
				ts.Add (slcLastName);
				slcCellPhone = new SimpleBoundLabelCell ("Mobile phone", "Phone");
				ts.Add (slcCellPhone);
				slcEmail = new SimpleBoundLabelCell ("Email", "Email");
				ts.Add (slcEmail);
			}

			srcCoParent = new SimpleBoundRadioCell ("Co-Parent", "Coparent");

			ts.Add (srcCoParent);
			tv.Root.Add (ts);
			stacker.Children.Add (tv);

		}

		protected SelectContactViewModel ViewModel
		{
			get { return this.BindingContext as SelectContactViewModel; }
			set { this.BindingContext = value; }
		}
	}
}

