using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace WhatsAPI.UniversalApps.Libs.Utils.Common
{
    public static class ImageHelper
    {
        public static void GetPhotoSizeByRatio(int width, int height, ref int newWidth, ref int newHeight)
        {
            if (width <= newWidth && height <= newHeight)
            {
                newWidth = width;
                newHeight = height;
                return;
            }

            if (width < height)
            {
                newWidth = newHeight * width / height;
            }
            else
            {
                newHeight = newWidth * height / width;
            }
        }

        public static async Task<StorageFile> ResizeImage(StorageFile BigFile, uint finalHeight, uint finalWidth)
        {
            using (var sourceStream = await BigFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);
                BitmapTransform transform = new BitmapTransform() { ScaledHeight = finalHeight, ScaledWidth = finalWidth };
                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.DoNotColorManage);
           
               
                byte[] buffer = pixelData.DetachPixelData();
               
                StorageFile file = await FileHelper.CreateLocalFile("newThumb.jpg", "Cache", true);
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                BitmapEncoder be = await BitmapEncoder.CreateForTranscodingAsync(stream, decoder);
                be.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, finalWidth, finalHeight, 96.0, 96.0, buffer);
                try
                {
                    await be.FlushAsync();
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }

                await stream.FlushAsync();
                stream.Dispose();

           

                return file;
            
            }
        }

        public static async Task<StorageFile> CropImage(IRandomAccessStream fs,  int newWidth, int newHeight)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fs);

            StorageFile file = await FileHelper.CreateLocalFile("cropped.jpg", "Cache", true);
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);

            BitmapEncoder enc = await BitmapEncoder.CreateForTranscodingAsync(stream, decoder);
            enc.BitmapTransform.ScaledWidth = (uint)newWidth;
            enc.BitmapTransform.ScaledHeight = (uint)newHeight;
            var MaxImageWidth = 100;
            var MaxImageHeight = 100;
            if (newWidth > MaxImageWidth || newHeight > MaxImageHeight)
            {
                BitmapBounds bounds = new BitmapBounds();
                if (newWidth > MaxImageWidth)
                {
                    bounds.Width = (uint)MaxImageWidth;
                    bounds.X =(uint) (newWidth - MaxImageWidth) / 2;
                }
                else bounds.Width = (uint)newWidth;
                if (newHeight > MaxImageHeight)
                {
                    bounds.Height =(uint) MaxImageHeight;
                    bounds.Y = (uint)(newHeight - MaxImageHeight) / 2;
                }
                else bounds.Height = (uint)newHeight;
                enc.BitmapTransform.Bounds = bounds;
            }

            try
            {
                await enc.FlushAsync();
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }

            await stream.FlushAsync();
            stream.Dispose();

            fs.Dispose();

            return file;
        }

        public static async Task<BitmapImage> ByteArrayToImageAsync(byte[] pixeByte)
        {
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                BitmapImage image = new BitmapImage();
                await stream.WriteAsync(pixeByte.AsBuffer());
                stream.Seek(0);
                image.SetSource(stream);
                return image;
            }
        }

        public static async Task<byte[]> ImageToByteArrayAsync(StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                using (DataReader reader = new DataReader(stream.GetInputStreamAt(0)))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    byte[] pixeByte = new byte[stream.Size];
                    reader.ReadBytes(pixeByte);
                    return pixeByte;
                }
            }
        }

    }
}
