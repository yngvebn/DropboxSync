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

namespace DropboxSync.Helpers
{
    public class MessageBoxHelpers
    {
        public static void Confirm(string message, string caption, Action ok, Action cancel)
        {

            if (MessageBox.Show(message, caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ok();
            }
            else
            {
                cancel();
            }

        }

        public static void Message(string message, string caption, Action ok)
        {
            if (MessageBox.Show(message, caption, MessageBoxButton.OK) == MessageBoxResult.OK)
            {
               if(ok != null) ok();
            }

        }
    }
}
