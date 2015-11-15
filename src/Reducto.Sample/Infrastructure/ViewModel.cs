using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Linq;

#pragma warning disable 67

namespace Reducto.Sample
{
    public class ViewModel : INotifyPropertyChanged
    {
        public Page Page { 
            get {
                Page view = (Page)Activator.CreateInstance (ViewTypeFromModel ());
                view.BindingContext = this;
                return view;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Type ViewTypeFromModel ()
        {
            var model = this.GetType ();
            var fullName = model.FullName;
            var nameParts = fullName.Split ('.');
            var modelName = nameParts[nameParts.Length - 1];
            var viewName = nameParts.Take (nameParts.Count () - 2).Aggregate ("", (acc, next) => {
                return acc + next + ".";
            }) + "Views." + modelName.Replace ("ViewModel", string.Empty);
            var viewType = Type.GetType (viewName);
            if (viewType == null) {
                throw new Exception ("Cannot find view " + viewName);
            }

            return viewType;
        }

        public virtual void Init ()
        {
        }
    }
    
}
