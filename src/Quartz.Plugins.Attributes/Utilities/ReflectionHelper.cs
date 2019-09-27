using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Quartz.Plugins.Attributes
{
    internal static class ReflectionHelper
    {
        public readonly static Type TypeOfQuartzJob = typeof(IJob);
        public readonly static Type TypeOfQuartzJobAttribute = typeof(QuartzJobAttribute);

        private static void AddIfNotContains<T>(this ICollection<T> collection, T item)
        {
            if (!collection.Contains(item))
                collection.Add(item);
        }

        public static IEnumerable<Type> GetAssembliesTypes()
        {
            IList<Assembly> assemblies = new List<Assembly>();
            assemblies.AddIfNotContains(Assembly.GetExecutingAssembly());
            assemblies.AddIfNotContains(Assembly.GetCallingAssembly());
            assemblies.AddIfNotContains(Assembly.GetEntryAssembly());

            return assemblies.SelectMany(e => e.GetTypes());
        }

        public static bool IsQuartzJobClass(Type type)
        {
            if (type == null)
                return false;

            return TypeOfQuartzJob.IsAssignableFrom(type) && !type.IsAbstract && type.IsClass;
        }
    }
}
