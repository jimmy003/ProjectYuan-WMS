using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.FC2J.DataStore.Interfaces.Codesets;
using Project.FC2J.Models.Customer;

namespace Project.FC2J.API.Controllers.Codesets
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeductionsController : ControllerBase
    {
        private readonly IDeductionRepository _repo;

        public DeductionsController(IDeductionRepository repository)
        {
            _repo = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(int isUsed, long customerId, string poNo)
        {
            if (string.IsNullOrEmpty(poNo))
            {
                if (isUsed == 2)
                {
                    var list = await _repo.GetList(customerId);
                    return Ok(list);
                }
                else
                {
                    var list = await _repo.GetList(isUsed, customerId);
                    return Ok(list);
                }
            }
            else
            {
                var list = await _repo.GetDeductions(poNo, customerId);
                return Ok(list);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(Deduction value)
        {
            var result = await _repo.Save(value);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Deduction value)
        {
            await _repo.Update(value);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id, long customerId)
        {
            await _repo.Remove(id, customerId);
            return Ok();
        }
    }
}
