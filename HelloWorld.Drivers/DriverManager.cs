using log4net;
using System.Reflection;

namespace HelloWorld.Drivers
{
    public class DriverManager : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DriverManager));

        public readonly string BaseDirectory;

        private readonly Dictionary<string, IDriver> _drivers = new();

        /// <summary>
        /// A read-only dictionary of all loaded drivers.
        /// </summary>
        public Dictionary<string, IDriver> Drivers
        {
            get { return new(_drivers); }
        }

        /// <summary>
        /// Load all drivers in the given directory.
        /// </summary>
        /// <param name="baseDirectory">
        /// The base directory to load drivers from.
        /// </param>
        public DriverManager(string baseDirectory)
        {
            BaseDirectory = baseDirectory;

            DirectoryInfo directoryInfo = new(baseDirectory);
            var files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                if (!file.Name.EndsWith(".dll"))
                {
                    continue;
                }
                Assembly assembly = Assembly.LoadFile(file.FullName);
                var driverType = assembly.GetType("Driver");
                if (driverType is null)
                {
                    Log.Warn("Driver class not found.");
                    continue;
                }
                if (!typeof(IDriver).IsAssignableFrom(driverType))
                {
                    Log.Error("Driver class must implement IDriver interface.");
                    continue;
                }
                var driver = (IDriver)Activator.CreateInstance(driverType)!;
                var driverInfo = driver.Info;
                _drivers.Add(driverInfo.Name, driver);
                Log.Info($"Loaded driver: {driverInfo.Name}");
            }
        }

        void IDisposable.Dispose()
        {
            try
            {
                foreach (var driver in _drivers.Values)
                {
                    ((IDisposable)driver).Dispose();
                }
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
