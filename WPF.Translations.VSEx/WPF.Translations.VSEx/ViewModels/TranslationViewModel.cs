using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WPF.Translations.VSEx.ViewModels
{
    public class TranslationViewModel : ViewModelBase
    {
        #region Fields

        private string masterLanguage;
        private ObservableCollection<Project> projectsWithTranslations = new ObservableCollection<Project>();
        private Project selectedProject;
        private ObservableCollection<ProjectTranslationFileViewModel> translationFiles = new ObservableCollection<ProjectTranslationFileViewModel>();
        private CollectionView translationFilesView;
        private CollectionViewSource translationFilesViewSource;
        private ObservableCollection<TranslationEntryViewModel> translations = new ObservableCollection<TranslationEntryViewModel>();

        #endregion

        #region Properties

        public string MasterLanguage
        {
            get => masterLanguage;
            set
            {
                masterLanguage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Project> ProjectsWithTranslations 
        { 
            get => projectsWithTranslations;
            set
            {
                projectsWithTranslations = value;
                OnPropertyChanged();
            }
        }

        public Project SelectedProject 
        { 
            get => selectedProject; 
            set
            {
                selectedProject = value;
                OnPropertyChanged();

                FindAndLoadMasterTranslationForSelectedProject();
                TranslationFilesView.Refresh();
            }
        }

        public ObservableCollection<ProjectTranslationFileViewModel> TranslationFiles 
        { 
            get => translationFiles; 
            set
            {
                translationFiles = value;
                OnPropertyChanged();
            }
        }

        public CollectionView TranslationFilesView
        {
            get => translationFilesView;
            set
            {
                translationFilesView = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TranslationEntryViewModel> Translations
        {
            get => translations;
            set
            {
                translations = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public TranslationViewModel()
        {
            translationFilesViewSource = new CollectionViewSource
            {
                Source = TranslationFiles
            };

            TranslationFilesView = (CollectionView)translationFilesViewSource.View;
            TranslationFilesView.Filter = FilterTranslationFilesForProject;
        }

        #endregion

        #region Methods

        private bool FilterTranslationFilesForProject(object obj)
        {
            ProjectTranslationFileViewModel item = obj as ProjectTranslationFileViewModel;

            if (item == null) return false;

            if (item.Project.Name != SelectedProject?.Name)
                return false;

            return true;
        }

        private void FindAndLoadMasterTranslationForSelectedProject()
        {
            // we select a project by default but just to be safe
            // (maybe the user shows the window with no solution loaded or has an empty solution)
            if (SelectedProject == null) return;

            string fullPath = string.Empty;

            foreach (SolutionItem item in SelectedProject.Children)
            {
                // translations directory
                if (item.Name.EndsWith("translations\\", StringComparison.OrdinalIgnoreCase))
                {
                    // the project contains a translations directory
                    foreach (SolutionItem childItem in item.Children)
                    {
                        if (childItem.Name.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
                        {
                            string culture = childItem.Text.Replace("Translations.", "").Replace(".xaml", "");

                            if (culture == MasterLanguage)
                                fullPath = childItem.FullPath;
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

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                System.Windows.MessageBox.Show($"A master translation file could not be found in the project {SelectedProject.Name}.", "No Master Translation");

                return;
            }

            ReadInTranslationsFromFile(fullPath);
        }

        private void ReadInTranslationsFromFile(string file)
        {
            Translations.Clear();

            List<TranslationEntryViewModel> ts = new List<TranslationEntryViewModel>();

            if (file.EndsWith("xaml", StringComparison.OrdinalIgnoreCase))
            {
                ResourceDictionary rd = new ResourceDictionary
                {
                    Source = new Uri(file)
                };

                foreach (string key in rd.Keys)
                {
                    // no duplicate keys
                    if (Translations.Any(x => x.Key == key)) continue;

                    ts.Add(new TranslationEntryViewModel 
                    {
                        Key = key,
                        Value = rd[key].ToString()
                    });
                }
            }
            else if (file.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
            {

            }
            else if (file.EndsWith("json", StringComparison.OrdinalIgnoreCase))
            {

            }
            else throw new NotSupportedException("File type not supported.");

            // order results
            ts = ts.OrderBy(x => x.Key).ToList();

            // add ordered results
            foreach (TranslationEntryViewModel t in ts) 
                Translations.Add(t);
        }

        #endregion
    }
}
