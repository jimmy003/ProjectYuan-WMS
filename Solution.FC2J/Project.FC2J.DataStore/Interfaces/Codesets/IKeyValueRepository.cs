using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Dtos;

namespace Project.FC2J.DataStore.Interfaces.Codesets
{
    public interface IKeyValueRepository
    {
        Task<List<KeyValueDto>> GetList();
        Task Save(List<KeyValueDto> values);
    }
}