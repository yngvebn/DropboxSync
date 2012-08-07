using DropboxSync.CQRS;
using DropboxSync.ViewModels.Pages;

namespace DropboxSync.Commands
{
    public class UpdatePicturesToSyncCommand: Command
    {
        public MainPageViewModel MainPageViewModel { get; set; }
    }
}