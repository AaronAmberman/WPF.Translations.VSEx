using System.Windows;

namespace WPF.Translations.VSEx.Testing
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ServiceLocator.Instance.MainWindowViewModel.Dispatcher = Dispatcher;

            DataContext = ServiceLocator.Instance.MainWindowViewModel;

            InitializeComponent();
        }
    }
}
