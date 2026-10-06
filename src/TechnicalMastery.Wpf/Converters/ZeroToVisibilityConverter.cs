using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TechnicalMastery.Wpf.Converters;

/// <summary>Value is zero → Visible; otherwise Collapsed.</summary>
public class ZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}