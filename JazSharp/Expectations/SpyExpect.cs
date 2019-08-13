using JazSharp.Spies;
using System;
using System.Linq;

namespace JazSharp.Expectations
{
    /// <summary>
    /// An object used to specify expectations against a spy.
    /// </summary>
    public class SpyExpect
    {
        private readonly Spy _spy;
        private readonly bool _inverted;

        /// <summary>
        /// Inverts the expectation such that if it would otherwise have passed, it will now fail.
        /// The message provided in the <see cref="JazExpectationException"/> will also be different
        /// to reflect the inverted nature of the check.
        /// </summary>
        public SpyExpect Not => new SpyExpect(_spy, !_inverted);

        private SpyExpect(Spy spy, bool inverted)
            : this(spy)
        {
            _inverted = inverted;
        }

        internal SpyExpect(Spy spy)
        {
            _spy = spy;
        }

        /// <summary>
        /// Tests that the method being spied on was called at least once.
        /// </summary>
        public void ToHaveBeenCalled()
        {
            var wasCalled = _spy.CallLog.Count > 0;

            if (!wasCalled && !_inverted)
            {
                throw new JazExpectationException("Expected spy to have been called, but it wasn't.");
            }

            if (wasCalled && _inverted)
            {
                throw new JazExpectationException("Expected spy to not have been called, but it was.");
            }
        }

        /// <summary>
        /// Tests that the method being spied on was called a specific number of times.
        /// </summary>
        /// <param name="count">The expected call count.</param>
        public void ToHaveBeenCalledTimes(int count)
        {
            var expectedCallCount = _spy.CallLog.Count == count;

            if (!expectedCallCount && !_inverted)
            {
                throw new JazExpectationException($"Expected spy to be called {count} times, but it was called {_spy.CallLog.Count} times.");
            }

            if (expectedCallCount && _inverted)
            {
                throw new JazExpectationException($"Expected spy to not be called {count} times, but it was.");
            }
        }

        /// <summary>
        /// Tests that the method being spied on was called at least once with the given set of parameters.
        /// Parameters are compared using deep equality checks, not reference checks.
        /// </summary>
        /// <param name="parameters">The expected parameters.</param>
        public void ToHaveBeenCalledWith(params object[] parameters)
        {
            if (parameters == null)
            {
                parameters = new object[0];
            }

            if (parameters.Length != _spy.Method.GetParameters().Length)
            {
                throw new ArgumentException("Incorrect number of parameters specified for ToHaveBeenCalled.", nameof(parameters));
            }

            var matchFound = false;

            foreach (var call in _spy.CallLog)
            {
                var isMatch = true;

                for (var i = 0; i < parameters.Length && isMatch; i++)
                {
                    var path = "p" + (i + 1);
                    var left = parameters[i];
                    var right = call[i];

                    isMatch = DeepCompareHelper.DeepCompare(left, right, ref path);
                }

                if (isMatch)
                {
                    matchFound = true;
                    break;
                }
            }

            if (!matchFound && !_inverted)
            {
                throw new JazExpectationException(
                    "Expected call to spy with [ "
                    + string.Join(", ", parameters)
                    + " ] but actual calls were ["
                    + string.Join(", ", _spy.CallLog.Select(x => " [ " + string.Join(", ", x) + "]"))
                    + " ].");
            }

            if (matchFound && _inverted)
            {
                throw new JazExpectationException("Expected no call to spy with [ " + string.Join(", ", parameters) + " ] but a call was made.");
            }
        }
    }
}
