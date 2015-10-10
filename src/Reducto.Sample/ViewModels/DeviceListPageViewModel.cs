using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Reducto.Sample
{
    public class DeviceSelectedAction : Action {
        public string deviceId;
    }
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

    public static class StoreMVVMExtensions {
        public static ICommand createActionCommand<State>(this Store<State> store, Func<Action> actionMaker) where State : new() {
            return new StoreActionCommand<State> (store, actionMaker);
        }
    }

    public class DeviceListPageViewModel : ViewModel
    {
        public ObservableCollection<DeviceSummary> Devices { get; set;}
        public Boolean Pulling { get; set;}
        public ICommand Clicked;


        public DeviceListPageViewModel (Store<AppState> store)
        {
            Clicked = store.createActionCommand(() => new DeviceSelectedAction{});
            store.Subscribe ((s) => {
                Pulling = s.DevicePage.inProgress;
                Devices = new ObservableCollection<DeviceSummary>(s.DevicePage.Devices.Select(d => new DeviceSummary{Name = d.Name, Location = d.Location}));
            });
        }
    }
}

