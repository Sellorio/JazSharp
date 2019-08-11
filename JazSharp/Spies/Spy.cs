using JazSharp.SpyLogic;
using JazSharp.SpyLogic.Behaviour;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JazSharp.Spies
{
    public class Spy : ISpy
    {
        private static readonly List<Spy> _spies = new List<Spy>();

        internal MethodInfo Method { get; }
        internal object Key { get; }
        internal List<object[]> CallLog { get; } = new List<object[]>();
        internal Queue<SpyBehaviourBase> Behaviours { get; } = new Queue<SpyBehaviourBase>();

        public SpyAnd And => new SpyAnd(this);

        Spy ISpy.Spy => this;

        private Spy(MethodInfo method, object key)
        {
            Method = method;
            Key = key;

            // setup up spy to only return a default value forever
            var defaultBehaviour = new DefaultBehaviour();
            defaultBehaviour.UpdateLifetime(int.MaxValue);
            Behaviours.Enqueue(defaultBehaviour);
        }

        internal static Spy Create(MethodInfo method, object key)
        {
            method = GetRootDefinition(method);

            Get(method, key)?.Dispose();

            var spy = new Spy(method, key);
            _spies.Add(spy);
            return spy;
        }

        internal static Spy Get(MethodInfo method, object key)
        {
            method = GetRootDefinition(method);
            return _spies.FirstOrDefault(x => x.Method == method && x.Key == key);
        }

        public void Dispose()
        {
            _spies.Remove(this);
        }

        internal static void ClearAll()
        {
            _spies.Clear();
        }

        private static MethodInfo GetRootDefinition(MethodInfo method)
        {
            method = method.IsGenericMethod ? method.GetGenericMethodDefinition() : method;
            method = method.GetBaseDefinition();
            return method;
        }
    }
}
