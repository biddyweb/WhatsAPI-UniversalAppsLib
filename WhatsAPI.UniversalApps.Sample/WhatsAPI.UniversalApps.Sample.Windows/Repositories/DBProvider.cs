using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Sample.Models;
using Windows.Storage;

namespace WhatsAPI.UniversalApps.Sample.Repositories
{
    public static class DBProvider
    {
        private static string DBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "whatsapi.sqlite");
        private static SQLite.Net.SQLiteConnection _connection = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), DBPath);


        public static SQLite.Net.SQLiteConnection DBConnection
        {
            get
            {
                return _connection;
            }
        }

        public static async Task<bool> CheckDB()
        {
            var exist = await StorageFile.GetFileFromPathAsync(DBPath);
            var attributes = await exist.GetBasicPropertiesAsync();
            if (exist == null || (attributes.Size == 0))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static async Task InitializeDB()
        {

            if (!await CheckDB())
            {
                DBConnection.CreateTable<Config>();
                DBConnection.CreateTable<Contacts>();
                DBConnection.CreateTable<Messages>();
            }

        }

    }
}
