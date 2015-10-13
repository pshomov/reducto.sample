using System;
using System.Windows.Input;

namespace Reducto.Sample.ViewModels
{
    public class LoginPageViewModel : ViewModel
    {
        public String Username { get; set; }
        public String Password { get; set; }
        public ICommand Login { get; set; }
        public bool InProgress { get; set; }

        public LoginPageViewModel (App app)
        {
            Login = new StoreAsyncActionCommand<AppState> (app.Store, () => app.LoginAction(new LoginInfo{Username = Username, Password = Password}));
            app.Store.Subscribe (s => {
                InProgress = s.LoginPage.inProgress;
            });
        }
    }
}

