﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reducto.Sample.ViewModels;
using Xamarin.Forms;

#pragma warning disable 4014 1998

namespace Reducto.Sample
{
    public struct LoggedIn
    {
        public string City;
        public string Username;
    }

    public struct LoginStarted
    {
        public string Username;
    }

    public struct LoginFailed
    {
    }

    public struct LoginServiceUnavailable
    {
    }

    public class AppStart
    {
    }

    public class DeviceListRefreshStarted
    {
    }

    public class DeviceListRefreshFinished
    {
        public List<DeviceInfo> Devices;
    }

    public class DeviceListRefreshFailed
    {
    }

    public class DeviceSelectedAction
    {
        public DeviceId deviceId;
    }

    public struct LoginInfo
    {
        public string Password;
        public string Username;
    }

    public class App
    {
        public Store<AppState>.AsyncAction DeviceListRefreshAction;
        public Store<AppState>.AsyncActionNeedsParam<LoginInfo> LoginAction;

        public Store<AppState> Store { get; private set; }

        public App()
        {
            var reducer = new CompositeReducer<AppState>()
                .Part(s => s.DevicePage, DeviceListReducer())
                .Part(s => s.LoginPage, LoginPageReducer());

            Store = new Store<AppState>(reducer);
        }

        public ViewModel BootPage()
        {
            if (!Store.GetState().LoginPage.LoggedIn) {
                return new LoginPageViewModel(this);
            } else {
                return new DeviceListPageViewModel(this);
            }
        }

        static SimpleReducer<LoginPageState> LoginPageReducer()
        {
            return new SimpleReducer<LoginPageState>( () => new LoginPageState { 
                InProgress = false, 
                ErrorMsg = "", 
                LoggedIn = false, 
                Error = false
            }).When<LoginStarted>((state, action) => {
                state.InProgress = true;
                state.LoggedIn = false;
                state.Error = false;
                state.ErrorMsg = "";
                return state;
            }).When<LoginFailed>((state, action) => {
                state.InProgress = false;
                state.Error = true;
                state.ErrorMsg = "Wrong username/password or user not found";
                return state;
            }).When<LoggedIn>((state, action) => {
                state.InProgress = false;
                state.LoggedIn = true;
                return state;
            }).When<LoginServiceUnavailable>((state, action) => {
                state.InProgress = false;
                state.Error = true;
                state.ErrorMsg = "Service currently unavailable, please try again later";
                return state;
            });
        }

        static SimpleReducer<DeviceListPageState> DeviceListReducer()
        {
            return new SimpleReducer<DeviceListPageState>(() => new DeviceListPageState {
                Devices = new List<DeviceInfo>(),
                Error = "",
                SelectedDeviceIndex = -1,
                InProgress = false
            }).When<DeviceListRefreshStarted>((state, action) => {
                state.Devices = new List<DeviceInfo>();
                state.InProgress = true;
                return state;
            }).When<DeviceSelectedAction>((state, action) => {
                state.SelectedDeviceIndex = 1;
                return state;
            }).When<DeviceListRefreshFinished>((state, action) => {
                state.Devices = new List<DeviceInfo>(action.Devices);
                state.InProgress = false;
                return state;
            });
        }

        public void SetupAsyncActions(INavigator nav, IServiceAPI serviceAPI)
        {
            DeviceListRefreshAction = async (dispatch, getState) => {
                dispatch(new DeviceListRefreshStarted());
                List<DeviceInfo> devices;
                devices = await serviceAPI.GetDevices();
                dispatch(new DeviceListRefreshFinished { Devices = devices });
            };

            LoginAction = Store.asyncActionVoid<LoginInfo>(async (dispatch, getState, userinfo) => {
                dispatch(new LoginStarted { Username = userinfo.Username }); // starting the login process
                UserInfo loggedIn;
                try {
                    loggedIn = await serviceAPI.AuthUser(userinfo.Username, userinfo.Password);
                } catch(Exception){
                    dispatch(new LoginServiceUnavailable()); // error communicating with the service
                    return;
                }
                if (loggedIn == UserInfo.NotFound) {
                    dispatch(new LoginFailed()); // user/pass did not match
                } else {
                    dispatch(new LoggedIn { 
                        Username = userinfo.Username, 
                        City = loggedIn.HomeCity 
                    }); // success
                    nav.PushAsync(() => new DeviceListPageViewModel(this));
                    await DeviceListRefreshAction(dispatch, getState);
                }
            });
        }

    }
}

