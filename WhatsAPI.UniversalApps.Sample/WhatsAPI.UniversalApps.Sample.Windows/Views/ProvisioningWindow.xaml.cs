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
using WhatsAPI.UniversalApps.Libs.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WhatsAPI.UniversalApps.Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProvisioningWindow : Page
    {
        public ProvisioningWindow()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var Register = await WhatsAPI.UniversalApps.Libs.Core.Registration.Register.RequestCode(txtPhoneNumber.Text.Trim()); //phonenumber without +
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
                MessageDialog dialog = new MessageDialog("Registration Success", "Registration Success");
                await dialog.ShowAsync();
                if ((Register.Response.GetJsonValue("type") != null && Register.Response.GetJsonValue("type") == "new")||   String.IsNullOrEmpty(Register.Password))
                {
                    if (this.Frame != null)
                    {
                        this.Frame.Navigate(typeof(InputCodeWindow), txtPhoneNumber.Text);
                    }
                }
                else
                {
                    if (this.Frame != null)
                    {
                        App.PhoneNumber = txtPhoneNumber.Text;
                        App.Password = Register.Password;
                        this.Frame.Navigate(typeof(InputNameWindow), txtPhoneNumber.Text);
                    }
                }
#endif
            }
        }
    }
}
