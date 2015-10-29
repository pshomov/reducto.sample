using System;
using Xamarin.Forms;

namespace Reducto.Sample
{
    public class Navigator : INavigator
    {
        INavigation navigation;

        public Navigator (INavigation navigation)
        {
            this.navigation = navigation;
        }

        public System.Threading.Tasks.Task PopToRootAsync (bool animated)
        {
            return navigation.PopToRootAsync (animated);
        }

        public System.Threading.Tasks.Task PushAsync<Model> () where Model : ViewModel
        {
            ViewModel vm = (ViewModel)Activator.CreateInstance (typeof(Model));
            vm.Init ();
            return navigation.PushAsync (vm.Page);
        }
        public System.Threading.Tasks.Task PushAsync<Model> (Func<Model> configureModel) where Model : ViewModel
        {
            var model = configureModel ();
            model.Init ();
            return navigation.PushAsync (model.Page);
        }
    }
}

