using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.FC2J.DataStore.Interfaces.Codesets;
using Project.FC2J.Models.Dtos;

namespace Project.FC2J.API.Controllers.Codesets
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KeyValuesController : ControllerBase
    {
        private readonly IKeyValueRepository _repo;

        public KeyValuesController(IKeyValueRepository repository)
        {
            _repo = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _repo.GetList();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ObjectWrapper value)
        {
            try
            {
                await _repo.Save(value.Data);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
