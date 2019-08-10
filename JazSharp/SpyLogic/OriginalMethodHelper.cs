using JazSharp.Testing;
using System;
using System.Collections.Generic;
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

        internal static MethodInfo GetMethod(string serializedInfo, Type calledMethodType)
        {
            var callingMethod = new StackTrace().GetFrame(3).GetMethod();

            var path = serializedInfo.Split(':');
            var assembly = GetAssembly(path[0]);

            //var typeInfo = path[1];
            //var indexOfGeneric = typeInfo.IndexOf('?');
            //var typeName = indexOfGeneric == -1 ? typeInfo : typeInfo.Substring(0, indexOfGeneric);
            //var genericTypeArgs = indexOfGeneric == -1 ? null : typeInfo.Substring(indexOfGeneric + 1).Split(' ').Select(x => GetType(x, genericTypeMapping)).ToArray();
            //var genericDefinition = assembly.GetType(typeName);
            //var type = genericDefinition;

            //if (genericDefinition.IsGenericTypeDefinition)
            //{
            //    type = genericDefinition.MakeGenericType(genericTypeArgs);
            //}

            var type = calledMethodType;

            var methodInfo = path[2];
            var indexOfGeneric = methodInfo.IndexOf('?');
            var methodDescription = indexOfGeneric == -1 ? methodInfo : methodInfo.Substring(0, indexOfGeneric);
            var genericMethodArgs = indexOfGeneric == -1 ? null : methodInfo.Substring(indexOfGeneric + 1).Split(' ').Select(GetType).ToArray();

            MethodInfo matchedMethod = null;

            if (type.IsGenericType)
            {
                var genericDefinition = type.GetGenericTypeDefinition();
                var methods = genericDefinition.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                
                for (var i = 0; i < methods.Length; i++)
                {
                    if (methods[i].ToString() == methodDescription)
                    {
                        matchedMethod = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)[i];
                        break;
                    }
                }
            }
            else
            {
                matchedMethod =
                    type
                        .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .FirstOrDefault(x => x.ToString() == methodDescription);
            }

            if (matchedMethod == null)
            {
                throw new JazSpyException("Unable to resolve original method call from spy.");
            }

            if (matchedMethod.IsGenericMethodDefinition)
            {
                matchedMethod = matchedMethod.MakeGenericMethod(genericMethodArgs);
            }

            return matchedMethod;
        }

        private static Type GetType(string serializedInfo)
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
