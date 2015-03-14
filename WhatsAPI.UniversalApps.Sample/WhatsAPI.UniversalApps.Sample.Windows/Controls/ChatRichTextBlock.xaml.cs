using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WhatsAPI.UniversalApps.Sample.Controls
{
    public enum MessageType
    {
        IMAGE,
        VIDEO,
        TEXT,
        AUDIO,
        VCARD
    }
    public sealed partial class ChatRichTextBlock : UserControl
    {
        public const int MaxLength = 10000;
        public ChatRichTextBlock()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(ChatRichTextBlock),
                                                new PropertyMetadata("", new PropertyChangedCallback(OnTextChanged)));



        public static readonly DependencyProperty IsParsingLinkEnabledProperty =
            DependencyProperty.RegisterAttached("IsParsingLinkEnabled", typeof(bool), typeof(ChatRichTextBlock), null);
        private static int CompareByIndex(KeywordMatch x, KeywordMatch y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; // equal. 
                else
                    return -1; // y is greater.
            }
            else
            {
                if (y == null)
                    return 1; // x is greater.
                else
                {
                    int retval = x.MatchResult.Index.CompareTo(y.MatchResult.Index);
                    return retval;
                }
            }
        }

        public class KeywordMatch
        {
            private Match _matchResult;
            private int _typeIndex;

            public Match MatchResult
            {
                get { return _matchResult; }
                set { _matchResult = value; }
            }
            public int TypeIndex
            {
                get { return _typeIndex; }
                set { _typeIndex = value; }
            }

            public KeywordMatch(Match match, int index)
            {
                _matchResult = match;
                _typeIndex = index;
            }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public bool IsParsingLinkEnabled
        {
            get { return (bool)GetValue(IsParsingLinkEnabledProperty); }
            set { SetValue(IsParsingLinkEnabledProperty, value); }
        }


        private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as ChatRichTextBlock).UpdateContent();
        }

        private MessageType ParseMessage(string message)
        {
            if (Uri.IsWellFormedUriString(message, UriKind.Absolute))
            {
                var fileName = message.Split('/').LastOrDefault();
                if (fileName.Split('.').LastOrDefault() == "mp4" || fileName.Split('.').LastOrDefault() == "avi" || fileName.Split('.').LastOrDefault() == "mov")
                {
                    return MessageType.VIDEO;
                }
                else if (fileName.Split('.').LastOrDefault() == "mp3" || fileName.Split('.').LastOrDefault() == "ogg")
                {
                    return MessageType.AUDIO;
                }
                else if (fileName.Split('.').LastOrDefault() == "jpg" || fileName.Split('.').LastOrDefault() == "png" || fileName.Split('.').LastOrDefault() == "gif")
                {
                    return MessageType.IMAGE;
                }
                else
                {
                    return MessageType.IMAGE;
                }
            }
            else
            {
                return MessageType.TEXT;
            }
        }

        internal void UpdateContent()
        {
            RichText.FontFamily = this.FontFamily;
            RichText.FontSize = this.FontSize;
            RichText.Foreground = this.Foreground;

            try
            {
                RichText.Blocks.Clear();
            }
            catch
            {

            }

            if (Text == null)
                return;

            long startTick = DateTime.UtcNow.Ticks;

            Paragraph myParagraph = new Paragraph();

            string srcText = Text;

            if (Text.Length > MaxLength)
                srcText = Text.Substring(0, MaxLength);

            int prevIndex = 0;
            List<KeywordMatch> matchList = new List<KeywordMatch>();
            RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;

            if (IsParsingLinkEnabled)
            {
                if (srcText.Length >= 7)
                {
                    string patternUrl = @"((ht)tp(s?):\/\/|www\.)[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?";

                    Regex reg = new System.Text.RegularExpressions.Regex(patternUrl, options);
                    MatchCollection mactches = reg.Matches(srcText);
                    foreach (Match match in mactches)
                        matchList.Add(new KeywordMatch(match, -1));
                }
                if (srcText.Length >= 5)
                {
                    string patternEmail = @"(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))";

                    Regex reg = new System.Text.RegularExpressions.Regex(patternEmail, options);
                    MatchCollection mactches = reg.Matches(srcText);
                    foreach (Match match in mactches)
                        matchList.Add(new KeywordMatch(match, -2));
                }
                if (srcText.Length >= 5)
                {
                    string patternNumber = @"(?:\+?)([0-9]{1,13})([- ])?([0-9]{1,13})([- ])?([0-9]{1,13})([- ])?([0-9]{1,13})([- ])?([0-9])";

                    Regex reg = new System.Text.RegularExpressions.Regex(patternNumber, options);
                    MatchCollection mactches = reg.Matches(srcText);
                    foreach (Match match in mactches)
                        matchList.Add(new KeywordMatch(match, -3));
                }
            }

            foreach (var m in matchList)
            {
                string text = srcText.Substring(prevIndex, m.MatchResult.Index - prevIndex);

                if (text.Length > 0)
                {
                    switch (ParseMessage(text))
                    {
                        case MessageType.TEXT:
                            {
                                Run myRun1 = new Run();
                                myRun1.Text = text;
                                myParagraph.Inlines.Add(myRun1);
                            }
                            break;
                        case MessageType.IMAGE:
                            {
                                Image myImage = new Image();
                                myImage.Stretch = Stretch.Fill;
                                myImage.Width = 300;
                                myImage.Height = 300;
                                myImage.Source = new BitmapImage(new Uri(text));

                                InlineUIContainer myUI = new InlineUIContainer();
                                myUI.Child = myImage;

                                myParagraph.Inlines.Add(myUI);
                            }
                            break;
                        default:
                            {
                                Run myRun1 = new Run();
                                myRun1.Text = text;
                                myParagraph.Inlines.Add(myRun1);
                            }
                            break;
                    }

                }

                if (m.TypeIndex >= 0)
                {
                    Image myImage = new Image();
                    myImage.Stretch = Stretch.Fill;
                    myImage.Width = 300;
                    myImage.Height = 300;
                    myImage.Source = new BitmapImage(new Uri(text));

                    InlineUIContainer myUI = new InlineUIContainer();
                    myUI.Child = myImage;

                    myParagraph.Inlines.Add(myUI);
                }
                else
                {
                    //Hyperlink link = new Hyperlink();
                    //link.Inlines.Add(m.MatchResult.Value);
                    //link.Foreground = new SolidColorBrush(Colors.Blue);
                    //link.Click += new RoutedEventHandler(link_Click);

                    //myParagraph.Inlines.Add(link);
                }

                prevIndex = m.MatchResult.Index + m.MatchResult.Length;
            }

            if (prevIndex < srcText.Length)
            {
                string text = srcText.Substring(prevIndex);

                switch (ParseMessage(text))
                {
                    case MessageType.TEXT:
                        {
                            Run myRun1 = new Run();
                            myRun1.Text = text;
                            myParagraph.Inlines.Add(myRun1);
                        }
                        break;
                    case MessageType.IMAGE:
                        {
                            Image myImage = new Image();
                            myImage.Stretch = Stretch.Fill;
                            myImage.Width = 300;
                            myImage.Height = 300;
                            myImage.Source = new BitmapImage(new Uri(text));

                            InlineUIContainer myUI = new InlineUIContainer();
                            myUI.Child = myImage;

                            myParagraph.Inlines.Add(myUI);
                        }
                        break;
                    default:
                        {
                            Run myRun1 = new Run();
                            myRun1.Text = text;
                            myParagraph.Inlines.Add(myRun1);
                        }
                        break;
                }
            }

            try
            {
                RichText.Blocks.Add(myParagraph);
            }
            catch
            {
            }

            long endtTick = DateTime.UtcNow.Ticks;
        }

        //void link_Click(object sender, RoutedEventArgs e)
        //{
        //    Run run = (sender as Hyperlink).Inlines.First() as Run;

        //    if (run.Text.Contains('@'))
        //    {
        //        EmailComposeTask task = new EmailComposeTask();
        //        task.To = run.Text;
        //        task.Show();
        //    }
        //    else if (run.Text.IndexOf("www", StringComparison.CurrentCultureIgnoreCase) >= 0 || run.Text.IndexOf("http", StringComparison.CurrentCultureIgnoreCase) >= 0)
        //    {
        //        string address = run.Text;
        //        if (address.IndexOf("http", StringComparison.CurrentCultureIgnoreCase) < 0)
        //            address = "http://" + address;

        //        WebBrowserTask task = new WebBrowserTask();
        //        task.Uri = new Uri(address);
        //        task.Show();
        //    }
        //    else
        //    {
        //        PhoneCallTask phoneCallTask = new PhoneCallTask();
        //        phoneCallTask.PhoneNumber = run.Text;
        //        phoneCallTask.Show();
        //    }

        //    //System.Diagnostics.Debug.WriteLine(run.Text);
        //}
    }
}
