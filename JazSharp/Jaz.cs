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

        public static Spy SpyOn(object @object, string nameOfMethod)
            => SpyOn(@object, nameOfMethod, null);

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

        public static Spy SpyOn(Type type, string nameOfMethod)
            => SpyOn(type, nameOfMethod, null);

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

        public static PropertySpy SpyOn<TObject, TProperty>(TObject @object, Expression<Func<TObject, TProperty>> property)
        {
            var propertyInfo = ExpressionHelper.GetPropertyFromExpression(property);
            return new PropertySpy(propertyInfo, @object);
        }

        public static PropertySpy SpyOn<TProperty>(Expression<Func<TProperty>> staticProperty)
        {
            var propertyInfo = ExpressionHelper.GetPropertyFromExpression(staticProperty);
            return new PropertySpy(propertyInfo, string.Empty);
        }

        public static object Any<T>()
        {
            return new AnyMatcher(typeof(T), false, false);
        }

        public static object AnyOrNull<T>()
            where T : class
        {
            return new AnyMatcher(typeof(T), false, true);
        }

        public static object Any()
        {
            return new AnyMatcher(typeof(object), false, false);
        }

        public static object AnyOrNull()
        {
            return new AnyMatcher(typeof(object), false, true);
        }

        public static object InstanceOf<T>()
        {
            return new AnyMatcher(typeof(T), true, false);
        }

        public static Action CreateSpy(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy(out spy);
        }

        public static Action<TParam1> CreateSpy<TParam1>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1>(out spy);
        }

        public static Action<TParam1, TParam2> CreateSpy<TParam1, TParam2>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1, TParam2>(out spy);
        }

        public static Action<TParam1, TParam2, TParam3> CreateSpy<TParam1, TParam2, TParam3>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1, TParam2, TParam3>(out spy);
        }

        public static Action<TParam1, TParam2, TParam3, TParam4> CreateSpy<TParam1, TParam2, TParam3, TParam4>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1, TParam2, TParam3, TParam4>(out spy);
        }

        public static Action<TParam1, TParam2, TParam3, TParam4, TParam5> CreateSpy<TParam1, TParam2, TParam3, TParam4, TParam5>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpy<TParam1, TParam2, TParam3, TParam4, TParam5>(out spy);
        }

        public static Func<TResult> CreateSpyFunc<TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TResult>(out spy);
        }

        public static Func<TParam1, TResult> CreateSpyFunc<TParam1, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TResult>(out spy);
        }

        public static Func<TParam1, TParam2, TResult> CreateSpyFunc<TParam1, TParam2, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TParam2, TResult>(out spy);
        }

        public static Func<TParam1, TParam2, TParam3, TResult> CreateSpyFunc<TParam1, TParam2, TParam3, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TParam2, TParam3, TResult>(out spy);
        }

        public static Func<TParam1, TParam2, TParam3, TParam4, TResult> CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TResult>(out spy);
        }

        public static Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult> CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>(out Spy spy)
        {
            return DynamicSpyHelper.CreateSpyFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>(out spy);
        }
    }
}
