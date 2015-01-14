using System;

namespace PickUpApp
{
	public class Util
	{
		public static DateTime RoundUp(DateTime dt, TimeSpan d)
		{
			return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
		}

		public Util ()
		{
		}
	}
}

