using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Reducto.Sample
{
    public interface INavigator
    {
        Task PushAsync<Model> () where Model : ViewModel;
        Task PushAsync<Model> (Action<Model> configureModel, List<Type> updateOnEvents) where Model : ViewModel;
        Task PopToRootAsync (bool animated);
    }
}

