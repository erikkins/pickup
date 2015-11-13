using System;
using System.Collections.Generic;

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
				stcFirstName = new SimpleBoundTextCell ("First name", "FirstName");
				ts.Add (stcFirstName);
				stcLastName = new SimpleBoundTextCell ("Last name", "LastName");
				ts.Add (stcLastName);
				stcCellPhone = new SimpleBoundTextCell ("Mobile phone", "Phone");
				ts.Add (stcCellPhone);
				stcEmail = new SimpleBoundTextCell ("Email", "Email");
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

