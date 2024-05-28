using Backend.Utils;
using FrontEnd.Model;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    public class CMDAsync(Func<Task> execute, bool onTaskRun = false) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Func<Task> task = execute;
        private readonly bool onTaskRun = onTaskRun;
        public bool CanExecute(object? parameter)
        {
            return InternetConnection.IsConnected();
        }

        public async void Execute(object? parameter) 
        {
            if (onTaskRun) await Task.Run(task);
            else await task.Invoke();
        } 

    }
    /// <summary>
    /// This class implements <see cref="ICommand"/>
    /// </summary>
    /// <param name="execute">An Action</param>
    public class CMD(Action execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action _execute = execute;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter) 
        {
            _execute();
        }
    }

    /// <summary>
    /// This class implements <see cref="ICommand"/> and deal with click events that take a <see cref="AbstractModel"/> object as argument.
    /// Objects of this class are tipically used to deal with CRUD operations.
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public class CMD<M> : ICommand where M : AbstractModel, new()
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action<M?>? _action;
        private readonly Func<M?, bool>? _fun;

        public CMD(Action<M?> execute) => _action = execute;    

        public CMD(Func<M?, bool> fun) => _fun = fun;

        public bool CanExecute(object? parameter)
        {
            return InternetConnection.IsConnected();
        }

        public void Execute(object? parameter)
        {
            if (_action != null)
                _action((M?)parameter);
            if (_fun != null)
                _fun((M?)parameter);

//            Window? win = Helper.GetActiveWindow();
//            RecordStatus? recordStatus = Helper.FindFirstChildOfType<RecordStatus>(win);

        }
    }
}
