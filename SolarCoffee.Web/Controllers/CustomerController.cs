using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Customer;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpPost("/api/customer")]
        public ActionResult CreateCustomer([FromBody] CustomerModel customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Creating customer");
            customer.CreatedOn = DateTime.UtcNow;
            customer.UpdatedOn = DateTime.UtcNow;
            var customerData = CustomerMapper.SerializeCustomer(customer);
            var newCustomer = _customerService.CreateCustomer(customerData);
            return Ok(newCustomer);
        }

        [HttpGet("/api/customer")]
        public ActionResult GetCustomers()
        {
            _logger.LogInformation("Getting customers");
            var customers = _customerService.GetAllCustomers();

            //Why are we doing it like this?
            //var cusomterModels = customers.Select(c => new CustomerModel
            //{
            //    FirstName = c.FirstName,
            //    LastName = c.LastName,
            //    PrimaryAddress = CustomerMapper.MapCustomerAddress(c.PrimaryAddress),
            //    CreatedOn = c.CreatedOn,
            //    UpdatedOn = c.UpdatedOn
            //})
            //.OrderByDescending(c => c.CreatedOn)
            //.ToList();

            //When we could just do this?
            var customerModels = customers.Select(c => CustomerMapper.SerializeCustomer(c))
                .OrderByDescending(c => c.CreatedOn)
                .ToList();
            return Ok(customerModels);
        }

        [HttpDelete("api/customer/{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            _logger.LogInformation("Deleting a customer");
            var response = _customerService.DeleteCustomer(id);
            return Ok(response);
        }
    }
}
