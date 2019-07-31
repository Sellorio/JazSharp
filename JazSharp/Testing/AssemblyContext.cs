using System.Reflection;
using System.Runtime.Loader;

namespace JazSharp.Testing
{
    internal class AssemblyContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
