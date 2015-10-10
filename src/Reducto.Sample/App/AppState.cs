using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Reducto.Sample
{
    public struct LoginPageStore {
        public bool LoggedIn;
        public string username;
        public string errMsg;
        public bool inProgress;
    }
    public struct DeviceListPageStore {
        public bool inProgress;
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
