﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;
        public InventoryController(IInventoryService inventory, ILogger<InventoryController> logger)
        {
            _inventoryService = inventory;
            _logger = logger;
        }

        [HttpGet("/api/inventory")]
        public ActionResult GetCurrentInventory()
        {
            _logger.LogInformation("Getting all inventory...");
            var inventory = _inventoryService.GetCurrentInventory()
                            .Select(pi => new ProductInventoryModel { 
                                Id = pi.Id,
                                IdealQuantity = pi.IdealQuantity,
                                QuantityOnHand = pi.QuantityOnHand,
                                Product = ProductMapper.SerializeProductModel(pi.Product)
                            })
                            .OrderBy(inv => inv.Product.Name)
                            .ToList();
            return Ok(inventory);
        }

        [HttpPatch("/api/inventory")]
        public ActionResult UpdateInventory([FromBody] ShipmentModel shipment)
        {
            _logger.LogInformation($"Updating inventory for {shipment.ProductId} - Adjustment: {shipment.Adjustment}");
            var id = shipment.ProductId;
            var adjustment = shipment.Adjustment;
            var inventory = _inventoryService.UpdateUnitsAvailable(id, adjustment);
            return Ok(inventory);           
        }

    }
}