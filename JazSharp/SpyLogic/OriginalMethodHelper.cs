using JazSharp.Testing;
using System;
using System.Linq;
using System.Reflection;

namespace JazSharp.SpyLogic
{
    internal static class OriginalMethodHelper
    {
        internal static MethodInfo GetMethod(string serializedInfo)
        {
            var path = serializedInfo.Split(':');
            var assembly = GetAssembly(path[0]);

            var typeInfo = path[1];
            var indexOfGeneric = typeInfo.IndexOf('?');
            var typeName = indexOfGeneric == -1 ? typeInfo : typeInfo.Substring(0, indexOfGeneric);
            var genericTypeArgs = indexOfGeneric == -1 ? null : typeInfo.Substring(indexOfGeneric + 1).Split(' ').Select(GetType).ToArray();
            var type = assembly.GetType(typeName);

            if (type.IsGenericTypeDefinition)
            {
                type = type.MakeGenericType(genericTypeArgs);
            }

            var methodInfo = path[2];
            indexOfGeneric = methodInfo.IndexOf('?');
            var methodDescription = indexOfGeneric == -1 ? methodInfo : methodInfo.Substring(0, indexOfGeneric);
            var genericMethodArgs = indexOfGeneric == -1 ? null : methodInfo.Substring(indexOfGeneric + 1).Split(' ').Select(GetType).ToArray();
            var method =
                type
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .First(x => x.ToString() == methodDescription);

            if (method.IsGenericMethodDefinition)
            {
                method = method.MakeGenericMethod(genericMethodArgs);
            }

            return method;
        }

        private static Type GetType(string serializedInfo)
        {
            var parts = serializedInfo.Split('/');
            var assembly = GetAssembly(parts[0]);
            return assembly.GetType(parts[1]);
        }

        private static Assembly GetAssembly(string name)
        {
            var result =
                AssemblyContext.Current.LoadedAssemblies.Values.FirstOrDefault(x => x.GetName().Name == name)
                    ?? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == name)
                    ?? AssemblyContext.Current.LoadByName(name)
                    ?? Assembly.Load(name); // should only be needed to load system assemblies

            if (result == null)
            {
                throw new JazSpyException("Unable to resolve original method call from spy.");
            }

            return result;
        }
    }
}
