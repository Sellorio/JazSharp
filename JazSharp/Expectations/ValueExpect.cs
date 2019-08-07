using System;
using System.Collections.Generic;
using System.Text;

namespace JazSharp.Expectations
{
    public class ValueExpect<TValue>
    {
        private readonly TValue _value;
        private readonly bool _inverted;

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

        public void ToEqual(object expected)
        {
            var path = "value";
            var areEqual = DeepCompareHelper.DeepCompare(expected, _value, ref path);

            ThrowIfFailed(
                areEqual,
                "Expected values to be equal but they differ at path " + path + ".",
                "Expected values to differ but they are equal.");
        }

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

        public void ToBeTrue()
        {
            var conditionMet = _value is bool b && b;
            ThrowIfFailed(conditionMet, $"Expected {SafeToString(_value)} to be true.", $"Expected {SafeToString(_value)} to not be true.");
        }

        public void ToBeFalse()
        {
            var conditionMet = _value is bool b && !b;
            ThrowIfFailed(conditionMet, $"Expected {SafeToString(_value)} to be false.", $"Expected {SafeToString(_value)} to not be false.");
        }

        public void ToBeDefault()
        {
            var conditionMet = _value == null || _value.GetType().IsValueType && _value.Equals(default(TValue));

            ThrowIfFailed(
                conditionMet,
                $"Expected value {SafeToString(_value)} to be default ({SafeToString(default(TValue))}).",
                $"Expected value to not be default ({SafeToString(default(TValue))}).");
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
