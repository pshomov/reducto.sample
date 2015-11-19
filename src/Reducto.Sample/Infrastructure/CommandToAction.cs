using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

#pragma warning disable 67

namespace Reducto.Sample
{
    
    public class CommandToAction<State> : ICommand
    {
        Store<State> store;
        Func<Object> execute;

        public CommandToAction(Store<State> store, Func<Object> execute)
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

    public class CommandToAction<State, T> : ICommand
    {
        Store<State> store;
        Func<T, Object> execute;

        public CommandToAction(Store<State> store, Func<T, Object> execute)
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

    public class CommandToAsyncAction<State> : ICommand
    {
        Store<State> store;
        Func<Store<State>.AsyncAction> action;

        public CommandToAsyncAction(Store<State> store, Func<Store<State>.AsyncAction> action)
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

    public static class ReductorMVVMExtensions
    {
        public static ICommand createActionCommand<State>(this Store<State> store, Func<Object> actionMaker)
        {
            return new CommandToAction<State>(store, actionMaker);
        }

        public static ICommand createActionCommand<State, T>(this Store<State> store, Func<T, Object> actionMaker)
        {
            return new CommandToAction<State, T>(store, actionMaker);
        }

        public static ICommand createAsyncActionCommand<State>(this Store<State> store, Func<Store<State>.AsyncAction> actionMaker)
        {
            return new CommandToAsyncAction<State>(store, actionMaker);
        }
    }

}
