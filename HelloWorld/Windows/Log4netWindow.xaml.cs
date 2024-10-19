using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static HelloWorld.Controls.Log4netBox;

namespace HelloWorld.Windows
{
    /// <summary>
    /// Log4netWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Log4netWindow : Window
    {
        private readonly NotifyCollectionChangedEventHandler _logsCollectionChangedEventHandler;
        public Log4netWindow(Window? parentWindow)
        {
            InitializeComponent();

            _logsCollectionChangedEventHandler = LogsCollectionChangedEventHandler;
            LogsBox.Logs.CollectionChanged += _logsCollectionChangedEventHandler;

            LogsBox.LogsChangedEvent += LogsChangedEventHandler;

            if (parentWindow != null)
            {
                Owner = parentWindow;
                Left = parentWindow.Left + parentWindow.Width;
                Top = parentWindow.Top;
                Width = parentWindow.Width * 2;
                Height = parentWindow.Height;
                Icon = parentWindow.Icon;

                parentWindow.LocationChanged += (sender, e) =>
                {
                    Left = parentWindow.Left + parentWindow.Width;
                    Top = parentWindow.Top;
                };
            }
        }

        private void LogsCollectionChangedEventHandler(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                LogsBox.ScrollToEnd();
            }
        }

        private void LogsChangedEventHandler(LogsChangedEventArgs e)
        {
            e.Old.CollectionChanged -= _logsCollectionChangedEventHandler;
            e.New.CollectionChanged += _logsCollectionChangedEventHandler;
        }
    }
}
