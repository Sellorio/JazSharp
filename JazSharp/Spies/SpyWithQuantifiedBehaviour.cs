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

        internal SpyWithQuantifiedBehaviour(Spy spy)
        {
            _spy = spy;
        }
    }
}
