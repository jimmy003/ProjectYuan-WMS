using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Project.FC2J.DataStore.Interfaces.Codesets;
using Project.FC2J.DataStore.Internal.DataAccess;
using Project.FC2J.Models.Dtos;

namespace Project.FC2J.DataStore.DataAccess.Codesets
{
    public class KeyValueRepository : IKeyValueRepository
    {
        public async Task<List<KeyValueDto>> GetList()
        {
            var value = await "spKeyValuePair_GetList".GetList<KeyValueDto>();
            return value;
        }

        public async Task Save(List<KeyValueDto> values)
        {
            foreach (var keyValueDto in values)
            {
                var _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Key", keyValueDto.Key),
                    new SqlParameter("@Value", keyValueDto.Value)
                };
                await "spKeyValuePair_Update".ExecuteNonQueryAsync(_sqlParameters.ToArray());
            }
        }
    }
}
