using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPF.Translations.VSEx.Types
{
    public class ProjectAdvisor : IProjectAdvisor
    {
        #region Fields

        private List<string> addedRemovedItems;
        private bool disposedValue;
        private Debouncer itemClosedDebouncer;
        private Debouncer itemOpenedDebouncer;

        #endregion

        #region Events

        public event EventHandler<List<SolutionItem>> ItemsAdded; // (new or existing)
        public event EventHandler<List<SolutionItem>> ItemsRemoved; 
        public event EventHandler<SolutionItem> ItemRenamed;

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
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private SolutionItem FindDescendant(List<SolutionItem> items, string newName)
        {
            SolutionItem match = null;

            foreach (SolutionItem item in items) 
            {
                foreach (SolutionItem child in item.Children)
                {
                    if (child.FullPath == newName)
                    {
                        match = child;
                        break;
                    }
                }

                if (match == null && item.Children.Count > 0)
                {
                    // go deeper into descendant tree
                    match = FindDescendant(item.Children, newName);
                }

                // match found in descendant tree
                if (match != null) break;
            }

            return match;
        }

        private List<SolutionItem> FindMissingDescendants(List<SolutionItem> storedChildren, List<SolutionItem> localChildren)
        {
            List<SolutionItem> results = new List<SolutionItem>();

            foreach (SolutionItem child in storedChildren)
            {
                SolutionItem match = localChildren.FirstOrDefault(si => si.FullPath == child.FullPath);

                if (match == null)
                {
                    results.Add(child);

                    continue;
                }

                // check the children as well
                List<SolutionItem> solutionItems = FindMissingDescendants(child.Children, match.Children);

                results.AddRange(solutionItems);
            }

            return results;
        }

        // added versus removed will feed the same objects in just as the different parameters
        // so in Added dev env solution snapshot is solution snapshot
        // so in Removed dev env solution snapshot is the local snapshot
        private List<SolutionItem> AddedRemovedItemsWorkflow(SolutionSnapshot solutionSnapshot, SolutionSnapshot localSnapshot)
        {
            // find all removed files
            List<SolutionItem> items = new List<SolutionItem>();

            foreach (var project in solutionSnapshot.Projects)
            {
                // get matching project
                Project match = localSnapshot.Projects.FirstOrDefault(p => p.Key.Name == project.Key.Name).Key;

                if (match == null) continue;

                // get files in dev env snapshot that are not in the local snapshot
                foreach (SolutionItem child in project.Key.Children)
                {
                    SolutionItem matchingItem = match.Children.FirstOrDefault(match => match.FullPath == child.FullPath);

                    if (matchingItem == null)
                    {
                        items.Add(child);

                        continue;
                    }

                    // check the children as well
                    List<SolutionItem> solutionItems = FindMissingDescendants(child.Children, matchingItem.Children);

                    items.AddRange(solutionItems);
                }
            }

            return items;
        }

        private void ItemClosedSnapshot()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                // multiple items could have been removed under different parents, find all removed files
                if (DevelopmentEnvironment.SolutionSnapshot == null) return;

                SolutionSnapshot solutionSnapshot = await DevelopmentEnvironment.TakeSnapshotAsync();

                if (DevelopmentEnvironment.SolutionSnapshot == null)
                {
                    DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                    return;
                }

                List<SolutionItem> removedItems = AddedRemovedItemsWorkflow(DevelopmentEnvironment.SolutionSnapshot, solutionSnapshot);
                List<SolutionItem> realRemovedItems = new List<SolutionItem>();

                // we only need the references that are equal our collection of removed items
                foreach (string removedItem in addedRemovedItems) 
                {
                    SolutionItem match = removedItems.FirstOrDefault(i => i.FullPath == removedItem);

                    if (match == null) continue;

                    realRemovedItems.Add(match);
                }

                DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                addedRemovedItems.Clear();

                ItemsRemoved?.Invoke(this, realRemovedItems);
            });
        }

        private void ItemOpenedSnapshot()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                // multiple items could have been added under different parents, find all added files
                if (DevelopmentEnvironment.SolutionSnapshot == null) return;

                SolutionSnapshot solutionSnapshot = await DevelopmentEnvironment.TakeSnapshotAsync();

                if (DevelopmentEnvironment.SolutionSnapshot == null)
                {
                    DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                    return;
                }

                List<SolutionItem> addedItems = AddedRemovedItemsWorkflow(solutionSnapshot, DevelopmentEnvironment.SolutionSnapshot);
                List<SolutionItem> realAddedItems = new List<SolutionItem>();

                // we only need the references that are equal our collection of removed items
                foreach (string addedItem in addedRemovedItems)
                {
                    SolutionItem match = addedItems.FirstOrDefault(i => i.FullPath == addedItem);

                    if (match == null) continue;

                    realAddedItems.Add(match);
                }

                DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

                addedRemovedItems.Clear();

                ItemsAdded?.Invoke(this, realAddedItems);
            });
        }

        private async Task ItemRenamedWorkflowAsync(string newName)
        {
            // todo : ensure this is not a project file (*.csproj or *.vbproj...are there others?)

            SolutionSnapshot solutionSnapshot = await DevelopmentEnvironment.TakeSnapshotAsync();

            SolutionItem solutionItem = null;

            // find the item that is in the new snapshot that matches the new name
            foreach (var project in solutionSnapshot.Projects)
            {
                solutionItem = project.Value.FirstOrDefault(si => si.FullPath == newName);

                // if the item is not found in the direct collection then start checking children
                if (solutionItem == null)
                    solutionItem = FindDescendant(project.Value, newName);

                // we found our match stop searching
                if (solutionItem != null) break;
            }

            DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

            if (solutionItem != null)
                ItemRenamed?.Invoke(this, solutionItem);
        }

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            // file(s) added
            addedRemovedItems = rgpszMkDocuments.ToList();

            itemOpenedDebouncer.Debounce();

            return VSConstants.S_OK;
        }

        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            // directory/directories added
            addedRemovedItems = new List<string>();

            foreach (string s in rgpszMkDocuments)
                addedRemovedItems.Add(s.EnsureDirectoryEndsWithBackSlash());

            itemOpenedDebouncer.Debounce();

            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            // file(s) removed
            addedRemovedItems = rgpszMkDocuments.ToList();

            itemClosedDebouncer.Debounce();

            return VSConstants.S_OK;
        }

        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            // directory/directories removed
            addedRemovedItems = new List<string>();

            foreach (string s in rgpszMkDocuments)
                addedRemovedItems.Add(s.EnsureDirectoryEndsWithBackSlash());

            itemClosedDebouncer.Debounce();

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
            ThreadHelper.JoinableTaskFactory.Run(async () => 
            {
                await ItemRenamedWorkflowAsync(rgszMkNewNames[0]);
            });

            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            // renamed directory
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ItemRenamedWorkflowAsync(rgszMkNewNames[0].EnsureDirectoryEndsWithBackSlash());
            });

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
