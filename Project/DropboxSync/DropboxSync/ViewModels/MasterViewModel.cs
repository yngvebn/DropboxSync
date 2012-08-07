using System;
using System.Windows;
using System.Windows.Navigation;
using DropboxSync.Helpers;
using DropboxSync.ViewModels.Pages;
using Microsoft.Phone.Controls;

namespace DropboxSync.ViewModels
{
    public class MasterViewModel
    {

        private PhoneApplicationPage CurrentPage
        {
            get
            {
                return ((App)Application.Current).RootFrame.Content as PhoneApplicationPage ?? new Home();
            }
        }


        public ViewModel CurrentViewModel { get; private set; }
        public NavigationService Navigation
        {
            get { return CurrentPage.NavigationService; }
        }
        public MasterViewModel()
        {
            Navigation.Navigated += new NavigatedEventHandler(Navigation_Navigated);
            
            CurrentViewModel = new MainPageViewModel();
            CurrentPage.DataContext = CurrentViewModel;
            CurrentViewModel.Init(CurrentPage);
        }

        void Navigation_Navigated(object sender, NavigationEventArgs e)
        {
            var viewModel = PageHelpers.FindCurrentView(e.Uri);
            if (viewModel == null) return;
            CurrentPage.DataContext = viewModel;

            App.SetCurrentViewModel(viewModel);
            viewModel.Init(CurrentPage);
        }

        public void SetCurrentViewModel(ViewModel viewModel)
        {
            CurrentViewModel = viewModel;
        }

        public void Goto<T>()
        {
            var type = typeof (T);
            var pageDefinition = type.PageDefinition();
            Navigation.Navigate(pageDefinition.RelativeUri);
        }


        public void ShowStatusMessage(string message)
        {
            CurrentViewModel.DisplayProgressMessage(message);
        }

        public void DisplayMessage(string message, string caption, Action ok = null)
        {
            CurrentPage.Dispatcher.BeginInvoke(() => MessageBoxHelpers.Message(message, caption, ok));

        }

        public void DisplayConfirm(string message, string caption, Action ok, Action cancel)
        {
            CurrentPage.Dispatcher.BeginInvoke(() => MessageBoxHelpers.Confirm(message, caption, ok, cancel));

        }

        public void NavigateBack()
        {
            Navigation.GoBack();
        }
    }
}
