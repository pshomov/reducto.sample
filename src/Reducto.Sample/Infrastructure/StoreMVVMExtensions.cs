using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Reducto.Sample
{

    public static class StoreMVVMExtensions {
        public static ICommand createActionCommand<State>(this Store<State> store, Func<Object> actionMaker) {
            return new StoreActionCommand<State> (store, actionMaker);
        }
        public static ICommand createAsyncActionCommand<State>(this Store<State> store, Func<Func<DispatcherDelegate, Store<State>.GetStateDelegate, Task>> actionMaker) {
            return new StoreAsyncActionCommand<State> (store, actionMaker);
        }
    }
    
}
