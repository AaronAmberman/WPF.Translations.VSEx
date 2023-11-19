using EnvDTE;
using Microsoft.VisualStudio.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WPF.Translations.VSEx.ViewModels;

namespace WPF.Translations.VSEx
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("cb52b525-31a1-46da-96d0-f5d31e7033d7")]
    public class TranslationStringsToolWindow : ToolWindowPane
    {
        #region Fields

        private TranslationStringsToolWindowControl control;
        private TranslationViewModel viewModel;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="TranslationStringsToolWindow"/> class.</summary>
        public TranslationStringsToolWindow() : base(null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DevelopmentEnvironment.SolutionEvents.AfterClosing += SolutionEvents_AfterClosing;
            DevelopmentEnvironment.SolutionEvents.Opened += SolutionEvents_Opened;
            DevelopmentEnvironment.SolutionEvents.ProjectAdded += SolutionEvents_ProjectAdded;
            DevelopmentEnvironment.SolutionEvents.ProjectRemoved += SolutionEvents_ProjectRemoved;
            DevelopmentEnvironment.SolutionEvents.Renamed += SolutionEvents_Renamed;

            DevelopmentEnvironment.ProjectEvents.ItemAdded += ProjectEvents_ItemAdded;
            DevelopmentEnvironment.ProjectEvents.ItemRemoved += ProjectEvents_ItemRemoved;
            DevelopmentEnvironment.ProjectEvents.ItemRenamed += ProjectEvents_ItemRenamed;

            // our custom settings
            General.Saved += General_Saved;

            // setup instance
            BitmapImageMoniker = KnownMonikers.Translate;
            Caption = "Translation Strings";

            viewModel = new TranslationViewModel();

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            control = new TranslationStringsToolWindowControl()
            {
                DataContext = viewModel
            };

            Content = control;
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.Dispose(disposing);

            // unassign events
            DevelopmentEnvironment.SolutionEvents.AfterClosing -= SolutionEvents_AfterClosing;
            DevelopmentEnvironment.SolutionEvents.Opened -= SolutionEvents_Opened;
            DevelopmentEnvironment.SolutionEvents.ProjectAdded -= SolutionEvents_ProjectAdded;
            DevelopmentEnvironment.SolutionEvents.ProjectRemoved -= SolutionEvents_ProjectRemoved;
            DevelopmentEnvironment.SolutionEvents.Renamed -= SolutionEvents_Renamed;

            DevelopmentEnvironment.ProjectEvents.ItemAdded -= ProjectEvents_ItemAdded;
            DevelopmentEnvironment.ProjectEvents.ItemRemoved -= ProjectEvents_ItemRemoved;
            DevelopmentEnvironment.ProjectEvents.ItemRenamed -= ProjectEvents_ItemRenamed;

            General.Saved -= General_Saved;

            control = null;
        }

        private void General_Saved(General settingsInstance)
        {
            if (!viewModel.MasterLanguage.Equals(settingsInstance.MasterLanguage))
            {
                DevelopmentEnvironment.Settings = settingsInstance;

                viewModel.MasterLanguage = settingsInstance.MasterLanguage;

                ThreadHelper.JoinableTaskFactory.Run(ReadInTranslationsAsync);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                DevelopmentEnvironment.Settings = await General.GetLiveInstanceAsync();

                if (DevelopmentEnvironment.Settings == null)
                    throw new InvalidOperationException("Unable to read settings for Visual Studio extension WPF.Translations.VSEx.");

                viewModel.MasterLanguage = DevelopmentEnvironment.Settings.MasterLanguage;

                await ReadInTranslationsAsync();
            });
        }

        private void ProjectEvents_ItemAdded(ProjectItem ProjectItem)
        {
            // todo : determine if item is a translation, if so update our stuff
        }

        private void ProjectEvents_ItemRemoved(ProjectItem ProjectItem)
        {
            // todo : determine if item is a translation, if so update our stuff
        }

        private void ProjectEvents_ItemRenamed(ProjectItem ProjectItem, string OldName)
        {
            // todo : is this the project being renamed or a file being renamed...or both?
            // todo : determine if item is a translation, if so update our stuff
        }

        private async Task ReadInTranslationsAsync()
        {
            List<ProjectTranslationFileViewModel> translatableFiles = await DevelopmentEnvironment.GetTranslationFilesForSolutionAsync();

            SetTranslationData(translatableFiles);
        }

        private void SetTranslationData(List<ProjectTranslationFileViewModel> projectTranslationFiles)
        {
            // clear old data
            viewModel.TranslationFiles.Clear();
            viewModel.ProjectsWithTranslations.Clear();
            viewModel.SelectedProject = null;

            // set new data
            foreach (ProjectTranslationFileViewModel file in projectTranslationFiles)
                viewModel.TranslationFiles.Add(file);

            // get distinct projects from the collection of translation files
            List<Community.VisualStudio.Toolkit.Project> projects = projectTranslationFiles.Select(x => x.Project).Distinct().ToList();

            foreach (Community.VisualStudio.Toolkit.Project project in projects)
                viewModel.ProjectsWithTranslations.Add(project);

            // select the first project in the list...if we have one
            if (viewModel.ProjectsWithTranslations.Count > 0)
                viewModel.SelectedProject = viewModel.ProjectsWithTranslations[0];
        }

        private void SolutionEvents_AfterClosing()
        {
            // todo : clean up our data
        }

        private void SolutionEvents_Opened()
        {
            // todo : read in our data
        }

        private void SolutionEvents_ProjectRemoved(EnvDTE.Project Project)
        {
            // todo : remove the project and translations from our data (if any)
        }

        private void SolutionEvents_ProjectAdded(EnvDTE.Project Project)
        {
            // todo : if the newly added project contains translations added it to our data (dev can add an existing project)
        }

        private void SolutionEvents_Renamed(string OldName)
        {
            // todo : is this the solution or the project(s) in the solution?
        }

        #endregion
    }
}
