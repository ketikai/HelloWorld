using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HelloWorld.Models.Util
{
    public abstract class NotifyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Dictionary<NotifyObject, string?> _parents = new();

        internal void AddParent(NotifyObject parent, [CallerMemberName] string? propertyName = null)
        {
            _parents.Add(parent, propertyName);
        }

        internal virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (_parents.Count == 0)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                foreach (var parent in _parents)
                {
                    var parentPropertyName = parent.Value;
                    if (parentPropertyName is null || propertyName is null)
                    {
                        parent.Key.OnPropertyChanged(null);
                    }
                    else
                    {
                        parent.Key.OnPropertyChanged($"{parentPropertyName}.{propertyName}");
                    }
                }
            }
        }
    }
}
