using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reducto.Sample
{
    public struct LoggedIn : Action
    {
        public string City;
        public string Username;
    }

    public struct LoggingIn : Action
    {
        public string Username;
    }
    public struct LoginFailed : Action
    {
    }
    public class AppStart : Action
    {
    }

    public class DeviceListRefreshStarted : Action
    {
    }

    public class DeviceSelectedAction : Action 
    {
        public string deviceId;
    }

    public class DeviceListRefreshFinished : Action
    {
        public List<DeviceInfo> Devices;
    }

    public struct LoginInfo
    {
        public string Password;
        public string Username;
    }

    public class AppStore
    {
        public Func<DispatcherDelegate, Store<AppState>.GetStateDelegate, Task> DeviceListRefreshAction;
        public Func<DispatcherDelegate, Store<AppState>.GetStateDelegate, Task> BootAppAction;
        public Func<LoginInfo, Func<DispatcherDelegate, Store<AppState>.GetStateDelegate, Task>> LoginAction;

        public Store<AppState> WireUpApp(INavigator nav, IServiceAPI serviceAPI)
        {
            var deviceList = new SimpleReducer<DeviceListPageStore>(
                () => new DeviceListPageStore
                {
                    Devices = new List<DeviceInfo>(),
                    Error = "",
                    SelectedDeviceIndex = -1,
                    inProgress = false
                })
                .When<DeviceListRefreshStarted>((state, action) =>
                    {
                        state.Devices = new List<DeviceInfo>();
                        state.inProgress = true;
                        return state;
                    })
                .When<DeviceSelectedAction>((s, a) => {
                    s.SelectedDeviceIndex = 1;
                    return s;
                })
                .When<DeviceListRefreshFinished>((state, action) => {
                    state.Devices = new List<DeviceInfo>(action.Devices);
                    state.inProgress = false;
                    return state;
                });
            var loginReducer = new SimpleReducer<LoginPageStore>()
                .When<LoggingIn>((s, a) =>
                    {
                        s.inProgress = true;
                        return s;
                    })
                .When<LoginFailed>((s, a) => 
                    {
                        s.inProgress = false;
                        s.errMsg = "Wrong username/password or user not found";
                        return s;
                    })
                .When<LoggedIn>((s, a) =>
                    {
                        s.inProgress = false;
                        return s;
                    });
            
            var reducer = new CompositeReducer<AppState>()
                .Part(s => s.DevicePage, deviceList)
                .Part(s => s.LoginPage, loginReducer);

            var store = new Store<AppState>(reducer);
            DeviceListRefreshAction = async (dispatch, getState) =>
            {
                dispatch(new DeviceListRefreshStarted());
                var devices = await serviceAPI.GetDevices();
                dispatch(new DeviceListRefreshFinished {Devices = devices});
            };
            LoginAction = store.asyncActionVoid<LoginInfo>(async (dispatch, getState, userinfo) =>
            {
                dispatch(new LoggingIn {Username = userinfo.Username});
                var loggedIn = await serviceAPI.AuthUser(userinfo.Username, userinfo.Password);
                if (loggedIn == UserInfo.NotFound){
                    dispatch(new LoginFailed());                    
                } else {
                    dispatch(new LoggedIn {Username = userinfo.Username, City = loggedIn.HomeCity});
                    nav.PushAsync<DeviceListPageViewModel>();
                }
            });
            BootAppAction = (disp, getState) => {
                if (!getState ().LoginPage.LoggedIn)
                    return nav.PushAsync<LoginPageViewModel> ();
                return nav.PushAsync<DeviceListPageViewModel> ();
            };
            return store;
        }

    }
}

