using JazSharp.SpyLogic.Behaviour;
using System;

namespace JazSharp.Spies
{
    public class SpyAnd : ISpy
    {
        private readonly Spy _spy;

        Spy ISpy.Spy => _spy;

        internal SpyAnd(Spy spy)
        {
            _spy = spy;
        }

        public SpyWithBehaviour CallThrough()
        {
            if (_spy.Key is Guid)
            {
                throw new InvalidOperationException("Cannot call through on dynamic spies.");
            }

            _spy.Behaviours.Clear();
            var behaviour = new CallThroughBehaviour();
            behaviour.UpdateLifetime(int.MaxValue);
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour Throw(Exception exception)
        {
            _spy.Behaviours.Clear();
            var behaviour = new ThrowBehaviour(exception);
            behaviour.UpdateLifetime(int.MaxValue);
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour Throw<TException>()
            where TException : Exception, new()
        {
            _spy.Behaviours.Clear();
            var behaviour = new ThrowBehaviour(typeof(TException));
            behaviour.UpdateLifetime(int.MaxValue);
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour ReturnValue(object value)
        {
            if (_spy.Method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException("Cannot specify a return value to use for an action.");
            }

            _spy.Behaviours.Clear();
            var behaviour = new ReturnValueBehaviour(value);
            behaviour.UpdateLifetime(int.MaxValue);
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public Spy ReturnValues(params object[] values)
        {
            _spy.Behaviours.Clear();
            new SpyThen(_spy).ReturnValues(values);

            return _spy;
        }
    }
}
