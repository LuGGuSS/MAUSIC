using System.Globalization;
using System.Resources;

namespace MAUSIC.Converters;

public class BoolToImageSourceConverter : IValueConverter
{
    public string TrueSource { get; set; }

    public string FalseSource { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var boolValue = (bool)value;

        return boolValue ? TrueSource : FalseSource;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // throw new NotImplementedException(); // Usually one-way binding
        return false;
    }
}