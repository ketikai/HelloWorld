using System.Windows;

namespace HelloWorld.Controls.Events
{

    public delegate void CheckChangedEventHandler(object sender, CheckChangedEventArgs e);

    public class CheckChangedEventArgs : RoutedEventArgs
    {

        public bool IsChecked { get; private set; }

        public CheckChangedEventArgs(RoutedEvent routedEvent, bool isChecked) : base(routedEvent)
        {
            IsChecked = isChecked;
        }
    }
}
