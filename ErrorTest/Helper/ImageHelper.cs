using ErrorTest.Service.PartialMethods;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorTest.Helper
{
    public class ImageHelper
    {
        public static byte[] ImagePathToByte(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        public static byte[] ResizeImage(byte[] imageData, float width, float height, int quality, bool blRatioMaintain)
        {
            ImageResizeService ImageService = new ImageResizeService();
            byte[] orientation = ImageService.ResizeImage(imageData, width, height, quality, blRatioMaintain);
            return orientation;
        }

        public static Microsoft.Maui.Controls.Image ByteToImage(byte[] byteArray)
        {
            Microsoft.Maui.Controls.Image image = new Microsoft.Maui.Controls.Image();
            Stream stream = new MemoryStream(byteArray);
            image.Source = ImageSource.FromStream(() => { return stream; });

            return image;
        }

        public static Microsoft.Maui.Controls.Image ImageThumnailMaker(string str_imagespath)
        {
            SkiaSharp.SKImage sKImage = null;
            try
            {
                #region Image Combine 부분 With SkiImage
                //liThumNailPath.Add(DependencyService.Get<IImageResize>().ResizeImage(item, "test", 40, 40));
                byte[] bimage = ImageHelper.ImagePathToByte(str_imagespath);
                //bimage.CopyTo();

                //이렇게 해야 나중에 Relative Layout을 Remove 할 때 Stream reference 오류가 발생하지 않는다.
                byte[] bimagecopy = bimage.ToArray(); //복사 하는 부분임

                //Image imgOrigin = ImageHelper.ByteToImage(bimage);
                //double targetRatio = imgOrigin.Width / imgOrigin.Height;

                byte[] resizebyteimage = ResizeImage(bimagecopy, 1000, 1000, 100, false); //Thumnail Image byte로 표현

                sKImage = CombineWithDelete(resizebyteimage, str_imagespath); //Image 결합함
                SKData encoded = sKImage.Encode(SKEncodedImageFormat.Png, 100);
                byte[] Skresizebyteimage = encoded.ToArray();

                Microsoft.Maui.Controls.Image imgThumnail = ImageHelper.ByteToImage(Skresizebyteimage);
                #endregion

                return imgThumnail;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
            finally
            {
                //clean up memory
                sKImage.Dispose();

            }
        }

        public static SKImage CombineWithDelete(byte[] files, string Imagepath)
        {
            //read all images into memory
            //List<SKBitmap> images = new List<SKBitmap>();
            Dictionary<string, SKBitmap> images = new Dictionary<string, SKBitmap>();
            SKImage finalImage = null;

            try
            {
                int width = 0;
                int height = 0;


                //create a bitmap from the file and add it to the list
                SKBitmap bitmap = SKBitmap.Decode(files);
                //update the size of the final bitmap
                width += bitmap.Width;
                height += bitmap.Height;

                images.Add("img", bitmap);

                SKBitmap cancelBitmap = SKBitmap.Decode(EmbeddedResourceProvider.Load(EmbeddedMedia.cancelPng));




                images.Add("cancel", cancelBitmap);






                //get a surface so we can draw an image
                //using (var tempSurface = SKSurface.Create(new SKImageInfo((int)(width + 100), (int)(height + 150))))
                using (var tempSurface = SKSurface.Create(new SKImageInfo((int)(width), (int)(height))))
                {
                    //get the drawing canvas of the surface
                    var canvas = tempSurface.Canvas;

                    //set background color
                    canvas.Clear(SKColors.Transparent);
                    //canvas.Clear(SKColors.White);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    int offsetTop = 0;

                    int realImageW = 0;
                    int realImageH = 0;

                    foreach (KeyValuePair<string, SKBitmap> image in images)
                    {
                        if (image.Key.Equals("img"))
                        {
                            //offsetTop += (int)((image.Value).Height / 0.3);
                            canvas.DrawBitmap(image.Value, SKRect.Create(offset, offsetTop, (int)((image.Value).Width), (int)((image.Value).Height)));

                            offset += (int)((image.Value).Width * 0.7); //x 축의 70%의 비율에서 그림을 그림
                            offsetTop = 0;

                            realImageW = (int)((image.Value).Width);
                            realImageH = (int)((image.Value).Height);

                        }
                        else if (image.Key.Equals("cancel"))
                        {
                            //깔끔한 이미지를 위해 덧 표현
                            SKPaint sKPaint = new SKPaint();
                            sKPaint.Color = SKColors.White;

                            //위치 확인을 위한 페인트
                            SKPaint sKPaint2 = new SKPaint();
                            sKPaint2.Color = SKColors.Red;

                            ////사각형 그려보기 Test
                            //canvas.DrawRect(offset, offsetTop, (image.Value).Width + standardRatio, (image.Value).Height + standardRatio, sKPaint);
                            //canvas.DrawRect(offset, offsetTop, realImageW - offset, realImageW - offset, sKPaint2);

                            ///원으로 그림을 그려서 뒷 배경을 깔끔하게 하기
                            ///각 변수는 다음을 의미함
                            ///(x축의 원 중심 좌표, Y축의 원 중심 좌표, 반지름, 채울 색)
                            canvas.DrawCircle(offset + ((realImageW - offset) / 2), offsetTop + ((realImageW - offset) / 2), (realImageW - offset) / 2, sKPaint);

                            ////취소이미지 처리 방법 1[잘못된 방법임] 여기서 Image value 값은 취소 이미지의 값임
                            //canvas.DrawBitmap(image.Value, SKRect.Create(offset, offsetTop, (image.Value).Width + standardRatio, (image.Value).Height + standardRatio));
                            ////취소이미지 처리 방법 2[Rect 생성으로 처리 필요]
                            //canvas.DrawBitmap(image.Value, SKRect.Create(offset, offsetTop, realImageW - offset, realImageW - offset));

                            ////취소이미지 처리 방법 3[최종 결정]
                            SKRect sKRect = SKRect.Create(offset, offsetTop, realImageW - offset, realImageW - offset);
                            canvas.DrawBitmap(image.Value, sKRect);

                        }

                    }

                    // return the surface as a manageable image
                    finalImage = tempSurface.Snapshot();
                }

                //return the image that was just drawn
                return finalImage;
            }
            catch (Exception e)
            {
                return null;
                throw e;

            }
            finally
            {
                //clean up memory
                foreach (KeyValuePair<string, SKBitmap> image in images)
                {
                    (image.Value).Dispose();
                }
            }
        }

    }
}
