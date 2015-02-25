using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using Windows.UI.Xaml;

namespace WhatsAPI.UniversalApps.Sample.ViewModels
{
    public class MainPageViewModel : BindableBase 
    {
        private FrameNavigationService frameNavigationService;
        private async void OnLogin()
        {
            if (PhoneNumber.Length == 0)
                return;
             var response = await WhatsAPI.UniversalApps.Libs.Core.Registration.Register.RequestCode(PhoneNumber, "sms");
             if (response.Password == null)
             {

             }
             else
             {
                    
             }
        }

        private string userName;
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
                OnPropertyChanged(() => UserName);
            }
        }

        private string phoneNumber;
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
                OnPropertyChanged(() => PhoneNumber);
            }
        }

        public DelegateCommand OnLoginCommand { get; set; }

        #region Constructor
        public MainPageViewModel()
        {
            OnLoginCommand = new DelegateCommand(OnLogin);
           
        }
        #endregion
    }
}
