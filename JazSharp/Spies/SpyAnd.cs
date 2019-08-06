using JazSharp.SpyLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JazSharp.Spies
{
    public class SpyAnd
    {
        private readonly Spy _spy;
        private readonly SpyInfo _spyInfo;
        private readonly object _key;

        internal SpyAnd(Spy spy, SpyInfo spyInfo, object key)
        {
            _spy = spy;
            _spyInfo = spyInfo;
            _key = key;
        }

        public Spy CallThrough()
        {
            _spyInfo.CallThroughMapping[_key] = true;

            if (_spyInfo.ReturnValuesMapping.ContainsKey(_key))
            {
                _spyInfo.ReturnValuesMapping.Remove(_key);
            }

            if (_spyInfo.ReturnValueMapping.ContainsKey(_key))
            {
                _spyInfo.ReturnValueMapping.Remove(_key);
            }

            if (_spyInfo.ThrowMapping.ContainsKey(_key))
            {
                _spyInfo.ThrowMapping.Remove(_key);
            }

            return _spy;
        }

        public Spy Throw(Exception exception)
        {
            _spyInfo.CallThroughMapping[_key] = false;

            if (_spyInfo.ReturnValuesMapping.ContainsKey(_key))
            {
                _spyInfo.ReturnValuesMapping.Remove(_key);
            }

            if (_spyInfo.ReturnValueMapping.ContainsKey(_key))
            {
                _spyInfo.ReturnValueMapping.Remove(_key);
            }

            _spyInfo.ThrowMapping[_key] = exception;

            return _spy;
        }

        public Spy ReturnValue(object value)
        {
            if (_spyInfo.Method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException("Cannot specify a return value to use for an action.");
            }

            _spyInfo.CallThroughMapping[_key] = false;

            if (_spyInfo.ReturnValuesMapping.ContainsKey(_key))
            {
                _spyInfo.ReturnValuesMapping.Remove(_key);
            }

            if (_spyInfo.ThrowMapping.ContainsKey(_key))
            {
                _spyInfo.ThrowMapping.Remove(_key);
            }

            _spyInfo.ReturnValueMapping[_key] = value;

            return _spy;
        }

        public Spy ReturnValues(params object[] values)
        {
            if (_spyInfo.Method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException("Cannot specify a return values to use for an action.");
            }

            _spyInfo.CallThroughMapping[_key] = false;

            if (_spyInfo.ReturnValueMapping.ContainsKey(_key))
            {
                _spyInfo.ReturnValueMapping.Remove(_key);
            }

            if (_spyInfo.ThrowMapping.ContainsKey(_key))
            {
                _spyInfo.ThrowMapping.Remove(_key);
            }

            _spyInfo.ReturnValuesMapping[_key] = new Queue<object>(values.Cast<object>());

            return _spy;
        }
    }
}
