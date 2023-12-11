using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPF.Translations.VSEx.Types;
using WPF.Translations.VSEx.ViewModels;

namespace WPF.Translations.VSEx
{
    /// <summary>A static helper class for Visual Studio level objects and methods.</summary>
    public static class DevelopmentEnvironment
    {
        #region Properties

        public static uint CookieProject { get; set; }

        public static uint CookieSolution { get; set; }

        public static DTE DTE { get; set; }

        public static DTE2 DTE2 { get; set; }

        public static Guid OutputWidowGuid { get; set; } = Guid.NewGuid();

        public static IVsOutputWindow OutputWindow { get; set; }

        public static IVsOutputWindowPane OutputWindowPane { get; set; }

        public static ProjectAdvisor ProjectAdvisor { get; set; }

        public static General Settings { get; set; }

        public static SolutionAdviser SolutionAdviser { get; set; }

        public static SolutionSnapshot SolutionSnapshot { get; set; }

        public static IVsTrackProjectDocuments2 VSProjectDocuments { get; set; }

        public static IVsSolution VsSolution { get; set; }

        #endregion

        #region Methods

        public static async Task<List<ProjectTranslationFileViewModel>> GetTranslationFilesForSolutionAsync()
        {
            // make snapshot of solution while we are going through the items (this will be our "default" snapshot)
            SolutionSnapshot solutionSnapshot = new SolutionSnapshot();            
            List<ProjectTranslationFileViewModel> projectTranslationFileViewModels = new List<ProjectTranslationFileViewModel>();

            // get translation files
            IEnumerable<Community.VisualStudio.Toolkit.Project> projects = await VS.Solutions.GetAllProjectsAsync();

            foreach (Community.VisualStudio.Toolkit.Project project in projects)
            {
                List<Types.SolutionItem> solutionItems = new List<Types.SolutionItem>();

                foreach (Community.VisualStudio.Toolkit.SolutionItem item in project.Children)
                {
                    solutionItems.Add(new Types.SolutionItem(item));

                    if (item.Name.IsTranslationsDirectory())
                    {
                        // the project contains a translations directory

                        foreach (Community.VisualStudio.Toolkit.SolutionItem childItem in item.Children)
                        {
                            if (childItem.Name.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
                            {
                                string childCulture = childItem.Text.GetCultureFromFileName(".xaml");

                                // if we cannot determine the culture on the file then skip it
                                if (string.IsNullOrWhiteSpace(childCulture)) continue;

                                ProjectTranslationFileViewModel ptfvm = new ProjectTranslationFileViewModel
                                {
                                    Project = project,
                                    SolutionItem = childItem,
                                };
                                ptfvm.FriendlyName = childItem.Text;
                                ptfvm.CultureName = childCulture;

                                // this is our translation XAML
                                projectTranslationFileViewModels.Add(ptfvm);
                            }
                            else if (childItem.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                            {
                                // todo
                            }
                            else if (childItem.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            {
                                // todo
                            }
                        }
                    }
                }

                solutionSnapshot.Projects.Add(new Types.Project(project), solutionItems);
            }

            DevelopmentEnvironment.SolutionSnapshot = solutionSnapshot;

            return projectTranslationFileViewModels;
        }

        public static int WriteToOutputWindow(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return OutputWindowPane.OutputStringThreadSafe(message);
        }

        #endregion
    }
}
