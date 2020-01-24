using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.FC2J.DataStore.Interfaces
{
    public interface IRepository<T> 
    {
        Task<List<T>> GetList();
        Task<T> Save(T value);
        Task Update(T value);
        Task Remove(long id);
    }
}
