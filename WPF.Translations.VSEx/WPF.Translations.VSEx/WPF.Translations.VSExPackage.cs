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
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.VSExString)]
    [ProvideToolWindow(typeof(TranslationStringsToolWindow), MultiInstances = false)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.SolutionOpening_string)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.Debugging_string)]
    public sealed class VSExPackage : ToolkitPackage
    {
        //private DTE2 devEnv;
        //private SolutionEvents solutionEvents;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await TranslationStringsToolWindowCommand.InitializeAsync(this);
        }
    }
}