using System;
using System.Windows.Input;

namespace WpScaffolding.Helpers
{
    public class RelayCommand<T>: ICommand where T: class
    {
        Action<T> _execute;
        Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute):
            this(execute, null){}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
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
			T param = parameter as T;
            return _canExecute == null ? true : _canExecute(param);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute((T)parameter);
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
