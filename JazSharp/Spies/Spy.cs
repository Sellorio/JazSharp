using JazSharp.SpyLogic;
using JazSharp.SpyLogic.Behaviour;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JazSharp.Spies
{
    public class Spy
    {
        private static readonly List<Spy> _spies = new List<Spy>();

        internal object Key { get; }
        internal MethodInfo Method { get; }
        internal List<object[]> CallLog { get; } = new List<object[]>();
        internal Queue<SpyBehaviourBase> Behaviours { get; } = new Queue<SpyBehaviourBase>();

        public SpyAnd And => new SpyAnd(this);
        public SpyThen Then => new SpyThen(this);

        private Spy(MethodInfo method, object key)
        {
            Method = method.GetBaseDefinition();
            Key = key;
            var defaultBehaviour = new DefaultBehaviour();
            defaultBehaviour.UpdateLifetime(int.MaxValue);
            Behaviours.Enqueue(defaultBehaviour);
        }

        internal static Spy Create(MethodInfo method, object key)
        {
            Get(method, key)?.Dispose();

            var spy = new Spy(method, key);
            _spies.Add(spy);
            return spy;
        }

        internal static Spy Get(MethodInfo method, object key)
        {
            method = method.GetBaseDefinition();
            method = method.IsGenericMethod ? method.GetGenericMethodDefinition() : method;
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
    }
}
