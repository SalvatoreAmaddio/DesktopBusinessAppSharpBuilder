using Backend.Utils;
using FrontEnd.Model;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This class represents an asynchronous command that implements <see cref="ICommand"/>.
    /// </summary>
    /// <param name="execute">A function to execute asynchronously.</param>
    /// <param name="onTaskRun">Determines if the task should run on a separate thread.</param>
    public class CMDAsync(Func<Task> execute, bool onTaskRun = false) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Func<Task> task = execute;
        private readonly bool onTaskRun = onTaskRun;

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>True if the command can execute; otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return InternetConnection.IsConnected();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public async void Execute(object? parameter)
        {
            if (onTaskRun) await Task.Run(task);
            else await task.Invoke();
        }
    }

    /// <summary>
    /// This class implements <see cref="ICommand"/> and represents a synchronous command.
    /// </summary>
    /// <param name="execute">An action to execute.</param>
    public class CMD : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action? _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="CMD"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        public CMD(Action execute)
        {
            _execute = execute;
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>True if the command can execute; otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object? parameter)
        {
            _execute?.Invoke();
        }
    }

    /// <summary>
    /// This class implements <see cref="ICommand"/> and represents a generic command with a parameter of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    public class Command<T> : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private readonly Action<T?> _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        public Command(Action<T?> execute) => _execute = execute;

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>True if the command can execute; otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object? parameter)
        {
            _execute?.Invoke((T?)parameter);
        }
    }

    /// <summary>
    /// This class implements <see cref="ICommand"/> and handles click events that take an <see cref="IAbstractModel"/> object as an argument.
    /// Objects of this class are typically used to handle CRUD operations.
    /// </summary>
    /// <typeparam name="M">An <see cref="IAbstractModel"/> object.</typeparam>
    public class CMD<M> : ICommand where M : IAbstractModel, new()
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action<M>? _action;
        private readonly Func<M?, bool>? _fun;

        /// <summary>
        /// Initializes a new instance of the <see cref="CMD{M}"/> class with an action to execute.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        public CMD(Action<M> execute) => _action = execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="CMD{M}"/> class with a function to execute.
        /// </summary>
        /// <param name="fun">The function to execute.</param>
        public CMD(Func<M?, bool> fun) => _fun = fun;

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>True if the command can execute; otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return InternetConnection.IsConnected();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object? parameter)
        {
            if (_action != null)
                _action((M?)parameter);
            if (_fun != null)
                _fun((M?)parameter);
        }
    }

}
