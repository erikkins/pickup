using System;

namespace PickUpApp
{
	public interface IHUD
	{
		void showHUD(string text);
		void hideHUD();
		void showToast(string text);
	}
}

