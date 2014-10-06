using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GoogleServicesClient.Command
{
    /// <summary>
    /// DelegateCommand class for executing commands
    /// through delegates.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region: Fields
        //Parameterless command delegate
        Action _execute;
        //Parameterized command delegate
        Action<object> _parameterizedExecute;
        Func<bool> _canExecute;
        #endregion

        #region: Constructors
        //Parameterless delegate constructor (canExecute is optional)
        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        //Parameterized delegate constructor (canExecute is optional)
        public DelegateCommand(Action<object> parameterizedExecute, Func<bool> canExecute = null)
        {
            _parameterizedExecute = parameterizedExecute;
            _canExecute = canExecute;
        }
        #endregion

        #region: ICommand Members

        #region: Event
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }
        #endregion

        #region: Methods
        public bool CanExecute(object parameter)
        {
            //Default to be true
            if (_canExecute == null)
                return true;
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            //Invoking the parameterless delegate
            if (parameter == null)
                _execute();
            else //Invoking parameterized delegate
                _parameterizedExecute(parameter);
        }
        #endregion

        #endregion
    }
}
