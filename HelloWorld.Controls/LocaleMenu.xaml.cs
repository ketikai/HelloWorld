using HelloWorld.Controls.Events;
using HelloWorld.Resources;
using HelloWorld.Resources.Languages;
using Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HelloWorld.Controls
{
    /// <summary>
    /// LocaleMenu.xaml 的交互逻辑
    /// </summary>
    public partial class LocaleMenu : MenuItem
    {

        public static readonly RoutedEvent LocaleChangedEvent = EventManager.RegisterRoutedEvent(
            "LocaleChanged", RoutingStrategy.Direct,
            typeof(LocaleChangedEventHandler), typeof(LocaleMenu)
            );

        public event LocaleChangedEventHandler LocaleChanged
        {
            add => AddHandler(LocaleChangedEvent, value);
            remove => RemoveHandler(LocaleChangedEvent, value);
        }

        public string Locale
        {
            get { return (string)GetValue(LocaleProperty); }
            set { SetValue(LocaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Locale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LocaleProperty =
            DependencyProperty.Register("Locale", typeof(string), typeof(LocaleMenu),
                new FrameworkPropertyMetadata(Thread.CurrentThread.CurrentUICulture.Name, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
                {
                    if (e.NewValue == e.OldValue)
                    {
                        return;
                    }
                    LocaleMenu menu = (LocaleMenu)d;
                    string locale =  (string) e.NewValue;
                    if (menu._localeItems.TryGetValue(locale, out var item))
                    {
                        menu.ChangeLocale(locale, item);
                        return;
                    }
                    locale = Thread.CurrentThread.CurrentUICulture.Name;
                    if (menu._localeItems.TryGetValue(locale, out item))
                    {
                        menu.ChangeLocale(locale, item);
                    }
                }));

        private readonly Dictionary<string, MenuItem> _localeItems = new();

        public LocaleMenu()
        {
            InitializeComponent();
            SetBinding(HeaderProperty, new Binding("Language_Or_Locale")
            {
                Source = I18nMenu.Instance,
                Mode = BindingMode.OneWay,
            });
            var resDir = new DirectoryInfo($"{AppContext.BaseDirectory}/{ResourceManagerX.BaseDirectory}");
            var localeResDirs = resDir.GetDirectories();
            foreach (var localeResDir in localeResDirs)
            {
                string locale = localeResDir.Name;
                var localeItem = new MenuItem();
                localeItem.SetBinding(
                    HeaderProperty,
                    new Binding(locale.Replace("-", "_"))
                    {
                        Source = I18nLocale.Instance,
                        Mode = BindingMode.OneWay,
                        Converter = new CultureInfoToDisplayNameConverter(locale)
                    }
                    );
                localeItem.Checked += (sender, e) =>
                {
                    ChangeLocale(locale, localeItem, true);
                };
                localeItem.IsChecked = false;
                localeItem.IsCheckable = true;
                _localeItems.Add(locale, localeItem);
                Items.Add(localeItem);
            }
            string initLocale = Thread.CurrentThread.CurrentUICulture.Name;
            if (_localeItems.TryGetValue(initLocale, out var item))
            {
                ChangeLocale(initLocale, item, Locale == "");
            }
        }

        private void ChangeLocale(string locale, MenuItem currentItem, bool setProperty = false)
        {
            foreach (var item in Items)
            {
                if (currentItem.Equals(item))
                {
                    continue;
                }
                if (item is MenuItem menuItem)
                {
                    menuItem.IsCheckable = true;
                    menuItem.IsChecked = false;
                }
            }
            CulturalManager.Current.SetCulture(locale);
            currentItem.IsCheckable = false;
            currentItem.IsChecked = true;
            if (setProperty)
            {
                Locale = locale;
            }
            var localeChangedEventArgs = new LocaleChangedEventArgs(LocaleChangedEvent, locale);
            RaiseEvent(localeChangedEventArgs);
        }
    }
    
    [ValueConversion(typeof(string), typeof(string))]
    internal class CultureInfoToDisplayNameConverter : IValueConverter
    {
        private readonly string _fallback;

        public CultureInfoToDisplayNameConverter(string fallback)
        {
            _fallback = fallback;
        }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not null && value is not string)
                throw new InvalidOperationException("The value must be a string or null.");
            string str = (string)(value ?? "");

            return str != "" ? str : _fallback;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
