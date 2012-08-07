using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DropboxSync.ViewModels.Pages;
using Microsoft.Phone.Controls;

namespace DropboxSync
{
    public partial class SelectiveSync : PhoneApplicationPage
    {
        public SelectiveSync()
        {
            InitializeComponent();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            (App.ViewModel.CurrentViewModel as SelectiveSyncViewModel).BackClicked();

        }
    }
}