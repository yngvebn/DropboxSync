using System.Windows.Input;
using DropboxSync.Core;
using DropboxSync.ViewModels.Pages;

namespace DropboxSync.ViewModels
{
    public class NavigationalPages
    {
        public ICommand Settings { get { return new DelegateCommand((o) => App.ViewModel.Goto<SettingsViewModel>()); } }
        public ICommand SelectiveSync { get { return new DelegateCommand((o) => App.ViewModel.Goto<SelectiveSyncViewModel>()); } }
        public ICommand DropboxAuthentication { get { return new DelegateCommand((o) => App.ViewModel.Goto<DropboxAuthenticationViewModel>()); } }
        public ICommand About { get { return new DelegateCommand((o) => App.ViewModel.Goto<HelpAboutViewModel>()); } }
        public ICommand DropboxFolder { get { return new DelegateCommand((o) => App.ViewModel.Goto<ChangeDropBoxFolderViewModel>()); } }
        public ICommand Home { get { return new DelegateCommand((o) => App.ViewModel.Goto<MainPageViewModel>()); } }
        public ICommand ChangeFolderTemplate { get { return new DelegateCommand((o) => App.ViewModel.Goto<ChangeFolderTemplateViewModel>()); } }
        public ICommand Previous { get{ return new DelegateCommand((o) => App.ViewModel.NavigateBack() );} }

    }
}