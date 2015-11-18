using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Reducto.Sample
{
    public struct LoginPageState
    {
        public bool LoggedIn;
        public string Username;
        public string ErrorMsg;
        public bool Error;
        public bool InProgress;
    }

    public struct DeviceListPageState
    {
        public bool InProgress;
        public List<DeviceInfo> Devices;
        public String Error;
        public int SelectedDeviceIndex;
    }

    public struct AppState
    {
        public LoginPageState LoginPage;
        public DeviceListPageState DevicePage;
    }

}
