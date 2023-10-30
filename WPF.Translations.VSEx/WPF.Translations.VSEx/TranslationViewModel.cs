using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPF.Translations.VSEx
{
    internal class TranslationViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<Tuple<string, Project>> projectsWithTranslations = new ObservableCollection<Tuple<string, Project>>();
        private int selectedProject;
        private Tuple<string, SolutionItem, Project> selectedTranslationFile;
        //private int selectedTranslationFileIndex;

        private ObservableCollection<Tuple<string, SolutionItem, Project>> translationFiles = new ObservableCollection<Tuple<string, SolutionItem, Project>>();
        private CollectionView translationFilesView;
        private CollectionViewSource translationFilesViewSource;

        private ObservableCollection<Tuple<string, string>> translations = new ObservableCollection<Tuple<string, string>>();

        #endregion

        #region Properties

        public CollectionView TranslationFilesView
        {
            get => translationFilesView;
            set
            {
                translationFilesView = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tuple<string, Project>> ProjectsWithTranslations 
        { 
            get => projectsWithTranslations;
            set
            {
                projectsWithTranslations = value;
                OnPropertyChanged();
            }
        }

        public int SelectedProject 
        { 
            get => selectedProject; 
            set
            {
                selectedProject = value;
                OnPropertyChanged();

                translationFilesView.Refresh();
            }
        }

        //public int SelectedTranslationFileIndex
        //{
        //    get => selectedTranslationFileIndex;
        //    set
        //    {
        //        selectedTranslationFileIndex = value;
        //        OnPropertyChanged();
        //    }
        //}

        public Tuple<string, SolutionItem, Project> SelectedTranslationFile 
        { 
            get => selectedTranslationFile;
            set
            {
                selectedTranslationFile = value;
                OnPropertyChanged();

                if (value == null) return;

                ReadInTranslationsFromFile(value.Item2.FullPath);
            }
        }

        public ObservableCollection<Tuple<string, SolutionItem, Project>> TranslationFiles 
        { 
            get => translationFiles; 
            set
            {
                translationFiles = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tuple<string, string>> Translations
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
            Tuple<string, SolutionItem, Project> item = obj as Tuple<string, SolutionItem, Project>;

            if (item == null) return false;

            if (item.Item3.Name != projectsWithTranslations[SelectedProject].Item2.Name)
                return false;

            return true;
        }

        private void ReadInTranslationsFromFile(string file)
        {
            Translations.Clear();

            if (file.EndsWith("xaml", StringComparison.OrdinalIgnoreCase))
            {
                ResourceDictionary rd = new ResourceDictionary
                {
                    Source = new Uri(file)
                };

                foreach (string key in rd.Keys)
                {
                    // no duplicate keys
                    if (Translations.Any(x => x.Item1 == key)) continue;

                    Translations.Add(new Tuple<string, string>(key, rd[key].ToString()));
                }
            }
            else if (file.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
            {

            }
            else if (file.EndsWith("json", StringComparison.OrdinalIgnoreCase))
            {

            }
            else throw new NotSupportedException("File type not supported.");
        }

        #endregion
    }
}
