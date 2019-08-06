using System;

namespace JazSharp.Expectations
{
    public class CallExpect
    {
        private readonly Exception _exception;

        internal CallExpect(Exception exception)
        {
            _exception = exception;
        }

        public TException ToThrow<TException>()
            where TException : Exception
        {
            if (_exception == null)
            {
                throw new JazExpectationException($"Expected a {typeof(TException).Name} to be thrown but it wasn't.");
            }

            var actualExceptionType = _exception.GetType();

            if (_exception.GetType() != typeof(TException))
            {
                throw new JazExpectationException($"Expected an exception of type {typeof(TException).Name} but encountered a {actualExceptionType.Name} instead.");
            }

            return (TException)_exception;
        }
    }
}
