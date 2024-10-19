using HelloWorld.Controls.Events;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HelloWorld.Controls
{
    /// <summary>
    /// TextLine.xaml 的交互逻辑
    /// </summary>
    public partial class TextLine : UserControl
    {
        public bool ShowStatus
        {
            get => (bool)GetValue(ShowStatusProperty);
            set => SetValue(ShowStatusProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowStatusProperty =
            DependencyProperty.Register(nameof(ShowStatus), typeof(bool), typeof(TextLine),
                new FrameworkPropertyMetadata(false, (d, e) =>
                {
                    TextLine textLine = (TextLine)d;
                    if (!textLine.ShowStatus)
                    {
                        textLine.HideStatus();
                    }
                }));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(TextLine),
                new FrameworkPropertyMetadata(""));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextLine),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(TextLine),
                new FrameworkPropertyMetadata(0));

        private Regex? _textPattern;

        public string? TextPattern
        {
            get => (string?)GetValue(TextPatternProperty);
            set => SetValue(TextPatternProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextPattern.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextPatternProperty =
            DependencyProperty.Register(nameof(TextPattern), typeof(string), typeof(TextLine),
                new FrameworkPropertyMetadata(
                    null,
                    (d, e) =>
                    {
                        var value = (string)e.NewValue;
                        ((TextLine)d)._textPattern = value == null ? null : new(value);
                    }
                    )
                );

        private Regex? _textInputPattern;

        public string? TextInputPattern
        {
            get => (string?)GetValue(TextInputPatternProperty);
            set => SetValue(TextInputPatternProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextInputPattern.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextInputPatternProperty =
            DependencyProperty.Register(
                nameof(TextInputPattern), typeof(string), typeof(TextLine),
                new FrameworkPropertyMetadata(
                    null,
                    (d, e) =>
                    {
                        var value = (string)e.NewValue;
                        ((TextLine)d)._textInputPattern = value == null ? null : new(value);
                    }
                    )
                );


        public static readonly RoutedEvent TextValidationEvent = EventManager.RegisterRoutedEvent(
            "TextValidation", RoutingStrategy.Direct,
            typeof(TextValidationEventHandler), typeof(TextLine)
            );

        public event TextValidationEventHandler TextValidation
        {
            add => AddHandler(TextValidationEvent, value);
            remove => RemoveHandler(TextValidationEvent, value);
        }

        public TextLine()
        {
            InitializeComponent();
        }

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            DisplayStatus();
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            HideStatus();
        }

        private void Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space || (_textInputPattern?.IsMatch(" ") ?? true)) return;
            e.Handled = true;
            SystemSounds.Hand.Play();
        }

        private void Input_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (_textInputPattern?.IsMatch(e.Text) ?? true) return;
            e.Handled = true;
            SystemSounds.Hand.Play();
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateStatus();
        }

        private void DisplayStatus()
        {
            if (!ShowStatus)
            {
                return;
            }
            UpdateStatus();
            Status.IsEnabled = true;
            Status.Visibility = Visibility.Visible;
            Status.Width = Status.Height = ActualHeight;
        }

        private void UpdateStatus()
        {
            var isValid = _textPattern?.IsMatch(Input.Text) ?? true;
            if (isValid)
            {
                var validationEventArgs = new TextValidationEventArgs(TextValidationEvent, Input.Text);
                RaiseEvent(validationEventArgs);
                isValid = !validationEventArgs.Handled;
            }
            Status.BorderBrush = Input.BorderBrush;
            Status.BorderThickness = Input.BorderThickness;
            Status.Background = isValid ? Brushes.Green : Brushes.Red;
        }

        private void HideStatus()
        {
            Status.IsEnabled = false;
            Status.Visibility = Visibility.Hidden;
            Status.Width = Status.Height = 0;
            Status.Background = Brushes.Transparent;
        }
    }
}
