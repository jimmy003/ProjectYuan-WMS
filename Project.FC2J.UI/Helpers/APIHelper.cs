using Project.FC2J.Models;
using Project.FC2J.Models.Dtos;
using Project.FC2J.Models.Token;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient { get; set; }

        private string Username { get; set; }

        private readonly IApiAppSetting _apiAppSetting;
        private readonly ILoggedInUser _loggedInUser;

        public APIHelper(IApiAppSetting apiAppSetting, ILoggedInUser loggedInUser)
        {
            InitializeClient();
            _apiAppSetting = apiAppSetting;
            _loggedInUser = loggedInUser;
        }

        public HttpClient ApiClient => _apiClient;

        private void InitializeClient()
        {
            string api = ConfigurationManager.AppSettings["api"];
            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        }

        public void LogOffUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }

        public async Task Authenticate(string username, string password)
        {

            var user = new UserForLoginDto
            {
                Username = username
            };

            user = await GetRecord<UserForLoginDto>(_apiAppSetting.AuthHash, user);

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Unauthorized");
            }
            user.Username = username;

            using (HttpResponseMessage response = await _apiClient.PostAsJsonAsync(_apiAppSetting.AuthLogin, user))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<Auth>();
                    _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                    _loggedInUser.Token = result.Token;
                    _loggedInUser.User = result.User;
                    _loggedInUser.User.PasswordX = password;
                    _loggedInUser.Setting = result.Setting;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<T> GetRecord<T>(string resource)
        {
            using (HttpResponseMessage response = await _apiClient.GetAsync(resource))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<T>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<T> GetRecord<T>(string resource, T value)
        {
            using (HttpResponseMessage response = await _apiClient.PostAsJsonAsync(resource, value))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<T>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<List<T>> GetList<T>(string resource)
        {
            using (HttpResponseMessage response = await _apiClient.GetAsync(resource))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<T>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<DataTable> GetDataTable(string resource, object value)
        {
            using (HttpResponseMessage response = await _apiClient.PostAsJsonAsync(resource, value))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<DataTable>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<T> Save<T>(string resource, T value)
        {

            using (HttpResponseMessage response = await _apiClient.PostAsJsonAsync(resource, value))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<T>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task Update<T>(string resource, T value)
        {
            using (HttpResponseMessage response = await _apiClient.PutAsJsonAsync(resource, value))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task Update(string resource)
        {
            using (HttpResponseMessage response = await _apiClient.PutAsync(resource, null ))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task Remove(string resource)
        {
            using (HttpResponseMessage response = await _apiClient.DeleteAsync(resource))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }


        public DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

    }
}
