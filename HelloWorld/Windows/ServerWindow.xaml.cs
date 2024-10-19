using HelloWorld.Models;
using HelloWorld.Models.Util;
using System;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace HelloWorld.Windows
{
    /// <summary>
    /// ServerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ServerWindow : Window
    {
        public static bool? Show(Window? parentWindow, NamedServer? server, Action<NamedServer> saved)
        {
            return new ServerWindow(parentWindow, server, saved).ShowDialog();
        }

        public ObservableSortedSet<string> DriversSource
        {
            get { return (ObservableSortedSet<string>)GetValue(DriversSourceProperty); }
            set { SetValue(DriversSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DriversSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DriversSourceProperty =
            DependencyProperty.Register("DriversSource", typeof(ObservableSortedSet<string>), typeof(ServerWindow),
                new FrameworkPropertyMetadata(new ObservableSortedSet<string>()));

        public ObservableSortedSet<string> AdaptersSource
        {
            get { return (ObservableSortedSet<string>)GetValue(AdaptersSourceProperty); }
            set { SetValue(AdaptersSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdaptersSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdaptersSourceProperty =
            DependencyProperty.Register("AdaptersSource", typeof(ObservableSortedSet<string>), typeof(ServerWindow),
                new FrameworkPropertyMetadata(new ObservableSortedSet<string>()));

        private readonly Action<NamedServer> _saved;

        private readonly NamedServer _prototype;

        private ServerWindow(Window? parentWindow, NamedServer? server, Action<NamedServer> saved)
        {
            InitializeComponent();

            DriversSource = new(((App) Application.Current).DriverManager.Drivers.Keys);
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var item in networkInterfaces)
            {
                AdaptersSource.Add(item.Name);
            }

            _saved = saved;

            if (server is not null)
            {
                string json = JsonSerializer.Serialize(server);
                _prototype = JsonSerializer.Deserialize<NamedServer>(json)!;
                DataContext = JsonSerializer.Deserialize<NamedServer>(json)!;

                TextLineName.IsEnabled = false;
            }
            else
            {
                _prototype = new();
                DataContext = new NamedServer();
            }

            if (parentWindow != null)
            {
                Owner = parentWindow;
                Left = parentWindow.Left + (parentWindow.Width - Width) / 2;
                Top = parentWindow.Top + (parentWindow.Height - Height) / 2;
                Icon = parentWindow.Icon;
            }

            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object? sender, EventArgs e)
        {
            ButtonSave.IsEnabled = !_prototype.ValuesEquals((NamedServer) DataContext!);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            NamedServer server = (NamedServer) DataContext!;
            _saved.Invoke(server);
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
