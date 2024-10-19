using System.Windows;
using System.Windows.Data;

namespace HelloWorld.Resources.Bindings.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class LengthToVisibilityConverter : IValueConverter
    {

        public object Convert(object? value, Type targetType, object? parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibility");
            if (value is string str)
            {
                return (str.Length > 0) ? Visibility.Visible : Visibility.Hidden;
            }
            if (value is int v)
            {
                return (v > 0) ? Visibility.Visible : Visibility.Hidden;
            }
            throw new InvalidOperationException("The value must be an integer");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
