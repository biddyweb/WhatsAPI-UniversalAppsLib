using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net;
using Windows.Storage.Streams;

namespace WhatsAPI.UniversalApps.Libs.Utils.Common
{
    public class FileHelper
    {
        public static async Task<Stream> LoadFileFromResources(string path)
        {
            var uri = new Uri("ms-appx://" + path, UriKind.RelativeOrAbsolute);
            StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            var randomAccessStream = await file.OpenReadAsync();
            Stream stream = randomAccessStream.AsStreamForRead();
            return stream;
        }

        public static async Task<BitmapImage> LoadImage(string fileName, string folderName = "")
        {
            BitmapImage img = new BitmapImage();

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder folder = null;

            if (string.IsNullOrEmpty(folderName))
                folder = localFolder;
            else
            {
                folder = await IsFolderExist(folderName);
                if (folder == null)
                    return null;
            }

            StorageFile file = null;
            file = await IsFileExist(folder, fileName);

            if (file == null)
                return null;

            var s = await file.OpenAsync(FileAccessMode.Read);
            img.SetSource(s);
            return img;
        }

        static public async Task<StorageFolder> IsFolderExist(string name)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFolder folder = null;
            try
            {
                folder = await localFolder.GetFolderAsync(name);
            }
            catch
            {
            }

            return folder;
        }

        public static async Task<StorageFile> CreateLocalFile(string fileName, string folderPath, bool replaceExisting = false)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder folder = null;

            if (folderPath.Contains("\\"))
            {
                string[] p = Helpers.Split(folderPath, new char[] { '\\' });

                folder = localFolder;
                foreach (var f in p)
                {
                    StorageFolder currentFolder = null;
                    try
                    {
                        currentFolder = await folder.GetFolderAsync(f);
                    }
                    catch
                    {
                    }

                    if (currentFolder == null)
                        folder = await folder.CreateFolderAsync(f, CreationCollisionOption.OpenIfExists);
                    else
                        folder = currentFolder;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(folderPath))
                    folder = localFolder;
                else
                {
                    folder = await IsFolderExist(folderPath);
                    if (folder == null)
                        folder = await localFolder.CreateFolderAsync(folderPath, CreationCollisionOption.OpenIfExists);
                }
            }

            StorageFile file = null;
            file = await IsFileExist(folder, fileName);

            if (replaceExisting && file != null)
            {
                await file.DeleteAsync();
                file = null;
            }

            if (file == null)
                file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

            return file;
        }
        public static async Task<StorageFile> IsFileExist(string name)
        {
            string folderName = "";
            string fileName = name;
            if (name.Contains("\\"))
            {
                string[] p = Helpers.Split(name, '\\');

                folderName = p[0];
                fileName = p[1];
            }

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            if (!string.IsNullOrEmpty(folderName))
                return await IsFileExist(folderName, fileName);
            else
                return await IsFileExist(localFolder, name);
        }

        public static async Task<StorageFile> IsFileExist(string folderPath, string name)
        {
            StorageFolder folder = null;
            if (folderPath.Contains("\\"))
            {
                string[] p = Helpers.Split(folderPath, '\\');

                folder = ApplicationData.Current.LocalFolder;
                foreach (var f in p)
                {
                    StorageFolder currentFolder = null;
                    try
                    {
                        currentFolder = await folder.GetFolderAsync(f);
                    }
                    catch
                    {
                    }

                    if (currentFolder == null)
                        return null;

                    folder = currentFolder;
                }
            }
            else
            {
                folder = await IsFolderExist(folderPath);
                if (folder == null)
                    return null;
            }

            return await IsFileExist(folder, name);
        }

        public static async Task<StorageFile> IsFileExist(StorageFolder folder, string name)
        {
            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(name);
            }
            catch { }

            return file;
        }

        public static async Task SaveImageFromWeb(IStorageFile file, Uri uri)
        {
     
            using (var client = new HttpClient())
            {
                try
                {
                    var bytes = await client.GetBufferAsync(uri);
                    await FileIO.WriteBytesAsync(file, bytes.ToArray());
                }
                catch (Exception ex)
                {
                    Logger.Log.WriteLog("Error When SaveImage : " + ex.Message);
                }
            }
        }

        public static async Task<StorageFile> SaveFileFromByteArray(byte[] imageArray,string fileName = "tempProfpic.jpg",string folderName = "Cache")
        {
                try
                {
                    var file = await CreateLocalFile(fileName, folderName, true);
                    await FileIO.WriteBytesAsync(file, imageArray);
                    return file;
                }
                catch (Exception ex)
                {
                    Logger.Log.WriteLog("Error When Save ByteArray to File : " + ex.Message);
                    return null;
                }
        }

        public static async Task DeleteFile(string name, string folderName = "")
        {
            StorageFolder folder = await IsFolderExist(folderName);

            if (folder == null)
                return;

            StorageFile file = await IsFileExist(folder, name);
            if (file != null)
                await file.DeleteAsync();
        }

        public static string RemoveSpaceFromFileName(string name)
        {
            return name.Replace(" ", "").Trim();
        }

        public static async Task<byte[]> ConvertStorageFileToByteArray(StorageFile file)
        {
            byte[] fileBytes = null;
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            return fileBytes;
        }
    }
}
