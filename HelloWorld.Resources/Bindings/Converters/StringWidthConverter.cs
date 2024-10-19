using System.Windows;
using System.Windows.Data;

namespace HelloWorld.Resources.Bindings.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StringWidthConverter : IValueConverter
    {

        public object Convert(object? value, Type targetType, object? parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is string str)
            {
                return (str.Length > 0) ? "Auto" : "0";
            }
            throw new InvalidOperationException("The value must be a string");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
