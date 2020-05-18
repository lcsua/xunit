using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
			var items = Assert.IsAssignableFrom<IEnumerable<Customer>>(
					Assert.IsType<List<Customer>>(result.Value));
			Assert.NotNull(result);
			Assert.True(result is OkObjectResult);
			Assert.Equal(2, items.Count());
			Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturn404_WhenCustomersAreNull()
		{
			// arrange
			_customerRepository.GetCustomers().ReturnsNull();
			// act
			var customers = await _customerController.GetAll();
			var result = customers as NotFoundResult;
			// assert
			Assert.NotNull(customers);
			Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
		}

		[Fact]
		public async Task GetCustomerByIdAsync_ShouldReturn200_WhenCustomerExist()
		{
			// arrange
			var customerId = 1;
			_customerRepository.GetCustomerByID(customerId).Returns(_customersList.FirstOrDefault(x => x.CustomerId == customerId));
			// act
			var customerResult = (await _customerController.Get(customerId)) as ObjectResult;

			// assert
			var customer = Assert.IsAssignableFrom<Customer>(
				Assert.IsType<Customer>(customerResult.Value));
			Assert.NotNull(customerResult);
			Assert.True(customerResult is OkObjectResult);
			Assert.Equal(customerId, customer.CustomerId);
			Assert.Equal(StatusCodes.Status200OK, customerResult.StatusCode);
		}

		[Fact]
		public async Task GetCustomerByIdAsync_ShouldReturn404_WhenCustomerDoesNotExist()
		{
			// arrange
			var customerId = 1;
			_customerRepository.GetCustomerByID(customerId).ReturnsNull();
			// act
			var customers = await _customerController.Get(customerId);
			var result = customers as NotFoundResult;
			// assert
			Assert.NotNull(customers);
			Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
		}

		[Fact]
		public async Task PostCustomerAsync_ShouldReturn202_WhenCustomerIsInsert()
		{
			// arrange
			var customerId = 3;
			var customer = new Customer
			{
				CustomerId = customerId,
				FirstName = "unit",
				LastName = "test",
				Status = CustomerStatus.Active,
				Address = "test"
			};
			_customerRepository.InsertCustomer(customer).Returns(customer);
			// act
			var customerInsert = (await _customerController.Post(customer)) as ObjectResult;
			// assert
			var customerResponse = Assert.IsAssignableFrom<Customer>(
				Assert.IsType<Customer>(customerInsert.Value));
			Assert.NotNull(customerInsert);
			Assert.Equal(customerId, customerResponse.CustomerId);
			Assert.Equal(StatusCodes.Status202Accepted, customerInsert.StatusCode);
		}

		[Fact]
		public async Task PostCustomerAsync_ShouldReturn400_WhenCustomerIsNull()
		{
			// arrange
			_customerRepository.InsertCustomer(null).ReturnsNull();
			// act
			var customerInsert = (await _customerController.Post(null)) as BadRequestObjectResult;
			// assert
			Assert.NotNull(customerInsert);
			Assert.Equal(StatusCodes.Status400BadRequest, customerInsert.StatusCode);
			Assert.StartsWith("the customer is required", customerInsert.Value.ToString());
		}

		[Fact]
		public async Task PostCustomerAsync_ShouldReturn294_WhenCustomerCouldNotBeInsert()
		{
			// arrange
			// arrange
			var customerId = 3;
			var customer = new Customer
			{
				CustomerId = customerId,
				FirstName = "unit",
				LastName = "test",
				Status = CustomerStatus.Active,
				Address = "test"
			};
			_customerRepository.InsertCustomer(customer).ReturnsNull();
			// act
			var customerInsert = (await _customerController.Post(customer)) as NoContentResult;
			// assert
			Assert.NotNull(customerInsert);
			Assert.Equal(StatusCodes.Status204NoContent, customerInsert.StatusCode);
		}
	}
}
