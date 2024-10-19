using System.Globalization;

namespace Localization
{
    public interface ICulturalManager
    {
        void AddCultural(ICultural cultural);

        ISet<ICultural> GetCulturals();

        void SetCulture(string cultureName)
        {
            SetCulture(CultureInfo.GetCultureInfo(cultureName));
        }

        void SetCulture(CultureInfo? culture)
        {
            if (culture != null)
            {
                var currentThread = Thread.CurrentThread;
                currentThread.CurrentCulture = culture;
                currentThread.CurrentUICulture = culture;
            }
            foreach (var cultural in GetCulturals())
            {
                if (culture == null)
                {
                    continue;
                }
                cultural.Culture = culture;
            }
        }

    }
}
