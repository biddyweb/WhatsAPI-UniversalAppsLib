using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WhatsAPI.UniversalApps.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var Register = await WhatsAPI.UniversalApps.Libs.Core.Registration.Register.RequestCode("xxxxxxxxxxxxxxxx");
            if (!Register.IsSuccess)
            {
#if !WINDOWS_PHONE_APP
                MessageDialog dialog = new MessageDialog("Registration Failed : " + Register.Response, "Registration Failed");
                await dialog.ShowAsync();
#endif
            }
            else
            {
#if !WINDOWS_PHONE_APP
                MessageDialog dialog = new MessageDialog("Registration Success : " + Register.Password, "Registration Success");
                await dialog.ShowAsync();
#endif
            }
        }

        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var Verify = await WhatsAPI.UniversalApps.Libs.Core.Registration.Register.RegisterCode("6285320399005", txtCode.Text.Trim());
            if (Verify.Length==0)
            {
#if !WINDOWS_PHONE_APP
                MessageDialog dialog = new MessageDialog("Verification Failed ", "Verification Failed");
                await dialog.ShowAsync();
#endif
            }
            else
            {
#if !WINDOWS_PHONE_APP
                MessageDialog dialog = new MessageDialog("Verification Success , Your Password Is : " + Verify, "Verification Success");
                await dialog.ShowAsync();
#endif
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!await this.CheckLogin(this.txtUsername.Text, this.txtPassword.Password))
            {
#if !WINDOWS_PHONE_APP
                MessageDialog dialog = new MessageDialog("Login Failed ", "Login Failed");
                await dialog.ShowAsync();
#endif
                return;
            }
        }
        public async Task<bool> CheckLogin(string user, string pass)
        {
            try
            {
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                    return false;

                SocketInstance.Create(user, pass, this.txtNickName.Text, true);
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

    }
}
