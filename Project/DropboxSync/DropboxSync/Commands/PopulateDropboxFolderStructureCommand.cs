using System.Collections.Generic;
using DropboxSync.CQRS;
using DropboxSync.ViewModels.Pages;

namespace DropboxSync.Commands
{
    public class PopulateDropboxFolderStructureCommand: Command
    {
        public string Root { get; set; }

        public ChangeDropBoxFolderViewModel ViewModel { get; set; }
    }
}