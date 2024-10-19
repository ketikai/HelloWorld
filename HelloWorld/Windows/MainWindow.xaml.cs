using HelloWorld.Controls.Events;
using HelloWorld.Drivers;
using HelloWorld.Models;
using HelloWorld.Resources.Languages;
using HelloWorld.Controls.Utils;
using log4net;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

using static System.Drawing.Icon;
using HelloWorld.Controls.Forms;
using System;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using System.Windows.Data;

namespace HelloWorld.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindow));

        private Configuration _configuration;

        public MainWindow()
        {
            InitializeComponent();

            _configuration = LoadConfig();
            _logsWindow = new(() =>
            {
                Log4netWindow window = new(this);
                window.Closing += (sender, args) =>
                {
                    window.Hide();
                    args.Cancel = true;
                    LogsMenuItem.IsChecked = false;
                };
                return window;
            });
            ConsoleAPI.StateChangedEvent += ChangeConsoleState;
            ConsoleMenuItem.IsChecked = ConsoleAPI.IsOpened;

            SystemTray.CreateSystemTray(this);
        }

        private Configuration LoadConfig()
        {
            var configsDirPath = Path.Combine(AppContext.BaseDirectory, "Configs");
            var configsDir = new DirectoryInfo(configsDirPath);
            if (!configsDir.Exists)
            {
                configsDir.Create();
            }
            var configFilePath = Path.Combine(configsDirPath, "Config.json");
            var configFile = new FileInfo(configFilePath);
            Configuration? configuration = null;
            if (configFile.Exists)
            {
                var json = File.ReadAllText(configFilePath);
                try
                {
                    configuration = JsonSerializer.Deserialize<Configuration>(json);
                }
                catch (JsonException e)
                {
                    Log.Error(e.Message, e);
                }
            }
            Configuration ret = configuration ?? new();
            DataContext = ret;
            return ret;
        }

        private void SaveConfig(Configuration configuration)
        {
            var configsDirPath = Path.Combine(AppContext.BaseDirectory, "Configs");
            var configsDir = new DirectoryInfo(configsDirPath);
            if (!configsDir.Exists)
            {
                configsDir.Create();
            }
            var configFilePath = Path.Combine(configsDirPath, "Config.json");
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(configuration, options);
            File.WriteAllText(configFilePath, json);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            Topmost = false;
            base.OnDeactivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var result = MessageBox.Show(
                this,
                I18nMessageBox.Instance.Exit,
                I18nMessageBox.Instance.Do_you_want_to_exit_,
                I18nMessageBox.Instance.Yes,
                I18nMessageBox.Instance.No
                );
            if (result != 0)
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveConfig(_configuration);
            base.OnClosed(e);
        }

        private void VirtualIPAddressValidator(object sender, TextValidationEventArgs e)
        {
            e.Handled = !Utils.Validation.IsIPAddress(e.Text);
        }

        private void OpenInformationWindow(object sender, RoutedEventArgs e)
        {

        }

        private void SetIsEnabled(bool isEnabled)
        {
            MenuToolsBar.IsEnabled = isEnabled;
            GroupBoxServerList.IsEnabled = isEnabled;
            GroupBoxGeneralConfiguration.IsEnabled = isEnabled;
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            SetIsEnabled(false);
            if (!_configuration.ServerList.List.TryGetValue(ComboBoxServerList.SelectedContent, out Server? server) || server is null)
            {
                return;
            }
            if (!((App) Application.Current).DriverManager.Drivers.TryGetValue(server.Driver, out IDriver? driver) || driver is null)
            {
                return;
            }
            ButtonConnect.IsEnabled = false;
            ButtonConnect.Click -= Connect;
            ButtonConnect.Click += Disconnect;
            ButtonConnect.SetBinding(Button.ContentProperty, new Binding("Disconnect")
            {
                Source = I18nMainWindow.Instance,
                Mode = BindingMode.OneWay
            });

            try
            {
                driver.Connect(server);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            finally
            {
                ButtonConnect.IsEnabled = true;
            }
        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            if (!_configuration.ServerList.List.TryGetValue(ComboBoxServerList.SelectedContent, out Server? server) || server is null)
            {
                return;
            }
            if (!((App) Application.Current).DriverManager.Drivers.TryGetValue(server.Driver, out IDriver? driver) || driver is null)
            {
                return;
            }
            ButtonConnect.IsEnabled = false;
            ButtonConnect.Click -= Disconnect;
            ButtonConnect.Click += Connect;
            ButtonConnect.SetBinding(Button.ContentProperty, new Binding("Connect")
            {
                Source = I18nMainWindow.Instance,
                Mode = BindingMode.OneWay
            });

            try
            {
                driver.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            finally
            {
                ButtonConnect.IsEnabled = true;
                SetIsEnabled(true);
            }
        }

        private readonly Lazy<Log4netWindow> _logsWindow;

        private Log4netWindow LogsWindow => _logsWindow.Value;

        private void OpenLogsWindow(object sender, RoutedEventArgs e)
        {
            if (LogsMenuItem.IsChecked || LogsWindow.IsVisible)
            {
                LogsWindow.Hide();
                LogsMenuItem.IsChecked = false;
            }
            else
            {
                LogsMenuItem.IsChecked = true;
                LogsWindow.Show();
            }
        }

        private void OpenConsole(object sender, RoutedEventArgs e)
        {
            ChangeConsoleState(!ConsoleMenuItem.IsChecked);
        }

        private void ChangeConsoleState(bool state)
        {
            if (state)
            {
                ConsoleAPI.OpenConsole();
            }
            else
            {
                ConsoleAPI.CloseConsole();
            }
            ConsoleMenuItem.IsChecked = state;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UpdateSever(NamedServer server)
        {
            var dictionary = _configuration.ServerList.List;
            string name = server.Name;
            if (dictionary.ContainsKey(name))
            {
                dictionary.Remove(name);
            }
            dictionary.Add(name, server.ToServer());
            NotifyServerListChanged();
        }

        private void NewServer(object sender, RoutedEventArgs e)
        {
            ServerWindow.Show(this, null, UpdateSever);
        }

        private static string? OpenFolderBrowserDialog()
        {
            string? selectedPath = null;
            using (System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new()
            {
                SelectedPath = AppContext.BaseDirectory
            })
            {
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedPath = folderBrowserDialog.SelectedPath;
                }
            }
            return selectedPath;
        }

        private static string[]? OpenFileBrowserDialog()
        {
            string[]? selectedFilePaths = null;
            using (System.Windows.Forms.OpenFileDialog fileBrowserDialog = new()
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "JSON (*.json)|*.json",
                RestoreDirectory = true,
                Multiselect = true
            })
            {
                if (fileBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedFilePaths = fileBrowserDialog.FileNames;
                }
            }
            return selectedFilePaths;
        }

        private void ImportServers(object sender, RoutedEventArgs e)
        {
            var selectedFilePaths = OpenFileBrowserDialog();
            if (selectedFilePaths is not null)
            {
                var dictionary = _configuration.ServerList.List;
                foreach (string selectedFilePath in selectedFilePaths)
                {
                    string json = File.ReadAllText(selectedFilePath);
                    Dictionary<string, Server> servers;
                    try
                    {
                        servers = JsonSerializer.Deserialize<Dictionary<string, Server>>(json)!;
                    }
                    catch (JsonException ex)
                    {
                        Log.Error(ex.Message, ex);
                        continue;
                    }
                    foreach (var server in servers)
                    {
                        string name = server.Key;
                        if (dictionary.ContainsKey(name))
                        {
                            dictionary.Remove(name);
                        }
                        dictionary.Add(name, server.Value);
                    }
                }
                NotifyServerListChanged();
            }
        }

        private void ExportServers(object sender, RoutedEventArgs e)
        {
            var selectedPath = OpenFolderBrowserDialog();
            if (selectedPath is not null)
            {
                DateTime now = DateTime.Now;
                string formattedDate = now.ToString("yyyy-MM-dd");
                long milliseconds = now.Ticks / TimeSpan.TicksPerMillisecond;
                string fileName = $"Servers-{formattedDate}-{milliseconds}.json";
                string path = Path.Combine(selectedPath, fileName);

                JsonSerializerOptions options = new()
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string json = JsonSerializer.Serialize(_configuration.ServerList.List, options);

                File.WriteAllText(path, json);
            }
        }

        private void ModifyServer(object sender, RoutedEventArgs e)
        {
            string name = ComboBoxServerList.SelectedContent;
            if (_configuration.ServerList.List.TryGetValue(name, out var server))
            {
                if (server is not null)
                {
                    ServerWindow.Show(this, new NamedServer(server)
                    {
                        Name = name
                    }, UpdateSever);
                }
            }
        }

        private void DeleteServer(object sender, RoutedEventArgs e)
        {
            _configuration.ServerList.List.Remove(ComboBoxServerList.SelectedContent);
            NotifyServerListChanged();
        }

        private void NotifyServerListChanged()
        {
            _configuration.ServerList.List = _configuration.ServerList.List;
        }
    }
}