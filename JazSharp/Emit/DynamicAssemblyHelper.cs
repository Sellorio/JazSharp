using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JazSharp.Emit
{
    internal static class DynamicAssemblyHelper
    {
        internal static ModuleBuilder Module { get; }

        static DynamicAssemblyHelper()
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
            Module = assemblyBuilder.DefineDynamicModule("Module");
        }
    }
}
