using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Controllers;
using WebApp.Core.Aplication.Interfaces;
using WebApp.Core.Domain.Entities;
using WebApp.Core.Domain.Enums;
using Xunit;

namespace Api.Test
{
	public class CustomerControllerTest
	{
		private readonly CustomersController _customerController;
		private readonly ICustomerRepository _customerRepository = Substitute.For<ICustomerRepository>();

		private readonly IEnumerable<Customer> _customersList = new List<Customer>() {
			new Customer
			{
				CustomerId=1,
				FirstName="lucas",
				LastName ="bolivar",
				Address = "jujuy",
				Status = CustomerStatus.Active
			},
			new Customer
			  {
				CustomerId=2,
				FirstName="julieta",
				LastName ="yamin",
				Address = "tucuman",
				Status = CustomerStatus.Paused
			}
			};


		public CustomerControllerTest()
		{
			_customerController = new CustomersController(_customerRepository);
		}




		[Fact]
		public async Task GetAllAsync_ShouldReturn200_WhenCustomerExist()
		{
			// arrange
			_customerRepository.GetCustomers().Returns(_customersList);
			// act
			var customers = await _customerController.GetAll();
			var result = customers as ObjectResult;
			// assert
			Assert.NotNull(result);
			Assert.Equal(200, result.StatusCode);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturn404_WhenCustomerDoesNotExist()
		{
			// arrange
			_customerRepository.GetCustomers().ReturnsNull();
			// act
			var customers = (IStatusCodeActionResult)await _customerController.GetAll();
			// var result = customers as NotFoundObjectResult;
			// assert
			Assert.NotNull(customers);
			Assert.Equal(404, customers.StatusCode);
		}


	}
}
