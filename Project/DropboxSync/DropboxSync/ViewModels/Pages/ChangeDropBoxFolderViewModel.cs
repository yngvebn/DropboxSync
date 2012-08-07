using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DropboxSync.Commands;
using DropboxSync.Core;

namespace DropboxSync.ViewModels.Pages
{
    [PageDefinition("/DropboxFolders.xaml")]
    public class ChangeDropBoxFolderViewModel : ViewModel
    {
        public ICommand SelectDropboxFolderCommand { get; set; }
        public ICommand CancelSelectionCommand { get; set; }
        public ObservableCollection<DropboxFolder> Folders { get; set; }
        public DropboxFolder CurrentPath { get; set; }
        public bool Test { get; set; }

        public ChangeDropBoxFolderViewModel()
        {
            Folders = new ObservableCollection<DropboxFolder>();
            SelectDropboxFolderCommand = new DelegateCommand(SelectDropboxFolder);
            CancelSelectionCommand = new DelegateCommand(CancelSelection);
        }

        public override void Init(Microsoft.Phone.Controls.PhoneApplicationPage phoneApplicationPage)
        {
            DisplayProgressMessage("Populating folder...");
            CurrentPath = new DropboxFolder("/", "/");
            App.Executor.ExecuteCommand(new PopulateDropboxFolderStructureCommand() { ViewModel = this, Root = CurrentPath.Path }, success: LoadingDone);
        }

        public void ChangeDropboxFolder(DropboxFolder folder)
        {
            DisplayProgressMessage("Populating folder...");
            if (!folder.Name.Equals(".."))
            {
                App.Executor.ExecuteCommand(new PopulateDropboxFolderStructureCommand() { ViewModel = this, Root = folder.Path }, success: LoadingDone);
                CurrentPath = folder;
            }
            else
            {
                App.Executor.ExecuteCommand(new PopulateDropboxFolderStructureCommand() { ViewModel = this, Root = CurrentPath.ParentPath }, success: LoadingDone);
                CurrentPath = new DropboxFolder(CurrentPath.ParentPath, CurrentPath.ParentPath);
            }
        }

        private void LoadingDone(object o)
        {
            DisplayProgressMessage("");
        }

        public void CancelSelection(object o)
        {
            App.ViewModel.NavigateBack();
        }
        public void SelectDropboxFolder(object o)
        {
            UserSettings.DropboxRootFolder = CurrentPath.Path;
            Goto.Settings.Execute(null);
        }

        public void BackClicked()
        {
            if(String.IsNullOrEmpty(CurrentPath.Path)) App.ViewModel.NavigateBack();

            ChangeDropboxFolder(new DropboxFolder("..", ".."));
        }
    }
}