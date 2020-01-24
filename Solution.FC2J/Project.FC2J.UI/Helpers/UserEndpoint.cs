using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers
{
    public class UserEndpoint : IUserEndpoint
    {
        private IApiAppSetting _apiAppSetting;
        private IAPIHelper _apiHelper;

        public UserEndpoint(IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
        }
        public async Task<List<User>> GetList()
        {
            return await _apiHelper.GetList<User>(_apiAppSetting.User);
        }

        public async Task<List<Role>> GetRoles()
        {
            return await _apiHelper.GetList<Role>(_apiAppSetting.Role);
        }

        public async Task Remove(long id)
        {
            await _apiHelper.Remove(_apiAppSetting.User + $"?id={id}");
        }

        public async Task<User> Save(User value)
        {
            return await _apiHelper.Save<User>(_apiAppSetting.User, value);
        }

        public async Task Update(User value)
        {
            await _apiHelper.Update<User>(_apiAppSetting.User, value);
        }
    }
}
