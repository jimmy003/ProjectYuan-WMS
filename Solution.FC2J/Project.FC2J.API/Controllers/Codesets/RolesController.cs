using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.DataStore.Interfaces.Codesets;

namespace Project.FC2J.API.Controllers.Codesets
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _repo;
        
        public RolesController(IRoleRepository repository)
        {
            _repo = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var list = await _repo.GetList();
            return Ok(list);
        }


    }
}
