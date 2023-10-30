using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Translations.VSEx
{
    /// <summary>Interaction logic for TranslationStringsToolWindowControl.</summary>
    public partial class TranslationStringsToolWindowControl : UserControl
    {
        #region Fields

        private TranslationViewModel viewModel;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="TranslationStringsToolWindowControl"/> class.</summary>
        public TranslationStringsToolWindowControl()
        {
            viewModel = new TranslationViewModel();

            DataContext = viewModel;

            Loaded += TranslationStringsToolWindowControl_Loaded;

            InitializeComponent();
        }

        #endregion

        #region Methods

        private void TranslationStringsToolWindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ThreadHelper.JoinableTaskFactory.Run(async () => 
            {
                IEnumerable<Project> projects = await VS.Solutions.GetAllProjectsAsync();
                
                foreach (Project project in projects) 
                {
                    foreach (SolutionItem item in project.Children)
                    {
                        // translations directory
                        if (item.Name.EndsWith("translations\\", StringComparison.OrdinalIgnoreCase))
                        {
                            // the project contains a translations directory
                            viewModel.ProjectsWithTranslations.Add(new Tuple<string, Project>(project.Name, project));

                            foreach(SolutionItem childItem in item.Children)
                            {
                                if (!(childItem.Name.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) ||
                                    childItem.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                    childItem.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)))
                                    continue;

                                // this is our translation XAML, JSON or TXT file (should we support XML? What XSD for the translation?)
                                viewModel.TranslationFiles.Add(new Tuple<string, SolutionItem, Project>(Path.GetFileName(childItem.FullPath), childItem, project));
                            }
                        }
                    }
                }
            });
        }

        #endregion
    }
}