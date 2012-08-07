using System;
using System.Collections.Generic;

namespace DropboxSync.CQRS
{
    public class ReflectingCommandTypesResolver
    {
        public IEnumerable<Type> ResolveCommandTypes()
        {
            foreach (var type in typeof(CommandTypesRegistry).Assembly.GetTypes())
            {
                if (type.BaseType == typeof(Command))
                    yield return type;
            }
        }
    }
}