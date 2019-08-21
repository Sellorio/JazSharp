using JazSharp.SpyLogic.Behaviour;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JazSharp.Spies
{
    /// <summary>
    /// An intermediary object used when defining the behaviour of a spy.
    /// </summary>
    public class SpyWithBehaviour : ISpy
    {
        private readonly Spy _spy;
        private readonly SpyBehaviourBase _behaviour;

        /// <summary>
        /// A transitionary property used before specifying additional behaviours for the spy.
        /// </summary>
        public SpyThen Then => new SpyThen(_spy);

        Spy ISpy.Spy => _spy;

        /// <summary>
        /// The calls that have been made to the spy at this point in time.
        /// </summary>
        public IReadOnlyList<SpyCall> Calls => ImmutableArray.CreateRange(_spy.CallLog.Select(x => new SpyCall(x)));

        internal SpyWithBehaviour(Spy spy, SpyBehaviourBase behaviour)
        {
            _spy = spy;
            _behaviour = behaviour;
        }

        /// <summary>
        /// Configures the spy to modify the parameter values for ref or out parameters after
        /// the main logic of the spy is complete. Use this to specify return values for
        /// out and ref parameters.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to set.</param>
        /// <param name="value">The value to update the parameter to.</param>
        /// <returns>The current spy configuration object.</returns>
        public SpyWithBehaviour ThenChangeParameter(string parameterName, object value)
        {
            var matchingParameters = _spy.Methods.SelectMany(x => x.GetParameters().Where(y => y.Name == parameterName)).ToList();

            if (matchingParameters.Count == 0)
            {
                throw new ArgumentException("parameterName does not match a parameter on the method.");
            }

            if (matchingParameters.All(x => !x.ParameterType.IsByRef))
            {
                throw new InvalidOperationException(
                    $"Changing a parameter's value after spy execution only makes sense for ref or out parameters. " +
                    $"If you intended to change a parameter value before call-through, call {nameof(ThenChangeParameter)} " +
                    $"immediately after {nameof(Spy.And)}.");
            }

            _behaviour.ParameterChangesAfterExecution.Add(parameterName, value);
            return this;
        }

        /// <summary>
        /// Specifies that the spy behaviour should execute once.
        /// </summary>
        /// <returns>The spy.</returns>
        public SpyWithQuantifiedBehaviour Once()
        {
            _behaviour.UpdateLifetime(1);
            return new SpyWithQuantifiedBehaviour(_spy);
        }

        /// <summary>
        /// Specifies that the spy behaviour should execute twice.
        /// </summary>
        /// <returns>The spy.</returns>
        public SpyWithQuantifiedBehaviour Twice()
        {
            _behaviour.UpdateLifetime(2);
            return new SpyWithQuantifiedBehaviour(_spy);
        }

        /// <summary>
        /// Specifies that the spy behaviour should execute a specified number of times.
        /// </summary>
        /// <returns>The spy.</returns>
        public SpyWithQuantifiedBehaviour Times(int repetitions)
        {
            _behaviour.UpdateLifetime(repetitions);
            return new SpyWithQuantifiedBehaviour(_spy);
        }

        /// <summary>
        /// Specifies that the spy behaviour should execute forever. This will be overridden
        /// with <see cref="Once"/> if another behaviour is added using <see cref="Then"/>.
        /// </summary>
        /// <returns>The spy.</returns>
        public SpyWithQuantifiedBehaviour Forever()
        {
            _behaviour.UpdateLifetime(int.MaxValue);
            return new SpyWithQuantifiedBehaviour(_spy);
        }
    }
}
