using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Reducto.Sample.ViewModels
{
    public class DeviceListPageViewModel : ViewModel
    {
        public ObservableCollection<DeviceSummary> Devices { get; set;}
        public Boolean Pulling { get; set;}
        public ICommand Clicked;
        public ICommand RefreshList { get; private set;}

        App app;

        public DeviceListPageViewModel (App app)
        {
            this.app = app;
            Devices = new ObservableCollection<DeviceSummary>();

            Clicked = app.Store.createActionCommand (() => new DeviceSelectedAction{ });
            RefreshList = app.Store.createAsyncActionCommand (() => app.DeviceListRefreshAction);
            app.Store.Subscribe ((s) => {
                Pulling = s.DevicePage.inProgress;
                Devices.Clear();
                foreach (var item in s.DevicePage.Devices.Select(d => new DeviceSummary{Name = d.Name, Location = d.Location})) {
                    Devices.Add(item);
                }
            });
        }
    }
}

