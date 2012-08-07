using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DropboxSync.Core;
using DropboxSync.ViewModels;

namespace DropboxSync.Helpers
{

    public static class PageHelpers
    {
        public static ViewModel FindCurrentView(Uri uri)
        {
            var type = TryToFindPageWithReflection(uri);
            if (type == null) return null;
            return Activator.CreateInstance(type) as ViewModel;
        }

        private static Type TryToFindPageWithReflection(Uri uri)
        {
            var page = FindSubClassesOf<ViewModel>().FirstOrDefault(c => c.GetCustomAttributes(true).OfType<PageDefinitionAttribute>().Any(p => p.RelativeUri == uri));
            return page;
        }
        public static IEnumerable<Type> FindSubClassesOf<TBaseType>()
        {
            var baseType = typeof(TBaseType);
            return  Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(baseType));
        }
    }

}
