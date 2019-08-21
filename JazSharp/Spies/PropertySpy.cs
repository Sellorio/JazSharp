using System;
using System.Reflection;

namespace JazSharp.Spies
{
    /// <summary>
    /// A spy container representing a spy on a property. It contains the spy instances
    /// for the getter and setter of the property.
    /// </summary>
    public class PropertySpy : IDisposable
    {
        /// <summary>
        /// The spy applied to the getter of the property.
        /// </summary>
        public Spy Getter { get; }

        /// <summary>
        /// The spy applied to the setter of the property.
        /// </summary>
        public Spy Setter { get; }

        internal PropertySpy(PropertyInfo propertyInfo, object key)
        {
            if (propertyInfo.GetMethod != null)
            {
                Getter = Spy.Create(new[] { propertyInfo.GetMethod }, key);
            }

            if (propertyInfo.SetMethod != null)
            {
                Setter = Spy.Create(new[] { propertyInfo.SetMethod }, key);
            }
        }

        /// <summary>
        /// Removes both the getter and the setter spies, reverting the target method to call-through always.
        /// </summary>
        public void Dispose()
        {
            Getter?.Dispose();
            Setter?.Dispose();
        }
    }
}
