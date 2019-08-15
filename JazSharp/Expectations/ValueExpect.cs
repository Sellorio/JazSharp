using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

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
                throw new JazExpectationException("Expected an enumerable value for ToBeEmpty.", 1);
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

        /// <summary>
        /// Tests that the value is less than the given other value.
        /// </summary>
        /// <typeparam name="TCompare">The type of the other value.</typeparam>
        /// <param name="other">The other value that should be less than the value being tested.</param>
        public void ToBeLessThan<TCompare>(TCompare other)
            where TCompare : IComparable<TValue>
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var conditionMet = other.CompareTo(_value) == 1;

            ThrowIfFailed(
                conditionMet,
                $"Expected value {SafeToString(_value)} to be less than {SafeToString(other)}.",
                $"Expected value {SafeToString(_value)} to not be less than {SafeToString(other)}.");
        }

        /// <summary>
        /// Tests that the value is less than or equal to the given other value.
        /// </summary>
        /// <typeparam name="TCompare">The type of the other value.</typeparam>
        /// <param name="other">The other value that should be less than the value being tested.</param>
        public void ToBeLessThanOrEqualTo<TCompare>(TCompare other)
            where TCompare : IComparable<TValue>
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var conditionMet = other.CompareTo(_value) != -1;

            ThrowIfFailed(
                conditionMet,
                $"Expected value {SafeToString(_value)} to be less than or equal to {SafeToString(other)}.",
                $"Expected value {SafeToString(_value)} to not be less than nor equal to {SafeToString(other)}.");
        }

        /// <summary>
        /// Tests that the value is greater than the given other value.
        /// </summary>
        /// <typeparam name="TCompare">The type of the other value.</typeparam>
        /// <param name="other">The other value that should be less than the value being tested.</param>
        public void ToBeGreaterThan<TCompare>(TCompare other)
            where TCompare : IComparable<TValue>
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var conditionMet = other.CompareTo(_value) == -1;

            ThrowIfFailed(
                conditionMet,
                $"Expected value {SafeToString(_value)} to be greater than {SafeToString(other)}.",
                $"Expected value {SafeToString(_value)} to not be greater than {SafeToString(other)}.");
        }

        /// <summary>
        /// Tests that the value is greater than the given other value.
        /// </summary>
        /// <typeparam name="TCompare">The type of the other value.</typeparam>
        /// <param name="other">The other value that should be less than the value being tested.</param>
        public void ToBeGreaterThanOrEqualTo<TCompare>(TCompare other)
            where TCompare : IComparable<TValue>
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var conditionMet = other.CompareTo(_value) != 1;

            ThrowIfFailed(
                conditionMet,
                $"Expected value {SafeToString(_value)} to be greater than or equal to {SafeToString(other)}.",
                $"Expected value {SafeToString(_value)} to not be greater than nor equal to {SafeToString(other)}.");
        }

        /// <summary>
        /// Tests that the value is a string that matches the given pattern.
        /// </summary>
        /// <param name="regex">The pattern to match against.</param>
        public void ToMatch(string regex)
        {
            ToMatchInternal(new Regex(regex));
        }

        /// <summary>
        /// Tests that the value is a string that matches the given pattern.
        /// </summary>
        /// <param name="regex">The pattern to match against.</param>
        public void ToMatch(Regex regex)
        {
            ToMatchInternal(regex);
        }

        /// <summary>
        /// Tests that the value is a string that contains the given text.
        /// </summary>
        /// <param name="value">The text that should be contained in the value.</param>
        /// <param name="ignoreCase">Whether or not to ignore the casing of the text. Defaults to false.</param>
        public void ToContain(string value, bool ignoreCase = false)
        {
            if (_value is string str)
            {
                if (ignoreCase)
                {
                    str = str.ToLowerInvariant();
                    value = str.ToLowerInvariant();
                }

                var conditionMet = str.Contains(value);

                ThrowIfFailed(
                    conditionMet,
                    $"Expected value {SafeToString(_value)} to contain /{SafeToString(value)}/.",
                    $"Expected value {SafeToString(_value)} not to contain /{SafeToString(value)}/.");

                return;
            }

            throw new JazExpectationException($"Expected value {SafeToString(_value)} to contain {SafeToString(value)} but it isn't a string.", 1);
        }

        /// <summary>
        /// Tests that the value is an object or list containing the given object's
        /// properties, list items or list items with a subset of properties.
        /// </summary>
        /// <param name="value"></param>
        public void ToContain(object value)
        {
            var path = string.Empty;
            var conditionMet = DeepCompareHelper.DeepContains(_value, value, ref path);

            ThrowIfFailed(
                conditionMet,
                $"Expected value {SafeToString(_value)} to contain /{SafeToString(value)}/.",
                $"Expected value {SafeToString(_value)} not to contain /{SafeToString(value)}/.");
        }

        private void ToMatchInternal(Regex regex)
        {
            if (_value is string str)
            {
                var conditionMet = regex.IsMatch(str);

                ThrowIfFailed(
                    conditionMet,
                    $"Expected value {SafeToString(_value)} to match /{regex}/.",
                    $"Expected value {SafeToString(_value)} not to match /{regex}/.",
                    3);

                return;
            }

            throw new JazExpectationException($"Expected value {SafeToString(_value)} to match /{regex}/ but it isn't a string.", 2);
        }

        private void ThrowIfFailed(bool conditionMet, string conditionNotMetFailure, string conditionMetFailure, int stackRowsToTrim = 2)
        {
            if (!conditionMet && !_inverted)
            {
                throw new JazExpectationException(conditionNotMetFailure, stackRowsToTrim);
            }

            if (conditionMet && _inverted)
            {
                throw new JazExpectationException(conditionMetFailure, stackRowsToTrim);
            }
        }

        private static string SafeToString(object value)
        {
            return $"<{value ?? "null"}>";
        }
    }
}
