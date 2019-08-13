using JazSharp.SpyLogic.Behaviour;
using System;
using System.Collections.Generic;

namespace JazSharp.Spies
{
    public class SpyAnd : ISpy
    {
        private readonly Dictionary<int, object> _parameterChanges = new Dictionary<int, object>();
        private readonly Spy _spy;

        Spy ISpy.Spy => _spy;

        internal SpyAnd(Spy spy)
        {
            _spy = spy;
        }

        public SpyWithBehaviour CallThrough()
        {
            if (_spy.Key is Guid)
            {
                throw new InvalidOperationException("Cannot call through on dynamic spies.");
            }

            _spy.Behaviours.Clear();
            var behaviour = new CallThroughBehaviour();
            behaviour.UpdateLifetime(int.MaxValue);
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour Throw(Exception exception)
        {
            _spy.Behaviours.Clear();
            var behaviour = new ThrowBehaviour(exception);
            behaviour.UpdateLifetime(int.MaxValue);
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour Throw<TException>()
            where TException : Exception, new()
        {
            _spy.Behaviours.Clear();
            var behaviour = new ThrowBehaviour(typeof(TException));
            behaviour.UpdateLifetime(int.MaxValue);
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour ReturnValue(object value)
        {
            _spy.Behaviours.Clear();
            var behaviour = AddReturnValue(value);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithReturnValues ReturnValues(params object[] values)
        {
            _spy.Behaviours.Clear();
            var behaviours = new List<SpyBehaviourBase>();

            foreach (var value in values)
            {
                var behaviour = AddReturnValue(value);
                behaviour.UpdateLifetime(1);
                behaviours.Add(behaviour);
            }

            return new SpyWithReturnValues(_spy, behaviours);
        }

        public SpyWithBehaviour DoNothing()
        {
            _spy.Behaviours.Clear();
            var behaviour = new DefaultBehaviour();
            behaviour.UpdateLifetime(int.MaxValue);
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyAnd ChangeParameterBefore(string parameterName, object value)
        {
            var parameters = _spy.Method.GetParameters();

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                if (parameter.Name == parameterName)
                {
                    _parameterChanges.Add(i, value);
                    return this;
                }
            }

            throw new ArgumentException("parameterName does not match a parameter on the method.");
        }

        private SpyBehaviourBase AddReturnValue(object value)
        {
            if (_spy.Method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException("Cannot specify a return value to use for an action.");
            }

            var behaviour = new ReturnValueBehaviour(value);
            behaviour.UpdateLifetime(int.MaxValue);
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);

            return behaviour;
        }
    }
}
