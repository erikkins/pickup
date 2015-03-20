using System;
using System.IO;


namespace PickUpApp
{
	public interface IImageResizer
	{
		byte[] ResizeImage(byte[] imageData, float width, float height, string filePath);
	}
}
	

//#if WINDOWS_PHONE
//using Microsoft.Phone;
//using System.Windows.Media.Imaging;
//#endif
	
//
//		#if WINDOWS_PHONE
//
//		public static byte[] ResizeImageWinPhone (byte[] imageData, float width, float height)
//		{
//		byte[] resizedData;
//
//		using (MemoryStream streamIn = new MemoryStream (imageData))
//		{
//		WriteableBitmap bitmap = PictureDecoder.DecodeJpeg (streamIn, (int)width, (int)height);
//
//		using (MemoryStream streamOut = new MemoryStream ())
//		{
//		bitmap.SaveJpeg(streamOut, (int)width, (int)height, 0, 100);
//		resizedData = streamOut.ToArray();
//		}
//		}
//		return resizedData;
//		}
//
//		#endif

	

