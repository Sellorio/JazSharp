using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;

namespace JazSharp.TestAdapter
{
    [Export(typeof(ITestContainer))]
    public class TestContainer : ITestContainer
    {
        private readonly DateTime _sourceModifiedAt;

        public ITestContainerDiscoverer Discoverer { get; }
        public string Source { get; }
        public IEnumerable<Guid> DebugEngines { get; }
        public FrameworkVersion TargetFramework { get; }
        public Architecture TargetPlatform { get; }
        public bool IsAppContainerTestContainer => false;

        public TestContainer(TestContainerDiscoverer discoverer, string source, DateTime sourceModifiedAt)
        {
            Discoverer = discoverer;
            Source = source;
            DebugEngines = new Guid[0]; // add Visual Studio here?
            TargetFramework = FrameworkVersion.FrameworkCore10;
            TargetPlatform = Architecture.AnyCPU;
            _sourceModifiedAt = sourceModifiedAt;
        }

        public int CompareTo(ITestContainer other)
        {
            if (!(other is TestContainer testContainer))
            {
                return -1;
            }

            var result = string.Compare(Source, testContainer.Source, StringComparison.OrdinalIgnoreCase);

            if (result != 0)
            {
                return result;
            }

            return _sourceModifiedAt.CompareTo(testContainer._sourceModifiedAt);
        }

        public IDeploymentData DeployAppContainer() => null;

        public ITestContainer Snapshot()
        {
            return new TestContainer((TestContainerDiscoverer)Discoverer, Source, _sourceModifiedAt);
        }
    }
}
