using System;

namespace JazSharp.Expectations
{
    /// <summary>
    /// An object used to specify expectations against a method call.
    /// </summary>
    public class CallExpect
    {
        private readonly Exception _exception;
        private readonly bool _inverted;

        internal CallExpect(Exception exception)
        {
            _exception = exception;
        }

        /// <summary>
        /// Inverts the expectation such that if it would otherwise have passed, it will now fail.
        /// The message provided in the <see cref="JazExpectationException"/> will also be different
        /// to reflect the inverted nature of the check.
        /// </summary>
        public CallExpect Not => new CallExpect(!_inverted);

        internal CallExpect(bool inverted)
        {
            _inverted = inverted;
        }

        public TException ToThrow<TException>()
            where TException : Exception
        {
            if (_inverted)
            {
                if (_exception != null && _exception.GetType() == typeof(TException))
                {
                    throw new JazExpectationException($"Unexpected exception of type {typeof(TException).Name} was thrown.", 1);
                }

                return null;
            }
            else
            {
                if (_exception == null)
                {
                    throw new JazExpectationException($"Expected a {typeof(TException).Name} to be thrown but it wasn't.", 1);
                }

                var actualExceptionType = _exception.GetType();

                if (_exception.GetType() != typeof(TException))
                {
                    throw new JazExpectationException($"Expected an exception of type {typeof(TException).Name} but encountered a {actualExceptionType.Name} instead.", 1);
                }

                return (TException)_exception;
            }
        }
    }
}
