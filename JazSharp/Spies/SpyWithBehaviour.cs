using JazSharp.SpyLogic.Behaviour;

namespace JazSharp.Spies
{
    public class SpyWithBehaviour
    {
        private readonly Spy _spy;
        private readonly SpyBehaviourBase _behaviour;

        public SpyThen Then => new SpyThen(_spy);

        internal SpyWithBehaviour(Spy spy, SpyBehaviourBase behaviour)
        {
            _spy = spy;
            _behaviour = behaviour;
        }

        public Spy Once()
        {
            _behaviour.UpdateLifetime(1);
            return _spy;
        }

        public Spy Twice()
        {
            _behaviour.UpdateLifetime(2);
            return _spy;
        }

        public Spy Times(int repetitions)
        {
            _behaviour.UpdateLifetime(repetitions);
            return _spy;
        }

        public Spy Forever()
        {
            _behaviour.UpdateLifetime(int.MaxValue);
            return _spy;
        }
    }
}
