using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Sample.Helpers;
using WhatsAPI.UniversalApps.Sample.Models;

namespace WhatsAPI.UniversalApps.Sample.ViewModels
{
    public class ChatPageViewModel : BindableBase
    {
        private ObservableCollection<Contacts> contacts;
        public ObservableCollection<Contacts> Contacts
        {
            get
            {
                return contacts;
            }
            set
            {
                contacts = value;
                OnPropertyChanged(() => Contacts);
            }
        }

        private void OnGetContact()
        {

        }

        public void RaisePropertyChangedContactList()
        {

            OnPropertyChanged(() => Contacts);
        }
        private void RefreshContacts()
        {
            contacts = ContactHelper.GetAllContacts();

        }

        public void UpdateImage()
        {
            foreach (var contact in Contacts)
            {
                var item = ContactHelper.GetContactByJid(contact.jid);
                if(item!=null && item.profileImage != null && contact.profileImage == null)
                {
                    contact.profileImage = item.profileImage;
                }
            }
            RaisePropertyChangedContactList();
        }
        public ChatPageViewModel()
        {
            contacts = new ObservableCollection<Contacts>();
            RefreshContacts();
        }
    }
}
