using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DropboxSync.Core
{
    public class UserSettings : INotifyPropertyChanged
    {
        public string DropboxRootFolder { get; set; }
        public bool AutomaticUpload { get; set; }
        public bool UploadOnlyOnWifi { get; set; }
        public bool UploadToSubFolder { get; set; }
        public bool HideUploaded { get; set; }
        public string FolderNameTemplate { get; set; }

        public string CurrentSubFolderName
        {
            get { return new FolderNameSubstitutions().ConstructFolderName(FolderNameTemplate, DateTime.Now); }
        }

        

       
        public UserSettings()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class FolderNameSubstitutions
    {
        public Dictionary<string, string> Substitutions { get; private set; }

        public FolderNameSubstitutions()
        {
            Substitutions = new Dictionary<string, string>();
            Substitutions.Add("%MMM%", "{0:MMM}");
            Substitutions.Add("%MM%", "{0:MM}");
            Substitutions.Add("%MMMM%", "{0:MMMM}");
            Substitutions.Add("%dd%", "{0:dd}");
            Substitutions.Add("%ddd%", "{0:ddd}");
            Substitutions.Add("%dddd%", "{0:dddd}");
            Substitutions.Add("%yy%", "{0:yy}");
            Substitutions.Add("%yyyy%", "{0:yyyy}");
            
        }
        public string ConstructFolderName(string template, DateTime now)
        {
            string result = template ?? "";
            foreach(var tmpl in Substitutions.Keys)
            {
                result = result.Replace(tmpl, String.Format(Substitutions[tmpl], now));
            }
            return result;
        }

    }
}