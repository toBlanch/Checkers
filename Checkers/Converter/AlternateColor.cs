using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Checkers.Converter;

public class AlternateColorConverter : IValueConverter
{
    bool isGray = false;
    int index = 0;
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((index > 7))
        {
            index = 0;
            isGray = !isGray;
        }
        index++;
        Brush returnBrush = isGray ? Brushes.Gray : Brushes.Brown;
        isGray = !isGray;
        return returnBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
