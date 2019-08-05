using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace JazSharp.Testing
{
    internal class AssemblyContext : IDisposable
    {
        private readonly MyAssemblyLoadContext _assemblyLoadContext;

        internal string[] DllSearchPaths { get; }
        internal Dictionary<string, Assembly> LoadedAssemblies { get; private set; } = new Dictionary<string, Assembly>();

        internal static AssemblyContext Current { get; private set; }

        internal AssemblyContext(string[] dllSearchPaths = null)
        {
            DllSearchPaths = dllSearchPaths;
            _assemblyLoadContext = new MyAssemblyLoadContext(dllSearchPaths);
            _assemblyLoadContext.OnLoad += a => LoadedAssemblies.Add(a.GetName().Name, a);
            _assemblyLoadContext.LoadFromAssemblyName(typeof(Jaz).Assembly.GetName());
        }

        internal Assembly Load(string assemblyPath)
        {
            var result = _assemblyLoadContext.LoadFromAssemblyPath(assemblyPath);

            if (result != null)
            {
                LoadedAssemblies.Add(result.GetName().Name, result);
            }

            return result;
        }

        internal Assembly LoadByName(string assemblyName)
        {
            return _assemblyLoadContext.Load(assemblyName);
        }

        public void Dispose()
        {
            //TODO: unload assemblies
        }
        
        internal static void SetupCurrent(string[] dllSearchPaths, Dictionary<string, Assembly> loadedAssemblies)
        {
            Current = new AssemblyContext(dllSearchPaths)
            {
                LoadedAssemblies = loadedAssemblies
            };
        }

        private class MyAssemblyLoadContext : AssemblyLoadContext
        {
            private readonly string[] _dllSearchPaths;

            public event Action<Assembly> OnLoad;

            public MyAssemblyLoadContext(string[] dllSearchPaths)
            {
                _dllSearchPaths = dllSearchPaths;
            }

            internal Assembly Load(string assemblyName)
            {
                if (_dllSearchPaths != null)
                {
                    var filename = assemblyName + ".dll";

                    foreach (var path in _dllSearchPaths)
                    {
                        var assemblyPath = Path.Combine(path, filename);

                        if (File.Exists(assemblyPath))
                        {
                            var result = LoadFromAssemblyPath(assemblyPath);

                            if (result != null)
                            {
                                OnLoad?.Invoke(result);
                            }

                            return result;
                        }
                    }
                }

                return null;
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                return Load(assemblyName.Name);
            }
        }
    }
}
