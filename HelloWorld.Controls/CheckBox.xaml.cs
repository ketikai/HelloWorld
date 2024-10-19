using HelloWorld.Controls.Events;
using System.Windows;

namespace HelloWorld.Controls
{
    /// <summary>
    /// CheckBox.xaml 的交互逻辑
    /// </summary>
    public partial class CheckBox : System.Windows.Controls.CheckBox
    {

        public static readonly RoutedEvent CheckChangedEvent = EventManager.RegisterRoutedEvent(
            "CheckChanged", RoutingStrategy.Direct,
            typeof(CheckChangedEventHandler), typeof(CheckBox)
            );

        public event CheckChangedEventHandler CheckChanged
        {
            add => AddHandler(CheckChangedEvent, value);
            remove => RemoveHandler(CheckChangedEvent, value);
        }

        public bool? IsUnchecked
        {
            get => (bool?)GetValue(IsUncheckedProperty);
            set => SetValue(IsUncheckedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsUnchecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUncheckedProperty =
            DependencyProperty.Register(nameof(IsUnchecked), typeof(bool?), typeof(CheckBox),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        if (e.OldValue == e.NewValue)
                        {
                            return;
                        }
                        var checkBox = (CheckBox)d;
                        var newValue = (bool?)e.NewValue;
                        checkBox.IsChecked = !newValue;
                    })
                );
        static CheckBox()
        {
            IsCheckedProperty.OverrideMetadata(typeof(CheckBox),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, e) =>
                {
                    if (e.OldValue == e.NewValue)
                    {
                        return;
                    }
                    var checkBox = (CheckBox)d;
                    var newValue = (bool?)e.NewValue;
                    checkBox.IsUnchecked = !newValue;
                })
                );
        }

        public CheckBox()
        {
            InitializeComponent();
        }

        private void Check_Checked(object sender, RoutedEventArgs e)
        {
            var checkChangedEventArgs = new CheckChangedEventArgs(CheckChangedEvent, true);
            RaiseEvent(checkChangedEventArgs);
            e.Handled = checkChangedEventArgs.Handled;
        }

        private void Check_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkChangedEventArgs = new CheckChangedEventArgs(CheckChangedEvent, false);
            RaiseEvent(checkChangedEventArgs);
            e.Handled = checkChangedEventArgs.Handled;
        }
    }
}
