using System;

namespace JazSharp.TestSetup
{
    internal sealed class Test
    {
        public bool IsFocused { get; }
        public bool IsExcluded { get; }
        public string[] Path { get; }
        public string Description { get; }
        public string FullName { get; }
        public Delegate Execution { get; }

        internal Test(string[] path, string description, Delegate execution, bool isFocused, bool isExcluded)
        {
            Path = path;
            Description = description;
            FullName = string.Join(" ", path) + " " + description;
            Execution = execution;
            IsFocused = isFocused;
            IsExcluded = isExcluded;
        }
    }
}
