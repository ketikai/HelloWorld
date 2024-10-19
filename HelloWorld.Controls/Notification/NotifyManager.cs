using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;
using System;

namespace HelloWorld.Controls.Notification
{
    public static class NotifyManager
    {
        private static readonly Dictionary<string, WindowsToastNotifier> _notifiers = new();

        public static INotifier GetNotifier(Type type)
        {
            return GetNotifier(type.FullName!);
        }

        public static INotifier GetNotifier(string name)
        {
            if (!_notifiers.TryGetValue(name, out var notifier) || notifier is null)
            {
                lock (_notifiers)
                {
                    if (!_notifiers.TryGetValue(name, out notifier) || notifier is null)
                    {
                        notifier = new WindowsToastNotifier(name);
                        _notifiers.Add(name, notifier);
                    }
                }
            }
            return notifier;
        }
    }

    public interface INotifier
    {
        string Name { get; }

        void Show(string? id = null, string title = "", string message = "");

        void Hide(string id);

        void Clear();
    }

    internal sealed class WindowsToastNotifier: INotifier
    {
        private static readonly ToastNotifierCompat Notifier = ToastNotificationManagerCompat.CreateToastNotifier();

        public string Name { get; private set; }

        internal WindowsToastNotifier(string name)
        {
            Name = name;
        }

        public void Show(string? id = null, string title = "", string message = "")
        {
            ToastContentBuilder builder = new();
            builder.AddText(title);
            builder.AddText(message);
            ToastNotification toast = new(builder.GetXml())
            {
                Group = Name
            };
            if (id is not null)
            {
                toast.Tag = id;
                Hide(id);
            }
            Notifier.Show(toast);
        }

        public void Hide(string id)
        {
            ToastNotificationManagerCompat.History.Remove(id, Name);
        }

        public void Clear()
        {
            ToastNotificationManagerCompat.History.RemoveGroup(Name);
        }
    }
}
