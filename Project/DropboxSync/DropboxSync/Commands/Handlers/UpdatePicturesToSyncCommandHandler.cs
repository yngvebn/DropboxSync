using System;
using System.Linq;
using DropboxSync.CQRS;
using DropboxSync.DomainEvents;
using Microsoft.Xna.Framework.Media;

namespace DropboxSync.Commands.Handlers
{
    public class UpdatePicturesToSyncCommandHandler: ICommandHandler<UpdatePicturesToSyncCommand>
    {
        public void Handle(UpdatePicturesToSyncCommand command, Action<object> success, Action<object> error, Action<CommandExecutingProgress> progress)
        {
            MediaLibrary mediaLibrary = new MediaLibrary();
            var cameraRoll = mediaLibrary.RootPictureAlbum.Albums.Single(c => c.Name == "Camera Roll");
            var uploaded = App.Settings.Uploaded;
            var all = cameraRoll.Pictures.Select(c => c.Name);
            var cameraRollOnlyPictures = all.Except(uploaded.Where(c => c.Album == "Camera Roll").Select(c => c.OriginalFilename));
            App.Raise(new PictureSyncCountUpdated() { NewCount = cameraRollOnlyPictures.Count() });
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}