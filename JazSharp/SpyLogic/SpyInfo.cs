using JazSharp.MethodInfoSources;
using JazSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JazSharp.SpyLogic
{
    internal class SpyInfo : IDisposable
    {
        private static readonly List<SpyInfo> _spies = new List<SpyInfo>();
        private int _originalMethodPointer;
        private bool _isAttached;

        internal MethodInfo Method { get; }
        internal Dictionary<object, bool> CallThroughMapping { get; } = new Dictionary<object, bool>();
        internal Dictionary<object, object> ReturnValueMapping { get; } = new Dictionary<object, object>();
        internal Dictionary<object, Queue<object>> ReturnValuesMapping { get; } = new Dictionary<object, Queue<object>>();
        internal Dictionary<object, List<object[]>> CallsLog { get; } = new Dictionary<object, List<object[]>>();
        internal Dictionary<object, Action<object[]>> CallbackMapping { get; } = new Dictionary<object, Action<object[]>>();

        internal SpyInfo(MethodInfo method)
        {
            Method = method;
        }

        internal void Attach()
        {
            if (!_isAttached)
            {
                _originalMethodPointer = MethodPointerHelper.ReplaceMethod(Method, SpyMethods.GetSpyForMethod(Method));
                _isAttached = true;
            }
        }

        internal void Detach()
        {
            if (_isAttached)
            {
                MethodPointerHelper.RestoreMethod(Method, _originalMethodPointer);
                _originalMethodPointer = default;
                _isAttached = false;
            }
        }

        internal void Detach(object key)
        {
            if (CallThroughMapping.ContainsKey(key))
            {
                CallThroughMapping.Remove(key);
            }

            if (CallbackMapping.ContainsKey(key))
            {
                CallbackMapping.Remove(key);
            }

            if (ReturnValueMapping.ContainsKey(key))
            {
                ReturnValueMapping.Remove(key);
            }

            if (ReturnValuesMapping.ContainsKey(key))
            {
                ReturnValuesMapping.Remove(key);
            }

            if (CallsLog.ContainsKey(key))
            {
                CallsLog.Remove(key);
            }

            if (CallsLog.Count == 0)
            {
                Detach();
            }
        }

        internal static SpyInfo Create(MethodInfo method)
        {
            var spy = new SpyInfo(method);
            _spies.Add(spy);
            return spy;
        }

        internal static SpyInfo Get(MethodInfo method)
        {
            return _spies.FirstOrDefault(x => x.Method == method);
        }

        internal static SpyInfo Get(string methodFullName)
        {
            var indexOfSpace = methodFullName.IndexOf(' ');
            var indexOfColon = methodFullName.IndexOf(':');
            var classFullName = methodFullName.Substring(indexOfSpace + 1, indexOfColon - indexOfSpace - 1).Replace('/', '+');
            var methodAsString = methodFullName.Substring(0, indexOfSpace).Replace("System.Void", "Void") + " " + methodFullName.Substring(indexOfColon + 2);

            return _spies.FirstOrDefault(x => x.Method.ToString() == methodAsString && x.Method.DeclaringType.FullName == classFullName);
        }

        public void Dispose()
        {
            Detach();
        }
    }
}
