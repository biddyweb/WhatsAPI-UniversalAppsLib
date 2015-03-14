using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WhatsAPI.UniversalApps.Libs.Constants;
using WhatsAPI.UniversalApps.Libs.Models;
using WhatsAPI.UniversalApps.Libs.Utils.Common;
using WhatsAPI.UniversalApps.Sample.Helpers;
using WhatsAPI.UniversalApps.Sample.Models;
using WhatsAPI.UniversalApps.Sample.Repositories;
using WhatsAPI.UniversalApps.Sample.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
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
        private static Contacts _contacts;
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
            if (this.user.Jid.Contains(from))
            {
                this.AddNewImage(this.user.Nickname, url);
            }
            var messages = new Messages() { jid = this.user.Jid, messages = url };
            DBProvider.DBConnection.Insert(messages);
        }

        private void Instance_OnGetMessage(Libs.Base.ProtocolTreeNode messageNode, string from, string id, string name, string message, bool receipt_sent)
        {
            if (this.user.Jid.Contains(from))
            {

                this.AddNewText(this.user.Nickname, message);
            }
            var messages = new Messages() { jid = this.user.Jid, messages = message };
            DBProvider.DBConnection.Insert(messages);
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
        private static ChatDetailPageViewModel defaultViewModel = new ChatDetailPageViewModel(_contacts);

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public static ChatDetailPageViewModel DefaultViewModel
        {
            get { return defaultViewModel; }
            set
            {
                defaultViewModel = value;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (e.Parameter != null)
            {
                Contacts contacts = e.Parameter as Contacts;
                string server = "";
                DefaultViewModel.SetContacts(contacts);
                if (contacts.jid != null)
                {
                    if (contacts.jid.Contains("-"))
                    {
                        server = WhatsAPI.UniversalApps.Libs.Constants.Information.WhatsGroupChat;
                        isGroup = true;
                    }
                    else
                    {
                        server = WhatsAPI.UniversalApps.Libs.Constants.Information.WhatsAppServer;
                        isGroup = false;
                    }
                }
                else
                {
                    contacts.jid = contacts.phoneNumber + "@" + Information.WhatsAppServer;
                    server = WhatsAPI.UniversalApps.Libs.Constants.Information.WhatsAppServer;
                    isGroup = false;

                }
                User user = new User(contacts.jid, server, contacts.name);
                this.user = user;
                if (!isGroup)
                {
                    processChat();
                    if (contacts.profileImage == null)
                    {
                        ContactHelper.SyncProfileContactImage(contacts.jid);
                        ChattingPage.DefaultViewModel.UpdateImage();
                    }
                }
                this.isTyping = false;
            }
            base.OnNavigatedTo(e);
        }

        private void LoadOldConversation()
        {
            var messageList = DBProvider.DBConnection.Table<Messages>().Where(x => x.jid == this.user.Jid).Take(25);
            foreach (var message in messageList)
            {
                AddNewText(this.user.Nickname, message.messages);

            }
        }
        private void processChat()
        {
            SocketInstance.Instance.SendQueryLastOnline(this.user.Jid);
            SocketInstance.Instance.SendPresenceSubscriptionRequest(this.user.Jid);
            LoadOldConversation();
        }
        private async void AddNewText(string from, string text)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
               {
                   DefaultViewModel.Messages.Add(new Messages() { jid = from, messages = text });
               });
        }

        private async void AddNewImage(string from, string text)
        {
            //{
            //    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //    {
            //        Image image = new Image()
            //        {
            //            Source = new BitmapImage(new Uri(text)),
            //            Width = 120,
            //            Height = 120
            //        };

            //        var inline = new InlineUIContainer();
            //        inline.Child = image;

            //        var p = new Paragraph();
            //        p.Inlines.Add(new Run() { Text = string.Format("{0}: {1}", from, Environment.NewLine) });
            //        p.Inlines.Add(inline);
            //        p.Inlines.Add(new Run() { Text = string.Format("{0}", Environment.NewLine) });
            //        this.tbChatWindow.Blocks.Add(p);
            //    });
            AddNewText(from, text);
        }

        private void txtChat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.isTyping)
            {
                this.isTyping = true;
                SocketInstance.Instance.SendComposing(this.user.GetFullJid());
                this.timerTyping.Interval = new TimeSpan(0, 0, 5);
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
        private Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
        private async void btnAttachImage_Click(object sender, RoutedEventArgs e)
        {
            var menu = new PopupMenu();
            menu.Commands.Add(new UICommand("Image", null, 1));
            menu.Commands.Add(new UICommand("Camera", null, 2));
            menu.Commands.Add(new UICommand("Video", null, 3));
            try
            {
                var chosenCommand = await menu.ShowForSelectionAsync(GetElementRect((FrameworkElement)sender));
                if (chosenCommand != null)
                {
                    switch ((int)chosenCommand.Id)
                    {
                        case 1:
                            {
                                FileOpenPicker pk = new FileOpenPicker();
                                pk.ViewMode = PickerViewMode.Thumbnail;
                                pk.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                                pk.FileTypeFilter.Add(".jpg");
                                pk.FileTypeFilter.Add(".jpeg");

                                StorageFile file = await pk.PickSingleFileAsync();
                                if (file != null)
                                {
                                    var byteFile = await WhatsAPI.UniversalApps.Libs.Utils.Common.FileHelper.ConvertStorageFileToByteArray(file);
                                    this.AddNewImage(App.UserName, file.Path);
                                    SocketInstance.Instance.SendMessageImage(this.user.GetFullJid(), byteFile, Enums.ImageType.JPEG);
                                }
                            }
                            break;

                        case 2:
                            {
                                //CameraCaptureUI dialog = new CameraCaptureUI();
                                //dialog.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.MediumXga;

                                //StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                                //if (file != null)
                                //{
                                //    await _viewModel.AttachPhoto(file);
                                //}
                            }
                            break;

                        case 3:
                            {
                                FileOpenPicker pk = new FileOpenPicker();
                                pk.ViewMode = PickerViewMode.Thumbnail;
                                pk.SuggestedStartLocation = PickerLocationId.VideosLibrary;
                                pk.FileTypeFilter.Add(".mp4");
                                pk.FileTypeFilter.Add(".avi");
                                pk.FileTypeFilter.Add(".mov");

                                StorageFile file = await pk.PickSingleFileAsync();
                                if (file != null)
                                {
                                    BasicProperties properties = await file.GetBasicPropertiesAsync();
                                    if (properties.Size > 1024 * 1024 * 10)
                                    {
                                        MessageDialog dialog = new MessageDialog("Too big");
                                        await dialog.ShowAsync();
                                        return;
                                    }

                                    var byteFile = await WhatsAPI.UniversalApps.Libs.Utils.Common.FileHelper.ConvertStorageFileToByteArray(file);
                                    SocketInstance.Instance.SendMessageVideo(this.user.GetFullJid(), byteFile, Enums.VideoType.MP4);
                                }
                            }
                            break;


                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }


    }
}
