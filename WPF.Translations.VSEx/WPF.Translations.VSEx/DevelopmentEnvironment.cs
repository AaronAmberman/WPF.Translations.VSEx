using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;
using WPF.Translations.VSEx.ViewModels;

namespace WPF.Translations.VSEx
{
    /// <summary>A static helper class for Visual Studio level objects and methods.</summary>
    public static class DevelopmentEnvironment
    {
        #region Properties

        public static DTE DTE { get; set; }

        public static DTE2 DTE2 { get; set; }

        public static EnvDTE.ProjectItemsEvents ProjectEvents { get; set; }

        public static General Settings { get; set; }

        public static EnvDTE.SolutionEvents SolutionEvents { get; set; }

        public static EnvDTE.Events VisualStudioEvents { get; set; }

        #endregion

        #region Methods

        public static async Task<List<ProjectTranslationFileViewModel>> GetTranslationFilesForSolutionAsync()
        {
            List<ProjectTranslationFileViewModel> projectTranslationFileViewModels = new List<ProjectTranslationFileViewModel>();

            // get translation files
            IEnumerable<Community.VisualStudio.Toolkit.Project> projects = await VS.Solutions.GetAllProjectsAsync();

            foreach (Community.VisualStudio.Toolkit.Project project in projects)
            {
                foreach (SolutionItem item in project.Children)
                {
                    if (item.Name.IsTranslationsDirectory())
                    {
                        // the project contains a translations directory

                        foreach (SolutionItem childItem in item.Children)
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

                            }
                            else if (childItem.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            {

                            }
                        }
                    }
                }
            }

            return projectTranslationFileViewModels;
        }

        #endregion
    }
}
