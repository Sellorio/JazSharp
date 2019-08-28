using JazSharp.SpyLogic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace JazSharp.Reflection
{
    internal static class AssemblyRewriteHelper
    {
        private static readonly MethodInfo GetMethodFromHandleMethod =
            typeof(MethodBase)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == "GetMethodFromHandle" && x.GetParameters().Length == 2);

        private static readonly MethodInfo MethodInvokeMethod = typeof(MethodBase).GetMethod(nameof(MethodBase.Invoke), new[] { typeof(object), typeof(object[]) });

        private static readonly MethodInfo HandleCallMethod =
            typeof(SpyExecutionHelper)
                .GetMethod(nameof(SpyExecutionHelper.HandleCall), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo InnerExceptionGetter =
            typeof(Exception).GetProperty(nameof(Exception.InnerException)).GetMethod;

        private static readonly MethodInfo ExceptionServicesCapture =
            typeof(ExceptionDispatchInfo).GetMethod(nameof(ExceptionDispatchInfo.Capture), BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo ExceptionServicesThrow =
            typeof(ExceptionDispatchInfo).GetMethod(nameof(ExceptionDispatchInfo.Throw), new Type[0]);

        private static readonly string[] DoNotWrapAssemblies =
        {
            typeof(Jaz).Assembly.GetName().Name
        };

        internal static void RewriteAssembly(string filename)
        {
            try
            {
                using (var assembly = AssemblyDefinition.ReadAssembly(filename, new ReaderParameters { ReadWrite = true, ReadSymbols = true }))
                {
                    foreach (var type in assembly.MainModule.Types.Where(x => !x.IsInterface))
                    {
                        RewriteType(type);
                    }

                    assembly.Write(new WriterParameters { WriteSymbols = true });
                }
            }
            catch (BadImageFormatException)
            {
                // native dll, no rewrite required
            }
        }

        private static void RewriteType(TypeDefinition type)
        {
            var byRefWrappers = new Dictionary<MethodDefinition, MethodDefinition>();

            // using ToList to copy list since sometimes RewriteMethod adds wrapper methods to the class
            foreach (var method in type.Methods.ToList())
            {
                if (!byRefWrappers.Values.Contains(method))
                {
                    RewriteMethod(method, byRefWrappers);
                }
            }

            foreach (var subType in type.NestedTypes.Where(x => !x.IsInterface))
            {
                RewriteType(subType);
            }
        }

        private static void RewriteMethod(MethodDefinition method, Dictionary<MethodDefinition, MethodDefinition> byRefWrappers)
        {
            if (method.IsPInvokeImpl || IsDelegate(method.DeclaringType))
            {
                return;
            }

            method.Body.SimplifyMacros();

            var getMethodByTokenMethod = method.Module.ImportReference(GetMethodFromHandleMethod);
            var ilProcessor = method.Body.GetILProcessor();

            var hasConstrainedModifier = false;

            // needs to be for loop to stop exception when replacing instructions
            for (var i = 0; i < method.Body.Instructions.Count; i++)
            {
                var instruction = method.Body.Instructions[i];

                if (instruction.OpCode == OpCodes.Constrained)
                {
                    hasConstrainedModifier = true;
                }
                else if ((instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt) && !hasConstrainedModifier)
                {
                    var calledMethod = (MethodReference)instruction.Operand;

                    var replacement = GetSpyInstruction(method, calledMethod, byRefWrappers, instruction.OpCode == OpCodes.Call);

                    if (replacement != null)
                    {
                        var firstInsertedInstruction = Instruction.Create(OpCodes.Ldtoken, method.Module.ImportReference(calledMethod));
                        ilProcessor.InsertBefore(instruction, firstInsertedInstruction);
                        ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldtoken, method.Module.ImportReference(calledMethod.DeclaringType)));
                        ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, getMethodByTokenMethod));

                        i += 3; // skip past newly inserted instructions

                        ilProcessor.Replace(instruction, replacement);

                        UpdateBranches(method.Body.Instructions, instruction, firstInsertedInstruction);
                    }
                }
                else
                {
                    hasConstrainedModifier = false;
                }
            }

            method.Body.OptimizeMacros();
        }

        private static void UpdateBranches(IList<Instruction> instructions, Instruction from, Instruction to)
        {
            foreach (var referencingInstruction in instructions.Where(x => x.Operand == from))
            {
                referencingInstruction.Operand = to;
            }
        }

        private static Instruction GetSpyInstruction(
            MethodDefinition method,
            MethodReference calledMethod,
            Dictionary<MethodDefinition, MethodDefinition> byRefWrappers,
            bool isUsingCallOpCode)
        {
            if (calledMethod != null)
            {
                if (!DoNotWrapAssemblies.Contains(calledMethod.DeclaringType.Resolve().Module.Assembly.Name.Name))
                {
                    var resolvedCalledMethod = calledMethod.Resolve();

                    // calls to base.XXX for properties or methods - cannot be spied on at this time.
                    var isBaseCall = resolvedCalledMethod != null && isUsingCallOpCode && resolvedCalledMethod.IsVirtual;
                    // cannot spy on constructors intentionally
                    var isConstructor = resolvedCalledMethod != null && resolvedCalledMethod.IsConstructor;

                    // compiler generated await logic
                    if (calledMethod.DeclaringType.FullName.StartsWith("System.Runtime.CompilerServices.AsyncTaskMethodBuilder")
                        || calledMethod.DeclaringType.FullName.StartsWith("System.Runtime.CompilerServices.TaskAwaiter"))
                    {
                        return null;
                    }

                    if (isBaseCall || isConstructor)
                    {
                        return null;
                    }

                    if (calledMethod.Parameters.Any(x => x.ParameterType.IsByReference))
                    {
                        return resolvedCalledMethod == null ? null : GetWrappedByRefCall(method, calledMethod, resolvedCalledMethod, byRefWrappers);
                    }

                    var isStatic = !calledMethod.HasThis;
                    var isFunc = calledMethod.ReturnType.FullName != "System.Void";

                    var parameterCount = isStatic ? calledMethod.Parameters.Count : calledMethod.Parameters.Count + 1;
                    var entryMethodName = (isFunc ? "Func" : "Action") + parameterCount;
                    var entryMethod = typeof(SpyEntryPoints).GetMethod(entryMethodName, BindingFlags.Static | BindingFlags.Public);

                    // method has more parameters than is supported by the current set of spy entry point methods
                    if (entryMethod == null)
                    {
                        return null;
                    }

                    var spyMethod = method.Module.ImportReference(entryMethod);
                    var genericMethod = new GenericInstanceMethod(spyMethod);

                    if (isFunc)
                    {
                        genericMethod.GenericArguments.Add(method.Module.ImportReference(ResolveTypeIfGeneric(calledMethod, calledMethod.ReturnType)));
                    }

                    if (!isStatic)
                    {
                        genericMethod.GenericArguments.Add(method.Module.ImportReference(calledMethod.DeclaringType));
                    }

                    foreach (var parameter in calledMethod.Parameters)
                    {
                        genericMethod.GenericArguments.Add(method.Module.ImportReference(ResolveTypeIfGeneric(calledMethod, parameter.ParameterType)));
                    }

                    return Instruction.Create(OpCodes.Call, genericMethod);
                }
            }

            return null;
        }

        private static Instruction GetWrappedByRefCall(
            MethodDefinition method,
            MethodReference calledMethod,
            MethodDefinition resolvedCalledMethod,
            Dictionary<MethodDefinition, MethodDefinition> byRefWrappers)
        {
            // might never get run since MethodReference is not IEquatable
            if (!byRefWrappers.TryGetValue(resolvedCalledMethod, out var wrapperMethodDefinition))
            {
                var isFunc = resolvedCalledMethod.ReturnType.FullName != "System.Void";
                var wrapperName = "JazSharpByRefWrapper" + Guid.NewGuid();

                var methodBaseType = method.Module.ImportReference(typeof(MethodBase));
                var handleCallMethod = method.Module.ImportReference(HandleCallMethod);
                var methodInvokeMethod = method.Module.ImportReference(MethodInvokeMethod);
                var getMethodByTokenMethod = method.Module.ImportReference(GetMethodFromHandleMethod);
                var objectType = method.Module.ImportReference(typeof(object));
                var objectArrayType = new ArrayType(objectType);
                var innerExceptionGetter = method.Module.ImportReference(InnerExceptionGetter);
                var exceptionServicesCapture = method.Module.ImportReference(ExceptionServicesCapture);
                var exceptionServicesThrow = method.Module.ImportReference(ExceptionServicesThrow);

                wrapperMethodDefinition =
                    new MethodDefinition(
                        wrapperName,
                        Mono.Cecil.MethodAttributes.Private | Mono.Cecil.MethodAttributes.Static,
                        calledMethod.ReturnType);

                wrapperMethodDefinition.DeclaringType = method.DeclaringType;

                foreach (var genericParameter in resolvedCalledMethod.DeclaringType.GenericParameters)
                {
                    wrapperMethodDefinition.GenericParameters.Add(new GenericParameter(genericParameter.Name, wrapperMethodDefinition));
                }

                foreach (var genericParameter in resolvedCalledMethod.GenericParameters)
                {
                    wrapperMethodDefinition.GenericParameters.Add(new GenericParameter(genericParameter.Name, wrapperMethodDefinition));
                }

                wrapperMethodDefinition.ReturnType = ReplaceGenericParameterReferences(method, calledMethod.ReturnType, wrapperMethodDefinition.GenericParameters);

                if (!resolvedCalledMethod.IsStatic)
                {
                    wrapperMethodDefinition.Parameters.Add(new ParameterDefinition(resolvedCalledMethod.DeclaringType));
                }

                foreach (var parameter in resolvedCalledMethod.Parameters)
                {
                    var actualParameterType =
                        ReplaceGenericParameterReferences(
                            method,
                            parameter.ParameterType,
                            wrapperMethodDefinition.GenericParameters);

                    wrapperMethodDefinition.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, actualParameterType));
                }

                wrapperMethodDefinition.Parameters.Add(new ParameterDefinition(methodBaseType));

                wrapperMethodDefinition.Body.Variables.Add(new VariableDefinition(objectArrayType));
                wrapperMethodDefinition.Body.Variables.Add(new VariableDefinition(method.Module.ImportReference(typeof(Exception))));

                if (isFunc)
                {
                    wrapperMethodDefinition.Body.Variables.Add(new VariableDefinition(wrapperMethodDefinition.ReturnType));
                }

                var ilProcessor = wrapperMethodDefinition.Body.GetILProcessor();

                for (var i = 0; i < wrapperMethodDefinition.Parameters.Count; i++)
                {
                    var parameter = wrapperMethodDefinition.Parameters[i];

                    if (parameter.IsOut)
                    {
                        ilProcessor.Emit(OpCodes.Ldarg, i);
                        ilProcessor.Emit(OpCodes.Initobj, ResolveIfByRefType(parameter.ParameterType));
                    }
                }

                ilProcessor.Emit(OpCodes.Ldc_I4, wrapperMethodDefinition.Parameters.Count - 1);
                ilProcessor.Emit(OpCodes.Newarr, objectType);

                for (var i = 0; i < wrapperMethodDefinition.Parameters.Count - 1; i++)
                {
                    var parameter = wrapperMethodDefinition.Parameters[i];

                    ilProcessor.Emit(OpCodes.Dup);
                    ilProcessor.Emit(OpCodes.Ldc_I4, i);

                    if (parameter.ParameterType.IsByReference)
                    {
                        var actualParameterType = ResolveIfByRefType(wrapperMethodDefinition.Parameters[i].ParameterType);

                        ilProcessor.Emit(OpCodes.Ldarg, i);
                        ilProcessor.Emit(OpCodes.Ldobj, actualParameterType);
                        ilProcessor.Emit(OpCodes.Box, actualParameterType);
                    }
                    else
                    {
                        ilProcessor.Emit(OpCodes.Ldarg, i);
                        ilProcessor.Emit(OpCodes.Box, parameter.ParameterType);
                    }

                    ilProcessor.Emit(OpCodes.Stelem_Ref);
                }

                ilProcessor.Emit(OpCodes.Stloc_0);

                var tryStart = Instruction.Create(OpCodes.Nop);
                ilProcessor.Append(tryStart);

                // get method info for SpyExecutionHelper.HandleCall to circumvent access checks
                ilProcessor.Emit(OpCodes.Ldtoken, handleCallMethod);
                ilProcessor.Emit(OpCodes.Ldtoken, handleCallMethod.DeclaringType);
                ilProcessor.Emit(OpCodes.Call, getMethodByTokenMethod);
                // pass null for the instance parameter of the MethodBase.Invoke method
                ilProcessor.Emit(OpCodes.Ldnull);
                // create a new array for the parameters to HandleCall
                ilProcessor.Emit(OpCodes.Ldc_I4_2);
                ilProcessor.Emit(OpCodes.Newarr, objectType);
                // pass in the value for the HandleCall parameters parameter
                ilProcessor.Emit(OpCodes.Dup);
                ilProcessor.Emit(OpCodes.Ldc_I4_0);
                ilProcessor.Emit(OpCodes.Ldloc_0);
                ilProcessor.Emit(OpCodes.Stelem_Ref);
                // pass in the value for the HandleCall method parameter
                ilProcessor.Emit(OpCodes.Dup);
                ilProcessor.Emit(OpCodes.Ldc_I4_1);
                ilProcessor.Emit(OpCodes.Ldarg, wrapperMethodDefinition.Parameters.Count - 1);
                ilProcessor.Emit(OpCodes.Stelem_Ref);
                // invoke HandleCall
                var invokeInstruction = Instruction.Create(OpCodes.Call, methodInvokeMethod);
                ilProcessor.Append(invokeInstruction);

                Instruction leaveAfterThisInstruction;

                if (isFunc)
                {
                    ilProcessor.Emit(OpCodes.Unbox_Any, wrapperMethodDefinition.ReturnType);
                    var stLoc = Instruction.Create(OpCodes.Stloc_2);
                    ilProcessor.Append(stLoc);
                    leaveAfterThisInstruction = stLoc;
                }
                else
                {
                    var pop = Instruction.Create(OpCodes.Pop);
                    ilProcessor.Append(pop);
                    leaveAfterThisInstruction = pop;
                }

                var catchStart = Instruction.Create(OpCodes.Stloc_1);
                ilProcessor.Append(catchStart);
                ilProcessor.Emit(OpCodes.Ldloc_1);
                ilProcessor.Emit(OpCodes.Callvirt, innerExceptionGetter);
                ilProcessor.Emit(OpCodes.Call, exceptionServicesCapture);
                ilProcessor.Emit(OpCodes.Callvirt, exceptionServicesThrow);
                var rethrow = Instruction.Create(OpCodes.Rethrow);
                ilProcessor.Append(rethrow);

                Instruction postCatchInstruction = null;

                for (var i = 0; i < wrapperMethodDefinition.Parameters.Count - 1; i++)
                {
                    var parameter = wrapperMethodDefinition.Parameters[i];

                    if (parameter.ParameterType.IsByReference)
                    {
                        var actualParameterType = ResolveIfByRefType(parameter.ParameterType);

                        var loadParameterInstruction = Instruction.Create(OpCodes.Ldarg, wrapperMethodDefinition.Parameters[i]);

                        if (postCatchInstruction == null)
                        {
                            postCatchInstruction = loadParameterInstruction;
                        }

                        ilProcessor.Append(loadParameterInstruction);
                        ilProcessor.Emit(OpCodes.Ldloc_0);
                        ilProcessor.Emit(OpCodes.Ldc_I4, i);
                        ilProcessor.Emit(OpCodes.Ldelem_Ref);
                        ilProcessor.Emit(OpCodes.Unbox_Any, actualParameterType);
                        ilProcessor.Emit(OpCodes.Stobj, actualParameterType);
                    }
                }

                if (isFunc)
                {
                    var loadReturnVariableInstruction = Instruction.Create(OpCodes.Ldloc_2);

                    if (postCatchInstruction == null)
                    {
                        postCatchInstruction = loadReturnVariableInstruction;
                    }

                    ilProcessor.Append(loadReturnVariableInstruction);
                }

                var returnInstruction = Instruction.Create(OpCodes.Ret);

                if (postCatchInstruction == null)
                {
                    postCatchInstruction = returnInstruction;
                }

                ilProcessor.Append(returnInstruction);

                ilProcessor.InsertAfter(leaveAfterThisInstruction, Instruction.Create(OpCodes.Leave, postCatchInstruction));

                wrapperMethodDefinition.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    CatchType = method.Module.ImportReference(typeof(TargetInvocationException)),
                    TryStart = tryStart,
                    TryEnd = catchStart,
                    HandlerStart = catchStart,
                    HandlerEnd = rethrow.Next
                });

                wrapperMethodDefinition.Body.OptimizeMacros();

                method.DeclaringType.Methods.Add(wrapperMethodDefinition);
                byRefWrappers.Add(resolvedCalledMethod, wrapperMethodDefinition);
            }

            MethodReference wrapperCall;
            var genericType = calledMethod.DeclaringType as GenericInstanceType;
            var genericMethod = calledMethod as GenericInstanceMethod;

            if (genericType != null || genericMethod != null)
            {
                var genericCall = new GenericInstanceMethod(wrapperMethodDefinition);

                if (genericType != null)
                {
                    foreach (var genericArgument in genericType.GenericArguments)
                    {
                        genericCall.GenericArguments.Add(genericArgument);
                    }
                }

                if (genericMethod != null)
                {
                    foreach (var genericArgument in genericMethod.GenericArguments)
                    {
                        genericCall.GenericArguments.Add(genericArgument);
                    }
                }

                wrapperCall = genericCall;
            }
            else
            {
                wrapperCall = wrapperMethodDefinition;
            }

            return Instruction.Create(OpCodes.Call, wrapperCall);
        }

        private static TypeReference ResolveTypeIfGeneric(MethodReference method, TypeReference type)
        {
            // reference to a type parameter on the method or it's declaring type
            if (type.IsGenericParameter)
            {
                int genericParameterIndex = -1;
                IGenericInstance genericArgSource = null;

                // references like !0 and !!0
                var indexBasedMatch = Regex.Match(type.Name, @"^\!(\!|)([0-9]+)$");

                if (indexBasedMatch.Success)
                {
                    genericParameterIndex = int.Parse(indexBasedMatch.Groups[2].Value);

                    if (indexBasedMatch.Groups[1].Value != string.Empty)
                    {
                        genericArgSource = (GenericInstanceMethod)method;
                    }
                    else
                    {
                        genericArgSource = (GenericInstanceType)method.DeclaringType;
                    }
                }
                else // references like TValue
                {
                    if (method is GenericInstanceMethod genericMethod)
                    {
                        var definition = genericMethod.Resolve();
                        var indexOfGenericParameter = IndexOfItem(definition.GenericParameters, x => x.Name == type.Name);

                        if (indexOfGenericParameter != -1)
                        {
                            genericParameterIndex = indexOfGenericParameter;
                            genericArgSource = genericMethod;
                        }
                    }

                    if (genericArgSource == null && method.DeclaringType is GenericInstanceType genericType)
                    {
                        var definition = genericType.Resolve();
                        var indexOfGenericParameter = IndexOfItem(definition.GenericParameters, x => x.Name == type.Name);

                        if (indexOfGenericParameter != -1)
                        {
                            genericParameterIndex = indexOfGenericParameter;
                            genericArgSource = genericType;
                        }
                    }
                }

                if (genericParameterIndex == -1 || genericArgSource == null)
                {
                    throw new InvalidOperationException("Failed to resolve generic parameter when rewriting assembly.");
                }

                return method.Module.ImportReference(genericArgSource.GenericArguments[genericParameterIndex]);
            }
            else if (type is GenericInstanceType genericType)
            {
                TypeReference genericTypeDefinition = genericType.Resolve();
                genericTypeDefinition = method.Module.ImportReference(genericTypeDefinition);
                var result = new GenericInstanceType(genericTypeDefinition);

                foreach (var arg in ((GenericInstanceType)type).GenericArguments)
                {
                    result.GenericArguments.Add(ResolveTypeIfGeneric(method, arg));
                }

                return method.Module.ImportReference(result);
            }
            else if (type is ArrayType arrayType)
            {
                var innerType = ResolveTypeIfGeneric(method, type.GetElementType());
                return new ArrayType(method.Module.ImportReference(innerType), arrayType.Rank);
            }

            return type;
        }

        private static TypeReference ResolveIfByRefType(TypeReference type)
        {
            if (type is ByReferenceType byReferenceType)
            {
                return byReferenceType.ElementType;
            }

            return type;
        }

        private static TypeReference ReplaceGenericParameterReferences(MethodDefinition method, TypeReference type, IList<GenericParameter> newParameterSource)
        {
            if (type is ByReferenceType refType)
            {
                return new ByReferenceType(ReplaceGenericParameterReferences(method, refType.ElementType, newParameterSource));
            }
            else if (type is GenericParameter genericParameter) // using the same type generic parameters
            {
                var flattenedPosition = genericParameter.Position;

                // method contains generic type parameters of original method as well
                if (genericParameter.DeclaringMethod != null)
                {
                    flattenedPosition += genericParameter.DeclaringMethod.DeclaringType.GenericParameters.Count;
                }

                return newParameterSource[flattenedPosition];
            }
            else if (type is GenericInstanceType genericInstanceType)
            {
                var result = new GenericInstanceType(method.Module.ImportReference(genericInstanceType.Resolve()));

                foreach (var arg in genericInstanceType.GenericArguments)
                {
                    result.GenericArguments.Add(ReplaceGenericParameterReferences(method, arg, newParameterSource));
                }

                return result;
            }
            else if (type is ArrayType arrayType)
            {
                var innerType = ReplaceGenericParameterReferences(method, arrayType.GetElementType(), newParameterSource);
                return new ArrayType(innerType, arrayType.Rank);
            }

            return method.Module.ImportReference(type);
        }

        private static int IndexOfItem<TObject>(IEnumerable<TObject> enumerable, Func<TObject, bool> matcher)
        {
            var index = 0;

            foreach (var item in enumerable)
            {
                if (matcher.Invoke(item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        private static bool IsDelegate(TypeDefinition typeDefinition)
        {
            return
                typeDefinition != null
                && (typeDefinition.FullName == typeof(Delegate).FullName || IsDelegate(typeDefinition.BaseType?.Resolve()));
        }
    }
}
