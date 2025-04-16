using System.Globalization;

namespace MAUSIC.Converters;

public class BoolToGridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVisible = (bool)value;
        return isVisible ? GridLength.Auto : new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // throw new NotImplementedException(); // Usually one-way binding
        return false;
    }
}