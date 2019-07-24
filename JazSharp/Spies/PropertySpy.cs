using JazSharp.SpyLogic;
using System;
using System.Reflection;

namespace JazSharp.Spies
{
    public class PropertySpy : IDisposable
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly object _key;

        private Spy _getterSpy;
        private Spy _setterSpy;

        public Spy Getter => _getterSpy ?? (_getterSpy = new Spy(SpyCreator.CreateSpy(_propertyInfo.GetMethod, _key), _key));
        public Spy Setter => _setterSpy ?? (_setterSpy = new Spy(SpyCreator.CreateSpy(_propertyInfo.SetMethod, _key), _key));

        internal PropertySpy(PropertyInfo propertyInfo, object key)
        {
            _propertyInfo = propertyInfo;
            _key = key;
        }

        public void Dispose()
        {
            _getterSpy?.Dispose();
            _setterSpy?.Dispose();
        }
    }
}
