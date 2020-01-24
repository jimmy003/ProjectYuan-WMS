using Caliburn.Micro;
using Project.FC2J.UI.EventModels;
using Project.FC2J.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.UI.ViewModels
{

    public class LoginViewModel : Screen 
    {
        private string _userName = "";
        private string _password = "Checkit1234";
        private string _errorMessage;
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Password = string.Empty;
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }


        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }


        public string ErrorMessage
        {
            get { return _errorMessage;  }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorVisible);
            }
        }

        public bool IsErrorVisible
        {
            get
            {
                bool output = false;
                if(ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output;
            }
        }
        public bool CanLogIn
        {
            get
            {
                bool output = false;
                if (UserName?.Length > 0 && Password?.Length > 0 && _clicked == 0)
                {
                    output = true;
                }
                return output;
            }
        }

        private int _clicked = 0;
        public async Task LogIn()
        {
            try
            {
                ErrorMessage = "";
                _clicked = 1;
                NotifyOfPropertyChange(() => CanLogIn);
                await _apiHelper.Authenticate(UserName, Password);
                _events.PublishOnUIThread(new LogOnEvent());
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                _clicked = 0;
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }
            

    }
}
