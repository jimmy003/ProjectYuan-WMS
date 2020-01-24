using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Report;

namespace Project.FC2J.UI.Helpers
{
    public interface IEndpoint<T>
    {
        Task<List<T>> GetList();
        Task<T> Save(T value);
        Task Update(T value);
        Task Remove(long id);
    }
}
