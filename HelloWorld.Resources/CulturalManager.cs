using Localization;
using System.Windows;

namespace HelloWorld.Resources
{
    public static class CulturalManager
    {
        public static ICulturalManager Current => (ICulturalManager)Application.Current;
    }

}
