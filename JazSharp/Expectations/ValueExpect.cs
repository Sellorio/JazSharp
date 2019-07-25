using System;
using System.Collections.Generic;
using System.Text;

namespace JazSharp.Expectations
{
    public class ValueExpect
    {
        private readonly object _value;
        private readonly bool _inverted;

        public ValueExpect Not => new ValueExpect(_value, !_inverted);

        private ValueExpect(object value, bool inverted)
        {
            _value = value;
            _inverted = inverted;
        }

        internal ValueExpect(object value)
            : this(value, false)
        {
        }

        public void ToEqual(object expected)
        {
            var path = "value";
            var areEqual = DeepCompareHelper.DeepCompare(expected, _value, ref path);

            if (!areEqual && !_inverted)
            {
                throw new JazExpectationException("Expected values to be equal but they differ at path " + path + ".");
            }

            if (areEqual && _inverted)
            {
                throw new JazExpectationException("Expected values to differ but they are equal.");
            }
        }
    }
}
