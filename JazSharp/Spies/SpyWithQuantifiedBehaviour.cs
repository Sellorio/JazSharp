using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JazSharp.Spies
{
    /// <summary>
    /// An intermediary object used when defining the behaviour of a spy.
    /// </summary>
    public class SpyWithQuantifiedBehaviour : ISpy
    {
        private readonly Spy _spy;

        /// <summary>
        /// A transitionary property used before specifying additional behaviours for the spy.
        /// </summary>
        public SpyThen Then => new SpyThen(_spy);

        Spy ISpy.Spy => _spy;

        /// <summary>
        /// The calls that have been made to the spy at this point in time.
        /// </summary>
        public IReadOnlyList<SpyCall> Calls => ImmutableArray.CreateRange(_spy.CallLog.Select(x => new SpyCall(x)));

        internal SpyWithQuantifiedBehaviour(Spy spy)
        {
            _spy = spy;
        }
    }
}
