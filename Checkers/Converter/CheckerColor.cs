using System.Globalization;
using System.Windows.Data;

namespace Checkers.Converter;

class CheckerColor : IValueConverter
{
    public object Convert(object value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        return value switch
        {
            'b' => "Black",
            'r' => "Red",
            'w' => "White",
            _ => "Transparent",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
