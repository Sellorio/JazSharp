using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace JazSharp.Testing.AssemblyLoading
{
    internal class MyAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly string[] _dllSearchPaths;

        public MyAssemblyLoadContext(string[] dllSearchPaths)
        {
            _dllSearchPaths = dllSearchPaths;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (_dllSearchPaths != null)
            {
                var dllName = assemblyName.Name + ".dll";

                foreach (var path in _dllSearchPaths)
                {
                    var dllPath = Path.Combine(path, dllName);

                    if (File.Exists(dllPath))
                    {
                        return LoadFromAssemblyPath(dllPath);
                    }
                }
            }

            return null;
        }
    }
}
