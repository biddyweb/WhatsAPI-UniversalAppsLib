using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WhatsAPI.UniversalApps.Libs.Utils.Common
{
    public class FileHelper
    {
        public static async Task<Stream> LoadFileFromResources(string path)
        {
            var uri = new Uri("ms-appx://"+path,UriKind.RelativeOrAbsolute);
            StorageFile file =await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            var randomAccessStream = await file.OpenReadAsync();
            Stream stream = randomAccessStream.AsStreamForRead();
            return stream;
        }


    }
}
