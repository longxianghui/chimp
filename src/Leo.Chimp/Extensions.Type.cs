using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Leo.Chimp
{
    public static class TypeExtensions
    {
        /// <summary>
        /// GetNonSystemAssembly
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static List<Assembly> GetNonSystemAssembly(this AppDomain domain)
        {
            var allAssemblies = domain.GetAssemblies();
            return allAssemblies.Where(x => !x.FullName.StartsWith("Microsoft") && !x.FullName.StartsWith("System"))
                .ToList();
        }

        public static List<Assembly> GetCurrentPathAssembly(this AppDomain domain)
        {
            var dlls = DependencyContext.Default.CompileLibraries
                .Where(x => !x.Name.StartsWith("Microsoft") && !x.Name.StartsWith("System"))
                .ToList();
            var list = new List<Assembly>();
            if (dlls.Any())
            {
                foreach (var dll in dlls)
                {
                    if (dll.Type == "project")
                    {
                        list.Add(Assembly.Load(dll.Name));
                    }
                }
            }
            return list;
        }

       
        public static bool HasImplementedRawGeneric(this Type type, Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));
            var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
            if (isTheRawGenericType) return true;
            while (type != null && type != typeof(object))
            {
                isTheRawGenericType = IsTheRawGenericType(type);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }
            return false;

            bool IsTheRawGenericType(Type test)
                => generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }
    }
}
