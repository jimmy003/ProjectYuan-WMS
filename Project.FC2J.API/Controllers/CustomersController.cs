using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Dtos;
using Project.FC2J.DataStore.Interfaces;

namespace Project.F2CJ.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repo; 

        public CustomersController(ICustomerRepository customerRepository)
        {
            _repo = customerRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCustomers(string partialName)
        {
            if (string.IsNullOrEmpty(partialName))
            {
                var list = await _repo.GetCustomers();
                return Ok(list);
            }
            else
            {
                var list = await _repo.GetCustomersByPartialName(partialName);
                return Ok(list);
            }
        }

        [Route("PriceList")]
        public async Task<IActionResult> GetCustomerPriceList(long id, int supplier)
        {
            var list = await _repo.GetCustomerPriceList(id, supplier);
            return Ok(list);
        }

        [Route("Payment")]
        public async Task<IActionResult> GetCustomerPayment(long id)
        {
            if (id > 0)
            {
                var list = await _repo.GetCustomerPayment(id);
                return Ok(list);
            }
            else
            {
                var payments = await _repo.GetPayments();
                return Ok(payments);
            }
        }
        
        
        [Route("Farm")]
        public async Task<IActionResult> GetFarms()
        {
            var list = await _repo.GetFarms();
            return Ok(list);
        }


        [Route("Product")]
        public async Task<IActionResult> GetCustomerProduct(long id)
        {
            var list = await _repo.GetCustomerProduct(id);
            return Ok(list);
        }

        [Route("ShipTo")]
        public async Task<IActionResult> GetCustomerShipTo(long id)
        {
            var list = await _repo.GetCustomerShipTo(id);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Customer customer)
        {
            var result = await _repo.SaveCustomer(customer);
            return Ok(result);
        }


        [HttpPost, Route("PriceList")]
        public async Task<IActionResult> SaveCustomerPriceList(PriceListProduct value)
        {
            await _repo.SaveCustomerPriceList(value);
            return Ok();
        }

        [HttpPost, Route("Payment")]
        public async Task<IActionResult> SaveCustomerPayment(CustomerPayment customerPayment)
        {
            await _repo.SaveCustomerPayment(customerPayment);
            return Ok();
        }


        [HttpPost, Route("Product")]
        public async Task<IActionResult> SaveCustomerProduct(CustomerProduct customerProduct)
        {
            await _repo.SaveCustomerProduct(customerProduct);
            return Ok();
        }

        [HttpPost, Route("ShipTo")]
        public async Task<IActionResult> SaveCustomerShipTo(CustomerShipTo customerShipTo)
        {
            await _repo.SaveCustomerShippingAddress(customerShipTo);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(Customer customer)
        {
            await _repo.UpdateCustomer(customer);
            return Ok();
        }

        [HttpPut, Route("Payment")]
        public async Task<IActionResult> UpdateCustomerPayment(CustomerPayment customerPayment)
        {
            await _repo.UpdateCustomerPayment(customerPayment);
            return Ok();
        }

        [HttpPut, Route("PriceList")]
        public async Task<IActionResult> UpdateCustomerPricelist(CustomerPricelist customerPricelist)
        {
            await _repo.UpdateCustomerPricelist(customerPricelist);
            return Ok();
        }

        [HttpPut, Route("Product")]
        public async Task<IActionResult> UpdateCustomerProduct(CustomerProduct customerProduct)
        {
            await _repo.UpdateCustomerProduct(customerProduct);
            return Ok();
        }

        [HttpPut, Route("ShipTo")]
        public async Task<IActionResult> UpdateCustomerShipTo(CustomerShipTo customerShipTo)
        {
            await _repo.UpdateCustomerShippingAddress(customerShipTo);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            await _repo.RemoveCustomer(id);
            return Ok();
        }



    }
}
