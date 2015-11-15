using System;
using System.Windows.Input;

namespace Reducto.Sample.ViewModels
{
    public class LoginPageViewModel : ViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public ICommand Login { get; set; }
        public bool InProgress { get; set; }
        public bool NotInProgress { get {return !InProgress;} }
        public bool Error { get; set; }
        public string ErrorMessage { get; set;}


        public LoginPageViewModel (App app)
        {
            Login = app.Store.createAsyncActionCommand(() => app.LoginAction(new LoginInfo{Username = Username, Password = Password}));
            app.Store.Subscribe (s => {
                InProgress = s.LoginPage.InProgress;
                Error = s.LoginPage.Error;
                ErrorMessage = s.LoginPage.ErrorMsg;
            });
        }
    }
}

