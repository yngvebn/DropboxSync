using Microsoft.Xna.Framework.Media;

namespace DropboxSync.DomainEvents
{
    internal class PictureUploadedToDropbox : IDomainEvent
    {
        public Picture Picture { get; set; }

        public string NewFilename { get; set; }

        public string Destination { get; set; }
    }
}