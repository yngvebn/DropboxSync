using System.Collections.Generic;
using DropboxSync.CQRS;
using Microsoft.Xna.Framework.Media;

namespace DropboxSync.Commands
{
    public class SyncImagesNowCommand: Command
    {
        public List<Picture> Pictures { get; set; }
    }
}