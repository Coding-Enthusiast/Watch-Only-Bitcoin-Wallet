using System;
using System.Windows.Input;

namespace MVVMLibrary
{
    public class BindableCommand<T> : ICommand
    {
        public BindableCommand(Action<T> parameterizedAction)
        {
            methodToExecute = parameterizedAction;
        }
        public BindableCommand(Action<T> parameterizedAction, Func<T, bool> canExecute)
        {
            methodToExecute = parameterizedAction;
            canExecuteMethod = canExecute;
        }


        private Action<T> methodToExecute;
        private Func<T, bool> canExecuteMethod;


        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }


        #region ICommand

        public bool CanExecute(object parameter)
        {
            if (canExecuteMethod != null)
            {
                return canExecuteMethod((T)parameter);
            }
            if (methodToExecute != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (methodToExecute != null)
            {
                methodToExecute((T)parameter);
            }
        }

        #endregion
    }



    public class BindableCommand : ICommand
    {
        public BindableCommand(Action actionToExecute)
        {
            methodToExecute = actionToExecute;
        }
        public BindableCommand(Action actionToExecute, Func<bool> canExecute)
        {
            methodToExecute = actionToExecute;
            canExecuteMethod = canExecute;
        }


        private Action methodToExecute;
        private Func<bool> canExecuteMethod;


        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }


        #region ICommand

        public bool CanExecute(object parameter)
        {
            if (canExecuteMethod != null)
            {
                return canExecuteMethod();
            }
            if (methodToExecute != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (methodToExecute != null)
            {
                methodToExecute();
            }
        }

        #endregion
    }
}
