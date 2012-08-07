using DropboxSync.CQRS;
using Microsoft.Phone.Controls;

namespace DropboxSync.Commands
{
    public class AuthenticateWithDropboxCommand: Command
    {
        public WebBrowser Browser { get; set; }
    }
}