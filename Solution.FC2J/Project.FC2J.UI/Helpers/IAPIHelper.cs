using Project.FC2J.Models.Customer;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Helpers
{
    public interface IAPIHelper
    {
        Task Authenticate(string username, string password);

        Task<List<T>> GetList<T>(string resource);

        Task<T> GetRecord<T>(string resource);

        Task<T> GetRecord<T>(string resource, T value);

        Task<T> Save<T>(string resource, T value);

        Task Update<T>(string resource, T customer);

        Task Update(string resource);

        Task Remove(string resource);
        HttpClient ApiClient { get; }

        void LogOffUser();

        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);

        DataTable ToDataTable<T>(IList<T> data);

        Task<DataTable> GetDataTable(string resource, object value);

    }
}