using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WPF.Translations.VSEx.ViewModels;

namespace WPF.Translations.VSEx
{
    /// <summary>Interaction logic for TranslationStringsToolWindowControl.</summary>
    public partial class TranslationStringsToolWindowControl : UserControl
    {
        #region Fields

        private General options;
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

            // listen for settings changes
            General.Saved += General_Saved;
        }

        #endregion

        #region Methods

        private void General_Saved(General obj)
        {
            
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;

            if (lb == null) return;

            // we do not want any selection in the list box
            lb.SelectedIndex = -1;
        }

        private void TranslationStringsToolWindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ThreadHelper.JoinableTaskFactory.Run(async () => 
            {
                // get master setting value first
                options = await General.GetLiveInstanceAsync();

                viewModel.MasterLanguage = options.MasterLanguage;

                // get translation files
                IEnumerable<Project> projects = await VS.Solutions.GetAllProjectsAsync();
                
                foreach (Project project in projects) 
                {
                    foreach (SolutionItem item in project.Children)
                    {
                        // translations directory
                        if (item.Name.EndsWith("translations\\", StringComparison.OrdinalIgnoreCase))
                        {
                            // the project contains a translations directory
                            viewModel.ProjectsWithTranslations.Add(project);

                            foreach(SolutionItem childItem in item.Children)
                            {
                                if (childItem.Name.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
                                {
                                    ProjectTranslationFileViewModel ptfvm = new ProjectTranslationFileViewModel
                                    {
                                        Project = project,
                                        SolutionItem = childItem,
                                    };
                                    ptfvm.FriendlyName = childItem.Text;
                                    ptfvm.CultureName = childItem.Text.Replace("Translations.", "").Replace(".xaml", "");

                                    // this is our translation XAML, JSON or TXT file (should we support XML? What XSD for the translation?)
                                    viewModel.TranslationFiles.Add(ptfvm);
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

                // select the first project in the list...if we have one
                if (viewModel.ProjectsWithTranslations.Count > 0)
                    viewModel.SelectedProject = viewModel.ProjectsWithTranslations[0];
            });
        }

        #endregion
    }
}