using System.ComponentModel;
using System.Runtime.CompilerServices;

using Orbit.Desktop.Annotations;

namespace Orbit.Desktop
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged<T>(ref T prop, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(prop, newValue))
            {
                prop = newValue;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}