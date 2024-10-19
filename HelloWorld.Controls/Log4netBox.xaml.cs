using HelloWorld.Controls.Log4net.Appender;
using HelloWorld.Controls.Notification;
using HelloWorld.Models.Util;
using HelloWorld.Resources.Languages;
using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static HelloWorld.Controls.Log4net.Appender.Log4netBoxAppender;

namespace HelloWorld.Controls
{
    /// <summary>
    /// Log4netBox.xaml 的交互逻辑
    /// </summary>
    public partial class Log4netBox : ListBox
    {
        public class LogsChangedEventArgs : EventArgs
        {
            public ObservableLimitedDeque<ListBoxItem> Old;
            public ObservableLimitedDeque<ListBoxItem> New;

            public LogsChangedEventArgs(ObservableLimitedDeque<ListBoxItem> old, ObservableLimitedDeque<ListBoxItem> @new)
            {
                Old = old;
                New = @new;
            }
        }
        public event Action<LogsChangedEventArgs>? LogsChangedEvent;
        public ObservableLimitedDeque<ListBoxItem> Logs
        {
            get { return (ObservableLimitedDeque<ListBoxItem>)GetValue(LogsProperty); }
            set
            {
                var old = Logs;
                SetValue(LogsProperty, value);
                LogsChangedEvent?.Invoke(new LogsChangedEventArgs(old, Logs));
            }
        }

        // Using a DependencyProperty as the backing store for Logs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LogsProperty =
            DependencyProperty.Register("Logs", typeof(ObservableLimitedDeque<ListBoxItem>), typeof(Log4netBox), new PropertyMetadata(new ObservableLimitedDeque<ListBoxItem>(DEFAULT_MAX_SIZE)));

        protected static readonly int DEFAULT_MAX_SIZE = 1000;

        public int MaxSize
        {
            get { return (int)GetValue(MaxSizeProperty); }
            set { SetValue(MaxSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxSizeProperty =
            DependencyProperty.Register(nameof(MaxSize), typeof(int), typeof(Log4netBox),
                new FrameworkPropertyMetadata(DEFAULT_MAX_SIZE, (d, e) => { ((Log4netBox)d).Logs = new((int)e.NewValue); }));

        public string Appender
        {
            get { return (string)GetValue(AppenderProperty); }
            set { SetValue(AppenderProperty, value); }
        }

        protected static readonly string DEFAULT_APPENDER = "";
        // Using a DependencyProperty as the backing store for Appender.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AppenderProperty =
            DependencyProperty.Register("Appender", typeof(string), typeof(Log4netBox),
                new FrameworkPropertyMetadata(DEFAULT_APPENDER, (d, e) =>
                {
                    ((Log4netBox) d).SetupAppender((string) e.NewValue);
                 }));

        private Decorator? Decorator => VisualTreeHelper.GetChild(this, 0) as Decorator;
        
        private ScrollViewer? _scrollViewer = null;
        private ScrollViewer? ScrollViewer
        {
            get
            {
                _scrollViewer ??= Decorator?.Child as ScrollViewer;
                return _scrollViewer;
            }
        }
        private bool IsScrollViewerAtBottom
        {
            get
            {
                ScrollViewer? scrollHost = ScrollViewer;
                if (scrollHost is null) return false;
                return scrollHost.VerticalOffset >= scrollHost.ScrollableHeight;
            }
        }

        public Log4netBox()
        {
            InitializeComponent();
            _coloredLoggingEventHandler = ColoredLoggingEventHandler;
        }

        public void ScrollToEnd()
        {
            _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                if (IsScrollViewerAtBottom)
                {
                    ScrollViewer?.ScrollToEnd();
                }
            });
        }

        private readonly Action<ColoredLoggingEventArgs> _coloredLoggingEventHandler;
        private Log4netBoxAppender? _lastAppender = null;

        private Log4netBoxAppender? LastAppender
        {
            get { return _lastAppender; }
            set
            {
                if (ReferenceEquals(value, _lastAppender))
                {
                    return;
                }
                if (_lastAppender is not null)
                {
                    _lastAppender.ColoredLoggingEvent -= _coloredLoggingEventHandler;
                }
                _lastAppender = value;
                if (_lastAppender is not null)
                {
                    _lastAppender.ColoredLoggingEvent += _coloredLoggingEventHandler;
                }
             }
        }

        private void SetupAppender(string apppenderName)
        {
            if (apppenderName == DEFAULT_APPENDER) return;
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            foreach (var appender in hierarchy.Root.Appenders)
            {
                if (appender.Name != apppenderName)
                {
                    continue;
                }
                if (appender is Log4netBoxAppender wpfConsoleAppender)
                {
                    LastAppender = wpfConsoleAppender;
                    break;
                }
            }
         }

        private void ColoredLoggingEventHandler(ColoredLoggingEventArgs args)
        {
            _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
                {
                    ListBoxItem item = new();
                    var brush = FindBrush(args.ForeColor);
                    if (brush is not null)
                    {
                        item.Foreground = brush;
                    }
                    brush = FindBrush(args.BackColor);
                    if (brush is not null)
                    {
                        item.Background = brush;
                    }
                    item.Content = args.Log;

                    Logs.AddLast(item);
                }
            );
        }

        private static readonly Dictionary<string, Brush?> BrushCache = new();

        protected static Brush? FindBrush(string? color)
        {
            if (color is null)
            {
                return null;
            }
            if (BrushCache.TryGetValue(color, out Brush? brush))
            {
                return brush;
            }
            var property = typeof(Brushes).GetProperty(color);
            if (typeof(Brush).IsAssignableFrom(property?.PropertyType))
            {
                brush = property?.GetValue(null) as Brush;
            }
            BrushCache.Add(color, brush);
            return brush;
        }

        protected static readonly string CLIPBOARD_TOAST_ID = "set_clipboard";
        private static readonly INotifier _notifier = NotifyManager.GetNotifier(typeof(Log4netBox));

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is MouseButton.Right && SelectedItem is ListBoxItem item)
            {
                Clipboard.SetText(item.Content.ToString());
                _notifier.Show(id: CLIPBOARD_TOAST_ID, message: I18nNotification.Instance.Content_copied_to_clipboard);
            }
        }
    }
}
