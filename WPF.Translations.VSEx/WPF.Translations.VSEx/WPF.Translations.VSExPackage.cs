global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Threading;

namespace WPF.Translations.VSEx
{
    /*
     * For a helpful example check out this extension that manages translation files and translations...
     * https://github.com/JoacimWall/Visual-studio-multilingual-extension
     * 
     * Using Google API to translate...
     * https://github.com/JoacimWall/Visual-studio-multilingual-extension/blob/main/src/MultilingualExtension/MultilingualExtension.SharedCode/Services/TranslationService.cs
     * 
     * VS Theme Colors...
     * https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.platformui.environmentcolors?view=visualstudiosdk-2022&redirectedfrom=MSDN
     * 
     * VS Resource Keys...
     * https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.vsresourcekeys?view=visualstudiosdk-2019
     */

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.VSExString)]
    [ProvideToolWindow(typeof(TranslationStringsToolWindow), MultiInstances = false)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.Debugging_string)]
    [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "WPF.Translations.VSEx", "General", 0, 0, true, SupportsProfiles = true)]
    public sealed class VSExPackage : ToolkitPackage
    {
        #region Methods

        protected override void Dispose(bool disposing)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.Dispose(disposing);

            // clean up objects            
            DevelopmentEnvironment.DTE = null;
            DevelopmentEnvironment.DTE2 = null;
            DevelopmentEnvironment.OutputWindow.DeletePane(DevelopmentEnvironment.OutputWidowGuid);
            DevelopmentEnvironment.OutputWindowPane = null;
            DevelopmentEnvironment.OutputWindow = null;
            DevelopmentEnvironment.OutputWidowGuid = Guid.Empty;
            DevelopmentEnvironment.VSProjectDocuments.UnadviseTrackProjectDocumentsEvents(DevelopmentEnvironment.CookieProject);
            DevelopmentEnvironment.VSProjectDocuments = null;
            DevelopmentEnvironment.VsSolution.UnadviseSolutionEvents(DevelopmentEnvironment.CookieSolution);
            DevelopmentEnvironment.VsSolution = null;
            DevelopmentEnvironment.CookieProject = 0;
            DevelopmentEnvironment.CookieSolution = 0;
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await TranslationStringsToolWindowCommand.InitializeAsync(this);

            // get services
            DevelopmentEnvironment.DTE = await GetServiceAsync(typeof(DTE)) as DTE;

            if (DevelopmentEnvironment.DTE == null)
                throw new InvalidOperationException("DTE cannot be found.");

            DevelopmentEnvironment.DTE2 = DevelopmentEnvironment.DTE as DTE2;

            if (DevelopmentEnvironment.DTE2 == null)
                throw new InvalidOperationException("DTE2 cannot be found.");

            // get event objects
            DevelopmentEnvironment.VsSolution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            DevelopmentEnvironment.VSProjectDocuments = await GetServiceAsync(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;

            DevelopmentEnvironment.ProjectAdvisor = new ProjectAdvisor();
            DevelopmentEnvironment.SolutionAdviser = new SolutionAdviser();

            DevelopmentEnvironment.VsSolution.AdviseSolutionEvents(DevelopmentEnvironment.SolutionAdviser, out uint solutionCookie);
            DevelopmentEnvironment.VSProjectDocuments.AdviseTrackProjectDocumentsEvents(DevelopmentEnvironment.ProjectAdvisor, out uint projectCookie);

            DevelopmentEnvironment.CookieSolution = solutionCookie;
            DevelopmentEnvironment.CookieProject = projectCookie;

            // get output window
            DevelopmentEnvironment.OutputWindow = await GetServiceAsync(typeof(IVsOutputWindow)) as IVsOutputWindow;
            
            IVsOutputWindowPane outputWindowPane;

            DevelopmentEnvironment.OutputWindow.CreatePane(DevelopmentEnvironment.OutputWidowGuid, "WPF.Translations.VSEx", 1, 1);
            DevelopmentEnvironment.OutputWindow.GetPane(DevelopmentEnvironment.OutputWidowGuid, out outputWindowPane);
            DevelopmentEnvironment.OutputWindowPane = outputWindowPane;
        }

        #endregion
    }
}