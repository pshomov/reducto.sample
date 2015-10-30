using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reducto.Sample.ViewModels;

namespace Reducto.Sample.Views
{
    public partial class DeviceListPage : ContentPage
    {
        public DeviceListPage ()
        {
            InitializeComponent ();
        }

        protected void DeviceSelected(object sender, SelectedItemChangedEventArgs e){
            if (e.SelectedItem == null)
                return;
            var model = this.BindingContext as DeviceListPageViewModel;
            model.Clicked.Execute(e.SelectedItem);
        }

    }
}

