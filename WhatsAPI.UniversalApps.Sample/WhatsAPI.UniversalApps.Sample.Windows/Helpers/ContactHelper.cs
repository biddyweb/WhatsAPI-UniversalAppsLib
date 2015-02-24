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

namespace WhatsAPI.UniversalApps.Sample.Helpers
{


    public static class ContactHelper
    {
        public static async Task SyncGoogleContactsAsync()
        {
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
    new Uri("ms-appx:///Assets/client_secrets.json"),
    new[] { ContactsService.Scope.FullContacts },
    "user",
    CancellationToken.None);

            var services = new ContactsService(new BaseClientService.Initializer()
            {

                ApplicationName = "WhassApp Windows 8",
                HttpClientInitializer = credential
            });
            List<string> numberList = new List<string>();
            try
            {
                PhoneNumbers.PhoneNumberUtil util = PhoneNumbers.PhoneNumberUtil.GetInstance();
                ContactList response = await services.Contact.Get(credential.UserId).ExecuteAsync();
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
            return result;
        }
    }
}
