using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SolarCoffee.Data;
using SolarCoffee.Services.Customer;
using SolarCoffee.Data.Models;
using FluentAssertions;
using System.Linq;
using Moq;

namespace SolarCoffee.test
{
    public class TestCustomerService
    {
        [Fact]
        public void CustomerService_GetAllCustomers_GivenTheyExist()
        {
            var options = new DbContextOptionsBuilder<SolarDbContext>()
                .UseInMemoryDatabase("gets_all").Options;

            using var context = new SolarDbContext(options);

            var sut = new CustomerService(context);

            sut.CreateCustomer(new Customer { Id = 123 });
            sut.CreateCustomer(new Customer { Id = 456 });
            sut.CreateCustomer(new Customer { Id = 789 });

            var allCustomers = sut.GetAllCustomers();

            allCustomers.Count.Should().Be(3);
        }

        [Fact]
        public void CustomerService_CreatesCustomer_GivenNewCustomerObject()
        {
            var options = new DbContextOptionsBuilder<SolarDbContext>()
                .UseInMemoryDatabase("add_writes_to_database").Options;

            using var context = new SolarDbContext(options);

            var sut = new CustomerService(context);

            sut.CreateCustomer(new Customer { Id = 123 });
            context.Customers.Single().Id.Should().Be(123);
        }

        [Fact]
        public void CustomerService_DeletesCustomer_GivenId()
        {
            var options = new DbContextOptionsBuilder<SolarDbContext>()
                .UseInMemoryDatabase("deletes_one").Options;

            using var context = new SolarDbContext(options);

            var sut = new CustomerService(context);

            sut.CreateCustomer(new Customer { Id = 123 });
            sut.DeleteCustomer(123);
            var allCustomers = sut.GetAllCustomers();
            allCustomers.Count().Should().Be(0);
        }

        [Fact]
        public void CustomerService_OrdersByLastName_WhenGetAllCustomersInvoked()
        {
            //Arrange
            var data = new List<Customer>
            {
                new Customer { Id = 123, LastName = "Lebowski"},
                new Customer { Id = 456, LastName = "Buscemi"},
                new Customer { Id = 789, LastName = "Dickinson"}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Customer>>();

            mockSet.As<IQueryable<Customer>>()
                .Setup(m => m.Provider)
                .Returns(data.Provider);

            mockSet.As<IQueryable<Customer>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);
            
            mockSet.As<IQueryable<Customer>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);

            mockSet.As<IQueryable<Customer>>()
                .Setup(m => m.GetEnumerator())
                .Returns(data.GetEnumerator());

            var mockContext = new Mock<SolarDbContext>();

            mockContext.Setup(c => c.Customers)
                .Returns(mockSet.Object);

            //Act
            var sut = new CustomerService(mockContext.Object);
            var customers = sut.GetAllCustomers();

            //Assert
            customers.Count.Should().Be(3);
            customers[0].Id.Should().Be(456);
            customers[1].Id.Should().Be(789);
            customers[2].Id.Should().Be(123);
        }
        
    }
}
