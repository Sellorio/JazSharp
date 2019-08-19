using System;
using System.Reflection;
using System.Runtime.Loader;

namespace JazSharp.Testing.AssemblyLoading
{
    internal class MyAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
