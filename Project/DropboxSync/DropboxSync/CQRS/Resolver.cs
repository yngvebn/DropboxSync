using System;
using System.Linq;
using System.Reflection;

namespace DropboxSync.CQRS
{
    public static class Resolver
    {
        private static Assembly Asm
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly(); }
        }




        public static T Get<T>() where T: class
        {
            var type = Resolve(typeof (T));
            return Activator.CreateInstance(type) as T;
        }

        public static Type Resolve(Type registration)
        {
            return Asm.GetTypes().SingleOrDefault(
                c => c.GetInterfaces().Any(d => d.FullName == registration.FullName));
            
        }
    }
}