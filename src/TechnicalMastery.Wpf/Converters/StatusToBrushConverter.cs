using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TechnicalMastery.Wpf.Models;

namespace TechnicalMastery.Wpf.Converters;

/// <summary>Maps a StudyStatus to a colored brush for badges.</summary>
public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is StudyStatus status)
        {
            return status switch
            {
                StudyStatus.NotStarted => new SolidColorBrush(Color.FromRgb(0x6C, 0x75, 0x7D)), // gray
                StudyStatus.Learning => new SolidColorBrush(Color.FromRgb(0x0D, 0x6E, 0xFD)),  // blue
                StudyStatus.Completed => new SolidColorBrush(Color.FromRgb(0x19, 0x87, 0x54)), // green
                StudyStatus.NeedsReview => new SolidColorBrush(Color.FromRgb(0xFD, 0x7E, 0x14)), // orange
                _ => new SolidColorBrush(Color.FromRgb(0x6C, 0x75, 0x7D))
            };
        }

        return new SolidColorBrush(Color.FromRgb(0x6C, 0x75, 0x7D));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}