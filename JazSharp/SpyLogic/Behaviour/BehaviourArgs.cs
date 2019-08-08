using JazSharp.Spies;
using System.Reflection;

namespace JazSharp.SpyLogic.Behaviour
{
    internal class BehaviourArgs
    {
        private object _result;

        /// <summary>
        /// The object instance of the instance method being spied on. This is null if
        /// the spied on method is a static.
        /// </summary>
        internal object Instance { get; }

        /// <summary>
        /// The parameters used for the call to the spy.
        /// </summary>
        internal object[] Parameters { get; }

        /// <summary>
        /// The information for the spy.
        /// </summary>
        internal Spy Spy { get; }

        /// <summary>
        /// The exact method that the spy wraps. This should be used instead of <see cref="Spy.Method"/>
        /// since it preserves inheritance and generic parameters.
        /// </summary>
        internal MethodInfo ExactMethod { get; }

        /// <summary>
        /// Whether or not the <see cref="ISpyFeature"/> has set the <see cref="Result"/>
        /// to return from the spied method.
        /// </summary>
        internal bool HasResult { get; private set; }

        /// <summary>
        /// The result to return for the spy method call. This must be explicitly set to null
        /// if null is the intended result.
        /// </summary>
        internal object Result
        {
            get => _result;
            set
            {
                HasResult = true;
                _result = value;
            }
        }

        /// <summary>
        /// Whether or not the feature's lifetime has ended and it should no longer be executed.
        /// </summary>
        internal bool FeatureCompleted { get; set; }

        internal BehaviourArgs(Spy spy, MethodInfo exactMethod, object instance, object[] parameters)
        {
            Spy = spy;
            ExactMethod = exactMethod;
            Instance = instance;
            Parameters = parameters;
        }
    }
}
