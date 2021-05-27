using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Dtos;

namespace Project.FC2J.UI.Helpers
{
    public interface IKeyValueEndpoint
    {
        Task<List<KeyValueDto>> GetList();
        Task Save(List<KeyValueDto> values);
    }
}