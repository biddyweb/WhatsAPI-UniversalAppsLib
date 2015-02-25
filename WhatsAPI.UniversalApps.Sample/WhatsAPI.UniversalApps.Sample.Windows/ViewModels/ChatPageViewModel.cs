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
        
        public ChatPageViewModel()
        {
            contacts = new ObservableCollection<Contacts>();
            Contacts = ContactHelper.GetAllContacts();
        }
    }
}
