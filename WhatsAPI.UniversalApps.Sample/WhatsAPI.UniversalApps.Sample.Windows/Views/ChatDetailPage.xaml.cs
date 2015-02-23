using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WhatsAPI.UniversalApps.Libs.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WhatsAPI.UniversalApps.Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChatDetailPage : Page
    {
        private User user;
        private bool isTyping;
        private DispatcherTimer timerTyping;
        private bool isGroup = false;

        public ChatDetailPage()
        {
            this.InitializeComponent();
            this.timerTyping = new DispatcherTimer();
            this.timerTyping.Tick += timerTyping_Tick;
            SocketInstance.Instance.OnGetMessage += Instance_OnGetMessage;
            SocketInstance.Instance.OnGetMessageImage += Instance_OnGetMessageImage;
            SocketInstance.Instance.OnConnectSuccess += Instance_OnConnectSuccess;

        }

        async void Instance_OnConnectSuccess()
        {
            await SocketInstance.Instance.Login();
        }

        void Instance_OnGetMessageImage(Libs.Base.ProtocolTreeNode mediaNode, string from, string id, string fileName, int fileSize, string url, byte[] preview)
        {
            this.AddNewImage(from, url);
        }

        private void Instance_OnGetMessage(Libs.Base.ProtocolTreeNode messageNode, string from, string id, string name, string message, bool receipt_sent)
        {
            this.AddNewText(from, message);
        }

        void timerTyping_Tick(object sender, object e)
        {
            if (this.isTyping)
            {
                this.isTyping = false;
                return;
            }

            SocketInstance.Instance.SendPaused(this.user.GetFullJid());
            this.timerTyping.Stop();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                string jid = e.Parameter as string;
                string server = "";

                if (jid.Contains("-"))
                {
                    server = WhatsAPI.UniversalApps.Libs.Constants.Information.WhatsGroupChat;
                    isGroup = true;
                }
                else
                {
                    server = WhatsAPI.UniversalApps.Libs.Constants.Information.WhatsAppServer;
                    isGroup = false;
                }
                User user = new User(jid, server, "Billy Iphone");
                this.user = user;
                if (!isGroup)
                    processChat();
                this.isTyping = false;
            }
            base.OnNavigatedTo(e);
        }

        private void processChat()
        {
            SocketInstance.Instance.SendQueryLastOnline(this.user.Jid);
            SocketInstance.Instance.SendPresenceSubscriptionRequest(this.user.Jid);
        }
        private async void AddNewText(string from, string text)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
             {
                 var p = new Paragraph();
                 p.Inlines.Add(new Run() { Text = string.Format("{0}: {1}{2}", from, text, Environment.NewLine) });
                 this.tbChatWindow.Blocks.Add(p);
             });
        }

        private async void AddNewImage(string from, string text)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Image image = new Image()
                {
                    Source = new BitmapImage(new Uri(text)),
                    Width = 120,
                    Height = 120
                };

                var inline = new InlineUIContainer();
                inline.Child = image;

                var p = new Paragraph();
                p.Inlines.Add(new Run() { Text = string.Format("{0}: {1}", from, Environment.NewLine) });
                p.Inlines.Add(inline);
                p.Inlines.Add(new Run() { Text = string.Format("{0}", Environment.NewLine) });
                this.tbChatWindow.Blocks.Add(p);
            });
        }

        private void txtChat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.isTyping)
            {
                this.isTyping = true;
                SocketInstance.Instance.SendComposing(this.user.GetFullJid());
                this.timerTyping.Interval = new TimeSpan(10);
                this.timerTyping.Start();
            }
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtChat.Text.Length == 0)
                return;

            SocketInstance.Instance.SendMessage(this.user.GetFullJid(), txtChat.Text);
            this.AddNewText(App.UserName, txtChat.Text);
            txtChat.Text = "";
        }

        private void txtChat_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (this.txtChat.Text.Length == 0)
                    return;

                SocketInstance.Instance.SendMessage(this.user.GetFullJid(), txtChat.Text);
                this.AddNewText(App.UserName, txtChat.Text);
                txtChat.Text = "";
            }
        }


    }
}
