using Mono.Cecil;
using System;

namespace JazSharp.Reflection
{
    internal static class CecilHelper
    {
        internal static TypeReference ResolveGenericParameter(TypeReference parameterType, MethodReference method, TypeReference declaringType)
        {
            if (parameterType.Name.StartsWith("!!") && method is GenericInstanceMethod genericInstanceMethod)
            {
                var parameterIndex = int.Parse(parameterType.Name.Substring(2));
                return genericInstanceMethod.GenericArguments[parameterIndex];
            }

            //TODO: Confirm this logic will work for a type nested in a generic type (with that type's method using the parent classes generic args).
            if (declaringType is GenericInstanceType genericInstanceType)
            {
                var parameterIndex = int.Parse(parameterType.Name.Substring(1));
                return genericInstanceType.GenericArguments[parameterIndex];
            }

            if (declaringType.IsNested)
            {
                return ResolveGenericParameter(parameterType, null, declaringType.DeclaringType);
            }

            throw new InvalidOperationException("Unable to resolve generic type parameter when rewriting the assembly.");
        }
    }
}
