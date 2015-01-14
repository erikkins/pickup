using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PickUpApp
{
	public class BaseModel:INotifyPropertyChanged
	{
		public BaseModel ()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}

