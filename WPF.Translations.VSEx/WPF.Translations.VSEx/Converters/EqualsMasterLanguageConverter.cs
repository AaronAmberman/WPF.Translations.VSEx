using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.Translations.VSEx.Converters
{
    public class EqualsMasterLanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if (DevelopmentEnvironment.Settings == null) return false;

            if (value.ToString().Equals(DevelopmentEnvironment.Settings.MasterLanguage)) return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
