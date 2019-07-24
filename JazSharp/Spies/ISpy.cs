using JazSharp.SpyLogic;
using System;

namespace JazSharp.Spies
{
    internal interface ISpy : IDisposable
    {
        SpyInfo SpyInfo { get; }
        object Key { get; }
    }
}
