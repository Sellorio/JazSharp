using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JazSharp.Testing.AssemblyLoading
{
    internal class AssemblyContext : IDisposable
    {
        private readonly MyAssemblyLoadContext _assemblyLoadContext;
        private List<DependencyResolver> _dependencyResolvers;

        internal Dictionary<string, Assembly> LoadedAssemblies { get; private set; } = new Dictionary<string, Assembly>();

        internal static AssemblyContext Current { get; private set; }

        internal AssemblyContext()
        {
            _assemblyLoadContext = new MyAssemblyLoadContext();
        }

        internal void InitialiseDependencyResolvers()
        {
            _dependencyResolvers = LoadedAssemblies.Select(x => new DependencyResolver(x.Value)).ToList();
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

        internal Assembly LoadByName(AssemblyName assemblyName)
        {
            var result = _assemblyLoadContext.LoadFromAssemblyName(assemblyName);

            if (result != null)
            {
                LoadedAssemblies.Add(result.GetName().Name, result);
            }

            return result;
        }

        public void Dispose()
        {
            _dependencyResolvers?.ForEach(x => x.Dispose());
            _dependencyResolvers = null;
            //TODO: unload assemblies
        }
        
        internal static void SetupCurrent(Dictionary<string, Assembly> loadedAssemblies)
        {
            Current = new AssemblyContext()
            {
                LoadedAssemblies = loadedAssemblies
            };
        }
    }
}
