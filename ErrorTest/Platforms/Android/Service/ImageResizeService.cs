using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorTest.Service.PartialMethods;

public partial class ImageResizeService
{
    public partial byte[] ResizeImage(byte[] imageData, float width, float height, int quality, bool blRatioMaintain)
    {
        // Load the bitmap
        Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

        float oldWidth = (float)originalImage.Width;
        float oldHeight = (float)originalImage.Height;
        float scaleFactor = 0f;

        if (oldWidth > oldHeight)
        {
            scaleFactor = width / oldWidth;
        }
        else
        {
            scaleFactor = height / oldHeight;
        }

        //높이 너비의 비율을 유지할 경우
        float newHeight;
        float newWidth;

        if (blRatioMaintain)
        {
            newHeight = oldHeight * scaleFactor;
            newWidth = oldWidth * scaleFactor;
        }
        else //비율 유지 하지 않을 경우
        {
            newHeight = height;
            newWidth = width;
        }


        Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

        using (MemoryStream ms = new MemoryStream())
        {
            resizedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
            return ms.ToArray();
        }
    }
}