using JazSharp.Expectations;
using JazSharp.Reflection;
using JazSharp.Spies;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using JazSharp.Testing;
using System.Threading;
using System.Text;
using JazSharp.SpyLogic;

[assembly: InternalsVisibleTo("JazSharp.TestAdapter")]

namespace JazSharp
{
    /// <summary>
    /// The main entry class for the JazSharp framework. This class exposes features including
    /// <see cref="SpyOn(object, string)"/>, <see cref="CurrentTest"/>, <see cref="Any"/>
    /// and <see cref="CreateSpy(out Spy)"/> and similar.
    /// </summary>
    public static class Jaz
    {
        internal static SemaphoreSlim CurrentTestSemaphore { get; } = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The context for the currently executing test. This can be used to add custom messages
        /// to the test's output.
        /// </summary>
        public static TestExecutionContext CurrentTest { get; internal set; }

        internal static void SetupTestExecutionContext(string testDescription, StringBuilder output)
        {
            Spy.ClearAll();
            CurrentTest = new TestExecutionContext(testDescription, output);
        }

        internal static void ClearTestExecutionContext()
        {
            CurrentTest = null;
            Spy.ClearAll();
        }

        /// <summary>
        /// Creates a spy on the specified method on the given instance. If the method has overloads,
        /// use <see cref="SpyOn(object, string, Type[])"/>.
        /// </summary>
        /// <param name="object">The instance to spy on.</param>
        /// <param name="nameOfMethod">The method to spy on. This can refer to a public or non-public method.</param>
        /// <returns>The spy that was created.</returns>
        public static Spy SpyOn(object @object, string nameOfMethod)
            => SpyOn(@object, nameOfMethod, null);

        /// <summary>
        /// Creates a spy on the specified method on the given instance with the specified
        /// parameter types. This is used to specify a specific overload of the method to
        /// spy on.
        /// </summary>
        /// <param name="object">The instance to spy on.</param>
        /// <param name="nameOfMethod">The method to spy on. This can refer to a public or non-public method.</param>
        /// <param name="overloadParameters">The expected parameters on the method which are used to pick a specific overload.</param>
        /// <returns>The spy that was created.</returns>
        public static Spy SpyOn(object @object, string nameOfMethod, Type[] overloadParameters)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            if (nameOfMethod == null)
            {
                throw new ArgumentNullException(nameof(nameOfMethod));
            }

            var methods =
                @object.GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.Name == nameOfMethod)
                    .ToList();

            if (overloadParameters != null)
            {
                methods = methods.Where(x => x.GetParameters().Select(y => y.ParameterType).SequenceEqual(overloadParameters)).ToList();
            }

            if (methods.Count == 0)
            {
                throw new ArgumentException("Name does not match any methods on the object.", nameof(nameOfMethod));
            }

            if (methods.Count > 1)
            {
                throw new ArgumentException("Name matches more than one methods.", nameof(nameOfMethod));
            }

