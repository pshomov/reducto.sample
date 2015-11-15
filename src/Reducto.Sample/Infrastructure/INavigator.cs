using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Reducto.Sample
{
    public interface INavigator
    {
        Task PushAsync<Model>() where Model : ViewModel;

        Task PushAsync<Model>(Func<Model> configureModel) where Model : ViewModel;

        Task PopToRootAsync(bool animated);
    }
}

