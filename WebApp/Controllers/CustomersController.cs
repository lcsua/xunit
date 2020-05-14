using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApp.Core.Aplication.Interfaces;
using WebApp.Core.Domain.Entities;

namespace WebApp.Controllers
{
 
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json",
              "application/xml")]
    [Route("api/v{version:apiVersion}/[controller]")]    
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

       
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<Customer>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var customers = _customerRepository.GetCustomers();
            if (customers!=null && customers.Any())
                return Ok(customers);
            else
                return NotFound();
        }

        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(Customer), 200)]
        public async Task<IActionResult> Get(int customerId)
        {
            return Ok(_customerRepository.GetCustomerByID(customerId));
        }
    }
}