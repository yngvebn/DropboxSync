using System;
using System.Windows;
using DropNet.Exceptions;
using DropNet.Models;
using DropboxSync.CQRS;
using DropboxSync.Core;
using Microsoft.Phone.Controls;

namespace DropboxSync.Commands.Handlers
{
    public class AuthenticateWithDropboxCommandHandler : ICommandHandler<AuthenticateWithDropboxCommand>
    {
        private readonly SettingsResolver<AppSettings> _settings; 
        public AuthenticateWithDropboxCommandHandler()
        {
            _settings = new SettingsResolver<AppSettings>();
        }


        private AuthenticateWithDropboxCommand _command;
        public void Handle(AuthenticateWithDropboxCommand command, Action<object> success, Action<object> error, Action<CommandExecutingProgress> progress)
        {
            _command = command;
            App.GlobalClient.GetTokenAsync(GetTokenSuccess, GetTokenError);

        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        private void GetTokenSuccess(UserLogin obj)
        {
            var tokenUrl = App.GlobalClient.BuildAuthorizeUrl();
            _command.Browser.LoadCompleted += LoginBrowserLoadCompleted;
            _command.Browser.Navigate(new Uri(tokenUrl));
        }

        private void GetTokenError(DropboxException obj)
        {
            
        }

        void LoginBrowserLoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.AbsolutePath == "/1/oauth/authorize")
            {
                App.GlobalClient.GetAccessTokenAsync(GetAccessTokenSuccess, GetAccessTokenError);
            }
        }

        private void GetAccessTokenError(DropboxException obj)
        {
        }

        private void GetAccessTokenSuccess(UserLogin response)
        {
            var token = response.Token;
            var secret = response.Secret;
            _settings.Settings.DropboxUserSecret = secret;
            _settings.Settings.DropboxUserToken = token;
            _settings.Save();
            App.ViewModel.Navigation.GoBack();
        }
    }
}