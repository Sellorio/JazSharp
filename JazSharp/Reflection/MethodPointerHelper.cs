using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace JazSharp.Reflection
{
    internal static class MethodPointerHelper
    {
        private static readonly bool Is64Bit = IntPtr.Size == 8;
        private static readonly MethodInfo GetDynamicMethodRuntimeHandle =
            typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Replaces a static method on the given type with the given method.
        /// </summary>
        /// <param name="type">The type to be modified.</param>
        /// <param name="method">The static method to replace on that type.</param>
        /// <param name="replacement">The replacement method.</param>
        /// <returns>The pointer to the original method implementation.</returns>
        internal static int ReplaceMethod(MethodInfo method, MethodInfo replacement)
        {
            var methodParameters = method.GetParameters();
            var replacementParameters = replacement.GetParameters();

            if (replacementParameters.Length != methodParameters.Length)
            {
                throw new ArgumentException("Replacement parameters do not match original method.");
            }

            RuntimeHelpers.PrepareMethod(method.MethodHandle);
            RuntimeHelpers.PrepareMethod(replacement.MethodHandle);

            int result;

            unsafe
            {

                if (Is64Bit)
                {
                    var originalPointer = ConvertMethodHandle64(method.MethodHandle);
                    var replacementPointer = ConvertMethodHandle64(replacement.MethodHandle);

                    result = OverrideMethodPointer(originalPointer, replacementPointer);
                }
                else
                {
                    var originalPointer = ConvertMethodHandle32(method.MethodHandle);
                    var replacementPointer = ConvertMethodHandle32(replacement.MethodHandle);

                    result = OverrideMethodPointer(originalPointer, replacementPointer);
                }
            }

            return result;
        }

        /// <summary>
        /// Restores the static method back to its original implementation.
        /// </summary>
        /// <param name="method">The method to be restored.</param>
        /// <param name="original">The original implementation's pointer as returned by <see cref="ReplaceMethod(MethodInfo, MethodInfo)"/>.</param>
        internal static void RestoreMethod(MethodInfo method, int original)
        {
            unsafe
            {
                if (Is64Bit)
                {
                    var replacedPointer = ConvertMethodHandle64(method.MethodHandle);

                    byte* tarInst = (byte*)*replacedPointer;
                    int* tarSrc = (int*)(tarInst + 1);
                    *tarSrc = original;
                }
                else
                {
                    var replacedPointer = ConvertMethodHandle32(method.MethodHandle);

                    byte* tarInst = (byte*)*replacedPointer;
                    int* tarSrc = (int*)(tarInst + 1);
                    *tarSrc = original;
                }
            }
        }

        internal static int GetReferencePoint(MethodInfo methodInfo)
        {
            unsafe
            {
                var pointer = ConvertMethodHandle64(methodInfo.MethodHandle);
                byte* funcInst = (byte*)*pointer;
                int* funcSrc = (int*)(funcInst + 1);
                var result = *(funcSrc + 6); // arbitrary - will hopefully help identify the original implementation
                return result;
            }
        }

        private static RuntimeMethodHandle GetMethodHandle(MethodInfo method)
        {
            if (method is DynamicMethod)
            {
                return (RuntimeMethodHandle)GetDynamicMethodRuntimeHandle.Invoke(method, null);
            }

            return method.MethodHandle;
        }

        private static unsafe int* ConvertMethodHandle32(RuntimeMethodHandle handle)
        {
            return (int*)handle.Value.ToPointer() + 2;
        }

        private static unsafe long* ConvertMethodHandle64(RuntimeMethodHandle handle)
        {
            return (long*)handle.Value.ToPointer() + 1;
        }

        private static unsafe int OverrideMethodPointer(int* method, int* replacement)
        {
            byte* injInst = (byte*)*replacement;
            byte* tarInst = (byte*)*method;

            int* injSrc = (int*)(injInst + 1);
            int* tarSrc = (int*)(tarInst + 1);

            var result = *tarSrc;

            *tarSrc = (int)injInst + 5 + *injSrc - ((int)tarInst + 5);

            return result;
        }

        private static unsafe int OverrideMethodPointer(long* method, long* replacement)
        {
            byte* injInst = (byte*)*replacement;
            byte* tarInst = (byte*)*method;

            int* injSrc = (int*)(injInst + 1);
            int* tarSrc = (int*)(tarInst + 1);

            var result = *tarSrc;

            *tarSrc = (int)injInst + 5 + *injSrc - ((int)tarInst + 5);

            return result;
        }
    }
}
