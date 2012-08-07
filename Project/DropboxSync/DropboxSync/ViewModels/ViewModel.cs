using System;
using System.ComponentModel;
using System.Windows.Navigation;
using DropboxSync.Core;
using Microsoft.Phone.Controls;

namespace DropboxSync.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public NavigationalPages Goto
        {
            get;
            set;
        }
        public UserSettings UserSettings { get; set; }
        public void DisplayProgressMessage(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                IsProgressVisible = false;
                CurrentStatusMessage = "";
            }
            else
            {
                IsProgressVisible = true;
                CurrentStatusMessage = message;
            }
        }

        public bool IsProgressVisible
        {
            get;
            private set;
        }

        public string CurrentStatusMessage
        {
            get;
            private set;
        }

        public ViewModel()
        {
            Goto = new NavigationalPages();

            UserSettings = new SettingsResolver<UserSettings>().Settings;
            UserSettings.PropertyChanged += UserSettings_PropertyChanged;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(object sender, string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
        }

        void UserSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SettingsResolver<UserSettings>.Save(UserSettings);
            if (e.PropertyName == "AutomaticUpload")
            {

            }
        }


        public virtual void Init(PhoneApplicationPage phoneApplicationPage)
        {

        }
    }
}