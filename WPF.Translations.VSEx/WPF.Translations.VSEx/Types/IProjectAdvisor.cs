using Microsoft.VisualStudio.Shell.Interop;

namespace WPF.Translations.VSEx.Types
{
    public interface IProjectAdvisor : IVsTrackProjectDocumentsEvents2, IDisposable
    {
    }
}
