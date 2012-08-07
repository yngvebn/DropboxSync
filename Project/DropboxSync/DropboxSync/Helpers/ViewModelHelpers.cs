using System;
using System.Linq;
using DropboxSync.Core;

namespace DropboxSync.Helpers
{
    public static class ViewModelHelpers
    {

        public static PageDefinitionAttribute PageDefinition(this Type modelType)
        {
            var attribute = modelType.GetCustomAttributes(true).OfType<PageDefinitionAttribute>().FirstOrDefault();
            if (attribute != null) return attribute;
            throw new InvalidOperationException(String.Format("PageDefinition is missing on {0}", modelType.Name));
        }

    }
}
