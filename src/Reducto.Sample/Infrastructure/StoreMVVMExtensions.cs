using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Reducto.Sample
{

    public static class StoreMVVMExtensions {
        public static ICommand createActionCommand<State>(this Store<State> store, Func<Object> actionMaker) {
            return new StoreActionCommand<State> (store, actionMaker);
        }
    }
    
}
