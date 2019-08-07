namespace JazSharp.Spies
{
    public class SpyWithQuantifiedBehaviour
    {
        private readonly Spy _spy;

        public SpyThen Then => new SpyThen(_spy);

        internal SpyWithQuantifiedBehaviour(Spy spy)
        {
            _spy = spy;
        }
    }
}
