using JazSharp.SpyLogic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private static readonly string[] DoNotWrapAssemblies =
        {
            typeof(Jaz).Assembly.GetName().Name
        };

        internal static void RewriteAssembly(string filename)
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

        private static void RewriteType(TypeDefinition type)
        {
            var byRefWrappers = new Dictionary<MethodDefinition, MethodDefinition>();

            for (var i = 0; i < type.Methods.Count; i++)
            {
                var method = type.Methods[i];

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
            method.Body.SimplifyMacros();

            var getMethodByTokenMethod = method.Module.ImportReference(GetMethodFromHandleMethod);
            var ilProcessor = method.Body.GetILProcessor();

            // needs to be for loop to stop exception when replacing instructions
            for (var i = 0; i < method.Body.Instructions.Count; i++)
            {
                var instruction = method.Body.Instructions[i];

                if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                {
                    var calledMethod = (MethodReference)instruction.Operand;
                    var replacement = GetSpyInstruction(method, calledMethod, byRefWrappers);

                    if (replacement != null)
                    {
                        var firstInsertedInstruction = Instruction.Create(OpCodes.Ldtoken, calledMethod);
                        ilProcessor.InsertBefore(instruction, firstInsertedInstruction);
                        ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldtoken, calledMethod.DeclaringType));
                        ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, getMethodByTokenMethod));

                        i += 3; // skip past newly inserted instructions

                        ilProcessor.Replace(instruction, replacement);

                        foreach (var referencingInstruction in method.Body.Instructions.Where(x => x.Operand == instruction))
                        {
                            referencingInstruction.Operand = firstInsertedInstruction;
                        }
                    }
                }
            }

            method.Body.OptimizeMacros();
        }

        private static Instruction GetSpyInstruction(MethodDefinition method, MethodReference calledMethod, Dictionary<MethodDefinition, MethodDefinition> byRefWrappers)
        {
            if (calledMethod != null)
            {
                if (!DoNotWrapAssemblies.Contains(calledMethod.DeclaringType.Resolve().Module.Assembly.Name.Name))
                {
                    var resolvedCalledMethod = calledMethod.Resolve();

                    if (resolvedCalledMethod != null && resolvedCalledMethod.IsConstructor)
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
                    var spyMethod = method.Module.ImportReference(entryMethod);
                    var genericMethod = new GenericInstanceMethod(spyMethod);

                    if (isFunc)
                    {
                        genericMethod.GenericArguments.Add(ResolveTypeIfGeneric(calledMethod, calledMethod.ReturnType));
                    }

                    if (calledMethod.HasThis)
                    {
                        genericMethod.GenericArguments.Add(calledMethod.DeclaringType);
                    }

                    foreach (var parameter in calledMethod.Parameters)
                    {
                        genericMethod.GenericArguments.Add(ResolveTypeIfGeneric(calledMethod, parameter.ParameterType));
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
                var spyExecutionHelperType = method.Module.ImportReference(typeof(SpyExecutionHelper));
                var objectType = method.Module.ImportReference(typeof(object));
                var objectArrayType = new ArrayType(objectType);

                wrapperMethodDefinition =
                    new MethodDefinition(
                        wrapperName,
                        Mono.Cecil.MethodAttributes.Private | Mono.Cecil.MethodAttributes.Static,
                        calledMethod.ReturnType);

                foreach (var genericParameter in resolvedCalledMethod.GenericParameters)
                {
                    wrapperMethodDefinition.GenericParameters.Add(genericParameter);
                }

                if (!resolvedCalledMethod.IsStatic)
                {
                    wrapperMethodDefinition.Parameters.Add(new ParameterDefinition(resolvedCalledMethod.DeclaringType));
                }

                foreach (var parameter in resolvedCalledMethod.Parameters)
                {
                    wrapperMethodDefinition.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, parameter.ParameterType));
                }

                wrapperMethodDefinition.Parameters.Add(new ParameterDefinition(methodBaseType));

                wrapperMethodDefinition.Body.Variables.Add(new VariableDefinition(objectArrayType));

                if (isFunc)
                {
                    wrapperMethodDefinition.Body.Variables.Add(new VariableDefinition(resolvedCalledMethod.ReturnType));
                }

                var defaultVariables = new Dictionary<TypeReference, VariableDefinition>();

                foreach (var outParameter in wrapperMethodDefinition.Parameters.Where(x => x.IsOut))
                {
                    if (!defaultVariables.ContainsKey(outParameter.ParameterType))
                    {
                        var variable = new VariableDefinition(ResolveIfByRefType(outParameter.ParameterType));
                        defaultVariables.Add(outParameter.ParameterType, variable);
                        wrapperMethodDefinition.Body.Variables.Add(variable);
                    }
                }

                var ilProcessor = wrapperMethodDefinition.Body.GetILProcessor();

                foreach (var defaultVariable in defaultVariables.Values)
                {
                    ilProcessor.Emit(OpCodes.Ldloca, defaultVariable);
                    ilProcessor.Emit(OpCodes.Initobj, defaultVariable.VariableType);
                }

                ilProcessor.Emit(OpCodes.Ldc_I4, wrapperMethodDefinition.Parameters.Count - 1);
                ilProcessor.Emit(OpCodes.Newarr, objectType);

                for (var i = 0; i < wrapperMethodDefinition.Parameters.Count - 1; i++)
                {
                    var parameter = wrapperMethodDefinition.Parameters[i];

                    ilProcessor.Emit(OpCodes.Dup);
                    ilProcessor.Emit(OpCodes.Ldc_I4, i);

                    if (parameter.IsOut)
                    {
                        ilProcessor.Emit(OpCodes.Ldloc, defaultVariables[parameter.ParameterType]);
                    }
                    else if (parameter.ParameterType.IsByReference)
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
                ilProcessor.Emit(OpCodes.Call, methodInvokeMethod);
                ilProcessor.Emit(OpCodes.Pop);

                if (isFunc)
                {
                    //ilProcessor.Emit(OpCodes.Unbox_Any, wrapperMethodDefinition.ReturnType);
                    //ilProcessor.Emit(OpCodes.Stloc_1);
                    ilProcessor.Emit(OpCodes.Ldloca, wrapperMethodDefinition.Body.Variables[1]);
                    ilProcessor.Emit(OpCodes.Initobj, wrapperMethodDefinition.ReturnType);
                }
                else
                {
                    //ilProcessor.Emit(OpCodes.Pop);
                }

                for (var i = 0; i < wrapperMethodDefinition.Parameters.Count - 1; i++)
                {
                    var parameter = wrapperMethodDefinition.Parameters[i];

                    if (parameter.ParameterType.IsByReference)
                    {
                        var actualParameterType = ResolveIfByRefType(parameter.ParameterType);

                        ilProcessor.Emit(OpCodes.Ldarg, i);
                        ilProcessor.Emit(OpCodes.Ldloc_0);
                        ilProcessor.Emit(OpCodes.Ldc_I4, i);
                        ilProcessor.Emit(OpCodes.Ldelem_Ref);
                        ilProcessor.Emit(OpCodes.Unbox_Any, actualParameterType);
                        ilProcessor.Emit(OpCodes.Stobj, actualParameterType);
                    }
                }

                if (isFunc)
                {
                    ilProcessor.Emit(OpCodes.Ldloc_1);
                }

                //if (isFunc)
                //{
                //    ilProcessor.Emit(OpCodes.Ldloca, wrapperMethodDefinition.Body.Variables[1]);
                //    ilProcessor.Emit(OpCodes.Initobj, wrapperMethodDefinition.ReturnType);
                //    ilProcessor.Emit(OpCodes.Ldloc_1);
                //}

                ilProcessor.Emit(OpCodes.Ret);

                //wrapperMethodDefinition.Body.OptimizeMacros();

                method.DeclaringType.Methods.Add(wrapperMethodDefinition);
                byRefWrappers.Add(resolvedCalledMethod, wrapperMethodDefinition);
            }

            MethodReference callReference;

            if (calledMethod is GenericInstanceMethod genericMethod)
            {
                var callGenericReference = new GenericInstanceMethod(wrapperMethodDefinition);
                
                foreach (var genericArgument in genericMethod.GenericArguments)
                {
                    callGenericReference.GenericArguments.Add(genericArgument);
                }

                callReference = callGenericReference;
            }
            else
            {
                callReference = wrapperMethodDefinition;
            }

            return Instruction.Create(OpCodes.Call, callReference);
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

                return genericArgSource.GenericArguments[genericParameterIndex];
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

                return result;
            }
            else if (type is ArrayType arrayType)
            {
                var innerType = ResolveTypeIfGeneric(method, type.GetElementType());
                return new ArrayType(innerType, arrayType.Rank);
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
    }
}
