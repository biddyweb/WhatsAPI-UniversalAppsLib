using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Services;
using System.Net.Http;
using Google.Apis.Util.Store;
using WhatsAPI.UniversalApps.Sample.Models;
using WhatsAPI.UniversalApps.Sample.Repositories;
using Google.Apis.Http;
using Google.Apis.Contacts.v3;
using Google.Apis.Contacts.v3.Data;
using System.Collections.ObjectModel;
using Google.Apis.Oauth2.v2;

namespace WhatsAPI.UniversalApps.Sample.Helpers
{


    public static class ContactHelper
    {
        public static async Task SyncGoogleContactsAsync()
        {
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
    new Uri("ms-appx:///Assets/client_secrets.json"),
    new[] { ContactsService.Scope.FullContacts, Oauth2Service.Scope.UserinfoProfile, Oauth2Service.Scope.UserinfoEmail },
    "user",
    CancellationToken.None);
            await SyncProfileContactImage();
            var services = new ContactsService(new BaseClientService.Initializer()
            {

                ApplicationName = "WhassApp Windows 8",
                HttpClientInitializer = credential
            });
            var oauthSerivce = new Oauth2Service(new BaseClientService.Initializer()
            {

                ApplicationName = "WhassApp Windows 8",
                HttpClientInitializer = credential
            });
            var userInfo = await oauthSerivce.Userinfo.Get().ExecuteAsync();
            List<string> numberList = new List<string>();
            try
            {
                PhoneNumbers.PhoneNumberUtil util = PhoneNumbers.PhoneNumberUtil.GetInstance();
                ContactList response = await services.Contact.Get(userInfo.Email).ExecuteAsync();
                foreach (var c in response.Feed.Entries)
                {
                    if (c.PhoneNumbers != null)
                    {
                        string phoneNumber = String.Empty;
                        if (c.PhoneNumbers.FirstOrDefault().Text != null)
                        {
                            try
                            {
                                PhoneNumbers.PhoneNumber num = util.Parse(c.PhoneNumbers.FirstOrDefault().Text, "ID");
                                phoneNumber = num.CountryCode.ToString() + num.NationalNumber.ToString();
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("Exception was thrown: " + ex.Message);
                                continue;
                            }


                        }
                        AddContact(new Contacts() { name = c.Name.Text, phoneNumber = phoneNumber });
                        var check = GetContactByPhoneNumber(phoneNumber);
                        if (check == null)
                            numberList.Add(phoneNumber);
                    }
                }
            }
            catch (Google.GoogleApiException ex)
            {

            }
            SocketInstance.Instance.OnGetSyncResult += Instance_OnGetSyncResult;
            SocketInstance.Instance.SendSync(numberList.ToArray(), Libs.Constants.Enums.SyncMode.Delta, Libs.Constants.Enums.SyncContext.Background);
        }

        static void Instance_OnGetSyncResult(int index, string sid, Dictionary<string, string> existingUsers, string[] failedNumbers)
        {

            foreach (var c in existingUsers)
            {
                UpdateJid(c.Key, c.Value);
            }

            foreach (var d in failedNumbers)
            {
                DeleteContact(d);
            }

        }

        public static void AddContact(Contacts obj)
        {
            if (GetContactByPhoneNumber(obj.phoneNumber) == null)
            {
                DBProvider.DBConnection.Insert(obj);
            }
        }
        public static void UpdateJid(string phoneNumber, string jid)
        {
            var objects = GetContactByPhoneNumber(phoneNumber);
            if (objects != null && objects.jid != jid)
            {
                objects.jid = jid;
                DBProvider.DBConnection.Update(objects);
            }

        }

        public static void DeleteContact(string phoneNumber)
        {
            var objects = GetContactByPhoneNumber(phoneNumber);
            if (objects != null)
            {
                DBProvider.DBConnection.Delete(objects);
            }

        }

        public static Models.Contacts GetContactByPhoneNumber(string phoneNumber)
        {
            var numb = phoneNumber.Replace("+", "");
            Models.Contacts result = (Models.Contacts)DBProvider.DBConnection.Table<Models.Contacts>().Where(x => x.phoneNumber.Contains(numb)).FirstOrDefault();
            return result;
        }

        public static ObservableCollection<Contacts> GetAllContacts()
        {
            var contacsList = DBProvider.DBConnection.Table<Contacts>();
            ObservableCollection<Contacts> result = new ObservableCollection<Contacts>();
            foreach (Contacts item in contacsList)
            {
                result.Add(item);
            }
            return new ObservableCollection<Contacts>(result.OrderBy(x => x.name.ToLower()));
        }

        public static void SyncProfileContactImage(string jid)
        {

            var item = GetContactByJid(jid);
            SocketInstance.Instance.OnGetPhotoPreview += Instance_OnGetPhotoPreview;

            if (item != null && !string.IsNullOrEmpty(item.jid))
            {
                SocketInstance.Instance.SendGetPhoto(item.jid, String.Empty, false);
            }


        }
        static bool isSyncPhotostillRunning = false;
        public static async Task SyncProfileContactImage()
        {


            SocketInstance.Instance.OnGetPhotoPreview += Instance_OnGetPhotoPreview;
            SocketInstance.Instance.OnError += Instance_OnError;
            int i = 0;
            var syncList = GetAllContacts();
            foreach (var item in syncList)
            {
                i++;
                while (isSyncPhotostillRunning)
                {
                    await Task.Delay(1000);
                }
                if (item != null && !string.IsNullOrEmpty(item.jid) && string.IsNullOrEmpty(item.profileImage))
                {
                    SocketInstance.Instance.SendGetPhoto(item.jid, String.Empty, false);
                    isSyncPhotostillRunning = true;
                    System.Diagnostics.Debug.WriteLine("Proccessing for " + i + " of " + syncList.Count());
                }
            }


        }

        static void Instance_OnError(string id, string from, int code, string text)
        {
            if (id.ToString().Contains("get_photo"))
            {
                isSyncPhotostillRunning = false;
            }
        }

        public static Contacts GetContactByJid(string jid)
        {
            Models.Contacts result = (Models.Contacts)DBProvider.DBConnection.Table<Models.Contacts>().Where(x => x.jid.Contains(jid)).FirstOrDefault();
            return result;
        }

        public static void UpdateProfileImageByJid(string jid, string path)
        {
            Models.Contacts result = GetContactByJid(jid);
            if (result != null)
            {
                result.profileImage = path;
                DBProvider.DBConnection.Update(result);
            }

        }

        public static void UpdateProfileImageByPhoneNumber(string phoneNumber, string path)
        {
            Models.Contacts result = GetContactByPhoneNumber(phoneNumber);
            if (result != null)
            {
                result.profileImage = path;
                DBProvider.DBConnection.Update(result);
            }

        }

        async static void Instance_OnGetPhotoPreview(string from, string id, byte[] data)
        {
            isSyncPhotostillRunning = false;
            var contacts = GetContactByJid(from);
            string fileName = String.Empty;
            if (contacts != null && !string.IsNullOrEmpty(contacts.name))
            {
                fileName = WhatsAPI.UniversalApps.Libs.Utils.Common.FileHelper.RemoveSpaceFromFileName(contacts.name) + ".jpg";
            }
            else
            {
                fileName = from.Split('@')[0] + ".jpg";
            }
            var file = await WhatsAPI.UniversalApps.Libs.Utils.Common.FileHelper.SaveFileFromByteArray(data, fileName, "ProfileCache");
            if (file != null)
            {
                await Task.Run(() =>
                UpdateProfileImageByJid(from, file.Path));
            }
        }
    }
}
