using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Dtos;

namespace Project.FC2J.UI.Helpers
{
    public class KeyValueEndpoint : IKeyValueEndpoint
    {
        private readonly IAPIHelper _apiHelper;
        private const string _resource = "/api/keyvalues";

        public KeyValueEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<KeyValueDto>> GetList()
        {
            var obj = await _apiHelper.GetList<KeyValueDto>(_resource);
            return obj;
        }

        public async Task Save(List<KeyValueDto> values)
        {
            await _apiHelper.GetRecord(_resource, new ObjectWrapper{ Data = values } );
        }
    }
}
