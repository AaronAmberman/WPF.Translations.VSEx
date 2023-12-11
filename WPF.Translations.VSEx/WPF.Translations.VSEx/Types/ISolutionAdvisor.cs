using Microsoft.VisualStudio.Shell.Interop;

namespace WPF.Translations.VSEx.Types
{
    public interface ISolutionAdvisor : IVsSolutionEvents, IVsSolutionEvents4, IDisposable
    {
    }
}
