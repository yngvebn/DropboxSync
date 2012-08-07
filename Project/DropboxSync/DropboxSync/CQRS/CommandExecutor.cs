using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DropboxSync.CQRS
{
    public class CommandExecutor
    {
        private readonly Assembly _containingAssembly;



        public CommandExecutor(Assembly containingAssembly): this()
        {
            _containingAssembly = containingAssembly;
        }

        public CommandExecutor()
        {
            handlers = new List<object>();
        }

        private List<object> handlers { get; set; } 
        public CommandResult ExecuteCommand<T>(T command, Action<object> success = null, Action<object> error = null, Action<CommandExecutingProgress> progress = null) where T : Command
        {
            ICommandHandler<T> handler = FindHandlerForCommand<T>();
            try
            {
                handlers.Add(handler);
                handler.Handle(command, success, error, progress);
                return CommandResult.Executed("Command executed successfully");
            }
            catch (Exception ex)
            {
                return CommandResult.Failed(ex.Message);
            }
            finally
            {
                    
            }
            
        }

        public void Cancel<T>()
        {
            var handler = FindHandlerForCommand<T>();
            handlers.Where(c => c.GetType() == handler.GetType()).Cast<ICommandHandler>().ToList().ForEach(o => o.Cancel());
        }


        private static ICommandHandler<T> FindHandlerForCommand<T>()
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(typeof(T));

            var handler = Resolver.Get<ICommandHandler<T>>();
            return handler;
        }
    }

    public class CommandExecutingProgress
    {
        public int ProgressPercent { get; set; }
        public string ProgressText { get; set; }
    }
}