using JazSharp.SpyLogic;
using JazSharp.SpyLogic.Behaviour;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace JazSharp.Spies
{
    /// <summary>
    /// A spy which has been applied to a method or property get/set.
    /// </summary>
    public class Spy : ISpy
    {
        private static readonly List<Spy> _spies = new List<Spy>();

        internal MethodInfo[] Methods { get; }
        internal object Key { get; }
        internal List<ImmutableArray<object>> CallLog { get; } = new List<ImmutableArray<object>>();
        internal Queue<SpyBehaviourBase> Behaviours { get; } = new Queue<SpyBehaviourBase>();

        Spy ISpy.Spy => this;

        /// <summary>
        /// A property used to begin specifying the behaviour for the spy.
        /// </summary>
        public SpyAnd And => new SpyAnd(this);

        /// <summary>
        /// The calls that have been made to the spy at this point in time.
        /// </summary>
        public IReadOnlyList<SpyCall> Calls => ImmutableArray.CreateRange(CallLog.Select(x => new SpyCall(x)));

        private Spy(MethodInfo[] methods, object key)
        {
            Methods = methods;
            Key = key;

            // setup up spy to only return a default value forever
            var defaultBehaviour = new DefaultBehaviour();
            defaultBehaviour.UpdateLifetime(int.MaxValue);
            Behaviours.Enqueue(defaultBehaviour);
        }

        /// <summary>
        /// Removes the spy, reverting the target method to call-through always.
        /// </summary>
        public void Dispose()
        {
            _spies.Remove(this);
        }

        internal static Spy Create(MethodInfo[] methods, object key)
        {
            methods = methods.Select(GetRootDefinitions).ToArray();

            foreach (var method in methods)
            {
                Get(method, key)?.Dispose();
            }

            var spy = new Spy(methods, key);
            _spies.Add(spy);
            return spy;
        }

        internal static Spy Get(MethodInfo method, object key)
        {
            method = GetRootDefinitions(method);
            return _spies.FirstOrDefault(x => x.Methods.Contains(method) && x.Key == key);
        }

        internal static void ClearAll()
        {
            _spies.Clear();
        }

        private static MethodInfo GetRootDefinitions(MethodInfo method)
        {
            method = method.IsGenericMethod ? method.GetGenericMethodDefinition() : method;
            method = method.GetBaseDefinition();
            return method;
        }
    }
}
