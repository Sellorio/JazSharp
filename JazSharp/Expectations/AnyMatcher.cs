using System;

namespace JazSharp.Expectations
{
    public class AnyMatcher
    {
        public Type Type { get; }
        public bool Exact { get; }

        internal AnyMatcher(Type type, bool exact)
        {
            Type = type;
            Exact = exact;
        }
    }
}
