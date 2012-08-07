using System;

namespace DropboxSync.CQRS
{
    public interface ICommandHandler
    {
        void Cancel();
    }
    public interface ICommandHandler<T>: ICommandHandler
    {
        void Handle(T command, Action<object> success, Action<object> error, Action<CommandExecutingProgress> progress);
        
    }
}