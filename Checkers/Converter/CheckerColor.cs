using System.Globalization;
using System.Windows.Data;

namespace Checkers.Converter;
/* 
   This class feels irrelevant, but since the board is initliased in the constructor, the initial fill of the elipses happens after the board values have been set.
   This means I need to set the initial fill values of the elipses, either by reinitialising the elipses after the constructor or setting the fill using a converter.
   Since I already have the logic for this, I'm fine with this solution
*/
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
