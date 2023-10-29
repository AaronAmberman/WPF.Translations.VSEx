using EnvDTE80;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Translations.VSEx
{
    /// <summary>Interaction logic for TranslationStringsToolWindowControl.</summary>
    public partial class TranslationStringsToolWindowControl : UserControl
    {
        #region Fields

        private DTE2 devEnv;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="TranslationStringsToolWindowControl"/> class.</summary>
        public TranslationStringsToolWindowControl()
        {
            this.InitializeComponent();

            //devEnv = ServiceProvider.GlobalProvider.GetService(typeof(DTE2)) as DTE2;
        }

        #endregion

        #region Methods



        #endregion
    }
}