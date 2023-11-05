using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WPF.Translations.VSEx
{
    internal partial class OptionsProvider
    {
        [ComVisible(true)]
        public class GeneralOptions : BaseOptionPage<General> { }
    }

    public class General : BaseOptionModel<General>
    {
        [Category("Master Configuration")]
        [DisplayName("Master Language")]
        [Description("The default language culture when using the WPF.Translation.VSEx Visual Studio extension. This should be the actual desired culture; en, en-US, en-GB, fr, it, de, etc.")]
        [DefaultValue("en")]
        public string MasterLanguage { get; set; } = "en";
    }
}
