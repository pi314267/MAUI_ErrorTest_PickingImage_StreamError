using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ErrorTest.Service.PartialMethods;

public partial class ImageResizeService
{
    public partial byte[] ResizeImage(byte[] imageData, float width, float height, int quality, bool blRatioMaintain)
    {
        UIImage originalImage = ImageFromByteArray(imageData);


        float oldWidth = (float)originalImage.Size.Width;
        float oldHeight = (float)originalImage.Size.Height;
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

        //create a 24bit RGB image
        using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
            (int)newWidth, (int)newHeight, 8,
            (int)(4 * newWidth), CGColorSpace.CreateDeviceRGB(),
            CGImageAlphaInfo.PremultipliedFirst))
        {

            //RectangleF imageRect = new RectangleF (0, 0, newWidth, newHeight);
            CGRect imageRect = new CGRect(0, 0, newWidth, newHeight);

            // draw the image
            context.DrawImage(imageRect, originalImage.CGImage);

            UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage());

            // save the image as a jpeg
            return resizedImage.AsJPEG((float)quality).ToArray();
        }
    }

    public static UIKit.UIImage ImageFromByteArray(byte[] data)
    {
        if (data == null)
        {
            return null;
        }

        UIKit.UIImage image;
        try
        {
            image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
        }
        catch (Exception e)
        {
            Console.WriteLine("Image load failed: " + e.Message);
            return null;
        }
        return image;
    }
}
