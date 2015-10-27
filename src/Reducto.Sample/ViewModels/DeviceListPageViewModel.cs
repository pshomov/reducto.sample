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

        App app;

        public DeviceListPageViewModel (App app)
        {
            this.app = app;
            Devices = new ObservableCollection<DeviceSummary>();

            Clicked = app.Store.createActionCommand (() => new DeviceSelectedAction{ });
            app.Store.Subscribe ((s) => {
                Pulling = s.DevicePage.inProgress;
                Devices.Clear();
                foreach (var item in s.DevicePage.Devices.Select(d => new DeviceSummary{Name = d.Name, Location = d.Location})) {
                    Devices.Add(new DeviceSummary{Name = item.Name});
                }
            });
        }

        public override void Init ()
        {
            app.Store.Dispatch (new {});
            app.Store.Dispatch (app.DeviceListRefreshAction);
        }
    }
}

