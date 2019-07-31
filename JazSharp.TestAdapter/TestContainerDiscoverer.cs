using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JazSharp.TestAdapter
{
    [Export(typeof(ITestContainerDiscoverer))]
    public class TestContainerDiscoverer : ITestContainerDiscoverer, IDisposable
    {
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher(Path.GetFullPath(string.Empty), "*.dll");

        public Uri ExecutorUri => TestAdapterConstants.ExecutorUri;
        public IEnumerable<ITestContainer> TestContainers { get; private set; }
        public event EventHandler TestContainersUpdated;

        [ImportingConstructor]
        public TestContainerDiscoverer([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            _watcher.Changed += (s, e) => UpdateTestContainers();
            UpdateTestContainers();
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }

        private void UpdateTestContainers()
        {
            File.WriteAllText(@"C:\Users\seamillo\Desktop\TestDisc.txt", "Success");
            var dllsInBin = Directory.GetFiles(string.Empty, "*.dll").Except(new[] { Assembly.GetExecutingAssembly().Location });
            TestContainers = dllsInBin.Select(x => new TestContainer(this, x, File.GetLastWriteTime(x))).ToList();
            TestContainersUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
