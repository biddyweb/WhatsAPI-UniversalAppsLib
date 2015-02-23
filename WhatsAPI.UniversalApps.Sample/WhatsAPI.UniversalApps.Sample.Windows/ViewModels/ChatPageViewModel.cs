using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Sample.ViewModels
{
    public class ChatPageViewModel : BindableBase
    {
        private void OnGetContact()
        {
            if (SocketInstance.Instance.ConnectionStatus == Libs.Constants.Enums.CONNECTION_STATUS.CONNECTED || SocketInstance.Instance.ConnectionStatus == Libs.Constants.Enums.CONNECTION_STATUS.LOGGEDIN)
            {
              
            }
        }
        
        public ChatPageViewModel()
        {

        }
    }
}
