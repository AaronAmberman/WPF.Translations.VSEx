using System.Windows.Controls;

namespace WPF.Translations.VSEx
{
    /// <summary>Interaction logic for TranslationStringsToolWindowControl.</summary>
    public partial class TranslationStringsToolWindowControl : UserControl
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="TranslationStringsToolWindowControl"/> class.</summary>
        public TranslationStringsToolWindowControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;

            if (lb == null) return;

            // we do not want any selection in the list box
            lb.SelectedIndex = -1;
        }

        #endregion
    }
}