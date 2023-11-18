namespace WPF.Translations.VSEx.ViewModels
{
    public class TranslationEntryViewModel : ViewModelBase
    {
        #region Fields

        private string key;
        private string value;

        #endregion

        #region Properties

        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                OnPropertyChanged();
            }
        }

        public string Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
