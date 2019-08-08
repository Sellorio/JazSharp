using JazSharp.Testing;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JazSharp.SpyLogic
{
    internal static class OriginalMethodHelper
    {
        private static readonly string[] BuiltInValueTypes =
        {
            typeof(void).ToString(),
            typeof(bool).ToString(),
            typeof(byte).ToString(),
            typeof(short).ToString(),
            typeof(int).ToString(),
            typeof(long).ToString(),
            typeof(float).ToString(),
            typeof(double).ToString(),
            typeof(decimal).ToString()
        };

        internal static MethodInfo GetMethod(string serializedInfo)
        {
            var callingMethod = new StackTrace().GetFrame(3).GetMethod();

            var path = serializedInfo.Split(':');
            var assembly = GetAssembly(path[0]);

            var typeInfo = path[1];
            var indexOfGeneric = typeInfo.IndexOf('?');
            var typeName = indexOfGeneric == -1 ? typeInfo : typeInfo.Substring(0, indexOfGeneric);
            var genericTypeArgs = indexOfGeneric == -1 ? null : typeInfo.Substring(indexOfGeneric + 1).Split(' ').Select(x => GetType(x, callingMethod)).ToArray();
            var genericDefinition = assembly.GetType(typeName);
            var type = genericDefinition;

            if (genericDefinition.IsGenericTypeDefinition)
            {
                type = genericDefinition.MakeGenericType(genericTypeArgs);
            }

            var methodInfo = path[2];
            indexOfGeneric = methodInfo.IndexOf('?');
            var methodDescription = indexOfGeneric == -1 ? methodInfo : methodInfo.Substring(0, indexOfGeneric);
            var genericMethodArgs = indexOfGeneric == -1 ? null : methodInfo.Substring(indexOfGeneric + 1).Split(' ').Select(x => GetType(x, callingMethod)).ToArray();

            if (genericDefinition.IsGenericTypeDefinition)
            {
                // replace references to class generic types in parameters
                methodDescription =
                    Regex.Replace(
                        methodDescription,
                        @"([\[,])([a-zA-Z0-9_]+)([\],])",
                        x => x.Groups[1].Value + ResolveClassGenericParameterReferences(x.Groups[2].Value, genericDefinition, type) + x.Groups[3].Value);

                // replaces references to class generic types in return type
                methodDescription =
                    Regex.Replace(
                        methodDescription,
                        "^[a-zA-Z0-9_]+ ",
                        x => ResolveClassGenericParameterReferences(x.Value.Substring(0, x.Value.Length - 1), genericDefinition, type, true) + ' ');
            }

            var method =
                type
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .FirstOrDefault(x => x.ToString() == methodDescription);

            if (method == null)
            {
                throw new JazSpyException("Unable to resolve original method call from spy.");
            }

            if (method.IsGenericMethodDefinition)
            {
                method = method.MakeGenericMethod(genericMethodArgs);
            }

            return method;
        }

        private static Type GetType(string serializedInfo, MethodBase callingMethod)
        {
            if (serializedInfo.Contains('/'))
            {
                var parts = serializedInfo.Split('/');
                var assembly = GetAssembly(parts[0]);
                return assembly.GetType(parts[1]);
            }
            else // generic type reference
            {
                throw new JazSpyException("Unable to resolve original method call from spy.");
            }
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

        private static string ResolveClassGenericParameterReferences(string type, Type genericDefinition, Type concreteType, bool forReturnType = false)
        {
            var genericArguments = genericDefinition.GetGenericArguments();

            for (var i = 0; i < genericArguments.Length; i++)
            {
                if (genericArguments[i].Name == type)
                {
                    var result = concreteType.GenericTypeArguments[i].FullName;

                    if (forReturnType && BuiltInValueTypes.Contains(result))
                    {
                        result = concreteType.GenericTypeArguments[i].Name;
                    }

                    return result;
                }
            }

            return type;
        }
    }
}
