using FrontEnd.Events;
using FrontEnd.Notifier;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontEnd.Model
{
    /// <summary>
    /// This abstract class implements <see cref="INotifier"/> and provides objects who are not a Model neither a Controller  a way of interacting with the GUI.
    /// </summary>
    public abstract class AbstractNotifier : INotifier
    {
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public void UpdateProperty<T>(ref T value, ref T _backProp, [CallerMemberName] string propName = "")
        {
            BeforeUpdateArgs args = new(value, _backProp, propName);
            BeforeUpdate?.Invoke(this, args);
            if (args.Cancel) return;
            _backProp = value;
            RaisePropertyChanged(propName);
            AfterUpdate?.Invoke(this, args);
        }
    }
}
