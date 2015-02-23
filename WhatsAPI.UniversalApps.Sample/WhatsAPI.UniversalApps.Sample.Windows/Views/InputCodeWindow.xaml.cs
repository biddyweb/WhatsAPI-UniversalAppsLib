using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace WhatsAPI.UniversalApps.Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InputCodeWindow : Page
    {
        public InputCodeWindow()
        {
            this.InitializeComponent();
            
        }

        private string phoneNumber;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                phoneNumber = e.Parameter as String;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var Register = await WhatsAPI.UniversalApps.Libs.Core.Registration.Register.RegisterCode(phoneNumber,txtCode.Text.Trim()); //phonenumber without +
            if (Register==null)
            {
#if !WINDOWS_PHONE_APP
                MessageDialog dialog = new MessageDialog("Registration Failed", "Registration Failed");
                await dialog.ShowAsync();
#endif
            }
            else
            {
#if !WINDOWS_PHONE_APP
                MessageDialog dialog = new MessageDialog("Registration Success", "Registration Success");
                await dialog.ShowAsync();
                
                App.PhoneNumber = phoneNumber;
                App.Password = Register;

                if (this.Frame != null)
                {
                    this.Frame.Navigate(typeof(InputNameWindow));
                }
#endif
            }
        }
    }
}
