namespace Cqrs.Commands.WP7
{
    public interface ICommandExecutor
    {
        CommandResult ExecuteCommand<T>(T command) where T : Command;
    }

}