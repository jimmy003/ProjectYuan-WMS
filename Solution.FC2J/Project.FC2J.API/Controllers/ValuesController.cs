﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.FC2J.DataStore;

namespace Project.F2CJ.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {


        // GET api/values       
        [HttpGet]
        //public async Task<IEnumerable<string>> Get()
        //{
        //    var list = new List<string>();

        //    await DbHelper.DoQuery(list);

        //    return list;
        //}

        public async Task<IActionResult> Get()
        {
            return Ok(await Task.Run(() => new[] { "Hi", "There" }) );
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}