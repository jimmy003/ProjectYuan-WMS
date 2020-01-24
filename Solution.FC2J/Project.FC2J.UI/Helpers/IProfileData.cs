using Project.FC2J.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.User;

namespace Project.FC2J.UI.Helpers
{
    public interface IProfileData
    {
        string UserName { get;  }

        string FullName { get; }
        string PasswordX { get; }

        void SetPasswordX(string value);
        Task<User> GetUserByUserNameAsync(string userName);
    }
}
