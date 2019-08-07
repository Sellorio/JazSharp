using System;
using System.Reflection;

namespace JazSharp.Spies
{
    public class PropertySpy : IDisposable
    {
        public Spy Getter { get; }
        public Spy Setter { get; }

        internal PropertySpy(PropertyInfo propertyInfo, object key)
        {
            if (propertyInfo.GetMethod != null)
            {
                Getter = Spy.Create(propertyInfo.GetMethod, key);
            }

            if (propertyInfo.SetMethod != null)
            {
                Setter = Spy.Create(propertyInfo.SetMethod, key);
            }
        }

        public void Dispose()
        {
            Getter?.Dispose();
            Setter?.Dispose();
        }
    }
}
