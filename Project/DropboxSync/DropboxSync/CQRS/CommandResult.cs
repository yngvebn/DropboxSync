namespace DropboxSync.CQRS
{
    public class CommandResult
    {
        public CommandStatus Status { get; private set; }
        public string Message { get; private set; }
        public static CommandResult Executed(string message)
        {
            return new CommandResult(CommandStatus.Executed, message );
        }

        public static CommandResult Failed(string message)
        {
            return new CommandResult(CommandStatus.Error, message);
        }
        private CommandResult(CommandStatus status, string message)
        {
            Message = message;
            Status = status;
        }
    }
}