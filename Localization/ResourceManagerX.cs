using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Localization
{
    public class ResourceManagerX : ResourceManager
    {
        public static readonly string BaseDirectory = "Localization";
        private static readonly FieldInfo ResourceSetsField = typeof(ResourceManager).GetField("_resourceSets", BindingFlags.NonPublic | BindingFlags.Instance)!;

        private readonly string _libraryName;
        private readonly Type _contextType;
        private Dictionary<string, ResourceSet> LocalResourceSets => (Dictionary<string, ResourceSet>)ResourceSetsField.GetValue(this)!;

        public ResourceManagerX(string libraryName, string baseName, Type contextType)
            : base(baseName, contextType.Assembly)
        {
            _libraryName = libraryName;
            _contextType = contextType;
        }

        protected override ResourceSet? InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            var neutralCulture = culture;
            //var neutralCulture = culture.IsNeutralCulture ? culture : culture.Parent;
            var neutralCultureName = neutralCulture.Name;

            var localResourceSets = LocalResourceSets;
            var path = Path.Combine(AppContext.BaseDirectory, BaseDirectory, neutralCultureName, _libraryName);
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(path);
            }
            catch (FileNotFoundException)
            {
                return base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
            }

            var resourceFileName = $"{_contextType.Name}.{neutralCultureName}.resources";
            var stream = assembly.GetManifestResourceStream(_contextType, resourceFileName);

            if (stream == null) return base.InternalGetResourceSet(culture, createIfNotExists, tryParents);

            var rs = new ResourceSet(stream);
            AddResourceSet(localResourceSets, neutralCultureName, ref rs);

            return rs;
        }

        private static void AddResourceSet(Dictionary<string, ResourceSet> localResourceSets, string cultureName, ref ResourceSet rs)
        {
            lock (localResourceSets)
            {
                if (localResourceSets.TryGetValue(cultureName, out var lostRace))
                {
                    if (ReferenceEquals(lostRace, rs)) return;
                    if (!localResourceSets.ContainsValue(rs))
                        rs.Dispose();
                    rs = lostRace;
                }
                else
                {
                    localResourceSets.Add(cultureName, rs);
                }
            }
        }
    }
}
