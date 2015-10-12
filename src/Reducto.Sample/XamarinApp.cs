using System;

using Xamarin.Forms;
using Reducto.Sample.ViewModels;
using Reducto.Sample.Views;
using Reducto.Sample.Services;

namespace Reducto.Sample
{
    public class XamarinApp : Application
    {
        App app;

        public XamarinApp ()
        {
            app = new App();
            var navigationPage = new NavigationPage (app.BootPage().Page);
            MainPage = navigationPage;
            var nav = new Navigator (navigationPage.Navigation);
            app.SetupAsyncActions (nav, null);
        }

        protected override void OnStart ()
        {
            // Handle when your app starts
        }

        protected override void OnSleep ()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Handle when your app resumes
        }
    }
}

