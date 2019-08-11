namespace JazSharp.Spies
{
    public class SpyWithQuantifiedBehaviour : ISpy
    {
        private readonly Spy _spy;

        public SpyThen Then => new SpyThen(_spy);

        Spy ISpy.Spy => _spy;

        internal SpyWithQuantifiedBehaviour(Spy spy)
        {
            _spy = spy;
        }
    }
}
