using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Sale;
using Project.FC2J.Models.Dtos;
using System.Net.Http;
using Project.FC2J.Models.Token;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers
{
    public class SaleData : ISaleData
    {
        private readonly IApiAppSetting _apiAppSetting;
        private readonly IAPIHelper _apiHelper;
        public SaleData(IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
        }
        public bool IsFromSalesList { get; set; }
        public SaleHeader Value { get; set; }
        public async Task Authenticate(string username, string password)
        {
            var user = new UserForLoginDto
            {
                Username = username
            };

            user = await _apiHelper.GetRecord<UserForLoginDto>(_apiAppSetting.AuthHash, user);

            if (!_apiHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Unauthorized");
            }
            user.Username = username;

            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync(_apiAppSetting.AuthLogin, user))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<Auth>();
                    var resultUser = result.User;
                    if(!resultUser.UserRole.RoleName.ToLower().Equals("admin") )
                    {
                        throw new Exception("Username is not allowed to override");
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
