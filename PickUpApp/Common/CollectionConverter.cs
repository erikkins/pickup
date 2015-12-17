using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace PickUpApp
{
	public class CollectionConvertor: IValueConverter
	{
		public CollectionConvertor(){}

		//TODO: Make this more generic!
		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{			
			return ((ObservableCollection<MessageView>)value).Count.ToString();
		}
		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException ();
		}
	}

}



