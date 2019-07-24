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
                return right == null;
            }

            if (right == null)
            {
                return false;
            }

            if (left.Equals(right))
            {
                return true;
            }

            var leftType = left.GetType();
            var rightType = right.GetType();

            if (_basicTypes.Contains(leftType) || _basicTypes.Contains(rightType))
            {
                if (leftType != rightType)
                {
                    return false;
                }
            }

            if (typeof(IEnumerable).IsAssignableFrom(leftType))
            {
                if (!typeof(IEnumerable).IsAssignableFrom(rightType))
                {
                    return false;
                }

                var leftList = ((IEnumerable)left).Cast<object>().ToList();
                var rightList = ((IEnumerable)right).Cast<object>().ToList();

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
            }

            var leftProperties = leftType.GetProperties().OrderBy(x => x.Name).ToList();
            var rightProperties = rightType.GetProperties().OrderBy(x => x.Name).ToList();

            // check that left and right both have the same properties (no extra, no missing)
            if (!Enumerable.SequenceEqual(leftProperties.Select(x => x.Name), rightProperties.Select(x => x.Name)))
            {
                return false;
            }

            for (var i = 0; i < leftProperties.Count; i++)
            {
                var childPath = path + "." + leftProperties[i].Name;
                var result = DeepCompare(leftProperties[i].GetValue(left), rightProperties[i].GetValue(right), ref childPath);

                if (!result)
                {
                    path = childPath;
                    return false;
                }
            }

            return true;
        }
    }
}
