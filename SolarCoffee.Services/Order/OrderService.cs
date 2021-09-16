using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Services.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarCoffee.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly SolarDbContext _db;
        private readonly ILogger<OrderService> _logger;
        private readonly IInventoryService _inventoryService;
        private readonly IProductService _productService;  
        public OrderService(SolarDbContext dbContext, 
                            ILogger<OrderService> logger, 
                            IInventoryService inventory,
                            IProductService product)
        {
            _db = dbContext;
            _logger = logger;
            _inventoryService = inventory;
            _productService = product;
        }

        //Creates an open SalesOrder
        public ServiceResponse<bool> GenerateOpenOrder(SalesOrder order)
        {
            var now = DateTime.UtcNow;

            _logger.LogInformation("Generating new order");

            foreach (var item in order.SalesOrderItems)
            {
                // 1. Why do we need to get the product by ProductId here? Why is this call to _productService needed?
                item.Product = _productService
                                    .GetProductById(item.Product.Id);

                // 2. When we are passing the same ProductId to _inventoryService.GetProductById()? Why can't we just pass item.Product.Id to 
                //    _inventoryService.GetByProductId() and remove the code above?

                var inventoryId = _inventoryService
                                    .GetByProductId(item.Product.Id).Id;

                _inventoryService
                    .UpdateUnitsAvailable(inventoryId, -item.Quantity);
            }

            try
            {
                _db.SalesOrders.Add(order);
                _db.SaveChanges();

                return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Open order created",
                    Time = now
                };
            }
            catch(Exception e)
            {
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = e.StackTrace,
                    Time = now
                };
            }
        }

        public List<SalesOrder> GetOrders()
        {
            return _db.SalesOrders
                    .Include(order => order.SalesOrderItems)
                        .ThenInclude(soi => soi.Product)
                    .Include(order => order.Customer)
                        .ThenInclude(c => c.PrimaryAddress)
                    .ToList();
        }

        public ServiceResponse<bool> MarkFulfilled(int id)
        {
            var now = DateTime.UtcNow;
            var order = _db.SalesOrders.Find(id);
            order.UpdatedOn = now;
            order.IsPaid = true;
            try
            {
                _db.SalesOrders.Update(order);
                _db.SaveChanges();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Time = now,
                    Message = $"Sales order {order.Id} closed. Invoice paid in full.",
                    IsSuccess = true
                };
            }
            catch(Exception e)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Time = now,
                    Message = e.StackTrace,
                    IsSuccess = false
                };
            }
            


        }
    }
}
