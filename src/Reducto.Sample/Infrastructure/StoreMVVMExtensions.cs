using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Reducto.Sample
{

    public static class StoreMVVMExtensions {
        public static ICommand createActionCommand<State>(this Store<State> store, Func<Action> actionMaker) where State : new() {
            return new StoreActionCommand<State> (store, actionMaker);
        }
    }
    
}
