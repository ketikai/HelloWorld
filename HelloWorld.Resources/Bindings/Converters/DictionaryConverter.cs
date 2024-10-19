using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Globalization;
using HelloWorld.Models;
using System.Collections.ObjectModel;

namespace HelloWorld.Resources.Bindings.Converters
{
    [ValueConversion(typeof(IDictionary<string, object>), typeof(ICollection<string>))]
    public class DictionaryConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not IDictionary dictionary)
                throw new ArgumentException("The value must be a IDictionary");
            var collection = new List<string>();
            foreach (var key in dictionary.Keys)
            {
                if (key is not string str)
                {
                    throw new ArgumentException("The key must be a string");
                }
                collection.Add(str);
            }

            ObservableCollection<string> ret = new();
            foreach (var item in collection)
            {
                ret.Add(item);
            }
            return ret;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
