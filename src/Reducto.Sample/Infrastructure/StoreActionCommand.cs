using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Reducto.Sample
{
    
    public class StoreActionCommand<State> : ICommand where State : new(){
        Store<State> store;

        Func<Action> execute;

        public StoreActionCommand (Store<State> store, Func<Action> execute)
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
    
}
