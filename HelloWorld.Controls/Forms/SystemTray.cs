using HelloWorld.Controls.Notification;
using HelloWorld.Resources.Languages;
using System;
using System.Windows;
using System.Windows.Forms;
using static System.Drawing.Icon;

namespace HelloWorld.Controls.Forms
{
    public partial class SystemTray
    {
        private static readonly INotifier Notifier = NotifyManager.GetNotifier(typeof(SystemTray));
        private static readonly string SYSTEM_TRAY_TOAST_ID = "SystemTray";
        public static NotifyIcon CreateSystemTray(System.Windows.Window window)
        {
            ContextMenuStrip contextMenuStrip = new();
            contextMenuStrip.Items.Add(new ToolStripMenuItemShow(window));
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(new ToolStripMenuItemExit(window));

            NotifyIcon notifyIcon = new()
            {
                Text = window.Title,
                Icon = ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath),
                Visible = window.WindowState == WindowState.Minimized,
                ContextMenuStrip = contextMenuStrip
            };
            
            notifyIcon.MouseDoubleClick += new MouseEventHandler((sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    window.WindowState = WindowState.Normal;
                    window.ShowInTaskbar = true;
                    window.Show();
                    window.Activate();
                }
            });

            window.StateChanged += (sender, args) =>
            {
                if (window.WindowState == WindowState.Minimized)
                {
                    Notifier.Show(id: SYSTEM_TRAY_TOAST_ID, message: I18nSystemTray.Instance.Minimize_to_the_system_tray);
                    notifyIcon.Visible = true;
                    window.ShowInTaskbar = false;
                }
                else
                {
                    notifyIcon.Visible = false;
                    window.ShowInTaskbar = true;
                }
            };

            return notifyIcon;
        }

        private partial class ToolStripMenuItemShow : ToolStripMenuItem
        {
            private readonly System.Windows.Window _window;
            public bool IsShowed => _window.WindowState != WindowState.Minimized;
            public string Title => IsShowed ? I18nSystemTray.Instance.Hide : I18nSystemTray.Instance.Show;

            public ToolStripMenuItemShow(System.Windows.Window window)
            {
                _window = window;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Text = Title;
                base.OnPaint(e);
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                if (IsShowed)
                {
                    _window.Hide();
                    _window.WindowState = WindowState.Minimized;
                    _window.ShowInTaskbar = false;
                }
                else
                {
                    _window.WindowState = WindowState.Normal;
                    _window.ShowInTaskbar = true;
                    _window.Show();
                    _window.Activate();
                }
            }
        }

        private partial class ToolStripMenuItemExit : ToolStripMenuItem
        {
            private readonly System.Windows.Window _window;
            public string Title => I18nSystemTray.Instance.Exit;

            public ToolStripMenuItemExit(System.Windows.Window window)
            {
                _window = window;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Text = Title;
                base.OnPaint(e);
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                _window.Close();
            }
        }
    }
}
