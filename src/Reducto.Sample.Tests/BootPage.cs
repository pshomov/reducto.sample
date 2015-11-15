using System;
using NUnit.Framework;
using Reducto.Sample.ViewModels;

namespace Reducto.Sample.Tests
{
    [TestFixture]
    public class BootPageTest
    {

        [SetUp]
        public void SetUp()
        {
            app = new App();
        }

        private App app;

        [Test]
        public void should_navigate_to_login_viewmodel_when_not_logged_in()
        {
            Assert.That(app.BootPage(), Is.TypeOf<LoginPageViewModel>());
        }

        [Test]
        public void should_navigate_to_device_list_viewmodel_when_logged_in()
        {
            app.Store.Dispatch(new LoggingIn{ Username = "John" });
            app.Store.Dispatch(new LoggedIn{ Username = "John" });
            Assert.That(app.BootPage(), Is.TypeOf<DeviceListPageViewModel>());
        }
    }
}

