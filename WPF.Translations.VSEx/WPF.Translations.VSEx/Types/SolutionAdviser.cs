using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;
using System.Threading.Tasks;

namespace WPF.Translations.VSEx.Types
{
    public class SolutionAdviser : ISolutionAdvisor
    {
        #region Fields

        private bool disposedValue;
        private Debouncer projectAddedDebouncer;
        private Debouncer projectRemovedDebouncer;

        #endregion

        #region Events

        public event EventHandler<Project> ProjectAdded; // project added (new or existing)
        public event EventHandler<Project> ProjectRemoved;
        public event EventHandler<Project> ProjectRenamed;

        public event EventHandler SolutionClosed;
        public event EventHandler SolutionOpened;

        #endregion

        #region Constructors

        public SolutionAdviser()
        {
            projectAddedDebouncer = new Debouncer(1000, ProjectAddedSnapshot);
            projectRemovedDebouncer = new Debouncer(1000, ProjectRemovedSnapshot);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    projectRemovedDebouncer.Dispose();
                    projectAddedDebouncer.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            // throttle the call to taking a snapshot
            projectAddedDebouncer.Debounce();

            return VSConstants.S_OK;
        }

        public int OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                if (DevelopmentEnvironment.SolutionSnapshot == null) return;

                SolutionSnapshot solutionSnapshot = await DevelopmentEnvironment.TakeSnapshotAsync();

                if (DevelopmentEnvironment.SolutionSnapshot == null)
                {
                    DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                    return;
                }

                // find the project that is not in the original snapshot but is in the new snapshot
                Project newProject = solutionSnapshot.Projects.FirstOrDefault(p =>
                    DevelopmentEnvironment.SolutionSnapshot.Projects.All(sp => sp.Key.Name != p.Key.Name)).Key;

                DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                if (newProject != null)
                    ProjectRenamed?.Invoke(this, newProject);
            });

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            // throttle the call to taking a snapshot
            projectRemovedDebouncer.Debounce();

            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            // we don't need the project opened to fire as well (this method fires after project opened)
            projectAddedDebouncer.Cancel();

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                SolutionSnapshot solutionSnapshot = await DevelopmentEnvironment.TakeSnapshotAsync();

                DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                SolutionOpened?.Invoke(this, EventArgs.Empty);
            });

            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            projectRemovedDebouncer.Cancel();
            projectAddedDebouncer.Cancel();

            DevelopmentEnvironment.SolutionSnapshot.Projects.Clear();
            DevelopmentEnvironment.SolutionSnapshot = null;

            SolutionClosed?.Invoke(this, EventArgs.Empty);

            return VSConstants.S_OK;
        }

        public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        private void ProjectAddedSnapshot()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                if (DevelopmentEnvironment.SolutionSnapshot == null) return;

                SolutionSnapshot solutionSnapshot = await DevelopmentEnvironment.TakeSnapshotAsync();

                if (DevelopmentEnvironment.SolutionSnapshot == null)
                {
                    DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                    return;
                }

                // find the project that is not in the original snapshot but is in the new snapshot
                Project newProject = solutionSnapshot.Projects.FirstOrDefault(p =>
                    DevelopmentEnvironment.SolutionSnapshot.Projects.All(sp => sp.Key.Name != p.Key.Name)).Key;

                DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                if (newProject != null)
                    ProjectAdded?.Invoke(this, newProject);
            });
        }

        private void ProjectRemovedSnapshot()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                if (DevelopmentEnvironment.SolutionSnapshot == null) return;

                SolutionSnapshot solutionSnapshot = await DevelopmentEnvironment.TakeSnapshotAsync();

                if (DevelopmentEnvironment.SolutionSnapshot == null)
                {
                    DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                    return;
                }

                // find the project that is in the original snapshot but is not in the new snapshot
                Project closedProject = DevelopmentEnvironment.SolutionSnapshot.Projects.FirstOrDefault(p =>
                    solutionSnapshot.Projects.All(sp => sp.Key.Name != p.Key.Name)).Key;

                DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                if (closedProject != null)
                    ProjectRemoved?.Invoke(this, closedProject);
            });
        }

        #endregion
    }
}
