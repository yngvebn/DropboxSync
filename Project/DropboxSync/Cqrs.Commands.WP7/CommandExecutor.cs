using System;
using System.Linq;
using System.Reflection;

namespace Cqrs.Commands.WP7
{
    public class CommandExecutor
    {
        private readonly Assembly _containingAssembly;



        public CommandExecutor(Assembly containingAssembly)
        {
            _containingAssembly = containingAssembly;
        }

        public CommandExecutor()
        {
            
        }
        public string ExecuteCommand<T>(T command) where T: Command
        {
            ICommandHandler<T> handler = FindHandlerForCommand<T>();
            try
            {
                handler.Handle(command);
                return "Yes";
                //return CommandResult.Executed("Command executed successfully");
            }
            catch (Exception ex)
            {
                return "No";
                //return CommandResult.Failed(ex.Message);
            }
            finally
            {
                    
            }
            
        }

      

        private ICommandHandler<T> FindHandlerForCommand<T>()
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(typeof(T));
            var handler =
                _containingAssembly.GetTypes().SingleOrDefault(c => c.GetInterfaces().Any(d => d.Name == handlerType.Name)) as ICommandHandler<T>;
            return handler;
        }
    }
}