using System;
using System.Collections.Generic;
using System.Linq;

namespace JazSharp.TestSetup
{
    internal static class SpecHelper
    {
        private static Stack<DescribeStackItem> _describeStack = new Stack<DescribeStackItem>();
        private static List<Test> _tests = new List<Test>();

        internal static void PushDescribe(string description, bool isFocused, bool isExcluded)
        {
            _describeStack.Push(new DescribeStackItem
            {
                Description = description,
                IsExcluded = isExcluded,
                IsFocused = isFocused && !isExcluded
            });
        }

        internal static void RegisterTest(string description, Delegate action, bool isFocused, bool isExcluded)
        {
            var actualIsExcluded = isExcluded || _describeStack.Any(x => x.IsExcluded);
            var actualIsFocused = !actualIsExcluded && isFocused || _describeStack.Any(x => x.IsFocused);

            _tests.Add(new Test(_describeStack.Select(x => x.Description).ToArray(), description, action, actualIsFocused, actualIsExcluded));
        }

        internal static Test[] GetTestsInSpec(Type spec)
        {
            _describeStack = new Stack<DescribeStackItem>();
            _tests = new List<Test>();

            Activator.CreateInstance(spec);

            var result = _tests.ToArray();
            _tests = null;
            _describeStack = null;

            return result;
        }

        private class DescribeStackItem
        {
            public string Description { get; set; }
            public bool IsFocused { get; set; }
            public bool IsExcluded { get; set; }
        }
    }
}
