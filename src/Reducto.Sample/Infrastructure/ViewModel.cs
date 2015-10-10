using System;
using Xamarin.Forms;

namespace Reducto.Sample
{
    public class ViewModel {
        public Page Page { get {return new Page(){BindingContext = this};}}        
    }
    
}
