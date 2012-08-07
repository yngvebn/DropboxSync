namespace DropboxSync.DomainEvents
{
    public class PictureSyncCountUpdated: IDomainEvent
    {
        public int NewCount { get; set; }
    }
}