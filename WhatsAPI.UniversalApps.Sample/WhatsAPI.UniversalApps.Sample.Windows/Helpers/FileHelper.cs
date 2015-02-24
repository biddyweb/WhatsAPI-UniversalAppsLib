using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace WhatsAPI.UniversalApps.Sample.Helpers
{
    public static class FileHelper
    {
        public static async Task WriteToFile(string file,byte[] data)
        {
            try
            {
                var buffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(data);
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(Windows.Storage.ApplicationData.Current.LocalFolder.Path);

                StorageFile sampleFile = await folder.CreateFileAsync(file, CreationCollisionOption.ReplaceExisting); ;

                await Windows.Storage.FileIO.WriteBufferAsync(sampleFile, buffer);
            }
            catch (Exception ex)
            {

            }
        }

        public static async Task<byte[]> ReadFile(string file)
        {
            try
            {
                StorageFile sampleFile = await StorageFile.GetFileFromPathAsync(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,Constants.ConfigKey.WhatsAppNextChallengeFile));
                var buffer = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);
                var passwordBytes = buffer.ToArray();
                return passwordBytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
