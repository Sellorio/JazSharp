using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JazSharp.Testing
{
    internal class TestExecution
    {
        internal ImmutableArray<Delegate> BeforeEach { get; private set; }
        internal ImmutableArray<Delegate> AfterEach { get; private set; }
        internal Delegate Main { get; }

        internal TestExecution(Delegate main)
        {
            Main = main;
        }

        internal TestExecution(IEnumerable<Delegate> beforeEach, IEnumerable<Delegate> afterEach, Delegate main)
        {
            BeforeEach = ImmutableArray.CreateRange(beforeEach);
            AfterEach = ImmutableArray.CreateRange(afterEach);
            Main = main;
        }

        internal Delegate[] GetDelegates()
        {
            return BeforeEach.Concat(new[] { Main }).Concat(AfterEach).ToArray();
        }

        internal void SetBeforeAndAfter(IEnumerable<Delegate> beforeEach, IEnumerable<Delegate> afterEach)
        {
            BeforeEach = ImmutableArray.CreateRange(beforeEach);
            AfterEach = ImmutableArray.CreateRange(afterEach);
        }
    }
}
