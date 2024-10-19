using System.Windows;

namespace HelloWorld.Controls.Events
{

    public delegate void TextValidationEventHandler(object sender, TextValidationEventArgs e);

    public class TextValidationEventArgs : RoutedEventArgs
    {

        public string Text { get; private set; }

        public TextValidationEventArgs(RoutedEvent routedEvent, string text) : base(routedEvent)
        {
            Text = text;
        }
    }
}
