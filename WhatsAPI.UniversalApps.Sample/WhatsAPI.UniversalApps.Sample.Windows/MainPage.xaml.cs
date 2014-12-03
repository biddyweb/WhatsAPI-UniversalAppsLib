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
    }
}
