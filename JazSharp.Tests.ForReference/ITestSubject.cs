using System;

namespace JazSharp.Tests.ForReference
{
    public interface ITestSubject
    {
        void Call(Action action);
    }
}
