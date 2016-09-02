using System;
using XLabs.Forms.Controls;
using Plugin.Calendars.Abstractions;
//using Calendars.Plugin.Abstractions;

namespace PickUpApp
{
	public class PUCalendarEvent: CalendarEvent
	{

		public string StartTime{
			get{
				return Start.ToString("t");
			}
		}
		public string EndTime{
			get {
				return End.ToString ("t");
			}
		}

	}
}

