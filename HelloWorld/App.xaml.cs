using HelloWorld.Drivers;
using HelloWorld.Controls.Utils;
using Localization;
using log4net;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System;
using System.Collections.Generic;

namespace HelloWorld
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ICulturalManager, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(App));
        public readonly DriverManager DriverManager;

        public App()
        {
#if DEBUG
            ConsoleAPI.OpenConsole();
#endif
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            Application.Current.DispatcherUnhandledException += DispatcherUnhandledExceptionEventHandler;
            DriverManager = newDriverManager();
        }

        private static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject;
            if (exception is Exception ex)
            {
                Log.Fatal(ex.Message, ex);
            }
            else
            {
                Log.Fatal(exception.ToString());
            }
            
        }

        private static void DispatcherUnhandledExceptionEventHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            Log.Fatal(exception.Message, exception);
            e.Handled = true;
        }

        private DriverManager newDriverManager()
        {
            var driversDirPath = Path.Combine(AppContext.BaseDirectory, "Drivers");
            var driversDir = new DirectoryInfo(driversDirPath);
            if (!driversDir.Exists)
            {
                driversDir.Create();
            }
            return new DriverManager(driversDirPath);
        }

        private readonly HashSet<ICultural> _culturals = new HashSet<ICultural>();

        public void AddCultural(ICultural cultural)
        {
            _culturals.Add(cultural);
        }

        public ISet<ICultural> GetCulturals()
        {
            return new HashSet<ICultural>(_culturals);
        }

        void IDisposable.Dispose()
        {
            try
            {
                ((IDisposable)DriverManager).Dispose();
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
