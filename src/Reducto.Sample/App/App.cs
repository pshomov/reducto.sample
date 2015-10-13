using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reducto.Sample.ViewModels;
using Xamarin.Forms;

#pragma warning disable 4014 1998

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

    public class App
    {
        public Func<DispatcherDelegate, Store<AppState>.GetStateDelegate, Task> DeviceListRefreshAction;
        public Func<LoginInfo, Func<DispatcherDelegate, Store<AppState>.GetStateDelegate, Task>> LoginAction;

        public Store<AppState> Store { get; private set;}

        public App(){
            var reducer = new CompositeReducer<AppState>()
                .Part(s => s.DevicePage, DeviceListReducer ())
                .Part(s => s.LoginPage, LoginPageReducer ());

            Store = new Store<AppState>(reducer);
        }

        public ViewModel BootPage ()
        {
            if (!Store.GetState ().LoginPage.LoggedIn)
                return new LoginPageViewModel(this);
            return new DeviceListPageViewModel(Store);
        }

        static SimpleReducer<LoginPageStore> LoginPageReducer ()
        {
            return new SimpleReducer<LoginPageStore> ().When<LoggingIn> ((s, a) =>  {
                s.inProgress = true;
                return s;
            }).When<LoginFailed> ((s, a) =>  {
                s.inProgress = false;
                s.errMsg = "Wrong username/password or user not found";
                return s;
            }).When<LoggedIn> ((s, a) =>  {
                s.inProgress = false;
                s.LoggedIn = true;
                return s;
            });
        }

        static SimpleReducer<DeviceListPageStore> DeviceListReducer ()
        {
            return new SimpleReducer<DeviceListPageStore> (() => new DeviceListPageStore {
                Devices = new List<DeviceInfo> (),
                Error = "",
                SelectedDeviceIndex = -1,
                inProgress = false
            }).When<DeviceListRefreshStarted> ((state, action) =>  {
                state.Devices = new List<DeviceInfo> ();
                state.inProgress = true;
                return state;
            }).When<DeviceSelectedAction> ((s, a) =>  {
                s.SelectedDeviceIndex = 1;
                return s;
            }).When<DeviceListRefreshFinished> ((state, action) =>  {
                state.Devices = new List<DeviceInfo> (action.Devices);
                state.inProgress = false;
                return state;
            });
        }

        public void SetupAsyncActions(INavigator nav, IServiceAPI serviceAPI)
        {
            DeviceListRefreshAction = async (dispatch, getState) =>
            {
                dispatch(new DeviceListRefreshStarted());
                var devices = await serviceAPI.GetDevices();
                dispatch(new DeviceListRefreshFinished {Devices = devices});
            };
            LoginAction = Store.asyncActionVoid<LoginInfo>(async (dispatch, getState, userinfo) =>
            {
                dispatch(new LoggingIn {Username = userinfo.Username});
                var loggedIn = await serviceAPI.AuthUser(userinfo.Username, userinfo.Password);
                if (loggedIn == UserInfo.NotFound){
                    dispatch(new LoginFailed());                    
                } else {
                    dispatch(new LoggedIn {Username = userinfo.Username, City = loggedIn.HomeCity});
                    nav.PushAsync(() => new DeviceListPageViewModel(Store));
                }
            });
        }

    }
}

