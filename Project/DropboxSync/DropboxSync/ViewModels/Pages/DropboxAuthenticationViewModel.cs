using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DropboxSync.Commands;
using DropboxSync.Core;
using Microsoft.Phone.Controls;

namespace DropboxSync.ViewModels.Pages
{
    [PageDefinition("/DropboxAuthenticate.xaml")]
    public class DropboxAuthenticationViewModel: ViewModel
    {
        public override void Init(PhoneApplicationPage phoneApplicationPage)
        {
            var browser = (phoneApplicationPage as DropboxAuthenticate).LoginBrowser;
            App.Executor.ExecuteCommand(new AuthenticateWithDropboxCommand() { Browser =browser});
        }
    }
}
