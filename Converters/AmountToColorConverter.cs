using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CashFlow.Converters;

public class AmountToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (decimal)value >= 0
            ? new SolidColorBrush(Colors.Green)
            : new SolidColorBrush(Colors.Red);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
