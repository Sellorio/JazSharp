using System;
using System.Collections;
using System.Linq;

namespace JazSharp.Expectations
{
    /// <summary>
    /// An object used to specify expectations against any object.
    /// </summary>
    /// <typeparam name="TValue">The type of the value being tested.</typeparam>
    public class ValueExpect<TValue>
    {
        private readonly TValue _value;
        private readonly bool _inverted;

        /// <summary>
        /// Inverts the expectation such that if it would otherwise have passed, it will now fail.
        /// The message provided in the <see cref="JazExpectationException"/> will also be different
        /// to reflect the inverted nature of the check.
        /// </summary>
        public ValueExpect<TValue> Not => new ValueExpect<TValue>(_value, !_inverted);

        private ValueExpect(TValue value, bool inverted)
        {
            _value = value;
            _inverted = inverted;
        }

        internal ValueExpect(TValue value)
            : this(value, false)
        {
        }

        /// <summary>
        /// Tests that the value matches the expected value by doing a deep comparison
        /// between the two values. This will recurse into lists and child objects and
        /// will not do a reference check at any point - only equality checks.
        /// </summary>
        /// <param name="expected"></param>
        public void ToEqual(object expected)
        {
            var path = "value";
            var areEqual = DeepCompareHelper.DeepCompare(expected, _value, ref path);

            ThrowIfFailed(
                areEqual,
                "Expected values to be equal but they differ at path " + path + ".",
                "Expected values to differ but they are equal.");
        }

        /// <summary>
        /// Tests that the value exactly matches the expected value. This will do an
        /// equality check for value types and a reference check for classes except
        /// for string (which will have an equality check).
        /// </summary>
        /// <param name="expected"></param>
        public void ToBe(TValue expected)
        {
            var conditionMet =
                _value != null && (_value.GetType().IsValueType || _value is string)
                    ? _value.Equals(expected)
                    : ReferenceEquals(_value, expected);

            ThrowIfFailed(
                conditionMet,
                $"Expected {SafeToString(_value)} to be {SafeToString(expected)}.",
                $"Expected {SafeToString(_value)} to not be {SafeToString(expected)}.");
        }

        /// <summary>
        /// Tests that the value is a boolean with the value of <see langword="true"/>.
        /// </summary>
        public void ToBeTrue()
        {
            var conditionMet = _value is bool b && b;
            ThrowIfFailed(conditionMet, $"Expected {SafeToString(_value)} to be true.", $"Expected {SafeToString(_value)} to not be true.");
        }

        /// <summary>
        /// Tests that the value is a boolean with the value of <see langword="false"/>.
        /// </summary>
        public void ToBeFalse()
        {
            var conditionMet = _value is bool b && !b;
            ThrowIfFailed(conditionMet, $"Expected {SafeToString(_value)} to be false.", $"Expected {SafeToString(_value)} to not be false.");
        }

        /// <summary>
        /// Tests that the value is it's default value. E.g. for integer, the expected value is zero
        /// and for string it would be null.
        /// </summary>
        public void ToBeDefault()
        {
            var conditionMet = _value == null || _value.GetType().IsValueType && _value.Equals(default(TValue));

            ThrowIfFailed(
                conditionMet,
                $"Expected value {SafeToString(_value)} to be default ({SafeToString(default(TValue))}).",
                $"Expected value to not be default ({SafeToString(default(TValue))}).");
        }

        /// <summary>
        /// Tests that the value is an empty enumerable or empty string.
        /// </summary>
        public void ToBeEmpty()
        {
            if (_value is IEnumerable enumerable)
            {
                var conditionMet = !enumerable.Cast<object>().Any();

                ThrowIfFailed(
                    conditionMet,
                    $"Expected value {SafeToString(_value)} to be empty.",
                    $"Expected value to not to be empty.");
            }
            else
            {
                throw new JazExpectationException("Expected an enumerable value for ToBeEmpty.");
            }
        }

        /// <summary>
        /// Tests that the value is between the two provided values inclusive.
        /// </summary>
        /// <typeparam name="TStart">The type of the lower bound.</typeparam>
        /// <typeparam name="TEnd">The type of the upper bound.</typeparam>
        /// <param name="start">The minimum value for the range.</param>
        /// <param name="end">The maximum value for the range.</param>
        public void ToBeBetween<TStart, TEnd>(TStart start, TEnd end)
            where TStart : IComparable<TValue>
            where TEnd : IComparable<TValue>
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            var conditionMet = start.CompareTo(_value) != 1 && end.CompareTo(_value) != -1;

            ThrowIfFailed(
                conditionMet,
                $"Expected value {SafeToString(_value)} to be between {SafeToString(start)} and {SafeToString(end)}.",
                $"Expected value {SafeToString(_value)} to be outside the range of {SafeToString(start)} to {SafeToString(end)}.");
        }

        private void ThrowIfFailed(bool conditionMet, string conditionNotMetFailure, string conditionMetFailure)
        {
            if (!conditionMet && !_inverted)
            {
                throw new JazExpectationException(conditionNotMetFailure);
            }

            if (conditionMet && _inverted)
            {
                throw new JazExpectationException(conditionMetFailure);
            }
        }

        private static string SafeToString(object value)
        {
            return $"<{value ?? "null"}>";
        }
    }
}
