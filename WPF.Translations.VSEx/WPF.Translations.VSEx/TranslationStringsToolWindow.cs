using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;

namespace WPF.Translations.VSEx
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("cb52b525-31a1-46da-96d0-f5d31e7033d7")]
    public class TranslationStringsToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationStringsToolWindow"/> class.
        /// </summary>
        public TranslationStringsToolWindow() : base(null)
        {
            this.BitmapImageMoniker = KnownMonikers.Translate;
            this.Caption = "Translation Strings";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new TranslationStringsToolWindowControl();
        }
    }
}
