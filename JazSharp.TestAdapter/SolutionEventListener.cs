using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace JazSharp.TestAdapter
{
    public sealed class SolutionEventListener : IVsSolutionEvents
    {
        public SolutionEventListener(IServiceProvider _)
        {
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return default;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return default;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return default;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return default;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return default;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return default;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return default;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return default;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return default;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return default;
        }
    }
}
