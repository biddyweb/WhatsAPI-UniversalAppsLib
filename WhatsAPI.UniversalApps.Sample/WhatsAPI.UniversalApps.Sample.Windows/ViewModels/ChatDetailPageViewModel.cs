using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Sample.Models;

namespace WhatsAPI.UniversalApps.Sample.ViewModels
{
    public class ChatDetailPageViewModel : BindableBase
    {
        private Contacts _contacts;
        public Contacts Contacts{
            get
            {
                return _contacts;
            }
            set
            {
                _contacts = value;
                OnPropertyChanged(() => Contacts);
            }
        }

        private ObservableCollection<Messages> _messages;
        public ObservableCollection<Messages> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                OnPropertyChanged(() => Messages);
            }
        }


        public ChatDetailPageViewModel(Contacts contacts)
        {
            this.Contacts = contacts;
            _messages = new ObservableCollection<Messages>();
        }

        public ChatDetailPageViewModel()
        {
            _messages = new ObservableCollection<Messages>();
        }

        public string _typingStatus;
        public string TypingStatus
        {
            get
            {
                return _typingStatus;
            }
            set
            {
                _typingStatus = value;
                OnPropertyChanged(() => TypingStatus);
            }
        }

        internal void SetContacts(Models.Contacts contacts)
        {
            Messages.Clear();
            this.Contacts = contacts;
            OnPropertyChanged(() => Contacts);
        }
    }
}
