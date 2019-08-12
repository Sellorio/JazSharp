using System.Collections.Generic;
using System.Collections.Immutable;

namespace JazSharp.Spies
{
    /// <summary>
    /// The information for a specific call to a spy.
    /// </summary>
    public class SpyCall
    {
        /// <summary>
        /// The values passed in as the parameters of the spy.
        /// </summary>
        public IReadOnlyList<object> Arguments { get; }

        internal SpyCall(ImmutableArray<object> arguments)
        {
            Arguments = arguments;
        }
    }
}
