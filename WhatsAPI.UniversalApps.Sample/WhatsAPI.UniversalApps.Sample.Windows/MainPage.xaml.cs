using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs;
using WhatsAPI.UniversalApps.Libs.Base;
using WhatsAPI.UniversalApps.Libs.Core.Events;
using WhatsAPI.UniversalApps.Libs.Models;
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
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
        }

        void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Page mainPage = (Page)sender;
            if (mainPage != null)
            {
                mainPage.Loaded -= MainPage_Loaded;
            }
            if (SocketInstance.Instance.ConnectionStatus == Libs.Constants.Enums.CONNECTION_STATUS.CONNECTED || SocketInstance.Instance.ConnectionStatus == Libs.Constants.Enums.CONNECTION_STATUS.LOGGEDIN)
            {
                SocketInstance.Instance.OnDisconnect += Instance_OnDisconnect;
                SocketInstance.Instance.Disconnect();
            }
            
        }


        void Instance_OnDisconnect(Exception ex)
        {
            SocketInstance.Instance.OnDisconnect -= Instance_OnDisconnect;
            if (ex != null)
                System.Diagnostics.Debug.WriteLine("Disconnect Error : " + ex.Message);
        }
        private User user;
        private bool isTyping;
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtNickName.Text = "BR";
            this.txtUsername.Text = "66958832367"; //phonenumber without +
            this.txtPassword.Password = "hYjVoXPu7TO5jo0N5fIu4LcadwM=";
        }

        private void WhatsEventHandlerOnIsTypingEvent(string @from, bool value)
        {
            if (!this.user.GetFullJid().Equals(from))
                return;


        }
        private void WhatsEventHandlerOnMessageRecievedEvent(FMessage mess)
        {
            var tmpMes = mess.data;
            System.Diagnostics.Debug.WriteLine("Terima Pesan =>" + mess.data + "Dari : " + mess.User.Jid);
            //this.AddNewText(this.user.UserName, tmpMes);
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var Register = await WhatsAPI.UniversalApps.Libs.Core.Registration.Register.RequestCode("66958832367"); //phonenumber without +
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
            var Verify = await WhatsAPI.UniversalApps.Libs.Core.Registration.Register.RegisterCode("66958832367", txtCode.Text.Trim());
            if (Verify==null)
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
            else
            {
#if !WINDOWS_PHONE_APP
                MessageDialog dialog = new MessageDialog("Login Success !!! You now can start to chat", "Login Success");
                await dialog.ShowAsync();
#endif
                btnSend.IsEnabled = true;
                EventsHandler.MessageRecievedEvent += WhatsEventHandlerOnMessageRecievedEvent;
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

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SocketInstance.Instance.SendMessage(txtDestination.Text, txtMessage.Text);
        }

    }
}
