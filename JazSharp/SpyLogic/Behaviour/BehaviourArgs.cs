using JazSharp.Spies;

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

        internal BehaviourArgs(Spy spy, object instance, object[] parameters)
        {
            Spy = spy;
            Instance = instance;
            Parameters = parameters;
        }
    }
}
