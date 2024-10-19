using System.Globalization;

namespace Localization
{
    public interface ICultural
    {
        CultureInfo? Culture { get; set; }
    }
}
