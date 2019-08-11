using JazSharp.SpyLogic.Behaviour;

namespace JazSharp.Spies
{
    public class SpyWithBehaviour : ISpy
    {
        private readonly Spy _spy;
        private readonly SpyBehaviourBase _behaviour;

        public SpyThen Then => new SpyThen(_spy);

        Spy ISpy.Spy => _spy;

        internal SpyWithBehaviour(Spy spy, SpyBehaviourBase behaviour)
        {
            _spy = spy;
            _behaviour = behaviour;
        }

        public SpyWithQuantifiedBehaviour Once()
        {
            _behaviour.UpdateLifetime(1);
            return new SpyWithQuantifiedBehaviour(_spy);
        }

        public SpyWithQuantifiedBehaviour Twice()
        {
            _behaviour.UpdateLifetime(2);
            return new SpyWithQuantifiedBehaviour(_spy);
        }

        public SpyWithQuantifiedBehaviour Times(int repetitions)
        {
            _behaviour.UpdateLifetime(repetitions);
            return new SpyWithQuantifiedBehaviour(_spy);
        }

        public SpyWithQuantifiedBehaviour Forever()
        {
            _behaviour.UpdateLifetime(int.MaxValue);
            return new SpyWithQuantifiedBehaviour(_spy);
        }
    }
}
