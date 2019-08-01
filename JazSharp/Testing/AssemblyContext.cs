using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace JazSharp.Testing
{
    internal class AssemblyContext : AssemblyLoadContext
    {
        internal Assembly JazSharp { get; }
        internal string[] DllSearchPaths { get; }

        internal AssemblyContext(string[] dllSearchPaths = null)
        {
            DllSearchPaths = dllSearchPaths;

            if (dllSearchPaths != null)
            {
                JazSharp = Load(typeof(Jaz).Assembly.GetName());
            }
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (DllSearchPaths != null)
            {
                var filename = assemblyName.Name + ".dll";

                foreach (var path in DllSearchPaths)
                {
                    var assemblyPath = Path.Combine(path, filename);

                    if (File.Exists(assemblyPath))
                    {
                        return LoadFromAssemblyPath(assemblyPath);
                    }
                }
            }

            return null;
        }
    }
}
