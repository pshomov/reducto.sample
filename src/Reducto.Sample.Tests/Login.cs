using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using System;
using NSubstitute;

#pragma warning disable 4014 1998

namespace Reducto.Sample.Tests
{
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

            appStore = new AppStore ();
            store = appStore.WireUpApp(nav, serviceAPI);
            store.Middleware(history.logger());
        }

        private INavigator nav;
        private IServiceAPI serviceAPI;
        private AppStore appStore;       
        private Store<AppState> store;
        private Logger<AppState> history = new Logger<AppState>();

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
        public async void should_perform_login_process_when_provided_username_password_and_navigate_to_device_list_view()
        {
            await store.Dispatch(appStore.LoginAction(new LoginInfo {Username = "john", Password = "secret"}));

            nav.Received().PushAsync<DeviceListPageViewModel>();
            Assert.That(history.FirstAction(typeof (LoggingIn)).LoginPage,
                Is.EqualTo(new LoginPageStore {inProgress = true}));
            Assert.That(history.FirstAction(typeof (LoggedIn)).LoginPage,
                Is.EqualTo(new LoginPageStore {inProgress = false}));
        }
       
    }
}