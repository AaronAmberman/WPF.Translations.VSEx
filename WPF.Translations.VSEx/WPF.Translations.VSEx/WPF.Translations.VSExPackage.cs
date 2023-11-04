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
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.SolutionOpening_string)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [ProvideToolWindowVisibility(typeof(TranslationStringsToolWindow), VSConstants.UICONTEXT.Debugging_string)]
    public sealed class VSExPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await TranslationStringsToolWindowCommand.InitializeAsync(this);
        }
    }
}