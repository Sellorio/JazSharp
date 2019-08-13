using JazSharp.SpyLogic.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JazSharp.Spies
{
    public class SpyThen : ISpy
    {
        private readonly Dictionary<int, object> _parameterChanges = new Dictionary<int, object>();
        private readonly Spy _spy;

        Spy ISpy.Spy => _spy;

        internal SpyThen(Spy spy)
        {
            _spy = spy;
        }

        public SpyWithBehaviour CallThrough()
        {
            if (_spy.Key is Guid)
            {
                throw new InvalidOperationException("Cannot call through on dynamic spies.");
            }

            ConstrainPreviousBehaviour();
            var behaviour = new CallThroughBehaviour();
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour Throw(Exception exception)
        {
            ConstrainPreviousBehaviour();
            var behaviour = new ThrowBehaviour(exception);
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour Throw<TException>()
            where TException : Exception, new()
        {
            ConstrainPreviousBehaviour();
            var behaviour = new ThrowBehaviour(typeof(TException));
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour ReturnValue(object value)
        {
            var behaviour = AddReturnValue(value);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithReturnValues ReturnValues(params object[] values)
        {
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
            ConstrainPreviousBehaviour();
            var behaviour = new DefaultBehaviour();
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyThen ChangeParameter(string parameterName, object value)
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
            ConstrainPreviousBehaviour();

            if (_spy.Method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException("Cannot specify a return value to use for an action.");
            }

            var behaviour = new ReturnValueBehaviour(value);
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);

            return behaviour;
        }

        private void ConstrainPreviousBehaviour()
        {
            if (_spy.Behaviours.Any())
            {
                var behaviour = _spy.Behaviours.Peek();

                if (behaviour.Lifetime > int.MaxValue / 2)
                {
                    behaviour.UpdateLifetime(1);
                }
            }
        }
    }
}
