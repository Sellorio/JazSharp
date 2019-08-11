using JazSharp.SpyLogic.Behaviour;
using System;
using System.Linq;

namespace JazSharp.Spies
{
    public class SpyThen : ISpy
    {
        private readonly Spy _spy;

        Spy ISpy.Spy => _spy;

        internal SpyThen(Spy spy)
        {
            _spy = spy;
        }

        public SpyWithBehaviour CallThrough()
        {
            ConstrainPreviousBehaviour();
            var behaviour = new CallThroughBehaviour();
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour Throw(Exception exception)
        {
            ConstrainPreviousBehaviour();
            var behaviour = new ThrowBehaviour(exception);
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour Throw<TException>()
            where TException : Exception, new()
        {
            ConstrainPreviousBehaviour();
            var behaviour = new ThrowBehaviour(typeof(TException));
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public SpyWithBehaviour ReturnValue(object value)
        {
            ConstrainPreviousBehaviour();

            if (_spy.Method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException("Cannot specify a return value to use for an action.");
            }

            var behaviour = new ReturnValueBehaviour(value);
            _spy.Behaviours.Enqueue(behaviour);
            return new SpyWithBehaviour(_spy, behaviour);
        }

        public Spy ReturnValues(params object[] values)
        {
            foreach (var value in values)
            {
                ReturnValue(value).Once();
            }

            return _spy;
        }

        private void ConstrainPreviousBehaviour()
        {
            if (_spy.Behaviours.Any())
            {
                var behaviour = _spy.Behaviours.Peek();

                if (behaviour.Lifetime > int.MaxValue / 2)
                {
                    behaviour.UpdateLifetime(1);
                }
            }
        }
    }
}
