using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;

namespace WPF.Translations.VSEx.Types
{
    public class SolutionAdviser : ISolutionAdvisor
    {
        #region Fields

        private Debouncer projectOpenedDebouncer;
        private bool disposedValue;

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
            projectOpenedDebouncer = new Debouncer(1000, ProjectOpenedSnapshot);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    projectOpenedDebouncer.Dispose();
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

        private void ProjectOpenedSnapshot()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                SolutionSnapshot solutionSnapshot = new SolutionSnapshot();

                await solutionSnapshot.TakeSolutionSnapshotAsync();

                // find the project that is not in the original snapshot but is in the new snapshot
                Project newProject = solutionSnapshot.Projects.FirstOrDefault(p => DevelopmentEnvironment.SolutionSnapshot.Projects.All(sp => sp.Key.Name != p.Key.Name)).Key;

                if (newProject != null)
                    ProjectOpened?.Invoke(this, newProject);

                DevelopmentEnvironment.SolutionSnapshot.Projects.Clear();
                DevelopmentEnvironment.SolutionSnapshot = null;
                DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;
            });
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            // project closed

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
            // solution opened

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
            // solution closed

            return VSConstants.S_OK;
        }

        public int OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            // project renamed

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

        #endregion
    }
}
