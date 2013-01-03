using System;
using System.Windows.Input;

namespace WpScaffolding.Helpers
{
    public class RelayCommand: ICommand
    {
        Action _execute;
        Func<bool> _canExecute;

        public RelayCommand(Action execute):
            this(execute, null){}

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if(execute == null)
            {
                throw new ArgumentNullException("execute must not be null");
            }

            this._execute = execute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
            CanExecute(null);
        }
    }
}
