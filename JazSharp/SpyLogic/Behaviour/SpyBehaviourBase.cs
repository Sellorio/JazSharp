using JazSharp.Spies;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JazSharp.SpyLogic.Behaviour
{
    internal abstract class SpyBehaviourBase
    {
        internal static SpyBehaviourBase Default => new DefaultBehaviour();

        internal Dictionary<string, object> ParameterChangesBeforeExecution { get; set; } = new Dictionary<string, object>();
        internal Dictionary<string, object> ParameterChangesAfterExecution { get; } = new Dictionary<string, object>();
        internal int Lifetime { get; private set; }

        internal object Execute(Spy spy, MethodInfo exactMethod, object instance, object[] parameters)
        {
            ApplyParameterChanges(exactMethod, parameters, ParameterChangesBeforeExecution);

            var args = new BehaviourArgs(spy, exactMethod, instance, parameters);

            Execute(args);

            Lifetime--;

            if (Lifetime <= 0)
            {
                spy.Behaviours.Dequeue();
            }

            ApplyParameterChanges(exactMethod, parameters, ParameterChangesAfterExecution);

            return args.HasResult ? args.Result : GetDefaultValue(exactMethod.ReturnType);
        }

        internal void UpdateLifetime(int lifetime)
        {
            Lifetime = Math.Max(1, lifetime);
        }

        protected abstract void Execute(BehaviourArgs args);

        private static void ApplyParameterChanges(MethodInfo exactMethod, object[] parameters, Dictionary<string, object> parameterChanges)
        {
            var methodParameters = exactMethod.GetParameters();

            for (var i = 0; i < methodParameters.Length; i++)
            {
                var parameterName = methodParameters[i].Name;

                foreach (var parameterChange in parameterChanges)
                {
                    if (parameterName == parameterChange.Key)
                    {
                        parameters[i] = parameterChange.Value;
                        break;
                    }
                }
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
