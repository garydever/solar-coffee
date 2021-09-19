﻿using SolarCoffee.Data.Models;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Serialization
{
    public static class OrderMapper
    {
        public static SalesOrder SerializeInvoiceToOrder(InvoiceModel invoice) 
        {
            //Maps an InvoiceModel to a SalesOrder datamodel.
            var salesOrderItems = invoice.LineItems
                                         .Select(item => new SalesOrderItem {
                                             Id = item.Id,
                                             Quantity = item.Quantity,
                                             Product = ProductMapper.SerializeProductModel(item.Product)
                                         }).ToList();
            return new SalesOrder
            {
                SalesOrderItems = salesOrderItems,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,

            };
        }

        public static List<OrderModel> SerializeOrdersToViewModels(IEnumerable<SalesOrder> orders) 
        {
            return orders.Select(order => new OrderModel
            {
                Id = order.Id,
                CreatedOn = order.CreatedOn,
                UpdatedOn = order.UpdatedOn,
                SalesOrderItems = SerializeSalesOrderItems(order.SalesOrderItems),
                Customer = CustomerMapper.SerializeCustomer(order.Customer),
                IsPaid = order.IsPaid
            }).ToList();                
        }

        //Maps a collection of SalesOrderItems to SalesOrderITemModels
        private static List<SalesOrderItemModel> SerializeSalesOrderItems(IEnumerable<SalesOrderItem> orderItems)
        {
            return orderItems.Select(item => new SalesOrderItemModel
            {
                Id = item.Id,
                Quantity = item.Quantity,
                Product = ProductMapper.SerializeProductModel(item.Product)
            }).ToList();
        } 


    }
}