using System;
using System.Collections;
using System.Linq;

namespace JazSharp.Expectations
{
    internal static class DeepCompareHelper
    {
        private static readonly Type[] _basicTypes =
        {
            typeof(bool),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(string),
            typeof(DateTime)
        };

        internal static bool DeepCompare(object left, object right, ref string path)
        {
            if (left == null)
            {
                return right == null || right is AnyMatcher anyMatcher && anyMatcher.AllowNull;
            }

            if (right == null)
            {
                return left is AnyMatcher anyMatcher && anyMatcher.AllowNull;
            }

            var leftType = left.GetType();
            var rightType = right.GetType();

            var result =
                CheckAnyMatcher(left, leftType, rightType) ??
                CheckAnyMatcher(right, rightType, leftType) ??
                CheckEquality(left, right) ??
                CheckNotBasicType(leftType) ??
                CheckNotBasicType(rightType) ??
                CheckEnumerable(left, leftType, right, rightType, ref path) ??
                CheckProperties(left, leftType, right, rightType, ref path) ??
                true;

            return result;
        }

        private static bool? CheckAnyMatcher(object object1, Type type1, Type type2)
        {
            if (type1 == typeof(AnyMatcher))
            {
                var matcher = (AnyMatcher)object1;
                return matcher.Exact ? matcher.Type == type2 : matcher.Type.IsAssignableFrom(type2);
            }

            return null;
        }

        private static bool? CheckEquality(object object1, object object2)
        {
            return object1.Equals(object2) || object2.Equals(object1) ? (bool?)true : null;
        }

        private static bool? CheckNotBasicType(Type type)
        {
            return _basicTypes.Contains(type) ? (bool?)false : null;
        }

        private static bool? CheckEnumerable(object object1, Type type1, object object2, Type type2, ref string path)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type1))
            {
                if (!typeof(IEnumerable).IsAssignableFrom(type2))
                {
                    return false;
                }

                var leftList = ((IEnumerable)object1).Cast<object>().ToList();
                var rightList = ((IEnumerable)object2).Cast<object>().ToList();

                if (leftList.Count != rightList.Count)
                {
                    path += ".Count";
                    return false;
                }

                for (var i = 0; i < leftList.Count; i++)
                {
                    var itemPath = path + "[" + i + "]";
                    var result = DeepCompare(leftList[i], rightList[i], ref itemPath);

                    if (!result)
                    {
                        path = itemPath;
                        return false;
                    }
                }

                return true;
            }

            return null;
        }

        private static bool? CheckProperties(object object1, Type type1, object object2, Type type2, ref string path)
        {
            var properties1 = type1.GetProperties().OrderBy(x => x.Name).ToList();
            var properties2 = type2.GetProperties().OrderBy(x => x.Name).ToList();

            // check that left and right both have the same properties (no extra, no missing)
            if (!Enumerable.SequenceEqual(properties1.Select(x => x.Name), properties2.Select(x => x.Name)))
            {
                return false;
            }

            for (var i = 0; i < properties1.Count; i++)
            {
                var childPath = path + "." + properties1[i].Name;
                var result = DeepCompare(properties1[i].GetValue(object1), properties2[i].GetValue(object2), ref childPath);

                if (!result)
                {
                    path = childPath;
                    return false;
                }
            }

            return null;
        }
    }
}
