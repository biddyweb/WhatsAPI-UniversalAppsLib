using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Sample.Constants;
using WhatsAPI.UniversalApps.Sample.Helpers;
using WhatsAPI.UniversalApps.Sample.Models;
using WhatsAPI.UniversalApps.Sample.Repositories;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WhatsAPI.UniversalApps.Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InputNameWindow : Page
    {
        public InputNameWindow()
        {
            this.InitializeComponent();

        }

        private string phoneNumber;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {

            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var hasLogin = await CheckLogin(App.PhoneNumber, App.Password);
            if (hasLogin)
            {
#if !WINDOWS_PHONE_APP
                if (this.Frame != null)
                {
                    this.Frame.Navigate(typeof(ChattingPage));
                }
#endif
            }
        }


        private async Task<bool> CheckLogin(string user, string pass)
        {
            try
            {
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                    return false;

                var Config = DBProvider.DBConnection.Table<Config>();
                if (DBProvider.DBConnection.Table<Config>().Where(x => x.key == ConfigKey.Username).Count() == 0)
                {
                    App.UserName = txtName.Text.Trim();
                    var configUsername = new Config() { key = ConfigKey.Username, value = App.UserName };
                    DBProvider.DBConnection.Insert(configUsername);

                    var configPhoneNumber = new Config() { key = ConfigKey.PhoneNumber, value = App.PhoneNumber };
                    DBProvider.DBConnection.Insert(configPhoneNumber);

                    var configPassword = new Config() { key = ConfigKey.Password, value = App.Password };
                    DBProvider.DBConnection.Insert(configPassword);
                }
                SocketInstance.Create(App.PhoneNumber, App.Password, txtName.Text.Trim(), true);
                SocketInstance.Instance.OnLoginSuccess += Instance_OnLoginSuccess;
                await SocketInstance.Instance.Connect();
                await SocketInstance.Instance.Login();

                //check login status
                if (SocketInstance.Instance.ConnectionStatus == WhatsAPI.UniversalApps.Libs.Constants.Enums.CONNECTION_STATUS.LOGGEDIN)
                {
                    return true;
                }
            }
            catch (Exception)
            { }
            return false;
        }

        async void Instance_OnLoginSuccess(string phoneNumber, byte[] data)
        {
            var challengeData = data;
            await FileHelper.WriteToFile(Constants.ConfigKey.WhatsAppNextChallengeFile, challengeData);
        }
    }
}
