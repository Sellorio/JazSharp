using JazSharp.Spies;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JazSharp.SpyLogic.Behaviour
{
    internal abstract class SpyBehaviourBase
    {
        internal static SpyBehaviourBase Default => new DefaultBehaviour();

        internal Dictionary<int, object> ParameterChangesBeforeExecution { get; set; } = new Dictionary<int, object>();
        internal Dictionary<int, object> ParameterChangesAfterExecution { get; } = new Dictionary<int, object>();
        internal int Lifetime { get; private set; }

        internal object Execute(Spy spy, MethodInfo exactMethod, object instance, object[] parameters)
        {
            ApplyParameterChanges(parameters, ParameterChangesBeforeExecution);

            var args = new BehaviourArgs(spy, exactMethod, instance, parameters);

            Execute(args);

            Lifetime--;

            if (Lifetime <= 0)
            {
                spy.Behaviours.Dequeue();
            }

            ApplyParameterChanges(parameters, ParameterChangesAfterExecution);

            return args.HasResult ? args.Result : GetDefaultValue(exactMethod.ReturnType);
        }

        internal void UpdateLifetime(int lifetime)
        {
            Lifetime = Math.Max(1, lifetime);
        }

        protected abstract void Execute(BehaviourArgs args);

        private static void ApplyParameterChanges(object[] parameters, Dictionary<int, object> parameterChanges)
        {
            foreach (var parameterChange in parameterChanges)
            {
                parameters[parameterChange.Key] = parameterChange.Value;
            }
        }

        private static object GetDefaultValue(Type type)
        {
            return
                type == typeof(void) || type.IsClass || type.IsInterface
                    ? null
                    : Activator.CreateInstance(type);
        }
    }
}
