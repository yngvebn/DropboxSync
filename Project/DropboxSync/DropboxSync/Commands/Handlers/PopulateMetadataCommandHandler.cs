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
using DropNet.Exceptions;
using DropNet.Models;
using DropboxSync.CQRS;
using DropboxSync.ViewModels.Pages;

namespace DropboxSync.Commands.Handlers
{
    public class PopulateMetadataCommandHandler: ICommandHandler<PopulateMetadataCommand>
    {
        private PopulateMetadataCommand _command;
        private Action<object> _success;
        public void Handle(PopulateMetadataCommand command, Action<object> success, Action<object> error, Action<CommandExecutingProgress> progress)
        {
            _success = success;
            _command = command;
            App.GlobalClient.AccountInfoAsync(AccountInfoSuccess, AccountInfoError);
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        private void AccountInfoError(DropboxException obj)
        {
            
        }

        private void AccountInfoSuccess(AccountInfo obj)
        {
            _command.MainPageViewModel.DropboxInfo.DisplayName = obj.display_name;
            if(_success != null)_success(null);
        }
    }
}
