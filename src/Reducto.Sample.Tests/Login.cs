﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using System;
using NSubstitute;
using Reducto.Sample.ViewModels;
using NSubstitute.ExceptionExtensions;

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
            nav.PushAsync<ViewModel>().Returns(Task.Delay(0));
            serviceAPI = Substitute.For<IServiceAPI>();
            serviceAPI.AuthUser("john", "secret")
                .Returns(Task.FromResult(new UserInfo { Username = "John", HomeCity = "Reykjavik" }));
            serviceAPI.AuthUser("john", "sdf")
                .Returns(Task.FromResult(UserInfo.NotFound));
            serviceAPI.AuthUser("john", "oh-noes")
                .Throws(new NullReferenceException());
            serviceAPI.GetDevices().Returns(Task.FromResult(new List<DeviceInfo>()));

            app = new App();
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
        public async void should_perform_login_process_when_provided_username_password_and_navigate_to_device_list_view()
        {
            await store.Dispatch(app.LoginAction(new LoginInfo { Username = "john", Password = "secret" }));

            nav.Received().PushAsync<DeviceListPageViewModel>(Arg.Any<Func<DeviceListPageViewModel>>());
            Assert.That(history.FirstAction<LoginStarted>().LoginPage,
                Is.EqualTo(new LoginPageState { InProgress = true, ErrorMsg = "" }));
            Assert.That(history.FirstAction<LoggedIn>().LoginPage,
                Is.EqualTo(new LoginPageState { InProgress = false, LoggedIn = true, ErrorMsg = "" }));
        }

        [Test]
        public async void should_provide_error_message_when_login_fails()
        {
            await store.Dispatch(app.LoginAction(new LoginInfo { Username = "john", Password = "sdf" }));

            Assert.That(history.FirstAction<LoginStarted>().LoginPage,
                Is.EqualTo(new LoginPageState { InProgress = true, ErrorMsg = "" }));
            Assert.That(history.FirstAction<LoginFailed>().LoginPage,
                Is.EqualTo(new LoginPageState {
                    InProgress = false,
                    LoggedIn = false,
                    Error = true,
                    ErrorMsg = "Wrong username/password or user not found"
                }));
        }

        [Test]
        public async void should_provide_error_message_when_login_service_fails()
        {
            await store.Dispatch(app.LoginAction(new LoginInfo { Username = "john", Password = "oh-noes" }));

            Assert.That(history.FirstAction<LoginStarted>().LoginPage,
                Is.EqualTo(new LoginPageState { InProgress = true, ErrorMsg = "" }));
            Assert.That(history.FirstAction<LoginServiceUnavailable>().LoginPage,
                Is.EqualTo(new LoginPageState {
                    InProgress = false,
                    LoggedIn = false,
                    Error = true,
                    ErrorMsg = "Service currently unavailable, please try again later"
                }));
        }

        [Test]
        public async void should_clear_the_error_after_successful_retry_of_credentials()
        {
            await store.Dispatch(app.LoginAction(new LoginInfo { Username = "john", Password = "sdf" }));
            await store.Dispatch(app.LoginAction(new LoginInfo { Username = "john", Password = "secret" }));

            Assert.That(history.FirstAction<LoggedIn>().LoginPage,
                Is.EqualTo(new LoginPageState { InProgress = false, LoggedIn = true, ErrorMsg = "" }));
        }

        [Test]
        public async void should_report_error_when_trying_wrong_credentials_after_successfully_logged_in()
        {
            await store.Dispatch(app.LoginAction(new LoginInfo { Username = "john", Password = "secret" }));
            await store.Dispatch(app.LoginAction(new LoginInfo { Username = "john", Password = "sdf" }));

            Assert.That(history.FirstAction<LoginFailed>().LoginPage,
                Is.EqualTo(new LoginPageState {
                    InProgress = false,
                    LoggedIn = false,
                    Error = true,
                    ErrorMsg = "Wrong username/password or user not found"
                }));
        }

    }
}