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
    public class SpyWithReturnValues : ISpy
    {
        private readonly Spy _spy;
        private readonly List<SpyBehaviourBase> _returnValuesBehaviours;

        /// <summary>
        /// A transitionary property used before specifying additional behaviours for the spy.
        /// </summary>
        public SpyThen Then => new SpyThen(_spy);

        Spy ISpy.Spy => _spy;

        /// <summary>
        /// The calls that have been made to the spy at this point in time.
        /// </summary>
        public IReadOnlyList<SpyCall> Calls => ImmutableArray.CreateRange(_spy.CallLog.Select(x => new SpyCall(x)));

        internal SpyWithReturnValues(Spy spy, List<SpyBehaviourBase> returnValuesBehaviours)
        {
            _spy = spy;
            _returnValuesBehaviours = returnValuesBehaviours;
        }

        /// <summary>
        /// Configures the spy to modify the parameter values for ref or out parameters after
        /// the main logic of the spy is complete. Use this to specify return values for
        /// out and ref parameters.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to set.</param>
        /// <param name="value">The value to update the parameter to.</param>
        /// <returns>The current spy configuration object.</returns>
        public SpyWithReturnValues ChangeParameter(string parameterName, object value)
        {
            var matchingParameters = _spy.Methods.SelectMany(x => x.GetParameters().Where(y => y.Name == parameterName)).ToList();

            if (matchingParameters.Count == 0)
            {
                throw new ArgumentException("parameterName does not match a parameter on the method.");
            }

            _returnValuesBehaviours.ForEach(x => x.ParameterChangesAfterExecution.Add(parameterName, value));
            return this;
        }
    }
}
