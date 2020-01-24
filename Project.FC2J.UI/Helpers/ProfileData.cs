using Project.FC2J.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers
{
    public class ProfileData : IProfileData
    {
        private ILoggedInUser _loggedInUser;
        private IApiAppSetting _apiAppSetting;
        private IAPIHelper _apiHelper;

        public ProfileData(ILoggedInUser loggedInUser, IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _loggedInUser = loggedInUser;
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;            
        }

        private string PadIfNotEmpty(string value)
        {
            if (value.Trim() != string.Empty)
            {
                return " " + value.Trim();
            }
            return "";            
        }
        public string FullName { get { return _loggedInUser.User.FirstName + PadIfNotEmpty( _loggedInUser.User.MiddleName)  + PadIfNotEmpty(_loggedInUser.User.LastName); } }
        public string UserName { get { return _loggedInUser.User.UserName; } }
        public string PasswordX { get { return _loggedInUser.User.PasswordX; } }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _apiHelper.GetRecord<User>(_apiAppSetting.GetUser + $"?userName={userName}");
        }

        public void SetPasswordX(string value)
        {
            _loggedInUser.User.PasswordX = value;
        }
    }
}
