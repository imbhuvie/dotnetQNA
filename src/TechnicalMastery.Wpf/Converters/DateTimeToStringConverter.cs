using System.Globalization;
using System.Windows.Data;

namespace TechnicalMastery.Wpf.Converters;

/// <summary>Formats a DateTime as "yyyy-MM-dd HH:mm".</summary>
public class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm");
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}