﻿using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace WPF.Translations.VSEx.Types
{
    public class ProjectAdvisor : IProjectAdvisor
    {
        #region Fields

        private bool disposedValue;
        private Debouncer itemClosedDebouncer;
        private Debouncer itemOpenedDebouncer;

        #endregion

        #region Events

        public event EventHandler<SolutionItem> ItemAdded; // (new or existing)
        public event EventHandler<SolutionItem> ItemRemoved; 
        public event EventHandler<SolutionItem> ItemRename;

        #endregion

        #region Constructors

        public ProjectAdvisor()
        {
            itemClosedDebouncer = new Debouncer(1000, ItemClosedSnapshot);
            itemOpenedDebouncer = new Debouncer(1000, ItemOpenedSnapshot);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    itemClosedDebouncer.Dispose();
                    itemOpenedDebouncer.Dispose();
                }

                disposedValue=true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        private void ItemClosedSnapshot()
        {

        }

        private void ItemOpenedSnapshot()
        {

        }

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            // file(s) added

            return VSConstants.S_OK;
        }

        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            // directory/directories added

            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            // file(s) removed

            return VSConstants.S_OK;
        }

        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            // directory/directories removed

            return VSConstants.S_OK;
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            // renamed file

            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            // renamed directory
            return VSConstants.S_OK;
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
