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
            appStore = new AppStore ();
        }

        private AppStore appStore;       

        [Test]
        public void should_navigate_to_login_viewmode_when_not_logged_in()
        {
            Assert.That (appStore.BootPage (), Is.TypeOf<LoginPageViewModel> ());
        }
    }
}

