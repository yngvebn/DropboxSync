using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cqrs.Commands.WP7
{
    public static class CommandTypesRegistry
    {
        private static readonly List<Type> CommandTypes = new List<Type>();

        public static void AddCommandType(Type commandType)
        {
            CommandTypes.Add(commandType);
        }

        public static void AddCommandTypes(IEnumerable<Type> commandTypes)
        {
            foreach (Type commandType in commandTypes)
            {
                AddCommandType(commandType);
            }
        }

        public static IEnumerable<Type> GetCommandTypes(ICustomAttributeProvider provider)
        {
            return CommandTypes;
        }

        public static bool Empty
        {
            get { return CommandTypes.Count == 0; }
        }
    }
}