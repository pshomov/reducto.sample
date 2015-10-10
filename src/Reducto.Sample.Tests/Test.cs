using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using System;
using NSubstitute;

#pragma warning disable 4014 1998

namespace Reducto.Sample
{
    public class LoggedAction<S>
    {
        public Action Action;
        public S StateAfter;
    }

    public static class TestHelpers
    {
        public static T FirstAction<T>(this List<LoggedAction<T>> history, Type action)
        {
            return history.Find(a => a.Action.GetType() == action).StateAfter;
        }
    }

    public class AppStart : Action
    {
    }

    public class UnhandledAction : Action
    {
    }

    public class DeviceListRefreshStarted : Action
    {
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

    public struct LoggedIn : Action
    {
        public string City;
        public string Username;
    }

    public struct LoggingIn : Action
    {
        public string Username;
    }


    [TestFixture]
    public class LoginTest
    {
        [SetUp]
        public void SetUp()
        {
            nav = Substitute.For<INavigator>();
            nav.PushAsync<object>().Returns(Task.Delay(0));
            serviceAPI = Substitute.For<IServiceAPI>();
            serviceAPI.AuthUser("john", "secret")
                .Returns(Task.FromResult(new UserInfo {Username = "John", HomeCity = "Reykjavik"}));
            serviceAPI.GetDevices().Returns(Task.FromResult(new List<DeviceInfo>
                {
                    new DeviceInfo {Id = new DeviceId("1"), Name = "D1", Online = true}
                }));

            store = WireUpApp();
            store.Middleware(logger());
        }

        private INavigator nav;
        private IServiceAPI serviceAPI;
        private Store<AppState> store;
        private CompositeReducer<AppState> reducer;
        private readonly List<LoggedAction<AppState>> history = new List<LoggedAction<AppState>>();
        private Func<LoginInfo, Func<DispatcherDelegate, Store<AppState>.GetStateDelegate, Task>> LoginAction;
        private Func<DispatcherDelegate, Store<AppState>.GetStateDelegate, Task> DeviceListRefreshAction;

        public Store<AppState> WireUpApp()
        {
            var loginReducer = new SimpleReducer<LoginPageStore>()
                .When<LoggingIn>((s, a) =>
                    {
                        s.inProgress = true;
                        return s;
                    })
                .When<LoggedIn>((s, a) =>
                    {
                        s.inProgress = false;
                        return s;
                    });
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
            reducer = new CompositeReducer<AppState>()
                .Part(s => s.LoginPage, loginReducer)
                .Part(s => s.DevicePage, deviceList);

            store = new Store<AppState>(reducer);
            LoginAction = store.asyncActionVoid<LoginInfo>(async (dispatch, getState, userinfo) =>
                {
                    dispatch(new LoggingIn {Username = userinfo.Username});
                    var loggedIn = await serviceAPI.AuthUser(userinfo.Username, userinfo.Password);
                    dispatch(new LoggedIn {Username = userinfo.Username, City = loggedIn.HomeCity});
                    nav.PushAsync<DeviceListPageViewModel>();
                });
            DeviceListRefreshAction = async (dispatch, getState) =>
            {
                dispatch(new DeviceListRefreshStarted());
                var devices = await serviceAPI.GetDevices();
                dispatch(new DeviceListRefreshFinished {Devices = devices});
            };
            return store;
        }

        private Middleware<AppState> logger()
        {
            return s => next => action =>
            {
                next(action);
                var after = s.GetState();
                history.Add(new LoggedAction<AppState>
                    {
                        Action = action,
                        StateAfter = after
                    });
            };
        }

        [Test]
        public async void should_navigate_to_login_viewmode_when_not_logged_in()
        {
            await store.Dispatch((disp, getState) =>
                {
                    if (!getState().LoginPage.LoggedIn)
                        return nav.PushAsync<LoginPageViewModel>();
                    return nav.PushAsync<DeviceListPageViewModel>();
                });

            nav.Received().PushAsync<LoginPageViewModel>();
        }

        [Test]
        public void should_not_modify_original_state()
        {
            var state = new AppState();
            var store = new Store<AppState>(reducer);
            store.Dispatch(new LoggingIn());
            Assert.That(store.GetState(), Is.Not.EqualTo(state));
        }

        [Test]
        public async void should_perform_login_process_when_provided_username_password_and_navigate_to_device_list_view()
        {
            await store.Dispatch(LoginAction(new LoginInfo {Username = "john", Password = "secret"}));

            nav.Received().PushAsync<DeviceListPageViewModel>();
            Assert.That(history.FirstAction(typeof (LoggingIn)).LoginPage,
                Is.EqualTo(new LoginPageStore {inProgress = true}));
            Assert.That(history.FirstAction(typeof (LoggedIn)).LoginPage,
                Is.EqualTo(new LoginPageStore {inProgress = false}));
        }

        [Test]
        public async void should_retrieve_device_list_when_device_refresh_is_requested()
        {
            await store.Dispatch(LoginAction(new LoginInfo {Username = "john", Password = "secret"}));
            await store.Dispatch(DeviceListRefreshAction);

            Assert.That(history.FirstAction(typeof (DeviceListRefreshStarted)).DevicePage.inProgress, Is.EqualTo(true));
            Assert.That(history.FirstAction(typeof (DeviceListRefreshFinished)).DevicePage.Devices,
                Is.EquivalentTo(new List<DeviceInfo>
                    {
                        new DeviceInfo {Id = new DeviceId("1"), Name = "D1", Online = true}
                    }));
        }

        [Test]
        public async void should_update_view_model_after_each_action(){
            await store.Dispatch (LoginAction(new LoginInfo{Username = "john", Password = "secret"}));
            var model = new DeviceListPageViewModel (store);
            store.Dispatch (new UnhandledAction ());
            Assert.That (model.Devices.Count, Is.EqualTo (0));
            await store.Dispatch (DeviceListRefreshAction);

            Assert.That(model.Devices.Count, Is.EqualTo(1));
        }

        [Test]
        public async void should_easily_dispatch_sync_actions_from_view_model(){
            await store.Dispatch (LoginAction(new LoginInfo{Username = "john", Password = "secret"}));
            Assert.That (store.GetState ().DevicePage.SelectedDeviceIndex, Is.EqualTo (-1));
            var model = new DeviceListPageViewModel (store);
            model.Clicked.Execute (null);
            Assert.That (store.GetState ().DevicePage.SelectedDeviceIndex, Is.EqualTo (1));
        }

    }
}