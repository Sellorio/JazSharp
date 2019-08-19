using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace JazSharp.Testing.AssemblyLoading
{
    internal class DependencyResolver : IDisposable
    {
        private readonly Assembly _assembly;
        private readonly ICompilationAssemblyResolver _assemblyResolver;
        private readonly DependencyContext _dependencyContext;
        private readonly AssemblyLoadContext _loadContext;

        public DependencyResolver(Assembly assembly)
        {
            _assembly = assembly;
            _dependencyContext = DependencyContext.Load(_assembly);

            _assemblyResolver =
                new CompositeCompilationAssemblyResolver(
                    new ICompilationAssemblyResolver[]
                    {
                        new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(assembly.Location)),
                        new ReferenceAssemblyPathResolver(),
                        new PackageCompilationAssemblyResolver()
                    });

            _loadContext = AssemblyLoadContext.GetLoadContext(_assembly);
            _loadContext.Resolving += OnResolving;
        }

        public void Dispose()
        {
            _loadContext.Resolving -= OnResolving;
        }

        private Assembly OnResolving(AssemblyLoadContext context, AssemblyName name)
        {
            RuntimeLibrary library = _dependencyContext.RuntimeLibraries.FirstOrDefault(x => string.Equals(x.Name, name.Name, StringComparison.OrdinalIgnoreCase));

            if (library != null)
            {
                var wrapper =
                    new CompilationLibrary(
                        library.Type,
                        library.Name,
                        library.Version,
                        library.Hash,
                        library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                        library.Dependencies,
                        library.Serviceable);

                var assemblies = new List<string>();
                _assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);

                if (assemblies.Count > 0)
                {
                    return _loadContext.LoadFromAssemblyPath(assemblies[0]);
                }
            }

            return null;
        }
    }
}
