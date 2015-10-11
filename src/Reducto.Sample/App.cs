using System;

using Xamarin.Forms;
using Reducto.Sample.ViewModels;
using Reducto.Sample.Views;
using Reducto.Sample.Services;

namespace Reducto.Sample
{
    public class App : Application
    {
        AppStore appStore;

        public App ()
        {
            appStore = new AppStore();
            var navigationPage = new NavigationPage (appStore.BootPage().Page);
            MainPage = navigationPage;
            var nav = new Navigator (navigationPage.Navigation);
            appStore.WireUpApp (nav, null);
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

