using JazSharp.SpyLogic.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JazSharp.Spies
{
    /// <summary>
    /// An intermediary object used when defining the behaviour of a spy.
    /// </summary>
    public class SpyAnd : ISpy
    {
        private readonly Dictionary<string, object> _parameterChanges = new Dictionary<string, object>();
        private readonly Spy _spy;

        Spy ISpy.Spy => _spy;

        internal SpyAnd(Spy spy)
        {
            _spy = spy;
        }

        /// <summary>
        /// Configures the spy to call-through to the original implementation after
        /// recording the call.
        /// </summary>
        /// <returns>The spy.</returns>
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

        /// <summary>
        /// Configures the spy to call-through to throw the given exception when
        /// the spied-on method is called.
        /// </summary>
        /// <returns>The spy.</returns>
        public SpyWithBehaviour Throw(Exception exception)
        {
            _spy.Behaviours.Clear();
            var behaviour = new ThrowBehaviour(exception);
            behaviour.UpdateLifetime(int.MaxValue);
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        /// <summary>
        /// Configures the spy to call-through to throw an exception of the given type
        /// when the spied-on method is called.
        /// </summary>
        /// <returns>The spy.</returns>
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

        /// <summary>
        /// Configures the spy to return a specific value when called.
        /// </summary>
        /// <returns>The spy.</returns>
        public SpyWithBehaviour ReturnValue(object value)
        {
            _spy.Behaviours.Clear();
            var behaviour = AddReturnValue(value);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        /// <summary>
        /// Configures the spy to return each value in the given sequence for each
        /// subsequent call to the spied on method.
        /// </summary>
        /// <returns>The spy.</returns>
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

        /// <summary>
        /// Configures the spy to do nothing. This is also the default behaviour
        /// for a new spy and results in a default value being returned if the
        /// method is a function.
        /// </summary>
        /// <returns>The spy.</returns>
        public SpyWithBehaviour DoNothing()
        {
            _spy.Behaviours.Clear();
            var behaviour = new DefaultBehaviour();
            behaviour.UpdateLifetime(int.MaxValue);
            behaviour.ParameterChangesBeforeExecution = _parameterChanges;
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        /// <summary>
        /// Specifies parameter changes to make before executing the spy's configured
        /// logic. The call log will still contain the original parameters but this
        /// method can allow the test to change the parameters used before a call-through.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to change.</param>
        /// <param name="value">The new value to assign that parameter.</param>
        /// <returns>The spy.</returns>
        public SpyAnd ChangeParameterBefore(string parameterName, object value)
        {
            var hasParameterWithGivenName = _spy.Methods.SelectMany(x => x.GetParameters()).Any(x => x.Name == parameterName);

            if (hasParameterWithGivenName)
            {
                _parameterChanges.Add(parameterName, value);
                return this;
            }

            throw new ArgumentException("parameterName does not match a parameter on the method.");
        }

        private SpyBehaviourBase AddReturnValue(object value)
        {
            if (_spy.Methods.All(x => x.ReturnType == typeof(void)))
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
