using DropboxSync.CQRS;
using Microsoft.Xna.Framework.Media;

namespace DropboxSync.Commands
{
    public class UploadPictureToDropbox: Command
    {
        public Picture Picture { get; set; }
    }
}