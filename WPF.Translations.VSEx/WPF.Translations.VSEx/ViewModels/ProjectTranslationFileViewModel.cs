namespace WPF.Translations.VSEx.ViewModels
{
    internal class ProjectTranslationFileViewModel : ViewModelBase
    {
        #region Fields

        private string cultureName;
        private string friendlyName;
        private Project project;
        private SolutionItem solutionItem;

        #endregion

        #region Properties

        public string CultureName
        { 
            get => cultureName;
            set
            {
                cultureName = value;
                OnPropertyChanged();
            }
        }

        public string FriendlyName
        {
            get => friendlyName;
            set
            {
                friendlyName = value;
                OnPropertyChanged();
            }
        }

        public Project Project
        {
            get => project;
            set
            {
                project = value;
                OnPropertyChanged();
            }
        }

        public SolutionItem SolutionItem
        {
            get => solutionItem;
            set
            {
                solutionItem = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
