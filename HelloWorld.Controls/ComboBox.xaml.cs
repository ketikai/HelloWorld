using System;
using System.Windows;
using System.Windows.Controls;

namespace HelloWorld.Controls
{
    /// <summary>
    /// ComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class ComboBox : System.Windows.Controls.ComboBox
    {
        public string SelectedContent
        {
            get
            {
                return (string)GetValue(SelectedContentProperty);
            }
            set { SetValue(SelectedContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedContentProperty =
            DependencyProperty.Register("SelectedContent", typeof(string), typeof(ComboBox),
                new FrameworkPropertyMetadata("", (d, e) =>
                {
                    if (Equals(e.NewValue, e.OldValue))
                    {
                        return;
                    }
                    ComboBox comboBox = (ComboBox)d;
                    string str = (string)e.NewValue;
                    var items = comboBox.Items;
                    for (int i = 0; i < items.Count; i++)
                    {
                        object item = items[i];
                        if (item is ComboBoxItem comboBoxItem)
                        {
                            if (comboBoxItem.Content is not string content)
                                continue;
                            if (content != str) continue;
                        }
                        else if (item is string content)
                        {
                            if (content != str) continue;
                        }
                        else
                        {
                            continue;
                        }
                        if (comboBox.SelectedIndex != i)
                        {
                            comboBox.SelectedIndex = i;
                        }
                        break;
                    }
                }));

        public ComboBox()
        {
            InitializeComponent();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.Handled)
            {
                return;
            }
            object item = SelectedItem;
            string str;
            if (item is ComboBoxItem comboBoxItem)
            {
                if (comboBoxItem.Content is not string content)
                    return;
                str = content;
            }
            else if (item is string content)
            {
                str = content;
            }
            else
            {
                return;
            }
            if (SelectedContent == str) return;
            SelectedContent = str;
        }
    }
}
