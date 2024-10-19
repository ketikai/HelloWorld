using HelloWorld.Resources.Languages;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HelloWorld.Windows
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBox : Window
    {
        public static int Show(string message, params string[] buttons)
        {
            return Show(null as Window, message, buttons);
        }

        public static int Show(Window? parentWindow, string message, params string[] buttons)
        {
            var messageBox = new MessageBox(parentWindow, message, buttons);
            messageBox.ShowDialog();
            return messageBox.Result;
        }
        public static int Show(string title, string message, params string[] buttons)
        {
            return Show(null, title, message, buttons);
        }

        public static int Show(Window? parentWindow, string title, string message, params string[] buttons)
        {
            var messageBox = new MessageBox(parentWindow, title, message, buttons);
            messageBox.ShowDialog();
            return messageBox.Result;
        }

        public readonly string Message;
        public readonly string[] Buttons;
        public int Result { get; private set; } = -1;

        public MessageBox(string message, params string[] buttons) : this(null as Window, message, buttons) { }

        public MessageBox(Window? parentWindow, string message, params string[] buttons) : this(parentWindow, I18nMessageBox.Instance.Message, message, buttons) { }

        public MessageBox(string title, string message, params string[] buttons) : this(null, title, message, buttons) { }

        public MessageBox(Window? parentWindow, string title, string message, params string[] buttons)
        {
            InitializeComponent();

            Title = title;
            Message = message;
            Buttons = buttons;

            var buttonWidth = 75.0D;
            var buttonHeight = 30.0D;
            var margin = 15.0D;
            int i = 0;
            foreach (var item in buttons)
            {
                MessageBoxButton button = new(i)
                {
                    Content = item,
                    Margin = new Thickness(margin, 0, 0, 0),
                    Width = buttonWidth,
                    Height = buttonHeight
                };
                button.Click += (_, _) =>
                {
                    Result = button.ResultId;
                    Close();
                };
                ButtonPanel.Children.Add(button);
                i++;
            }
            Width = Math.Min(
                Math.Max(
                    (buttonWidth + margin) * i,
                    MinWidth
                    ),
                MaxWidth
                );

            MessageTextBlock.TextWrapping = TextWrapping.NoWrap;
            MessageTextBlock.TextTrimming = TextTrimming.None;
            MessageTextBlock.Text = Message;
            var vSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            MessageTextBlock.Measure(vSize);
            var diff = margin * 2;
            var width = MessageTextBlock.DesiredSize.Width + diff;
            if (width > Width)
            {
                Width = width;
                if (Width > MaxWidth)
                {
                    Width = MaxWidth;
                    MessageTextBlock.Width = Width - diff - 30;
                    MessageTextBlock.TextWrapping = TextWrapping.Wrap;
                    MessageTextBlock.Measure(vSize);
                    diff = margin * 4 + buttonHeight;
                    var height = MessageTextBlock.DesiredSize.Height + diff;
                    if (height > Height)
                    {
                        Height = height;
                        if (Height > MaxHeight)
                        {
                            Height = MaxHeight;
                            MessageTextBlock.Height = Height - diff - 30;
                            MessageTextBlock.VerticalAlignment = VerticalAlignment.Top;
                            MessageTextBlock.TextTrimming = TextTrimming.CharacterEllipsis;
                        }
                    }
                }
            }

            if (parentWindow != null)
            {
                Owner = parentWindow;
                Left = parentWindow.Left + (parentWindow.Width - Width) / 2;
                Top = parentWindow.Top + (parentWindow.Height - Height) / 2;
                Icon = parentWindow.Icon;
            }
        }

        protected override void OnDeactivated(EventArgs e)
        {
            Topmost = false;
            base.OnDeactivated(e);
        }

        private class MessageBoxButton : Button
        {
            public readonly int ResultId;

            public MessageBoxButton(int resultId)
            {
                ResultId = resultId;
            }
        }
    }
}
