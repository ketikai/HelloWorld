using System.Windows;

namespace HelloWorld.Controls.Events
{

    public delegate void LocaleChangedEventHandler(object sender, LocaleChangedEventArgs e);

    public class LocaleChangedEventArgs : RoutedEventArgs
    {

        public string Locale { get; private set; }

        public LocaleChangedEventArgs(RoutedEvent routedEvent, string locale) : base(routedEvent)
        {
            Locale = locale;
        }
    }
}
