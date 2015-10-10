using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Reducto.Sample
{
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

