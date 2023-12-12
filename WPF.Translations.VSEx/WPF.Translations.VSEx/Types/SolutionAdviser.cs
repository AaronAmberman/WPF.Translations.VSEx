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
        private Debouncer projectClosedDebouncer;
        private Debouncer projectOpenedDebouncer;
        private Debouncer projectRenamedDebouncer;

        #endregion

        #region Events

        public event EventHandler<Project> ProjectClosed;
        public event EventHandler<Project> ProjectOpened; // project added (new or existing)
        public event EventHandler<Project> ProjectRenamed;

        public event EventHandler SolutionClosed;
        public event EventHandler SolutionOpened;

        #endregion

        #region Constructors

        public SolutionAdviser()
        {
            projectClosedDebouncer = new Debouncer(1000, ProjectClosedSnapshot);
            projectOpenedDebouncer = new Debouncer(1000, ProjectOpenedSnapshot);
            projectRenamedDebouncer = new Debouncer(1000, ProjectRenamedSnapshot);
        }

        #endregion

        #region Methods

        private void AssignNewSnapshot(SolutionSnapshot solutionSnapshot)
        {
            DevelopmentEnvironment.SolutionSnapshot?.Projects.Clear();
            DevelopmentEnvironment.SolutionSnapshot = null;
            DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    projectClosedDebouncer.Dispose();
                    projectOpenedDebouncer.Dispose();
                    projectRenamedDebouncer.Dispose();
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
            projectOpenedDebouncer.Debounce();

            return VSConstants.S_OK;
        }

        public int OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            // throttle the call to taking a snapshot
            projectRenamedDebouncer.Debounce();

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            // throttle the call to taking a snapshot
            projectClosedDebouncer.Debounce();

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
            projectOpenedDebouncer.Cancel();

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                SolutionSnapshot solutionSnapshot = await TakeSnapshotAsync();

                AssignNewSnapshot(solutionSnapshot);

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
            projectClosedDebouncer.Cancel();
            projectOpenedDebouncer.Cancel();
            projectRenamedDebouncer.Cancel();

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

        private void ProjectClosedSnapshot()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                if (DevelopmentEnvironment.SolutionSnapshot == null) return;

                SolutionSnapshot solutionSnapshot = await TakeSnapshotAsync();

                if (DevelopmentEnvironment.SolutionSnapshot == null)
                {
                    AssignNewSnapshot(solutionSnapshot);

                    return;
                }

                // find the project that is in the original snapshot but is not in the new snapshot
                Project closedProject = DevelopmentEnvironment.SolutionSnapshot.Projects.FirstOrDefault(p =>
                    solutionSnapshot.Projects.All(sp => sp.Key.Name != p.Key.Name)).Key;

                if (closedProject != null)
                    ProjectClosed?.Invoke(this, closedProject);

                AssignNewSnapshot(solutionSnapshot);
            });
        }

        private void ProjectOpenedSnapshot()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                if (DevelopmentEnvironment.SolutionSnapshot == null) return;

                SolutionSnapshot solutionSnapshot = await TakeSnapshotAsync();

                if (DevelopmentEnvironment.SolutionSnapshot == null)
                {
                    AssignNewSnapshot(solutionSnapshot);

                    return;
                }

                // find the project that is not in the original snapshot but is in the new snapshot
                Project newProject = solutionSnapshot.Projects.FirstOrDefault(p =>
                    DevelopmentEnvironment.SolutionSnapshot.Projects.All(sp => sp.Key.Name != p.Key.Name)).Key;

                if (newProject != null)
                    ProjectOpened?.Invoke(this, newProject);

                AssignNewSnapshot(solutionSnapshot);
            });
        }

        private void ProjectRenamedSnapshot()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                if (DevelopmentEnvironment.SolutionSnapshot == null) return;

                SolutionSnapshot solutionSnapshot = await TakeSnapshotAsync();

                if (DevelopmentEnvironment.SolutionSnapshot == null)
                {
                    AssignNewSnapshot(solutionSnapshot);

                    return;
                }

                // find the project that is not in the original snapshot but is in the new snapshot
                Project newProject = solutionSnapshot.Projects.FirstOrDefault(p =>
                    DevelopmentEnvironment.SolutionSnapshot.Projects.All(sp => sp.Key.Name != p.Key.Name)).Key;

                if (newProject != null)
                    ProjectRenamed?.Invoke(this, newProject);

                AssignNewSnapshot(solutionSnapshot);
            });
        }

        protected async Task<SolutionSnapshot> TakeSnapshotAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            SolutionSnapshot solutionSnapshot = new SolutionSnapshot();
            await solutionSnapshot.TakeSolutionSnapshotAsync();

            return solutionSnapshot;
        }

        #endregion
    }
}