            return Spy.Create(methods[0], @object);
        }

        /// <summary>
        /// Creates a spy on the specified static method on the given type. If the method has overloads,
        /// use <see cref="SpyOn(Type, string, Type[])"/>.
        /// </summary>
        /// <param name="type">The type to spy on.</param>
        /// <param name="nameOfMethod">The method to spy on. This can refer to a public or non-public method.</param>
        /// <returns>The spy that was created.</returns>
        public static Spy SpyOn(Type type, string nameOfMethod)
            => SpyOn(type, nameOfMethod, null);

        /// <summary>
        /// Creates a spy on the specified static method on the given type with the specified
        /// parameter types. This is used to specify a specific overload of the method to
        /// spy on.
        /// </summary>
        /// <param name="type">The type to spy on.</param>
        /// <param name="nameOfMethod">The method to spy on. This can refer to a public or non-public method.</param>
        /// <param name="overloadParameters">The expected parameters on the method which are used to pick a specific overload.</param>
        /// <returns>The spy that was created.</returns>
        public static Spy SpyOn(Type type, string nameOfMethod, Type[] overloadParameters)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (nameOfMethod == null)
            {
                throw new ArgumentNullException(nameof(nameOfMethod));
            }

            var methods =
                type
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.Name == nameOfMethod)
                    .ToList();

            if (overloadParameters != null)
            {
                methods = methods.Where(x => x.GetParameters().Select(y => y.ParameterType).SequenceEqual(overloadParameters)).ToList();
            }

            if (methods.Count == 0)
            {
                throw new ArgumentException("Name does not match any static methods on the type.", nameof(nameOfMethod));
            }

            if (methods.Count > 1)
            {
                throw new ArgumentException("Name matches more than one static methods.", nameof(nameOfMethod));
            }

            return Spy.Create(methods[0], string.Empty);
        }

        /// <summary>
        /// Creates a spy on the specified property which creates spies for the getter
        /// and/or setter methods.
        /// </summary>
        /// <param name="object">The instance to spy on.</param>
        /// <param name="propertyName">The property to spy on.</param>
        /// <returns>A container with both getter and setter spies in it.</returns>
        public static PropertySpy SpyOnProperty(object @object, string propertyName)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var property = @object.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (property == null)
            {
                throw new ArgumentException("Name does not match any properties on the object.", nameof(propertyName));
            }

            return new PropertySpy(property, @object);
        }

        /// <summary>
        /// Creates a spy on the specified static property which creates spies for the
        /// getter and/or setter methods.
        /// </summary>
        /// <param name="type">The type to spy on.</param>
        /// <param name="propertyName">The property to spy on.</param>
        /// <returns>A container with both getter and setter spies in it.</returns>
        public static PropertySpy SpyOnProperty(Type type, string propertyName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (property == null)
            {
                throw new ArgumentException("Name does not match any static properties on the type.", nameof(propertyName));
            }

            return new PropertySpy(property, string.Empty);
        }

        /// <summary>
        /// Creates a matcher for use with <see cref="ValueExpect{TValue}.ToEqual(object)"/> and
        /// <see cref="ValueExpect{TValue}.ToContain(object)"/> which matches on any instance
        /// of the given type. Nulls will fail the match. Instances which inherit from the given
        /// type will be successfully matched.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <returns>The created matcher.</returns>
        public static object Any<T>()
        {
            return new AnyMatcher(typeof(T), false, false);
        }

        /// <summary>
        /// Creates a matcher for use with <see cref="ValueExpect{TValue}.ToEqual(object)"/> and
        /// <see cref="ValueExpect{TValue}.ToContain(object)"/> which matches on any instance
        /// of the given type as well as matching on null values. Instances which inherit from the given
        /// type will also be successfully matched.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <returns>The created matcher.</returns>
        public static object AnyOrNull<T>()
            where T : class
        {
            return new AnyMatcher(typeof(T), false, true);
        }

        /// <summary>
        /// Creates a matcher for use with <see cref="ValueExpect{TValue}.ToEqual(object)"/> and
        /// <see cref="ValueExpect{TValue}.ToContain(object)"/> which will accept any non-null value.
        /// </summary>
        /// <returns>The created matcher.</returns>
        public static object Any()
        {
            return new AnyMatcher(typeof(object), false, false);
        }

        /// <summary>
        /// Creates a matcher for use with <see cref="ValueExpect{TValue}.ToEqual(object)"/> and
        /// <see cref="ValueExpect{TValue}.ToContain(object)"/> which will accept any value, including null.
        /// </summary>
        /// <returns>The created matcher.</returns>
        public static object AnyOrNull()
        {
            return new AnyMatcher(typeof(object), false, true);
        }

        /// <summary>
        /// Creates a matcher for use with <see cref="ValueExpect{TValue}.ToEqual(object)"/> and
        /// <see cref="ValueExpect{TValue}.ToContain(object)"/> which will only accept a value of
        /// the exact type given - inheriting types and null values are not accepted.
        /// </summary>
        /// <typeparam name="T">The type of the expected value.</typeparam>
        /// <returns>The created matcher.</returns>
        public static object InstanceOf<T>()
        {
            return new AnyMatcher(typeof(T), true, false);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Action CreateSpy(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Action<TParam1> CreateSpy<TParam1>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Action<TParam1, TParam2> CreateSpy<TParam1, TParam2>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1, TParam2>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Action<TParam1, TParam2, TParam3> CreateSpy<TParam1, TParam2, TParam3>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1, TParam2, TParam3>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Action<TParam1, TParam2, TParam3, TParam4> CreateSpy<TParam1, TParam2, TParam3, TParam4>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1, TParam2, TParam3, TParam4>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Action<TParam1, TParam2, TParam3, TParam4, TParam5> CreateSpy<TParam1, TParam2, TParam3, TParam4, TParam5>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1, TParam2, TParam3, TParam4, TParam5>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Func<TResult> CreateSpyFunc<TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TResult>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Func<TParam1, TResult> CreateSpyFunc<TParam1, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TResult>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Func<TParam1, TParam2, TResult> CreateSpyFunc<TParam1, TParam2, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TParam2, TResult>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Func<TParam1, TParam2, TParam3, TResult> CreateSpyFunc<TParam1, TParam2, TParam3, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TParam2, TParam3, TResult>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Func<TParam1, TParam2, TParam3, TParam4, TResult> CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TResult>(out spy);
        }

        /// <summary>
        /// Creates a delegate with a spy on it. This can be used to test calls to parameters which
        /// are methods as well as spying on events.
        /// </summary>
        /// <param name="spy">The created spy.</param>
        /// <returns>The created delegate.</returns>
        public static Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult> CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>(out spy);
        }
    }
}
