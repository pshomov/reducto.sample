using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

#pragma warning disable 67

namespace Reducto.Sample
{
    
    public class StoreActionCommand<State> : ICommand
    {
        Store<State> store;
        Func<Object> execute;

        public StoreActionCommand(Store<State> store, Func<Object> execute)
        {
            this.execute = execute;
            this.store = store;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            store.Dispatch(execute());
        }
    }

    public class StoreActionCommand<State, T> : ICommand
    {
        Store<State> store;
        Func<T, Object> execute;

        public StoreActionCommand(Store<State> store, Func<T, Object> execute)
        {
            this.execute = execute;
            this.store = store;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            store.Dispatch(execute((T)parameter));
        }
    }

    public class StoreAsyncActionCommand<State> : ICommand
    {
        Store<State> store;
        Func<Store<State>.AsyncAction> action;

        public StoreAsyncActionCommand(Store<State> store, Func<Store<State>.AsyncAction> action)
        {
            this.action = action;
            this.store = store;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            store.Dispatch(action());
        }
    }
}
