using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using System;
using NSubstitute;
using Reducto.Sample.Tests;
using Reducto.Sample.ViewModels;

#pragma warning disable 4014 1998

namespace Reducto.Sample
{
    public class UnhandledAction : Action
    {
    }

    [TestFixture]
    public class DeviceListTest
    {
        [SetUp]
        public void SetUp()
        {
            nav = Substitute.For<INavigator>();
            nav.PushAsync<ViewModel>().Returns(Task.Delay(0));
            serviceAPI = Substitute.For<IServiceAPI>();
            serviceAPI.AuthUser("john", "secret")
                .Returns(Task.FromResult(new UserInfo {Username = "John", HomeCity = "Reykjavik"}));
            serviceAPI.GetDevices().Returns(Task.FromResult(new List<DeviceInfo>
                {
                    new DeviceInfo {Id = new DeviceId("1"), Name = "D1", Online = true}
                }));

            app = new App ();
            app.SetupAsyncActions(nav, serviceAPI);
            store = app.Store;
            store.Middleware(history.logger());
        }

        private INavigator nav;
        private IServiceAPI serviceAPI;
        private App app;
        private Store<AppState> store;
        private Logger<AppState> history = new Logger<AppState>();

        [Test]
        public async void should_retrieve_device_list_when_device_refresh_is_requested()
        {
            await store.Dispatch(app.LoginAction(new LoginInfo {Username = "john", Password = "secret"}));
            await store.Dispatch(app.DeviceListRefreshAction);

            Assert.That(history.FirstAction(typeof (DeviceListRefreshStarted)).DevicePage.inProgress, Is.EqualTo(true));
            Assert.That(history.FirstAction(typeof (DeviceListRefreshFinished)).DevicePage.Devices,
                Is.EquivalentTo(new List<DeviceInfo>
                    {
                        new DeviceInfo {Id = new DeviceId("1"), Name = "D1", Online = true}
                    }));
        }

        [Test]
        public async void should_update_view_model_after_each_action(){
            await store.Dispatch (app.LoginAction(new LoginInfo{Username = "john", Password = "secret"}));
            var model = new DeviceListPageViewModel (store);
            store.Dispatch (new UnhandledAction ());
            Assert.That (model.Devices.Count, Is.EqualTo (0));
            await store.Dispatch (app.DeviceListRefreshAction);

            Assert.That(model.Devices.Count, Is.EqualTo(1));
        }

        [Test]
        public async void should_easily_dispatch_sync_actions_from_view_model(){
            await store.Dispatch (app.LoginAction(new LoginInfo{Username = "john", Password = "secret"}));
            Assert.That (store.GetState ().DevicePage.SelectedDeviceIndex, Is.EqualTo (-1));
            var model = new DeviceListPageViewModel (store);
            model.Clicked.Execute (null);
            Assert.That (store.GetState ().DevicePage.SelectedDeviceIndex, Is.EqualTo (1));
        }

    }
}