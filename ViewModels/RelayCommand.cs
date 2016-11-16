using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace ViewModels
{
    /// <summary>
    /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
    /// </summary>
    public class RelayCommand : ICommand
    {

        Action _TargetExecuteMethod;
        Func<bool> _TargetCanExecuteMethod;

        public RelayCommand(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
        #region ICommand Members

        bool ICommand.CanExecute(object parameter)
        {
            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod();
            }
            if (_TargetExecuteMethod != null)
            {
                return true;
            }
            return false;
        }

        // Beware - should use weak references if command instance lifetime is longer than lifetime of UI objects that get hooked up to command
        // Prism commands solve this in their implementation
        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod();
            }
        }
        #endregion







        //private Action<object> execute;
        //private Predicate<object> canExecute;
        //private event EventHandler CanExecuteChangedInternal;

        //public RelayCommand(Action<object> executeAction)
        //    : this(executeAction, DefaultCanExecute)
        //{
        //}

        //public RelayCommand(Action<object> executeAction, Predicate<object> canExecutePredicate)
        //{
        //    if (executeAction == null)
        //    {
        //        throw new ArgumentNullException("execute");
        //    }

        //    if (canExecutePredicate == null)
        //    {
        //        throw new ArgumentNullException("canExecute");
        //    }

        //    this.execute = executeAction;
        //    this.canExecute = canExecutePredicate;
        //}

        //public event EventHandler CanExecuteChanged
        //{
        //    add
        //    {
        //        CommandManager.RequerySuggested += value;
        //        this.CanExecuteChangedInternal += value;
        //    }

        //    remove
        //    {
        //        CommandManager.RequerySuggested -= value;
        //        this.CanExecuteChangedInternal -= value;
        //    }
        //}

        //public bool CanExecute(object parameter)
        //{
        //    return this.canExecute != null && this.canExecute(parameter);
        //}

        //public void Execute(object parameter)
        //{
        //    this.execute(parameter);
        //}

        //public void OnCanExecuteChanged()
        //{
        //    EventHandler handler = this.CanExecuteChangedInternal;
        //    if (handler != null)
        //    {
        //        //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
        //        handler.Invoke(this, EventArgs.Empty);
        //    }
        //}

        //public void Destroy()
        //{
        //    this.canExecute = _ => false;
        //    this.execute = _ => { return; };
        //}

        //private static bool DefaultCanExecute(object parameter)
        //{
        //    return true;
        //}







        //#region Fields

        //readonly Action<object> _execute;
        //readonly Predicate<object> _canExecute;

        //#endregion // Fields

        //#region Constructors

        //public RelayCommand(Action<object> execute)
        //    : this(execute, null)
        //{
        //}

        //public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        //{
        //    if (execute == null)
        //        throw new ArgumentNullException("execute");

        //    _execute = execute;
        //    _canExecute = canExecute;
        //}
        //#endregion // Constructors

        //#region ICommand Members

        //public bool CanExecute(object parameter)
        //{
        //    return _canExecute == null ? true : _canExecute(parameter);
        //}

        //public event EventHandler CanExecuteChanged
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}

        //public void Execute(object parameter)
        //{
        //    _execute(parameter);
        //}

        //#endregion // ICommand Members
    }
}
