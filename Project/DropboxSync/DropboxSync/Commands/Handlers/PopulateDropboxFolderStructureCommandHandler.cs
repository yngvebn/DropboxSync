using System;
using System.Collections.Generic;
using System.Linq;
using DropNet.Exceptions;
using DropNet.Models;
using DropboxSync.CQRS;
using DropboxSync.ViewModels.Pages;

namespace DropboxSync.Commands.Handlers
{
    public class PopulateDropboxFolderStructureCommandHandler: ICommandHandler<PopulateDropboxFolderStructureCommand>
    {
        private PopulateDropboxFolderStructureCommand _command;
        private Action<object> _success;
        public void Handle(PopulateDropboxFolderStructureCommand command, Action<object> success, Action<object> error, Action<CommandExecutingProgress> progress)
        {
            _success = success;
            _command = command;
            _command.ViewModel.Folders.Clear();
            App.GlobalClient.GetMetaDataAsync(command.Root, GetMetaDataSuccess, GetMetaDataError);
            if (!_command.Root.Equals("/") && !string.IsNullOrEmpty(_command.Root))
            {
                _command.ViewModel.Folders.Add(new DropboxFolder("..", new DropboxFolder("dummy", _command.Root).ParentPath));
            }
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        private void GetMetaDataSuccess(MetaData obj)
        {
            foreach(var item in obj.Contents.Where(c => c.Is_Dir))
            {
                
                _command.ViewModel.Folders.Add(new DropboxFolder(item.Name, item.Path));
            }
            if(_success != null) _success(null);
        }

        private void GetMetaDataError(DropboxException obj)
        {
        }
    }
}