using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Globalization;
using HelloWorld.Models;
using System.Collections.ObjectModel;

namespace HelloWorld.Resources.Bindings.Converters
{
    [ValueConversion(typeof(ICollection<INamed>), typeof(ICollection<string>))]
    public class NamedCollectionConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not IEnumerable enumerable)
                throw new ArgumentException("The value must be a IEnumerable");
            var enumerator = enumerable.GetEnumerator();
            ICollection<INamed> collection = new List<INamed>();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is not INamed named)
                {
                    throw new ArgumentException("The value must be a INamed collection");
                }
                collection.Add(named);
            }

            ObservableCollection<string> ret = new();
            foreach (var item in collection)
            {
                ret.Add(item.Name);
            }
            return ret;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
