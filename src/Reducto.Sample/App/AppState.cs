using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Reducto.Sample
{
    public struct LoginPageStore
    {
        public bool LoggedIn;
        public string Username;
        public string ErrorMsg;
        public bool InProgress;
    }

    public struct DeviceListPageStore
    {
        public bool InProgress;
        public List<DeviceInfo> Devices;
        public String Error;
        public int SelectedDeviceIndex;
    }

    public struct AppState
    {
        public LoginPageStore LoginPage;
        public DeviceListPageStore DevicePage;
    }

}
