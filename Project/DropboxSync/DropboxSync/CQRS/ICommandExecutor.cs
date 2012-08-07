namespace DropboxSync.CQRS
{
    public interface ICommandExecutor
    {
        CommandResult ExecuteCommand<T>(T command) where T : Command;
    }

}