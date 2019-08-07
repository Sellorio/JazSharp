using System;
using System.Collections.Generic;
using System.Linq;

namespace JazSharp.Testing
{
    internal static class SpecHelper
    {
        private static Stack<DescribeStackItem> _describeStack;
        private static Dictionary<Test, DescribeStackItem> _tests;
        private static List<TestExecution> _executions;
        private static Type _testClass;

        internal static void PushDescribe(string description, bool isFocused, bool isExcluded)
        {
            _describeStack.Push(new DescribeStackItem
            {
                Parent = _describeStack.Any() ? _describeStack.Peek() : null,
                Description = description,
                IsExcluded = isExcluded,
                IsFocused = isFocused && !isExcluded
            });
        }

        internal static void PopDescribe()
        {
            _describeStack.Pop();
        }

        internal static void AddBeforeEach(Delegate beforeEach)
        {
            if (_describeStack.Count == 0)
            {
                throw new InvalidOperationException("BeforeEach can only be defined inside a Describe.");
            }

            _describeStack.Peek().BeforeEach.Add(beforeEach);
        }

        internal static void AddAfterEach(Delegate afterEach)
        {
            if (_describeStack.Count == 0)
            {
                throw new InvalidOperationException("AfterEach can only be defined inside a Describe.");
            }

            _describeStack.Peek().AfterEach.Add(afterEach);
        }

        internal static void RegisterTest(string description, Delegate action, bool isFocused, bool isExcluded, string sourceFilename, int lineNumber)
        {
            if (_describeStack.Count == 0)
            {
                throw new InvalidOperationException("A test can only be defined inside a Describe.");
            }

            var actualIsExcluded = isExcluded || _describeStack.Any(x => x.IsExcluded);
            var actualIsFocused = !actualIsExcluded && isFocused || _describeStack.Any(x => x.IsFocused);

            _tests.Add(
                new Test(
                    _testClass,
                    _describeStack.Select(x => x.Description).Reverse().ToArray(),
                    description,
                    new TestExecution(action),
                    actualIsFocused,
                    actualIsExcluded,
                    sourceFilename,
                    lineNumber),
                _describeStack.Peek());
        }

        internal static Test[] GetTestsInSpec(Type spec)
        {
            _describeStack = new Stack<DescribeStackItem>();
            _testClass = spec;
            _tests = new Dictionary<Test, DescribeStackItem>();

            Activator.CreateInstance(spec);

            foreach (var test in _tests)
            {
                test.Key.Execution.SetBeforeAndAfter(GetBeforeEachMethods(test.Value), GetAfterEachMethods(test.Value));
            }

            var result = _tests.Keys.ToArray();
            _tests = null;
            _testClass = null;
            _describeStack = null;

            return result;
        }

        internal static Delegate[][] GetTestExecutionMethods(Type spec, string testFullName)
        {
            var tests = GetTestsInSpec(spec);
            Delegate[][] result = null;

            for (var i = 0; i < tests.Length; i++)
            {
                if (tests[i].FullName == testFullName)
                {
                    result = new Delegate[][]
                    {
                        tests[i].Execution.BeforeEach.ToArray(),
                        new[] { tests[i].Execution.Main },
                        tests[i].Execution.AfterEach.ToArray()
                    };

                    break;
                }
            }

            _executions = null;

            return result;
        }

        private static IEnumerable<Delegate> GetBeforeEachMethods(DescribeStackItem describeStackItem)
        {
            List<Delegate> result = new List<Delegate>();

            do
            {
                result.AddRange(describeStackItem.BeforeEach);
                describeStackItem = describeStackItem.Parent;
            }
            while (describeStackItem != null);

            return Enumerable.Reverse(result);
        }

        private static IEnumerable<Delegate> GetAfterEachMethods(DescribeStackItem describeStackItem)
        {
            List<Delegate> result = new List<Delegate>();

            do
            {
                result.AddRange(describeStackItem.AfterEach);
                describeStackItem = describeStackItem.Parent;
            }
            while (describeStackItem != null);

            return result;
        }

        private class DescribeStackItem
        {
            public DescribeStackItem Parent { get; set; }
            public string Description { get; set; }
            public bool IsFocused { get; set; }
            public bool IsExcluded { get; set; }
            public List<Delegate> BeforeEach { get; } = new List<Delegate>();
            public List<Delegate> AfterEach { get; } = new List<Delegate>();
            public List<int> TestIndexes { get; } = new List<int>();
        }
    }
}
