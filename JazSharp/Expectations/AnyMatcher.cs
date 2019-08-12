using System;

namespace JazSharp.Expectations
{
    internal class AnyMatcher
    {
        public Type Type { get; }
        public bool Exact { get; }
        public bool AllowNull { get; }

        internal AnyMatcher(Type type, bool exact, bool allowNull)
        {
            Type = type;
            Exact = exact;
            AllowNull = allowNull;
        }
    }
}
