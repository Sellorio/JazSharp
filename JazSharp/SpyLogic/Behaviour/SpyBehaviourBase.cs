using JazSharp.Spies;
using System;

namespace JazSharp.SpyLogic.Behaviour
{
    internal abstract class SpyBehaviourBase
    {
        internal static SpyBehaviourBase Default => new DefaultBehaviour();

        internal int Lifetime { get; private set; }

        internal object Execute(Spy spy, object instance, object[] parameters)
        {
            var args = new BehaviourArgs(spy, instance, parameters);

            Execute(args);

            Lifetime--;

            if (Lifetime <= 0)
            {
                spy.Behaviours.Dequeue();
            }

            return args.HasResult ? args.Result : GetDefaultValue(spy.Method.ReturnType);
        }

        internal void UpdateLifetime(int lifetime)
        {
            Lifetime = Math.Max(1, lifetime);
        }

        protected abstract void Execute(BehaviourArgs args);

        private static object GetDefaultValue(Type type)
        {
            return type == typeof(void) || type.IsClass ? null : Activator.CreateInstance(type);
        }
    }
}
