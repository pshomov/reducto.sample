using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Reducto.Sample
{
    
    public class StoreActionCommand<State> : ICommand {
        Store<State> store;

        Func<Object> execute;

        public StoreActionCommand (Store<State> store, Func<Object> execute)
        {
            this.execute = execute;
            this.store = store;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute (object parameter)
        {
            return true;
        }

        public void Execute (object parameter)
        {
            store.Dispatch (execute ());
        }
    }
    
    public class StoreAsyncActionCommand<State> : ICommand where State : new(){
        Store<State> store;

        Func<Func<DispatcherDelegate, Store<State>.GetStateDelegate, Task>> action;

        public StoreAsyncActionCommand (Store<State> store, Func<Func<DispatcherDelegate, Store<State>.GetStateDelegate, Task>> action)
        {
            this.action = action;
            this.store = store;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute (object parameter)
        {
            return true;
        }

        public void Execute (object parameter)
        {
            store.Dispatch (action ());
        }
    }
}
